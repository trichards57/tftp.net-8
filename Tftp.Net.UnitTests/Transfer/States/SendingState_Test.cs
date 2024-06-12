// <copyright file="SendingState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.IO;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests.Transfer.States;

[TestFixture]
internal class SendingState_Test
{
    private TransferStub transfer;

    [Test]
    public void BlockCountWrapsAroundTo0()
    {
        SetupTransferThatWillWrapAroundBlockCount();

        RunTransferUntilBlockCount(65535);
        transfer.OnCommand(new Acknowledgement { BlockNumber = 65535 });

        Assert.That(((Data)transfer.SentCommands[^1]).BlockNumber, Is.EqualTo(0));
    }

    [Test]
    public void BlockCountWrapsAroundTo1()
    {
        SetupTransferThatWillWrapAroundBlockCount();
        transfer.BlockCounterWrapping = BlockCounterWrapAround.ToOne;

        RunTransferUntilBlockCount(65535);
        transfer.OnCommand(new Acknowledgement { BlockNumber = 65535 });

        Assert.That(((Data)transfer.SentCommands[^1]).BlockNumber, Is.EqualTo(1));
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
    public void HandlesAcknowledgment()
    {
        transfer.SentCommands.Clear();
        Assert.That(transfer.CommandWasSent(typeof(Data)), Is.False);

        transfer.OnCommand(new Acknowledgement { BlockNumber = 1 });
        Assert.That(transfer.CommandWasSent(typeof(Data)), Is.True);
    }

    [Test]
    public void HandlesError()
    {
        bool onErrorWasCalled = false;
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
    public void IgnoresWrongAcknowledgment()
    {
        transfer.SentCommands.Clear();
        Assert.That(transfer.CommandWasSent(typeof(Data)), Is.False);

        transfer.OnCommand(new Acknowledgement { BlockNumber = 0 });
        Assert.That(transfer.CommandWasSent(typeof(Data)), Is.False);
    }

    [Test]
    public void IncreasesBlockCountWithEachAcknowledgement()
    {
        Assert.That(((Data)transfer.SentCommands[^1]).BlockNumber, Is.EqualTo(1));

        transfer.OnCommand(new Acknowledgement { BlockNumber = 1 });

        Assert.That(((Data)transfer.SentCommands[^1]).BlockNumber, Is.EqualTo(2));
    }

    [Test]
    public void RaisesProgress()
    {
        bool onProgressWasCalled = false;
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
        transfer.OnCommand(new Acknowledgement { BlockNumber = 1 });
        Assert.That(onProgressWasCalled, Is.True);
    }

    [Test]
    public void ResendsPacket()
    {
        var transferWithLowTimeout = new TransferStub(new MemoryStream(new byte[5000]))
        {
            RetryTimeout = new TimeSpan(0),
        };
        transferWithLowTimeout.SetState(new Sending(NullLogger.Instance));

        Assert.That(transferWithLowTimeout.CommandWasSent(typeof(Data)), Is.True);
        transferWithLowTimeout.SentCommands.Clear();

        transferWithLowTimeout.OnTimer();
        Assert.That(transferWithLowTimeout.CommandWasSent(typeof(Data)), Is.True);
    }

    [Test]
    public void SendsPacket()
    {
        Assert.That(transfer.CommandWasSent(typeof(Data)), Is.True);
    }

    [SetUp]
    public void Setup()
    {
        transfer = new TransferStub(new MemoryStream(new byte[5000]));
        transfer.SetState(new Sending(NullLogger.Instance));
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
        transferWithLowTimeout.SetState(new Sending(NullLogger.Instance));

        transferWithLowTimeout.OnTimer();
        Assert.That(transferWithLowTimeout.HadNetworkTimeout, Is.False);
        transferWithLowTimeout.OnTimer();
        Assert.That(transferWithLowTimeout.HadNetworkTimeout, Is.True);
    }

    private void RunTransferUntilBlockCount(int targetBlockCount)
    {
        while (((Data)transfer.SentCommands[^1]).BlockNumber != targetBlockCount)
        {
            transfer.OnCommand(new Acknowledgement { BlockNumber = ((Data)transfer.SentCommands[^1]).BlockNumber });
        }
    }

    private void SetupTransferThatWillWrapAroundBlockCount()
    {
        transfer = new TransferStub(new MemoryStream(new byte[70000]))
        {
            BlockSize = 1,
        };
        transfer.SetState(new Sending(NullLogger.Instance));
    }
}
