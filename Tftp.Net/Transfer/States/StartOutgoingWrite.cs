// <copyright file="StartOutgoingWrite.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class StartOutgoingWrite(ILogger logger) : BaseState
{
    private readonly ILogger logger = logger;

    public override void OnCancel(TftpErrorPacket reason)
    {
        logger.StateCancelled(nameof(StartOutgoingWrite), reason.ErrorCode, reason.ErrorMessage);
        Context.SetState(new Closed(logger));
    }

    public override void OnStart()
    {
        logger.StateStarted(nameof(StartOutgoingWrite));
        Context.FillOrDisableTransferSizeOption();
        Context.SetState(new SendWriteRequest(logger));
    }
}
