// <copyright file="StartOutgoingRead.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net.Transfer.States;

internal class StartOutgoingRead : BaseState
{
    public override void OnCancel(TftpErrorPacket reason)
    {
        Context.SetState(new Closed());
    }

    public override void OnStart()
    {
        Context.SetState(new SendReadRequest());
    }
}
