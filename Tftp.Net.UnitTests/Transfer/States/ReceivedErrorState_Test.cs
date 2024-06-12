// <copyright file="ReceivedErrorState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Tftp.Net.Transfer.States;
using Xunit;

namespace Tftp.Net.UnitTests;

public class ReceivedErrorState_Test
{
    private readonly TransferStub transfer;

    public ReceivedErrorState_Test()
    {
        transfer = new TransferStub();
        transfer.SetState(new ReceivedError(new TftpErrorPacket(123, "Error"), NullLogger.Instance));
    }

    [Fact]
    public void CallsOnError()
    {
        var onErrorWasCalled = false;
        var tx = new TransferStub();
        tx.OnError += (t, error) =>
        {
            onErrorWasCalled = true;
            t.Should().Be(tx);
            var err = error.Should().BeOfType<TftpErrorPacket>().Subject;

            err.ErrorCode.Should().Be(123);
            err.ErrorMessage.Should().Be("My Error");
        };

        onErrorWasCalled.Should().BeFalse();
        tx.SetState(new ReceivedError(new TftpErrorPacket(123, "My Error"), NullLogger.Instance));
        onErrorWasCalled.Should().BeTrue();
    }

    [Fact]
    public void TransitionsToClosed()
    {
        transfer.State.Should().BeOfType<Closed>();
    }
}
