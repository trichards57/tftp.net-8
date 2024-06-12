// <copyright file="ClosedState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Tftp.Net.Transfer.States;
using Xunit;

namespace Tftp.Net.UnitTests;

public class ClosedState_Test
{
    private readonly TransferStub transfer;

    public ClosedState_Test()
    {
        transfer = new TransferStub();
        transfer.SetState(new Closed(NullLogger.Instance));
    }

    [Fact]
    public void CanNotCancel()
    {
        transfer.Cancel(TftpErrorPacket.IllegalOperation);
        transfer.State.Should().BeOfType<Closed>();
    }

    [Fact]
    public void IgnoresCommands()
    {
        transfer.OnCommand(new Error { ErrorCode = 10, Message = "Test" });
        transfer.State.Should().BeOfType<Closed>();
    }
}
