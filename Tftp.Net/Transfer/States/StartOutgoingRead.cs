// <copyright file="StartOutgoingRead.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class StartOutgoingRead(ILogger logger) : BaseState
{
    private readonly ILogger logger = logger;

    public override void OnCancel(TftpErrorPacket reason)
    {
        logger.StateCancelled(nameof(StartOutgoingRead), reason.ErrorCode, reason.ErrorMessage);
        Context.SetState(new Closed(logger));
    }

    public override void OnStart()
    {
        logger.StateStarted(nameof(StartOutgoingRead));
        Context.SetState(new SendReadRequest(logger));
    }
}
