// <copyright file="SendOptionAcknowledgementBase.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using Tftp.Net.Trace;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer;

internal class SendOptionAcknowledgementBase(ILogger logger) : StateThatExpectsMessagesFromDefaultEndPoint(logger)
{
    private readonly ILogger logger = logger;

    public override void OnStateEnter()
    {
        logger.StateEntered(nameof(SendOptionAcknowledgementBase));
        base.OnStateEnter();
        SendAndRepeat(new OptionAcknowledgement { Options = Context.NegotiatedOptions.ToOptionList() });
    }

    public override void OnError(Error command)
    {
        logger.StateError(nameof(SendOptionAcknowledgementBase), command.ErrorCode, command.Message);
        Context.SetState(new ReceivedError(command, logger));
    }

    public override void OnCancel(TftpErrorPacket reason)
    {
        logger.StateCancelled(nameof(SendOptionAcknowledgementBase), reason.ErrorCode, reason.ErrorMessage);
        Context.SetState(new CancelledByUser(reason, logger));
    }
}
