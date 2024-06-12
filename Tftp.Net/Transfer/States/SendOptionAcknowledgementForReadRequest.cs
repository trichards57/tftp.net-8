// <copyright file="SendOptionAcknowledgementForReadRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net.Transfer.States;

internal class SendOptionAcknowledgementForReadRequest : SendOptionAcknowledgementBase
{
    public override void OnAcknowledgement(Acknowledgement command)
    {
        if (command.BlockNumber == 0)
        {
            // We received an OACK, so let's get going ;)
            Context.SetState(new Sending());
        }
    }
}
