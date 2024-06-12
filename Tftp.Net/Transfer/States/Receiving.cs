// <copyright file="Receiving.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class Receiving(ILogger logger) : StateThatExpectsMessagesFromDefaultEndPoint(logger)
{
    private readonly ILogger logger = logger;
    private ushort lastBlockNumber = 0;
    private ushort nextBlockNumber = 1;
    private long bytesReceived = 0;

    public override void OnData(Data command)
    {
        logger.StateDataReceived(nameof(Receiving));
        if (command.BlockNumber == nextBlockNumber)
        {
            logger.DataBlockReceived(command.BlockNumber, Context.Filename);

            // We received a new block of data
            Context.InputOutputStream.Write(command.Bytes, 0, command.Bytes.Length);
            SendAcknowledgement(command.BlockNumber);

            // Was that the last block of data?
            if (command.Bytes.Length < Context.BlockSize)
            {
                Context.RaiseOnFinished();
                Context.SetState(new Closed(logger));
            }
            else
            {
                lastBlockNumber = command.BlockNumber;
                nextBlockNumber = Context.BlockCounterWrapping.CalculateNextBlockNumber(command.BlockNumber);
                bytesReceived += command.Bytes.Length;
                Context.RaiseOnProgress(bytesReceived);
            }
        }
        else if (command.BlockNumber == lastBlockNumber)
        {
            logger.DataBlockResent(command.BlockNumber, Context.Filename);

            // We received the previous block again. Re-sent the acknowledgement
            SendAcknowledgement(command.BlockNumber);
        }
    }

    public override void OnCancel(TftpErrorPacket reason)
    {
        logger.StateCancelled(nameof(Receiving), reason.ErrorCode, reason.ErrorMessage);
        Context.SetState(new CancelledByUser(reason, logger));
    }

    public override void OnError(Error command)
    {
        logger.StateError(nameof(Receiving), command.ErrorCode, command.Message);
        Context.SetState(new ReceivedError(command, logger));
    }

    private void SendAcknowledgement(ushort blockNumber)
    {
        var ack = new Acknowledgement { BlockNumber = blockNumber };
        Context.GetConnection().Send(ack);
        ResetTimeout();
    }
}
