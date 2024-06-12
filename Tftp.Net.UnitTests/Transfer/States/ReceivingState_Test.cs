// <copyright file="ReceivingState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;
using Tftp.Net.Transfer.States;
using Xunit;

namespace Tftp.Net.UnitTests.Transfer.States;

public class ReceivingState_Test
{
    private readonly MemoryStream ms;
    private readonly TransferStub transfer;

    public ReceivingState_Test()
    {
        ms = new MemoryStream();
        transfer = new TransferStub(ms);
        transfer.SetState(new Receiving(NullLogger.Instance));
    }

    [Fact]
    public void BlockCounterWrapsAroundToOne()
    {
        transfer.BlockCounterWrapping = BlockCounterWrapAround.ToOne;
        TransferUntilBlockCounterWrapIsAboutToWrap();

        transfer.OnCommand(new Data { BlockNumber = 1, Bytes = new byte[1] });

        ((Acknowledgement)transfer.SentCommands[^1]).BlockNumber.Should().Be(1);
    }

    [Fact]
    public void BlockCounterWrapsAroundToZero()
    {
        TransferUntilBlockCounterWrapIsAboutToWrap();

        transfer.OnCommand(new Data { BlockNumber = 0, Bytes = new byte[1] });

        ((Acknowledgement)transfer.SentCommands[^1]).BlockNumber.Should().Be(0);
    }

    [Fact]
    public void CanCancel()
    {
        transfer.Cancel(TftpErrorPacket.IllegalOperation);
        transfer.CommandWasSent(typeof(Error)).Should().BeTrue();
        transfer.State.Should().BeOfType<Closed>();
    }

    [Fact]
    public void HandlesError()
    {
        var onErrorWasCalled = false;
        transfer.OnError += (t, error) => { onErrorWasCalled = true; };

        onErrorWasCalled.Should().BeFalse();
        transfer.OnCommand(new Error { ErrorCode = 123, Message = "Test Error" });
        onErrorWasCalled.Should().BeTrue();
        transfer.State.Should().BeOfType<Closed>();
    }

    [Fact]
    public void IgnoresWrongPackets()
    {
        transfer.OnCommand(new Data { BlockNumber = 2, Bytes = new byte[100] });
        transfer.CommandWasSent(typeof(Acknowledgement)).Should().BeFalse();
        ms.Length.Should().Be(0);
    }

    [Fact]
    public void RaisesFinished()
    {
        var onFinishedWasCalled = false;
        transfer.OnFinished += t =>
        {
            t.Should().Be(transfer);
            onFinishedWasCalled = true;
        };

        onFinishedWasCalled.Should().BeFalse();
        transfer.OnCommand(new Data { BlockNumber = 1, Bytes = new byte[100] });
        onFinishedWasCalled.Should().BeTrue();
        transfer.State.Should().BeOfType<Closed>();
    }

    [Fact]
    public void RaisesProgress()
    {
        var onProgressWasCalled = false;
        transfer.OnProgress += (t, progress) =>
        {
            t.Should().Be(transfer);
            progress.TransferredBytes.Should().BeGreaterThan(0);
            onProgressWasCalled = true;
        };

        onProgressWasCalled.Should().BeFalse();
        transfer.OnCommand(new Data { BlockNumber = 1, Bytes = new byte[1000] });
        onProgressWasCalled.Should().BeTrue();
        transfer.State.Should().BeOfType<Receiving>();
    }

    [Fact]
    public void ReceivesPacket()
    {
        transfer.OnCommand(new Data { BlockNumber = 1, Bytes = new byte[100] });
        transfer.CommandWasSent(typeof(Acknowledgement)).Should().BeTrue();
        ms.Length.Should().Be(100);
    }

    [Fact]
    public void SendsAcknowledgement()
    {
        transfer.OnCommand(new Data { BlockNumber = 1, Bytes = new byte[100] });
        transfer.CommandWasSent(typeof(Acknowledgement)).Should().BeTrue();
    }

    [Fact]
    public void TimeoutWhenNoDataIsReceivedAndRetryCountIsExceeded()
    {
        var transferWithLowTimeout = new TransferStub(new MemoryStream(new byte[5000]))
        {
            RetryTimeout = new TimeSpan(0),
            RetryCount = 1,
        };
        transferWithLowTimeout.SetState(new Receiving(NullLogger.Instance));

        transferWithLowTimeout.OnTimer();
        transferWithLowTimeout.HadNetworkTimeout.Should().BeFalse();
        transferWithLowTimeout.OnTimer();
        transferWithLowTimeout.HadNetworkTimeout.Should().BeTrue();
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
