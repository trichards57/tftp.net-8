// <copyright file="ReceivedErrorState_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class ReceivedErrorState_Test
{
    private TransferStub transfer;

    [Test]
    public void CallsOnError()
    {
        var onErrorWasCalled = false;
        var tx = new TransferStub();
        tx.OnError += (t, error) =>
        {
            onErrorWasCalled = true;
            Assert.Multiple(() =>
            {
                Assert.That(t, Is.EqualTo(tx));

                Assert.That(error, Is.InstanceOf<TftpErrorPacket>());

                Assert.That(((TftpErrorPacket)error).ErrorCode, Is.EqualTo(123));
                Assert.That(((TftpErrorPacket)error).ErrorMessage, Is.EqualTo("My Error"));
            });
        };

        Assert.That(onErrorWasCalled, Is.False);
        tx.SetState(new ReceivedError(new TftpErrorPacket(123, "My Error"), NullLogger.Instance));
        Assert.That(onErrorWasCalled, Is.True);
    }

    [SetUp]
    public void Setup()
    {
        transfer = new TransferStub();
        transfer.SetState(new ReceivedError(new TftpErrorPacket(123, "Error"), NullLogger.Instance));
    }

    [TearDown]
    public void Teardown()
    {
        transfer.Dispose();
    }

    [Test]
    public void TransitionsToClosed()
    {
        Assert.That(transfer.State, Is.InstanceOf<Closed>());
    }
}
