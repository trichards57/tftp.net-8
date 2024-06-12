// <copyright file="CancelledByUser.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class CancelledByUser(TftpErrorPacket reason, ILogger logger) : BaseState
{
    private readonly ILogger logger = logger;
    private readonly TftpErrorPacket reason = reason;

    public override void OnStateEnter()
    {
        logger.StateEntered(nameof(CancelledByUser));
        Error command = new() { ErrorCode = reason.ErrorCode, Message = reason.ErrorMessage };
        Context.GetConnection().Send(command);
        Context.SetState(new Closed(logger));
    }
}
