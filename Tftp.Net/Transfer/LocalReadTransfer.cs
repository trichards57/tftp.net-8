// <copyright file="LocalReadTransfer.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stateless;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Tftp.Net.Channel;

namespace Tftp.Net.Transfer.StateMachine;

internal sealed class LocalReadTransfer : ITftpTransfer
{
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<ushort, IPEndPoint> acknowledgeTrigger;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<ushort, string> cancelTrigger;
    private readonly ITransferChannel connection;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<ITftpTransferError> errorTrigger;
    private readonly IList<TransferOption> options;
    private readonly StateMachine<State, Trigger> stateMachine;
    private readonly ITimer timeoutTimer;
    private long bytesSent;
    private Stream data;
    private ushort lastBlockNumber = 1;
    private ITftpCommand lastCommand;
    private byte[] lastData = null;
    private bool lastPacketWasSent = false;
    private int retriesUsed;

    public LocalReadTransfer(IList<TransferOption> options, ITransferChannel connection, string filename, TimeProvider timeProvider)
    {
        Filename = filename;
        this.options = options;
        this.connection = connection;
        connection.OnCommandReceived += CommandReceived;
        connection.OnError += ErrorReceived;
        connection.Open();
        timeoutTimer = timeProvider.CreateTimer(HandleTimer, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        var opts = new TransferOptionSet(options);
        BlockSize = opts.BlockSize;
        ExpectedSize = opts.TransferSize;
        RetryTimeout = TimeSpan.FromSeconds(opts.Timeout);

        stateMachine = new StateMachine<State, Trigger>(State.Start);

        cancelTrigger = stateMachine.SetTriggerParameters<ushort, string>(Trigger.Cancel);
        acknowledgeTrigger = stateMachine.SetTriggerParameters<ushort, IPEndPoint>(Trigger.Acknowledge);
        errorTrigger = stateMachine.SetTriggerParameters<ITftpTransferError>(Trigger.Error);

        stateMachine.Configure(State.Start)
            .Permit(Trigger.Cancel, State.Cancelled)
            .PermitIf(Trigger.Start, State.SendOptionAcknowledgement, () => this.options.Count > 0)
            .PermitIf(Trigger.Start, State.Sending, () => this.options.Count == 0);

        stateMachine.Configure(State.Cancelled)
            .Permit(Trigger.Close, State.Closed)
            .OnEntryFrom(cancelTrigger, (ushort code, string message) =>
            {
                connection.Send(new Error { ErrorCode = code, Message = message });
                stateMachine.Fire(Trigger.Close);
            });

        stateMachine.Configure(State.SendOptionAcknowledgement)
            .OnEntry(() =>
            {
                lastBlockNumber = 0;
                SendAndRepeat(new OptionAcknowledgement { Options = this.options });
            })
            .Permit(Trigger.Cancel, State.Cancelled)
            .Permit(Trigger.Error, State.ReceivedError)
            .Permit(Trigger.Acknowledge, State.Sending);

        stateMachine.Configure(State.Sending)
            .OnEntry(() =>
            {
                SendNextPacket(lastBlockNumber);
            })
            .PermitReentryIf(Trigger.Acknowledge, () => !lastPacketWasSent)
            .Permit(Trigger.Cancel, State.Cancelled)
            .Permit(Trigger.Error, State.ReceivedError)
            .Permit(Trigger.Close, State.Closed);

        stateMachine.Configure(State.Closed);
    }

    public event TftpErrorHandler OnError;

    public event TftpEventHandler OnFinished;

    public event TftpProgressHandler OnProgress;

    public BlockCounterWrapAround BlockCounterWrapping { get; set; } = BlockCounterWrapAround.ToZero;

    public int BlockSize { get; set; }

    public long ExpectedSize { get; set; }

    public string Filename { get; }

    public int RetryCount { get; set; } = 5;

    public TimeSpan RetryTimeout { get; set; }

    public TftpTransferMode TransferMode { get; set; }

    public object UserContext { get; set; }

    public void Cancel(TftpErrorPacket reason)
    {
        stateMachine.Fire(cancelTrigger, reason.ErrorCode, reason.ErrorMessage);
    }

    public void Dispose()
    {
        timeoutTimer.Dispose();
    }

    public void Start(Stream data)
    {
        this.data = data;
        stateMachine.Fire(Trigger.Start);
    }

    private void CommandReceived(ITftpCommand command, IPEndPoint endpoint)
    {
        if (command is Acknowledgement ack)
        {
            HandleAcknowledgement(ack);
        }
        else
        {
            stateMachine.Fire(errorTrigger, new InvalidMessageError());
        }
    }

    private void ErrorReceived(ITftpTransferError error)
    {
        stateMachine.Fire(errorTrigger, error);
    }

    private void HandleAcknowledgement(Acknowledgement command)
    {
        if (command.BlockNumber != lastBlockNumber)
        {
            return;
        }

        StopTimeout();

        if (lastData != null)
        {
            bytesSent += lastData.Length;
            OnProgress?.Invoke(this, new TftpTransferProgress(bytesSent, ExpectedSize));

            if (lastPacketWasSent)
            {
                OnFinished?.Invoke(this);
                stateMachine.Fire(Trigger.Close);
            }
            else
            {
                var oldBlockNumber = lastBlockNumber;
                lastBlockNumber = BlockCounterWrapping.CalculateNextBlockNumber(lastBlockNumber);
                stateMachine.Fire(acknowledgeTrigger, oldBlockNumber, connection.RemoteEndpoint);
            }
        }
        else
        {
            stateMachine.Fire(acknowledgeTrigger, (ushort)0, connection.RemoteEndpoint);
        }
    }

    private void HandleTimer(object state)
    {
        if (retriesUsed < RetryCount && lastCommand != null)
        {
            connection.Send(lastCommand);
            retriesUsed++;
        }
        else
        {
            timeoutTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            stateMachine.Fire(errorTrigger, new TimeoutError(RetryTimeout, RetryCount));
        }
    }

    private void ResetTimeout()
    {
        timeoutTimer.Change(RetryTimeout, RetryTimeout);
        retriesUsed = 0;
    }

    private void SendAndRepeat(ITftpCommand command)
    {
        connection.Send(command);
        lastCommand = command;
        ResetTimeout();
    }

    private void SendNextPacket(ushort blockNumber)
    {
        if (data == null)
        {
            return;
        }

        lastData = new byte[BlockSize];
        var packetLength = data.Read(lastData, 0, lastData.Length);

        if (packetLength != lastData.Length)
        {
            lastPacketWasSent = true;
            Array.Resize(ref lastData, packetLength);
        }

        SendAndRepeat(new Data { BlockNumber = blockNumber, Bytes = lastData });
    }

    private void StopTimeout()
    {
        timeoutTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        retriesUsed = 0;
    }
}
