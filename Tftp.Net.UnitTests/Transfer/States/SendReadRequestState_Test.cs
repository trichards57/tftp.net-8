// <copyright file="SendReadRequestState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.IO;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class SendReadRequestState_Test
{
    private MemoryStream ms;
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
    public void HandlesData()
    {
        transfer.OnCommand(new Data { BlockNumber = 1, Bytes = new byte[10] });
        Assert.Multiple(() =>
        {
            Assert.That(transfer.CommandWasSent(typeof(Acknowledgement)), Is.True);
            Assert.That(transfer.State, Is.InstanceOf<Closed>());
            Assert.That(ms.Length, Is.EqualTo(10));
        });
    }

    [Test]
    public void HandlesError()
    {
        bool onErrorWasCalled = false;
        transfer.OnError += (t, error) => onErrorWasCalled = true;

        Assert.That(onErrorWasCalled, Is.False);
        transfer.OnCommand(new Error { ErrorCode = 123, Message = "Test Error" });
        Assert.Multiple(() =>
        {
            Assert.That(onErrorWasCalled, Is.True);

            Assert.That(transfer.State, Is.InstanceOf<Closed>());
        });
    }

    [Test]
    public void HandlesMissingOptionAcknowledgement()
    {
        transfer.BlockSize = 999;
        transfer.OnCommand(new Data { BlockNumber = 1, Bytes = new byte[10] });
        Assert.That(transfer.BlockSize, Is.EqualTo(512));
    }

    [Test]
    public void HandlesOptionAcknowledgement()
    {
        transfer.BlockSize = 999;
        transfer.OnCommand(new OptionAcknowledgement { Options = [new TransferOption("blksize", "999")] });
        Assert.Multiple(() =>
        {
            Assert.That(transfer.CommandWasSent(typeof(Acknowledgement)), Is.True);
            Assert.That(transfer.BlockSize, Is.EqualTo(999));
        });
    }

    [Test]
    public void ResendsRequest()
    {
        var transferWithLowTimeout = new TransferStub(new MemoryStream())
        {
            RetryTimeout = new TimeSpan(0),
        };
        transferWithLowTimeout.SetState(new SendReadRequest(NullLogger.Instance));

        Assert.That(transferWithLowTimeout.CommandWasSent(typeof(ReadRequest)), Is.True);
        transferWithLowTimeout.SentCommands.Clear();

        transferWithLowTimeout.OnTimer();
        Assert.That(transferWithLowTimeout.CommandWasSent(typeof(ReadRequest)), Is.True);
    }

    [Test]
    public void SendsRequest()
    {
        Assert.That(transfer.CommandWasSent(typeof(ReadRequest)), Is.True);
    }

    [SetUp]
    public void Setup()
    {
        ms = new MemoryStream();
        transfer = new TransferStub(ms);
        transfer.SetState(new SendReadRequest(NullLogger.Instance));
    }

    [TearDown]
    public void Teardown()
    {
        transfer.Dispose();
    }

    [Test]
    public void TimeoutWhenNoAnswerIsReceivedAndRetryCountIsExceeded()
    {
        var transferWithLowTimeout = new TransferStub(new MemoryStream(new byte[5000]))
        {
            RetryTimeout = new TimeSpan(0),
            RetryCount = 1,
        };
        transferWithLowTimeout.SetState(new SendReadRequest(NullLogger.Instance));

        transferWithLowTimeout.OnTimer();
        Assert.That(transferWithLowTimeout.HadNetworkTimeout, Is.False);
        transferWithLowTimeout.OnTimer();
        Assert.That(transferWithLowTimeout.HadNetworkTimeout, Is.True);
    }
}
