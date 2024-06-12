// <copyright file="SendingState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.IO;
using Tftp.Net.Transfer.States;
using Xunit;

namespace Tftp.Net.UnitTests.Transfer.States;

public class SendingState_Test
{
    private TransferStub transfer;

    public SendingState_Test()
    {
        transfer = new TransferStub(new MemoryStream(new byte[5000]));
        transfer.SetState(new Sending(NullLogger.Instance));
    }

    [Fact]
    public void BlockCountWrapsAroundTo0()
    {
        SetupTransferThatWillWrapAroundBlockCount();

        RunTransferUntilBlockCount(65535);
        transfer.OnCommand(new Acknowledgement { BlockNumber = 65535 });

        ((Data)transfer.SentCommands[^1]).BlockNumber.Should().Be(0);
    }

    [Fact]
    public void BlockCountWrapsAroundTo1()
    {
        SetupTransferThatWillWrapAroundBlockCount();
        transfer.BlockCounterWrapping = BlockCounterWrapAround.ToOne;

        RunTransferUntilBlockCount(65535);
        transfer.OnCommand(new Acknowledgement { BlockNumber = 65535 });

        ((Data)transfer.SentCommands[^1]).BlockNumber.Should().Be(1);
    }

    [Fact]
    public void CanCancel()
    {
        transfer.Cancel(TftpErrorPacket.IllegalOperation);
        transfer.CommandWasSent(typeof(Error)).Should().BeTrue();
        transfer.State.Should().BeOfType<Closed>();
    }

    [Fact]
    public void HandlesAcknowledgment()
    {
        transfer.SentCommands.Clear();
        transfer.CommandWasSent(typeof(Error)).Should().BeFalse();

        transfer.OnCommand(new Acknowledgement { BlockNumber = 1 });
        transfer.CommandWasSent(typeof(Data)).Should().BeTrue();
    }

    [Fact]
    public void HandlesError()
    {
        bool onErrorWasCalled = false;
        transfer.OnError += (t, error) => { onErrorWasCalled = true; };

        onErrorWasCalled.Should().BeFalse();
        transfer.OnCommand(new Error { ErrorCode = 123, Message = "Test Error" });
        onErrorWasCalled.Should().BeTrue();
        transfer.State.Should().BeOfType<Closed>();
    }

    [Fact]
    public void IgnoresWrongAcknowledgment()
    {
        transfer.SentCommands.Clear();
        transfer.CommandWasSent(typeof(Data)).Should().BeFalse();

        transfer.OnCommand(new Acknowledgement { BlockNumber = 0 });
        transfer.CommandWasSent(typeof(Data)).Should().BeFalse();
    }

    [Fact]
    public void IncreasesBlockCountWithEachAcknowledgement()
    {
        ((Data)transfer.SentCommands[^1]).BlockNumber.Should().Be(1);

        transfer.OnCommand(new Acknowledgement { BlockNumber = 1 });

        ((Data)transfer.SentCommands[^1]).BlockNumber.Should().Be(2);
    }

    [Fact]
    public void RaisesProgress()
    {
        bool onProgressWasCalled = false;
        transfer.OnProgress += (t, progress) =>
        {
            t.Should().Be(transfer);
            progress.TransferredBytes.Should().BePositive();
            onProgressWasCalled = true;
        };

        onProgressWasCalled.Should().BeFalse();
        transfer.OnCommand(new Acknowledgement { BlockNumber = 1 });
        onProgressWasCalled.Should().BeTrue();
    }

    [Fact]
    public void ResendsPacket()
    {
        var transferWithLowTimeout = new TransferStub(new MemoryStream(new byte[5000]))
        {
            RetryTimeout = new TimeSpan(0),
        };
        transferWithLowTimeout.SetState(new Sending(NullLogger.Instance));

        transfer.CommandWasSent(typeof(Data)).Should().BeTrue();
        transferWithLowTimeout.SentCommands.Clear();

        transferWithLowTimeout.OnTimer();
        transfer.CommandWasSent(typeof(Data)).Should().BeTrue();
    }

    [Fact]
    public void SendsPacket()
    {
        transfer.CommandWasSent(typeof(Data)).Should().BeFalse();
    }

    [Fact]
    public void TimeoutWhenNoAnswerIsReceivedAndRetryCountIsExceeded()
    {
        var transferWithLowTimeout = new TransferStub(new MemoryStream(new byte[5000]))
        {
            RetryTimeout = new TimeSpan(0),
            RetryCount = 1,
        };
        transferWithLowTimeout.SetState(new Sending(NullLogger.Instance));

        transferWithLowTimeout.OnTimer();
        transferWithLowTimeout.HadNetworkTimeout.Should().BeFalse();
        transferWithLowTimeout.OnTimer();
        transferWithLowTimeout.HadNetworkTimeout.Should().BeTrue();
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
