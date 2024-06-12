// <copyright file="StartOutgoingWrite_Test.cs" company="Tony Richards">
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
internal class StartOutgoingWrite_Test
{
    private TransferStub transfer;

    [Test]
    public void CanCancel()
    {
        transfer.Cancel(TftpErrorPacket.IllegalOperation);
        Assert.That(transfer.State, Is.InstanceOf<Closed>());
    }

    [Test]
    public void CanStart()
    {
        transfer.Start(new MemoryStream());
        Assert.That(transfer.State, Is.InstanceOf<SendWriteRequest>());
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
        transfer.OnCommand(new Error { ErrorCode  = 5, Message = "Hallo Welt" });
        Assert.That(transfer.State, Is.InstanceOf<StartOutgoingWrite>());
    }

    [SetUp]
    public void Setup()
    {
        transfer = new TransferStub();
        transfer.SetState(new StartOutgoingWrite(NullLogger.Instance));
    }

    [TearDown]
    public void Teardown()
    {
        transfer.Dispose();
    }

    private bool WasTransferSizeOptionRequested()
    {
        WriteRequest wrq = (WriteRequest)transfer.SentCommands[^1];
        return wrq.Options.Any(x => x.Name == "tsize");
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
