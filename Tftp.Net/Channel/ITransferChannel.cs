// <copyright file="ITransferChannel.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;

namespace Tftp.Net.Channel;

internal delegate void TftpChannelErrorHandler(ITftpTransferError error);

internal delegate void TftpCommandHandler(ITftpCommand command, IPEndPoint endpoint);

/// <summary>
/// Represents a transfer channel that can send and receive TFTP commands.
/// </summary>
internal interface ITransferChannel : IDisposable
{
    /// <summary>
    /// Event raised when a command is received.
    /// </summary>
    event TftpCommandHandler OnCommandReceived;

    /// <summary>
    /// Event raised when an error is received.
    /// </summary>
    event TftpChannelErrorHandler OnError;

    /// <summary>
    /// Gets or sets the remote endpoint for the channel.
    /// </summary>
    IPEndPoint RemoteEndpoint { get; set; }

    /// <summary>
    /// Opens the channel.
    /// </summary>
    void Open();

    /// <summary>
    /// Sends the provided TFTP command.
    /// </summary>
    /// <param name="command">The command to send.</param>
    void Send(ITftpCommand command);

    /// <summary>
    /// Closes the channel.
    /// </summary>
    void Close();
}
