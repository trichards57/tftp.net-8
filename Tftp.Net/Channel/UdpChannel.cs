// <copyright file="UdpChannel.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Tftp.Net.Channel;

internal sealed class UdpChannel(UdpClient client) : ITransferChannel
{
    private readonly object lockObject = new();
    private UdpClient client = client;
    private IPEndPoint endpoint = null;

    public event TftpCommandHandler OnCommandReceived;

    public event TftpChannelErrorHandler OnError;

    public EndPoint RemoteEndpoint
    {
        get => endpoint;
        set
        {
            ObjectDisposedException.ThrowIf(client == null, this);

            if (value is not IPEndPoint)
            {
                throw new NotSupportedException("UdpChannel can only connect to IPEndPoints.");
            }

            endpoint = (IPEndPoint)value;
        }
    }

    public void Dispose()
    {
        lock (lockObject)
        {
            if (client == null)
            {
                return;
            }

            client.Close();
            client = null;
        }
    }

    public void Open()
    {
        ObjectDisposedException.ThrowIf(client == null, this);

        client.BeginReceive(UdpReceivedCallback, null);
    }

    public void Send(ITftpCommand command)
    {
        ObjectDisposedException.ThrowIf(client == null, this);

        if (endpoint == null)
        {
            throw new InvalidOperationException("RemoteEndpoint needs to be set before you can send TFTP commands.");
        }

        using var stream = new MemoryStream();
        CommandSerializer.Serialize(command, stream);
        byte[] data = stream.GetBuffer();
        client.Send(data, (int)stream.Length, endpoint);
    }

    private void RaiseOnCommand(ITftpCommand command, IPEndPoint endpoint)
    {
        OnCommandReceived?.Invoke(command, endpoint);
    }

    private void RaiseOnError(ITftpTransferError error)
    {
        OnError?.Invoke(error);
    }

    private void UdpReceivedCallback(IAsyncResult result)
    {
        var ep = new IPEndPoint(0, 0);
        ITftpCommand command = null;

        try
        {
            byte[] data = null;

            lock (lockObject)
            {
                if (client == null)
                {
                    return;
                }

                data = client.EndReceive(result, ref ep);
            }

            command = CommandParser.Parse(data);
        }
        catch (SocketException e)
        {
            // Handle receive error
            RaiseOnError(new NetworkError(e));
        }
        catch (TftpParserException e2)
        {
            // Handle parser error
            RaiseOnError(new NetworkError(e2));
        }

        if (command != null)
        {
            RaiseOnCommand(command, ep);
        }

        lock (lockObject)
        {
            client?.BeginReceive(UdpReceivedCallback, null);
        }
    }
}
