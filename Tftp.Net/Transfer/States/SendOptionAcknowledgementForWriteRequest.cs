// <copyright file="SendOptionAcknowledgementForWriteRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net.Transfer.States;

internal class SendOptionAcknowledgementForWriteRequest : SendOptionAcknowledgementBase
{
    public override void OnData(Data command)
    {
        if (command.BlockNumber == 1)
        {
            // The client confirmed the options, so let's start receiving
            var nextState = new Receiving();
            Context.SetState(nextState);
            nextState.OnCommand(command, Context.GetConnection().RemoteEndpoint);
        }
    }
}
