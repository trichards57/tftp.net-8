// <copyright file="SendReadRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using System.Net;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class SendReadRequest(ILogger logger) : StateWithNetworkTimeout(logger)
{
    private readonly ILogger logger = logger;

    /// <inheritdoc/>
    public override void OnCancel(TftpErrorPacket reason)
    {
        logger.StateCancelled(nameof(SendReadRequest), reason.ErrorCode, reason.ErrorMessage);
        Context.SetState(new CancelledByUser(reason, logger));
    }

    /// <inheritdoc/>
    public override void OnCommand(ITftpCommand command, IPEndPoint endpoint)
    {
        logger.StateCommandReceived(nameof(SendReadRequest));

        if (command is Data || command is OptionAcknowledgement)
        {
            // The server acknowledged our read request.
            // Fix out remote endpoint
            Context.GetConnection().RemoteEndpoint = endpoint;
        }

        if (command is Data)
        {
            if (Context.NegotiatedOptions == null)
            {
                Context.FinishOptionNegotiation(TransferOptionSet.NewEmptySet());
            }

            // Switch to the receiving state...
            var nextState = new Receiving(logger);
            Context.SetState(nextState);

            // ...and let it handle the data packet
            nextState.OnCommand(command, endpoint);
        }
        else if (command is OptionAcknowledgement oAck)
        {
            // Check which options were acknowledged
            Context.FinishOptionNegotiation(new TransferOptionSet(oAck.Options));

            // the server acknowledged our options. Confirm the final options
            SendAndRepeat(new Acknowledgement { BlockNumber = 0 });
        }
        else if (command is Error error)
        {
            Context.SetState(new ReceivedError(error, logger));
        }
        else
        {
            base.OnCommand(command, endpoint);
        }
    }

    public override void OnStateEnter()
    {
        logger.StateEntered(nameof(SendReadRequest));
        base.OnStateEnter();
        SendRequest(); // Send a read request to the server
    }

    private void SendRequest()
    {
        SendAndRepeat(new ReadRequest(Context.Filename, Context.TransferMode, Context.ProposedOptions.ToOptionList()));
    }
}
