// <copyright file="ChannelStub.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Net;
using Tftp.Net.Channel;

namespace Tftp.Net.UnitTests;

internal sealed class ChannelStub : ITransferChannel
{
    public ChannelStub()
    {
        RemoteEndpoint = new IPEndPoint(IPAddress.Loopback, 69);
    }

    public event TftpCommandHandler OnCommandReceived;

    public event TftpChannelErrorHandler OnError;

    public EndPoint RemoteEndpoint { get; set; }

    public List<ITftpCommand> SentCommands { get; } = [];

    public void Dispose()
    {
    }

    public void Open()
    {
    }

    public void RaiseCommandReceived(ITftpCommand command, EndPoint endpoint) => OnCommandReceived?.Invoke(command, endpoint);

    public void RaiseOnError(ITftpTransferError error) => OnError?.Invoke(error);

    public void Send(ITftpCommand command) => SentCommands.Add(command);
}
