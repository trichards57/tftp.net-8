// <copyright file="SendWriteRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using System.Net;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class SendWriteRequest(ILogger logger) : StateWithNetworkTimeout(logger)
{
    private readonly ILogger logger = logger;

    /// <inheritdoc/>
    public override void OnCancel(TftpErrorPacket reason)
    {
        logger.StateCancelled(nameof(SendWriteRequest), reason.ErrorCode, reason.ErrorMessage);
        Context.SetState(new CancelledByUser(reason, logger));
    }

    /// <inheritdoc/>
    public override void OnCommand(ITftpCommand command, IPEndPoint endpoint)
    {
        logger.StateCommandReceived(nameof(SendWriteRequest));
        if (command is OptionAcknowledgement oAck)
        {
            var acknowledged = new TransferOptionSet(oAck.Options);
            Context.FinishOptionNegotiation(acknowledged);
            BeginSendingTo(endpoint);
        }
        else if (command is Acknowledgement ack && ack.BlockNumber == 0)
        {
            Context.FinishOptionNegotiation(TransferOptionSet.NewEmptySet());
            BeginSendingTo(endpoint);
        }
        else if (command is Error error)
        {
            // The server denied our request
            Context.SetState(new ReceivedError(error, logger));
        }
        else
        {
            base.OnCommand(command, endpoint);
        }
    }

    /// <inheritdoc/>
    public override void OnStateEnter()
    {
        logger.StateEntered(nameof(SendWriteRequest));
        base.OnStateEnter();
        SendRequest();
    }

    private void BeginSendingTo(IPEndPoint endpoint)
    {
        // Switch to the endpoint that we received from the server
        Context.GetConnection().RemoteEndpoint = endpoint;

        // Start sending packets
        Context.SetState(new Sending(logger));
    }

    private void SendRequest()
    {
        var request = new WriteRequest(Context.Filename, Context.TransferMode, Context.ProposedOptions.ToOptionList());
        SendAndRepeat(request);
    }
}
