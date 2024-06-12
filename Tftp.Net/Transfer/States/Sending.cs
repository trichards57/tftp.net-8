// <copyright file="Sending.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using System;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class Sending(ILogger logger) : StateThatExpectsMessagesFromDefaultEndPoint(logger)
{
    private readonly ILogger logger = logger;
    private long bytesSent = 0;
    private ushort lastBlockNumber;
    private byte[] lastData;
    private bool lastPacketWasSent = false;

    public override void OnAcknowledgement(Acknowledgement command)
    {
        logger.StateAcknowledged(nameof(Sending), command.BlockNumber);

        // Drop acknowledgments for other packets than the previous one
        if (command.BlockNumber != lastBlockNumber)
        {
            return;
        }

        // Notify our observers about our progress
        bytesSent += lastData.Length;
        Context.RaiseOnProgress(bytesSent);

        if (lastPacketWasSent)
        {
            // We're done here
            Context.RaiseOnFinished();
            Context.SetState(new Closed(logger));
        }
        else
        {
            SendNextPacket(Context.BlockCounterWrapping.CalculateNextBlockNumber(lastBlockNumber));
        }
    }

    public override void OnCancel(TftpErrorPacket reason)
    {
        logger.StateCancelled(nameof(Sending), reason.ErrorCode, reason.ErrorMessage);
        Context.SetState(new CancelledByUser(reason, logger));
    }

    public override void OnError(Error command)
    {
        logger.StateError(nameof(Sending), command.ErrorCode, command.Message);
        Context.SetState(new ReceivedError(command, logger));
    }

    public override void OnStateEnter()
    {
        logger.StateEntered(nameof(Sending));
        base.OnStateEnter();
        lastData = new byte[Context.BlockSize];
        SendNextPacket(1);
    }

    private void SendNextPacket(ushort blockNumber)
    {
        if (Context.InputOutputStream == null)
        {
            return;
        }

        int packetLength = Context.InputOutputStream.Read(lastData, 0, lastData.Length);
        lastBlockNumber = blockNumber;

        if (packetLength != lastData.Length)
        {
            // This means we just sent the last packet
            lastPacketWasSent = true;
            Array.Resize(ref lastData, packetLength);
        }

        ITftpCommand dataCommand = new Data { BlockNumber = blockNumber, Bytes = lastData };
        SendAndRepeat(dataCommand);
    }
}
