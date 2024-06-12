// <copyright file="StartIncomingWrite.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class StartIncomingWrite(IEnumerable<TransferOption> optionsRequestedByClient, ILogger logger) : BaseState
{
    private readonly ILogger logger = logger;
    private readonly IEnumerable<TransferOption> optionsRequestedByClient = optionsRequestedByClient;

    public override void OnCancel(TftpErrorPacket reason)
    {
        logger.StateCancelled(nameof(StartIncomingWrite), reason.ErrorCode, reason.ErrorMessage);
        Context.SetState(new CancelledByUser(reason, logger));
    }

    public override void OnStart()
    {
        logger.StateStarted(nameof(StartIncomingWrite));

        // Do we have any acknowledged options?
        Context.FinishOptionNegotiation(Context.ProposedOptions);
        List<TransferOption> options = Context.NegotiatedOptions.ToOptionList();
        if (options.Count > 0)
        {
            Context.SetState(new SendOptionAcknowledgementForWriteRequest(logger));
        }
        else
        {
            // Start receiving
            Context.SetState(new AcknowledgeWriteRequest(logger));
        }
    }

    public override void OnStateEnter()
    {
        logger.StateEntered(nameof(StartIncomingWrite));
        Context.ProposedOptions = new TransferOptionSet(optionsRequestedByClient);
    }
}
