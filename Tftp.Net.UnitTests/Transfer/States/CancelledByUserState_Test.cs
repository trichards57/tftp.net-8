// <copyright file="CancelledByUserState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class CancelledByUserState_Test
{
    private TransferStub transfer;

    [Test]
    public void SendsErrorToClient()
    {
        Assert.That(transfer.CommandWasSent(typeof(Error)), Is.True);
    }

    [SetUp]
    public void Setup()
    {
        transfer = new TransferStub();
        transfer.SetState(new CancelledByUser(TftpErrorPacket.IllegalOperation));
    }

    [TearDown]
    public void Teardown()
    {
        transfer.Dispose();
    }

    [Test]
    public void TransitionsToClosedState()
    {
        Assert.That(transfer.State, Is.InstanceOf<Closed>());
    }
}
