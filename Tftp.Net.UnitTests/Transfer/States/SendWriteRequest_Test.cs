// <copyright file="SendWriteRequest_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using System;
using System.IO;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class SendWriteRequest_Test
{
    private TransferStub transfer;

    [SetUp]
    public void Setup()
    {
        transfer = new TransferStub(new MemoryStream(new byte[5000]));
        transfer.SetState(new SendWriteRequest());
    }

    [Test]
    public void CanCancel()
    {
        transfer.Cancel(TftpErrorPacket.IllegalOperation);
        Assert.Multiple(() =>
        {
            Assert.That(transfer.State, Is.InstanceOf<Closed>());
            Assert.That(transfer.CommandWasSent(typeof(Error)), Is.True);
        });
    }

    [Test]
    public void SendsWriteRequest()
    {
        var tx = new TransferStub(new MemoryStream(new byte[5000]));
        tx.SetState(new SendWriteRequest());
        Assert.That(tx.CommandWasSent(typeof(WriteRequest)), Is.True);
    }

    [Test]
    public void HandlesAcknowledgement()
    {
        transfer.OnCommand(new Acknowledgement(0));
        Assert.That(transfer.State, Is.InstanceOf<Sending>());
    }

    [Test]
    public void IgnoresWrongAcknowledgement()
    {
        transfer.OnCommand(new Acknowledgement(5));
        Assert.That(transfer.State, Is.InstanceOf<SendWriteRequest>());
    }

    [Test]
    public void HandlesOptionAcknowledgement()
    {
        transfer.BlockSize = 999;
        transfer.OnCommand(new OptionAcknowledgement([new TransferOption("blksize", "800")]));
        Assert.That(transfer.BlockSize, Is.EqualTo(800));
    }

    [Test]
    public void HandlesMissingOptionAcknowledgement()
    {
        transfer.BlockSize = 999;
        transfer.OnCommand(new Acknowledgement(0));
        Assert.That(transfer.BlockSize, Is.EqualTo(512));
    }

    [Test]
    public void HandlesError()
    {
        bool onErrorWasCalled = false;
        transfer.OnError += (t, error) => { onErrorWasCalled = true; };

        Assert.That(onErrorWasCalled, Is.False);
        transfer.OnCommand(new Error(123, "Test Error"));
        Assert.Multiple(() =>
        {
            Assert.That(onErrorWasCalled, Is.True);

            Assert.That(transfer.State, Is.InstanceOf<Closed>());
        });
    }

    [Test]
    public void ResendsRequest()
    {
        var transferWithLowTimeout = new TransferStub(new MemoryStream())
        {
            RetryTimeout = new TimeSpan(0),
        };
        transferWithLowTimeout.SetState(new SendWriteRequest());

        Assert.That(transferWithLowTimeout.CommandWasSent(typeof(WriteRequest)), Is.True);
        transferWithLowTimeout.SentCommands.Clear();

        transferWithLowTimeout.OnTimer();
        Assert.That(transferWithLowTimeout.CommandWasSent(typeof(WriteRequest)), Is.True);
    }

    [Test]
    public void TimeoutWhenNoAnswerIsReceivedAndRetryCountIsExceeded()
    {
        var transferWithLowTimeout = new TransferStub(new MemoryStream(new byte[5000]))
        {
            RetryTimeout = new TimeSpan(0),
            RetryCount = 1,
        };
        transferWithLowTimeout.SetState(new SendWriteRequest());

        transferWithLowTimeout.OnTimer();
        Assert.That(transferWithLowTimeout.HadNetworkTimeout, Is.False);
        transferWithLowTimeout.OnTimer();
        Assert.That(transferWithLowTimeout.HadNetworkTimeout, Is.True);
    }

    [TearDown]
    public void Teardown()
    {
        transfer.Dispose();
    }
}
