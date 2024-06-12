// <copyright file="ReceivingState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.IO;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests.Transfer.States;

[TestFixture]
internal class ReceivingState_Test
{
    private MemoryStream ms;
    private TransferStub transfer;

    [Test]
    public void BlockCounterWrapsAroundToOne()
    {
        transfer.BlockCounterWrapping = BlockCounterWrapAround.ToOne;
        TransferUntilBlockCounterWrapIsAboutToWrap();

        transfer.OnCommand(new Data { BlockNumber = 1, Bytes = new byte[1] });

        Assert.That(((Acknowledgement)transfer.SentCommands[^1]).BlockNumber, Is.EqualTo(1));
    }

    [Test]
    public void BlockCounterWrapsAroundToZero()
    {
        TransferUntilBlockCounterWrapIsAboutToWrap();

        transfer.OnCommand(new Data { BlockNumber = 0, Bytes = new byte[1] });

        Assert.That(((Acknowledgement)transfer.SentCommands[^1]).BlockNumber, Is.EqualTo(0));
    }

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
    public void HandlesError()
    {
        var onErrorWasCalled = false;
        transfer.OnError += (t, error) => { onErrorWasCalled = true; };

        Assert.That(onErrorWasCalled, Is.False);
        transfer.OnCommand(new Error { ErrorCode = 123, Message = "Test Error" });
        Assert.Multiple(() =>
        {
            Assert.That(onErrorWasCalled, Is.True);
            Assert.That(transfer.State, Is.InstanceOf<Closed>());
        });
    }

    [Test]
    public void IgnoresWrongPackets()
    {
        transfer.OnCommand(new Data { BlockNumber = 2, Bytes = new byte[100] });
        Assert.Multiple(() =>
        {
            Assert.That(transfer.CommandWasSent(typeof(Acknowledgement)), Is.False);
            Assert.That(ms.Length, Is.EqualTo(0));
        });
    }

    [Test]
    public void RaisesFinished()
    {
        var onFinishedWasCalled = false;
        transfer.OnFinished += t =>
        {
            Assert.That(t, Is.EqualTo(transfer));
            onFinishedWasCalled = true;
        };

        Assert.That(onFinishedWasCalled, Is.False);
        transfer.OnCommand(new Data { BlockNumber = 1, Bytes = new byte[100] });
        Assert.Multiple(() =>
        {
            Assert.That(onFinishedWasCalled, Is.True);
            Assert.That(transfer.State, Is.InstanceOf<Closed>());
        });
    }

    [Test]
    public void RaisesProgress()
    {
        var onProgressWasCalled = false;
        transfer.OnProgress += (t, progress) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(t, Is.EqualTo(transfer));
                Assert.That(progress.TransferredBytes, Is.GreaterThan(0));
            });
            onProgressWasCalled = true;
        };

        Assert.That(onProgressWasCalled, Is.False);
        transfer.OnCommand(new Data { BlockNumber = 1, Bytes = new byte[1000] });
        Assert.Multiple(() =>
        {
            Assert.That(onProgressWasCalled, Is.True);
            Assert.That(transfer.State, Is.InstanceOf<Receiving>());
        });
    }

    [Test]
    public void ReceivesPacket()
    {
        transfer.OnCommand(new Data { BlockNumber = 1, Bytes = new byte[100] });
        Assert.Multiple(() =>
        {
            Assert.That(transfer.CommandWasSent(typeof(Acknowledgement)), Is.True);
            Assert.That(ms.Length, Is.EqualTo(100));
        });
    }

    [Test]
    public void SendsAcknowledgement()
    {
        transfer.OnCommand(new Data { BlockNumber = 1, Bytes = new byte[100] });
        Assert.That(transfer.CommandWasSent(typeof(Acknowledgement)), Is.True);
    }

    [SetUp]
    public void Setup()
    {
        ms = new MemoryStream();
        transfer = new TransferStub(ms);
        transfer.SetState(new Receiving(NullLogger.Instance));
    }

    [TearDown]
    public void Teardown()
    {
        transfer.Dispose();
    }

    [Test]
    public void TimeoutWhenNoDataIsReceivedAndRetryCountIsExceeded()
    {
        var transferWithLowTimeout = new TransferStub(new MemoryStream(new byte[5000]))
        {
            RetryTimeout = new TimeSpan(0),
            RetryCount = 1,
        };
        transferWithLowTimeout.SetState(new Receiving(NullLogger.Instance));

        transferWithLowTimeout.OnTimer();
        Assert.That(transferWithLowTimeout.HadNetworkTimeout, Is.False);
        transferWithLowTimeout.OnTimer();
        Assert.That(transferWithLowTimeout.HadNetworkTimeout, Is.True);
    }

    private void TransferUntilBlockCounterWrapIsAboutToWrap()
    {
        transfer.BlockSize = 1;
        for (ushort i = 1; i <= 65535; i++)
        {
            transfer.OnCommand(new Data { BlockNumber = i, Bytes = new byte[1] });
        }
    }
}
