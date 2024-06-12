// <copyright file="BaseState.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Net;

namespace Tftp.Net.Transfer.States;

internal abstract class BaseState : ITransferState
{
    public TftpTransfer Context { get; set; }

    public virtual void OnCancel(TftpErrorPacket reason)
    {
    }

    public virtual void OnCommand(ITftpCommand command, IPEndPoint endpoint)
    {
    }

    public virtual void OnStart()
    {
    }

    public virtual void OnStateEnter()
    {
        // no-op
    }

    public virtual void OnTimer()
    {
        // Ignore timer events
    }
}
