// <copyright file="TftpServer.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Tftp.Net.Channel;
using Tftp.Net.Transfer;
using Tftp.Net.Transfer.StateMachine;

namespace Tftp.Net;

public delegate void TftpServerErrorHandler(ITftpTransferError error);

public delegate void TftpServerEventHandler(ITftpTransfer transfer, EndPoint client);

/// <summary>
/// A simple TFTP server class.
/// </summary>
/// <remarks>
/// The server's socket is closed when this item is disposed.
/// </remarks>
public sealed class TftpServer : IDisposable
{
    /// <summary>
    /// The default port for TFTP servers.
    /// </summary>
    public const int DefaultServerPort = 69;

    /// <summary>
    /// Server port that we're listening on.
    /// </summary>
    private readonly ITransferChannel serverSocket;

    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TftpServer"/> class.
    /// </summary>
    /// <param name="localEndpoint">The local endpoint to bind to.</param>
    public TftpServer(IPEndPoint localEndpoint, ILogger logger = null)
    {
        ArgumentNullException.ThrowIfNull(localEndpoint);

        this.logger = logger ?? NullLogger.Instance;

        serverSocket = new UdpChannel(new UdpClient(localEndpoint));
        serverSocket.OnCommandReceived += new TftpCommandHandler(ServerSocket_OnCommandReceived);
        serverSocket.OnError += new TftpChannelErrorHandler(ServerSocket_OnError);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TftpServer"/> class.
    /// </summary>
    /// <param name="localAddress">The local address to bind to.</param>
    /// <param name="port">The UDP port to listen to.</param>
    public TftpServer(IPAddress localAddress, int port = DefaultServerPort, ILogger logger = null)
            : this(new IPEndPoint(localAddress, port), logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TftpServer"/> class.
    /// </summary>
    /// <param name="port">The UDP port to listen to.</param>
    public TftpServer(int port = DefaultServerPort, ILogger logger = null)
            : this(new IPEndPoint(IPAddress.Any, port), logger)
    {
    }

    /// <summary>
    /// Fired when the server encounters an error (for example, a non-parseable request)
    /// </summary>
    public event TftpServerErrorHandler OnError;

    /// <summary>
    /// Fired when the server receives a new read request.
    /// </summary>
    public event TftpServerEventHandler OnReadRequest;

    /// <summary>
    /// Fired when the server receives a new write request.
    /// </summary>
    public event TftpServerEventHandler OnWriteRequest;

    /// <inheritdoc/>
    public void Dispose() => serverSocket.Dispose();

    /// <summary>
    /// Start accepting incoming connections.
    /// </summary>
    public void Start() => serverSocket.Open();

    private void RaiseOnError(ITftpTransferError error) => OnError?.Invoke(error);

    private void RaiseOnReadRequest(LocalReadTransfer transfer, EndPoint client)
    {
        if (OnReadRequest != null)
        {
            OnReadRequest(transfer, client);
        }
        else
        {
            transfer.Cancel(new TftpErrorPacket(0, "Server did not provide a OnReadRequest handler."));
        }
    }

    private void RaiseOnWriteRequest(LocalWriteTransfer transfer, EndPoint client)
    {
        if (OnWriteRequest != null)
        {
            OnWriteRequest(transfer, client);
        }
        else
        {
            transfer.Cancel(new TftpErrorPacket(0, "Server did not provide a OnWriteRequest handler."));
        }
    }

    private void ServerSocket_OnCommandReceived(ITftpCommand command, IPEndPoint endpoint)
    {
        // Ignore all other commands
        if (command is not ReadOrWriteRequest request)
        {
            return;
        }

        // Open a connection to the client
        ITransferChannel channel = new UdpChannel(new UdpClient(new IPEndPoint(IPAddress.Any, 0))) { RemoteEndpoint = endpoint };

        if (command is ReadRequest)
        {
            RaiseOnReadRequest(new LocalReadTransfer(request.Options.ToList(), channel, request.Filename, TimeProvider.System), endpoint);
        }
        else if (command is WriteRequest)
        {
            RaiseOnWriteRequest(new LocalWriteTransfer(channel, request.Filename, request.Options, logger), endpoint);
        }
        else
        {
            throw new InvalidOperationException($"Unexpected TFTP transfer request: {command}");
        }
    }

    private void ServerSocket_OnError(ITftpTransferError error) => RaiseOnError(error);
}
