// <copyright file="CancelledByUserState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Tftp.Net.Transfer.States;
using Xunit;

namespace Tftp.Net.UnitTests;

public class CancelledByUserState_Test
{
    private readonly TransferStub transfer;

    [Fact]
    public void SendsErrorToClient()
    {
        transfer.CommandWasSent(typeof(Error)).Should().BeTrue();
    }

    public CancelledByUserState_Test()
    {
        transfer = new TransferStub();
        transfer.SetState(new CancelledByUser(TftpErrorPacket.IllegalOperation, NullLogger.Instance));
    }

    [Fact]
    public void TransitionsToClosedState()
    {
        transfer.State.Should().BeOfType<Closed>();
    }
}
