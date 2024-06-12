﻿// <copyright file="Sending.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tftp.Net.Transfer.States;

internal class Sending : StateThatExpectsMessagesFromDefaultEndPoint
{
    private long bytesSent = 0;
    private ushort lastBlockNumber;
    private byte[] lastData;
    private bool lastPacketWasSent = false;

    public override void OnAcknowledgement(Acknowledgement command)
    {
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
            Context.SetState(new Closed());
        }
        else
        {
            SendNextPacket(Context.BlockCounterWrapping.CalculateNextBlockNumber(lastBlockNumber));
        }
    }

    public override void OnCancel(TftpErrorPacket reason)
    {
        Context.SetState(new CancelledByUser(reason));
    }

    public override void OnError(Error command)
    {
        Context.SetState(new ReceivedError(command));
    }

    public override void OnStateEnter()
    {
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

        ITftpCommand dataCommand = new Data(blockNumber, lastData);
        SendAndRepeat(dataCommand);
    }
}
