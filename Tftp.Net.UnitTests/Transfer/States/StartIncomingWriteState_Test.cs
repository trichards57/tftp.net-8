// <copyright file="StartIncomingWriteState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests.Transfer.States;

[TestFixture]
internal class StartIncomingWriteState_Test
{
    private TransferStub transfer;

    [Test]
    public void CanCancel()
    {
        transfer.Cancel(TftpErrorPacket.IllegalOperation);
        Assert.Multiple(() =>
        {
            Assert.That(transfer.CommandWasSent(typeof(Error)), Is.True);
            Assert.That(transfer.State, Is.InstanceOf<Closed>());
        });
    }

    [Test]
    public void CanStartWithOptions()
    {
        transfer.SetState(new StartIncomingWrite([new TransferOption("blksize", "999")], NullLogger.Instance));
        Assert.That(transfer.BlockSize, Is.EqualTo(999));
        transfer.Start(new MemoryStream(new byte[50000]));
        OptionAcknowledgement cmd = (OptionAcknowledgement)transfer.SentCommands[^1];
        cmd.Options.Contains(new TransferOption("blksize", "999"));
        Assert.That(transfer.State, Is.InstanceOf<SendOptionAcknowledgementForWriteRequest>());
    }

    [Test]
    public void CanStartWithoutOptions()
    {
        transfer.Start(new MemoryStream(new byte[50000]));

        Assert.Multiple(() =>
        {
            Assert.That(transfer.CommandWasSent(typeof(Acknowledgement)), Is.True);
            Assert.That(transfer.State, Is.InstanceOf<AcknowledgeWriteRequest>());
        });
    }

    [Test]
    public void IgnoresCommands()
    {
        transfer.OnCommand(new Error { ErrorCode = 5, Message = "Hallo Welt" });
        Assert.That(transfer.State, Is.InstanceOf<StartIncomingWrite>());
    }

    [SetUp]
    public void Setup()
    {
        transfer = new TransferStub();
        transfer.SetState(new StartIncomingWrite([], NullLogger.Instance));
    }

    [TearDown]
    public void Teardown()
    {
        transfer.Dispose();
    }
}
