// <copyright file="StartIncomingReadState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class StartIncomingReadState_Test
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
        // Simulate that we got a request for a option
        transfer.SetState(new StartIncomingRead([new TransferOption("blksize", "999")]));
        Assert.That(transfer.BlockSize, Is.EqualTo(999));
        transfer.Start(new MemoryStream(new byte[50000]));
        Assert.That(transfer.State, Is.InstanceOf<SendOptionAcknowledgementForReadRequest>());
        OptionAcknowledgement cmd = (OptionAcknowledgement)transfer.SentCommands[^1];
        Assert.That(cmd.Options, Contains.Item(new TransferOption("blksize", "999")));
    }

    [Test]
    public void CanStartWithoutOptions()
    {
        transfer.SetState(new StartIncomingRead([]));
        transfer.Start(new MemoryStream(new byte[50000]));
        Assert.That(transfer.State, Is.InstanceOf<Sending>());
    }

    [Test]
    public void DoesNotFillTransferSizeWhenNotAvailable()
    {
        transfer.Start(new StreamThatThrowsExceptionWhenReadingLength());
        Assert.That(WasTransferSizeOptionRequested(), Is.False);
    }

    [Test]
    public void FillsTransferSizeFromStreamIfPossible()
    {
        transfer.Start(new MemoryStream([1]));
        Assert.That(WasTransferSizeOptionRequested(), Is.True);
    }

    [Test]
    public void FillsTransferSizeIfPossible()
    {
        transfer.ExpectedSize = 123;
        transfer.Start(new StreamThatThrowsExceptionWhenReadingLength());
        Assert.That(WasTransferSizeOptionRequested(), Is.True);
    }

    [Test]
    public void IgnoresCommands()
    {
        transfer.OnCommand(new Error(5, "Hallo Welt"));
        Assert.That(transfer.State, Is.InstanceOf<StartIncomingRead>());
    }

    [SetUp]
    public void Setup()
    {
        transfer = new TransferStub();
        transfer.SetState(new StartIncomingRead([new TransferOption("tsize", "0")]));
    }

    [TearDown]
    public void Teardown()
    {
        transfer.Dispose();
    }

    private bool WasTransferSizeOptionRequested()
    {
        return transfer.SentCommands[^1] is OptionAcknowledgement oAck && oAck.Options.Any(x => x.Name == "tsize");
    }

    private class StreamThatThrowsExceptionWhenReadingLength : MemoryStream
    {
        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }
    }
}
