// <copyright file="ClosedState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class ClosedState_Test
{
    private TransferStub transfer;

    [Test]
    public void CanNotCancel()
    {
        transfer.Cancel(TftpErrorPacket.IllegalOperation);
        Assert.That(transfer.State, Is.InstanceOf<Closed>());
    }

    [Test]
    public void IgnoresCommands()
    {
        transfer.OnCommand(new Error { ErrorCode = 10, Message = "Test" });
        Assert.That(transfer.State, Is.InstanceOf<Closed>());
    }

    [SetUp]
    public void Setup()
    {
        transfer = new TransferStub();
        transfer.SetState(new Closed(NullLogger.Instance));
    }

    [TearDown]
    public void Teardown()
    {
        transfer.Dispose();
    }
}
