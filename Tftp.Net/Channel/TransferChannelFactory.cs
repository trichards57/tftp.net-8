// <copyright file="TransferChannelFactory.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using System.Net.Sockets;

namespace Tftp.Net.Channel;

internal static class TransferChannelFactory
{
    public static ITransferChannel CreateConnection(EndPoint remoteAddress)
    {
        if (remoteAddress is IPEndPoint address)
        {
            return CreateConnectionUdp(address);
        }

        throw new NotSupportedException("Unsupported endpoint type.");
    }

    public static ITransferChannel CreateServer(EndPoint localAddress)
    {
        if (localAddress is IPEndPoint address)
        {
            return CreateServerUdp(address);
        }

        throw new NotSupportedException("Unsupported endpoint type.");
    }

    private static UdpChannel CreateConnectionUdp(IPEndPoint remoteAddress)
    {
        return new UdpChannel(new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
        {
            RemoteEndpoint = remoteAddress,
        };
    }

    private static UdpChannel CreateServerUdp(IPEndPoint localAddress)
    {
        return new UdpChannel(new UdpClient(localAddress));
    }
}
