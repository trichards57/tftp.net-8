// <copyright file="LoggingStateDecorator.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Net;
using Tftp.Net.Transfer;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Trace;

internal class LoggingStateDecorator(ITransferState decoratee, TftpTransfer transfer) : ITransferState
{
    private readonly ITransferState decoratee = decoratee;
    private readonly TftpTransfer transfer = transfer;

    public TftpTransfer Context
    {
        get => decoratee.Context;
        set => decoratee.Context = value;
    }

    public string GetStateName()
    {
        return "[" + decoratee.GetType().Name + "]";
    }

    public void OnCancel(TftpErrorPacket reason)
    {
        TftpTrace.Trace(GetStateName() + " OnCancel: " + reason, transfer);
        decoratee.OnCancel(reason);
    }

    public void OnCommand(ITftpCommand command, IPEndPoint endpoint)
    {
        TftpTrace.Trace(GetStateName() + " OnCommand: " + command + " from " + endpoint, transfer);
        decoratee.OnCommand(command, endpoint);
    }

    public void OnStart()
    {
        TftpTrace.Trace(GetStateName() + " OnStart", transfer);
        decoratee.OnStart();
    }

    public void OnStateEnter()
    {
        TftpTrace.Trace(GetStateName() + " OnStateEnter", transfer);
        decoratee.OnStateEnter();
    }

    public void OnTimer()
    {
        decoratee.OnTimer();
    }
}
