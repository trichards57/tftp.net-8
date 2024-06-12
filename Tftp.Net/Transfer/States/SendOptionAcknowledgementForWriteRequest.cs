// <copyright file="SendOptionAcknowledgementForWriteRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class SendOptionAcknowledgementForWriteRequest(ILogger logger) : SendOptionAcknowledgementBase(logger)
{
    private readonly ILogger logger = logger;

    public override void OnData(Data command)
    {
        logger.StateDataReceived(nameof(SendOptionAcknowledgementForWriteRequest));

        if (command.BlockNumber == 1)
        {
            // The client confirmed the options, so let's start receiving
            var nextState = new Receiving(logger);
            Context.SetState(nextState);
            nextState.OnCommand(command, Context.GetConnection().RemoteEndpoint);
        }
    }
}
