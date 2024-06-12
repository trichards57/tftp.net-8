// <copyright file="CancelledByUser.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net.Transfer.States;

internal class CancelledByUser(TftpErrorPacket reason) : BaseState
{
    private readonly TftpErrorPacket reason = reason;

    public override void OnStateEnter()
    {
        Error command = new(reason.ErrorCode, reason.ErrorMessage);
        Context.GetConnection().Send(command);
        Context.SetState(new Closed());
    }
}
