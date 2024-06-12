// <copyright file="AcknowledgeWriteRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net.Transfer.States;

internal class AcknowledgeWriteRequest : StateThatExpectsMessagesFromDefaultEndPoint
{
    public override void OnCancel(TftpErrorPacket reason)
    {
        Context.SetState(new CancelledByUser(reason));
    }

    public override void OnData(Data command)
    {
        var nextState = new Receiving();
        Context.SetState(nextState);
        nextState.OnCommand(command, Context.GetConnection().RemoteEndpoint);
    }

    public override void OnError(Error command)
    {
        Context.SetState(new ReceivedError(command));
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        SendAndRepeat(new Acknowledgement(0));
    }
}
