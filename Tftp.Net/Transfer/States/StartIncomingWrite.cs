﻿// <copyright file="StartIncomingWrite.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Tftp.Net.Transfer.States;

internal class StartIncomingWrite(IEnumerable<TransferOption> optionsRequestedByClient) : BaseState
{
    private readonly IEnumerable<TransferOption> optionsRequestedByClient = optionsRequestedByClient;

    public override void OnCancel(TftpErrorPacket reason)
    {
        Context.SetState(new CancelledByUser(reason));
    }

    public override void OnStart()
    {
        // Do we have any acknowledged options?
        Context.FinishOptionNegotiation(Context.ProposedOptions);
        List<TransferOption> options = Context.NegotiatedOptions.ToOptionList();
        if (options.Count > 0)
        {
            Context.SetState(new SendOptionAcknowledgementForWriteRequest());
        }
        else
        {
            // Start receiving
            Context.SetState(new AcknowledgeWriteRequest());
        }
    }

    public override void OnStateEnter()
    {
        Context.ProposedOptions = new TransferOptionSet(optionsRequestedByClient);
    }
}
