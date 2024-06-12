// <copyright file="TftpClient.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using System.Net.Sockets;
using Tftp.Net.Channel;
using Tftp.Net.Transfer;

namespace Tftp.Net;

/// <summary>
/// A TFTP client that can connect to a TFTP server.
/// </summary>
public class TftpClient
{
    private const int DefaultServerPort = 69;
    private readonly IPEndPoint remoteAddress;

    /// <summary>
    /// Initializes a new instance of the <see cref="TftpClient"/> class.
    /// </summary>
    /// <param name="remoteAddress">Address of the server that you would like to connect to.</param>
    public TftpClient(IPEndPoint remoteAddress)
    {
        this.remoteAddress = remoteAddress;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TftpClient"/> class.
    /// </summary>
    /// <param name="ip">Address of the server that you want connect to.</param>
    /// <param name="port">Port on the server that you want connect to.</param>
    public TftpClient(IPAddress ip, int port = DefaultServerPort)
        : this(new IPEndPoint(ip, port))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TftpClient"/> class.
    /// </summary>
    /// <param name="host">Hostname or IP to connect to.</param>
    /// <param name="port">Port to connect to.</param>
    public TftpClient(string host, int port = DefaultServerPort)
    {
        IPAddress ip = Array.Find(Dns.GetHostAddresses(host), x => x.AddressFamily == AddressFamily.InterNetwork) ?? throw new ArgumentException("Could not convert '" + host + "' to an IPv4 address.", nameof(host));
        remoteAddress = new IPEndPoint(ip, port);
    }

    /// <summary>
    /// GET a file from the server.
    /// You have to call Start() on the returned ITftpTransfer to start the transfer.
    /// </summary>
    /// <param name="filename">The filename to download.</param>
    /// <returns>The read transfer.</returns>
    public ITftpTransfer Download(string filename)
    {
        ITransferChannel channel = TransferChannelFactory.CreateConnection(remoteAddress);
        return new RemoteReadTransfer(channel, filename);
    }

    /// <summary>
    /// PUT a file from the server.
    /// You have to call Start() on the returned ITftpTransfer to start the transfer.
    /// </summary>
    /// <param name="filename">The filename to download.</param>
    /// <returns>The write transfer.</returns>
    public ITftpTransfer Upload(string filename)
    {
        ITransferChannel channel = TransferChannelFactory.CreateConnection(remoteAddress);
        return new RemoteWriteTransfer(channel, filename);
    }
}
