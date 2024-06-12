// <copyright file="UdpChannel_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Tftp.Net.Channel;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class UdpChannel_Test
{
    private UdpChannel tested;

    [Test]
    public void DeniesSendingOnClosedConnections()
    {
        Assert.Throws<InvalidOperationException>(() => tested.Send(new Acknowledgement(1)));
    }

    [Test]
    public void DeniesSendingWhenNoRemoteAddressIsSet()
    {
        tested.Open();
        Assert.Throws<InvalidOperationException>(() => tested.Send(new Acknowledgement(1)));
    }

    [Test]
    public void SendsRealUdpPackets()
    {
        var remote = OpenRemoteUdpClient();

        tested.Open();
        tested.RemoteEndpoint = (IPEndPoint)remote.Client.LocalEndPoint;
        tested.Send(new Acknowledgement(1));

        AssertBytesReceived(remote, TimeSpan.FromMilliseconds(500));
    }

    [SetUp]
    public void Setup()
    {
        tested = new UdpChannel(new UdpClient(0));
    }

    [TearDown]
    public void Teardown()
    {
        tested.Dispose();
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
