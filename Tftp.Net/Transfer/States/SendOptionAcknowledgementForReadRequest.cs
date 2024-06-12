// <copyright file="SendOptionAcknowledgementForReadRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class SendOptionAcknowledgementForReadRequest(ILogger logger) : SendOptionAcknowledgementBase(logger)
{
    private readonly ILogger logger = logger;

    public override void OnAcknowledgement(Acknowledgement command)
    {
        logger.StateAcknowledged(nameof(SendOptionAcknowledgementForReadRequest), command.BlockNumber);
        if (command.BlockNumber == 0)
        {
            Context.SetState(new Sending(logger));
        }
    }
}
