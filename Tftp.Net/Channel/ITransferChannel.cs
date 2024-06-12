// <copyright file="ITransferChannel.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;

namespace Tftp.Net.Channel;

internal delegate void TftpChannelErrorHandler(ITftpTransferError error);

internal delegate void TftpCommandHandler(ITftpCommand command, IPEndPoint endpoint);

internal interface ITransferChannel : IDisposable
{
    event TftpCommandHandler OnCommandReceived;

    event TftpChannelErrorHandler OnError;

    IPEndPoint RemoteEndpoint { get; set; }

    void Open();

    void Send(ITftpCommand command);
}
