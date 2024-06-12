// <copyright file="UdpChannel_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Tftp.Net.Channel;
using Tftp.Net.Commands;
using Xunit;

namespace Tftp.Net.UnitTests;

public class UdpChannel_Test
{
    private readonly UdpChannel tested = new(new(0));

    [Fact]
    public void DeniesSendingOnClosedConnections()
    {
        var func = () => tested.Send(new Acknowledgement { BlockNumber = 1 });
        func.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void DeniesSendingWhenNoRemoteAddressIsSet()
    {
        tested.Open();
        var func = () => tested.Send(new Acknowledgement { BlockNumber = 1 });
        func.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SendsRealUdpPackets()
    {
        var remote = OpenRemoteUdpClient();

        tested.Open();
        tested.RemoteEndpoint = (IPEndPoint)remote.Client.LocalEndPoint;
        tested.Send(new Acknowledgement { BlockNumber = 1 });

        AssertBytesReceived(remote, TimeSpan.FromMilliseconds(500));
    }

    private static void AssertBytesReceived(UdpClient remote, TimeSpan timeout)
    {
        double msecs = timeout.TotalMilliseconds;
        while (msecs > 0)
        {
            if (remote.Available > 0)
            {
                return;
            }

            Thread.Sleep(50);
            msecs -= 50;
        }

        Assert.Fail("Remote client did not receive anything within " + timeout.TotalMilliseconds + "ms");
    }

    private static UdpClient OpenRemoteUdpClient()
    {
        return new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
    }
}
