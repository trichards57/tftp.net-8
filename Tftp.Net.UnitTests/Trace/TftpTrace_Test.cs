// <copyright file="TftpTrace_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using System.Diagnostics;
using Tftp.Net.Trace;

namespace Tftp.Net.UnitTests.Trace;

[TestFixture]
internal class TftpTrace_Test
{
    private TraceListenerMock listener;

    [Test]
    public void CallsTrace()
    {
        TftpTrace.Enabled = true;
        Assert.That(listener.WriteWasCalled, Is.False);
        TftpTrace.Trace("Test", new TransferStub());
        Assert.That(listener.WriteWasCalled, Is.True);
    }

    [Test]
    public void DoesNotWriteWhenDisabled()
    {
        TftpTrace.Enabled = false;
        TftpTrace.Trace("Test", new TransferStub());
        Assert.That(listener.WriteWasCalled, Is.False);
    }

    [SetUp]
    public void Setup()
    {
        listener = new TraceListenerMock();
        System.Diagnostics.Trace.Listeners.Add(listener);
    }

    [TearDown]
    public void Teardown()
    {
        System.Diagnostics.Trace.Listeners.Remove(listener);
        listener.Dispose();
        TftpTrace.Enabled = false;
    }

    private class TraceListenerMock : TraceListener
    {
        public bool WriteWasCalled { get; set; } = false;

        public override void Write(string message)
        {
            WriteWasCalled = true;
        }

        public override void WriteLine(string message)
        {
            WriteWasCalled = true;
        }
    }
}
