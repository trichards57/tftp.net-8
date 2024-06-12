// <copyright file="SendWriteRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net.Transfer.States;

internal class SendWriteRequest : StateWithNetworkTimeout
{
    /// <inheritdoc/>
    public override void OnCancel(TftpErrorPacket reason)
    {
        Context.SetState(new CancelledByUser(reason));
    }

    /// <inheritdoc/>
    public override void OnCommand(ITftpCommand command, System.Net.EndPoint endpoint)
    {
        if (command is OptionAcknowledgement)
        {
            var acknowledged = new TransferOptionSet((command as OptionAcknowledgement).Options);
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
            Context.SetState(new ReceivedError(error));
        }
        else
        {
            base.OnCommand(command, endpoint);
        }
    }

    /// <inheritdoc/>
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        SendRequest();
    }

    private void BeginSendingTo(System.Net.EndPoint endpoint)
    {
        // Switch to the endpoint that we received from the server
        Context.GetConnection().RemoteEndpoint = endpoint;

        // Start sending packets
        Context.SetState(new Sending());
    }

    private void SendRequest()
    {
        var request = new WriteRequest(Context.Filename, Context.TransferMode, Context.ProposedOptions.ToOptionList());
        SendAndRepeat(request);
    }
}
