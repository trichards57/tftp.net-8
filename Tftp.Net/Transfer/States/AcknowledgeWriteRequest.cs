// <copyright file="AcknowledgeWriteRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class AcknowledgeWriteRequest(ILogger logger) : StateThatExpectsMessagesFromDefaultEndPoint(logger)
{
    private readonly ILogger logger = logger;

    public override void OnCancel(TftpErrorPacket reason)
    {
        logger.StateCancelled(nameof(AcknowledgeWriteRequest), reason.ErrorCode, reason.ErrorMessage);
        Context.SetState(new CancelledByUser(reason, logger));
    }

    public override void OnData(Data command)
    {
        logger.StateDataReceived(nameof(AcknowledgeWriteRequest));
        var nextState = new Receiving(logger);
        Context.SetState(nextState);
        nextState.OnCommand(command, Context.GetConnection().RemoteEndpoint);
    }

    public override void OnError(Error command)
    {
        logger.StateError(nameof(AcknowledgeWriteRequest), command.ErrorCode, command.Message);
        Context.SetState(new ReceivedError(command, logger));
    }

    public override void OnStateEnter()
    {
        logger.StateEntered(nameof(AcknowledgeWriteRequest));
        base.OnStateEnter();
        SendAndRepeat(new Acknowledgement { BlockNumber = 0 });
    }
}
