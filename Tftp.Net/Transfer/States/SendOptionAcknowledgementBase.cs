// <copyright file="SendOptionAcknowledgementBase.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer;

internal class SendOptionAcknowledgementBase : StateThatExpectsMessagesFromDefaultEndPoint
{
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        SendAndRepeat(new OptionAcknowledgement(Context.NegotiatedOptions.ToOptionList()));
    }

    public override void OnError(Error command)
    {
        Context.SetState(new ReceivedError(command));
    }

    public override void OnCancel(TftpErrorPacket reason)
    {
        Context.SetState(new CancelledByUser(reason));
    }
}
