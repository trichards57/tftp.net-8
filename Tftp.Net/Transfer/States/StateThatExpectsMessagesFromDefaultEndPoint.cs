// <copyright file="StateThatExpectsMessagesFromDefaultEndPoint.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using System;
using System.Net;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class StateThatExpectsMessagesFromDefaultEndPoint(ILogger logger) : StateWithNetworkTimeout(logger), ITftpCommandVisitor
{
    private readonly ILogger logger = logger;

    public virtual void OnAcknowledgement(Acknowledgement command)
    {
        throw new NotImplementedException();
    }

    public override void OnCommand(ITftpCommand command, IPEndPoint endpoint)
    {
        logger.StateCommandReceived(nameof(StateThatExpectsMessagesFromDefaultEndPoint));

        if (!endpoint.Equals(Context.GetConnection().RemoteEndpoint))
        {
            throw new InvalidOperationException("Received message from illegal endpoint. Actual: " + endpoint + ". Expected: " + Context.GetConnection().RemoteEndpoint);
        }

        command.Visit(this);
    }

    public virtual void OnData(Data command)
    {
        throw new NotImplementedException();
    }

    public virtual void OnError(Error command)
    {
        throw new NotImplementedException();
    }

    public virtual void OnOptionAcknowledgement(OptionAcknowledgement command)
    {
        throw new NotImplementedException();
    }

    public virtual void OnReadRequest(ReadRequest command)
    {
        throw new NotImplementedException();
    }

    public virtual void OnWriteRequest(WriteRequest command)
    {
        throw new NotImplementedException();
    }
}
