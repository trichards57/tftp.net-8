// <copyright file="TftpTransfer.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Net;
using System.Threading;
using Tftp.Net.Channel;
using Tftp.Net.Trace;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer;

internal class TftpTransfer : ITftpTransfer
{
    private readonly object lockObject = new();
    private readonly Timer timer;
    private BlockCounterWrapAround wrapping = BlockCounterWrapAround.ToZero;

    public TftpTransfer(ITransferChannel connection, string filename, ITransferState initialState)
    {
        ProposedOptions = TransferOptionSet.NewDefaultSet();
        Filename = filename;
        RetryCount = 5;
        SetState(initialState);
        Connection = connection;
        Connection.OnCommandReceived += new TftpCommandHandler(Connection_OnCommandReceived);
        Connection.OnError += new TftpChannelErrorHandler(Connection_OnError);
        Connection.Open();
        timer = new Timer(Timer_OnTimer, null, 500, 500);
    }

    public event TftpErrorHandler OnError;

    public event TftpEventHandler OnFinished;

    public event TftpProgressHandler OnProgress;

    public virtual BlockCounterWrapAround BlockCounterWrapping
    {
        get => wrapping;
        set
        {
            ThrowExceptionIfTransferAlreadyStarted();
            wrapping = value;
        }
    }

    public virtual int BlockSize
    {
        get => NegotiatedOptions != null ? NegotiatedOptions.BlockSize : ProposedOptions.BlockSize;
        set
        {
            ThrowExceptionIfTransferAlreadyStarted();
            ProposedOptions.BlockSize = value;
        }
    }

    public virtual long ExpectedSize
    {
        get => NegotiatedOptions != null ? NegotiatedOptions.TransferSize : ProposedOptions.TransferSize;
        set
        {
            ThrowExceptionIfTransferAlreadyStarted();
            ProposedOptions.TransferSize = value;
        }
    }

    public string Filename { get; private set; }

    public Stream InputOutputStream { get; protected set; }

    public TransferOptionSet NegotiatedOptions { get; private set; }

    public TransferOptionSet ProposedOptions { get; set; }

    public int RetryCount { get; set; }

    public virtual TimeSpan RetryTimeout
    {
        get => TimeSpan.FromSeconds(NegotiatedOptions != null ? NegotiatedOptions.Timeout : ProposedOptions.Timeout);
        set
        {
            ThrowExceptionIfTransferAlreadyStarted();
            ProposedOptions.Timeout = value.Seconds;
        }
    }

    public virtual TftpTransferMode TransferMode { get; set; }

    public object UserContext { get; set; }

    public bool WasStarted { get; private set; }

    protected ITransferChannel Connection { get; }

    protected ITransferState State { get; set; }

    public void Cancel(TftpErrorPacket reason)
    {
        ArgumentNullException.ThrowIfNull(reason);

        lock (lockObject)
        {
            State.OnCancel(reason);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Start(Stream data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (WasStarted)
        {
            throw new InvalidOperationException("This transfer has already been started.");
        }

        WasStarted = true;
        InputOutputStream = data;

        lock (lockObject)
        {
            State.OnStart();
        }
    }

    public override string ToString()
    {
        return GetHashCode() + " (" + Filename + ")";
    }

    internal void FillOrDisableTransferSizeOption()
    {
        try
        {
            ProposedOptions.TransferSize = (int)InputOutputStream.Length;
        }
        catch (NotSupportedException)
        {
            // No action
        }
        finally
        {
            if (ProposedOptions.TransferSize <= 0)
            {
                ProposedOptions.IncludesTransferSizeOption = false;
            }
        }
    }

    internal void FinishOptionNegotiation(TransferOptionSet negotiated)
    {
        NegotiatedOptions = negotiated;
        if (!NegotiatedOptions.IncludesBlockSizeOption)
        {
            NegotiatedOptions.BlockSize = TransferOptionSet.DefaultBlockSize;
        }

        if (!NegotiatedOptions.IncludesTimeoutOption)
        {
            NegotiatedOptions.Timeout = TransferOptionSet.DefaultTimeoutSeconds;
        }
    }

    internal ITransferChannel GetConnection()
    {
        return Connection;
    }

    internal void RaiseOnError(ITftpTransferError error)
    {
        OnError?.Invoke(this, error);
    }

    internal void RaiseOnFinished()
    {
        OnFinished?.Invoke(this);
    }

    internal void RaiseOnProgress(long bytesTransferred)
    {
        OnProgress?.Invoke(this, new TftpTransferProgress(bytesTransferred, ExpectedSize));
    }

    internal void SetState(ITransferState newState)
    {
        State = DecorateForLogging(newState);
        State.Context = this;
        State.OnStateEnter();
    }

    protected virtual ITransferState DecorateForLogging(ITransferState state)
    {
        return TftpTrace.Enabled ? new LoggingStateDecorator(state, this) : state;
    }

    protected virtual void Dispose(bool disposing)
    {
        lock (lockObject)
        {
            timer.Dispose();
            Cancel(new TftpErrorPacket(0, "ITftpTransfer has been disposed."));

            if (InputOutputStream != null)
            {
                InputOutputStream.Close();
                InputOutputStream = null;
            }

            Connection.Dispose();
        }
    }

    private void Connection_OnCommandReceived(ITftpCommand command, IPEndPoint endpoint)
    {
        lock (lockObject)
        {
            State.OnCommand(command, endpoint);
        }
    }

    private void Connection_OnError(ITftpTransferError error)
    {
        lock (lockObject)
        {
            RaiseOnError(error);
        }
    }

    private void ThrowExceptionIfTransferAlreadyStarted()
    {
        if (WasStarted)
        {
            throw new InvalidOperationException("You cannot change tftp transfer options after the transfer has been started.");
        }
    }

    private void Timer_OnTimer(object context)
    {
        try
        {
            lock (lockObject)
            {
                State.OnTimer();
            }
        }
        catch (Exception e)
        {
            TftpTrace.Trace("Ignoring unhandled exception: " + e, this);
        }
    }
}
