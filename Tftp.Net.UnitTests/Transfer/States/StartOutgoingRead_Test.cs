// <copyright file="StartOutgoingRead_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.IO;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class StartOutgoingRead_Test
{
    private TransferStub transfer;

    [TearDown]
    public void Teardown()
    {
        transfer.Dispose();
    }

    [SetUp]
    public void Setup()
    {
        transfer = new TransferStub();
        transfer.SetState(new StartOutgoingRead(NullLogger.Instance));
    }

    [Test]
    public void CanCancel()
    {
        transfer.Cancel(TftpErrorPacket.IllegalOperation);
        Assert.That(transfer.State, Is.InstanceOf<Closed>());
    }

    [Test]
    public void IgnoresCommands()
    {
        transfer.OnCommand(new Error { ErrorCode = 5, Message = "Hallo Welt" });
        Assert.That(transfer.State, Is.InstanceOf<StartOutgoingRead>());
    }

    [Test]
    public void CanStart()
    {
        transfer.Start(new MemoryStream());
        Assert.Multiple(() =>
        {
            Assert.That(transfer.CommandWasSent(typeof(ReadRequest)), Is.True);
            Assert.That(transfer.State, Is.InstanceOf<SendReadRequest>());
        });
    }
}
