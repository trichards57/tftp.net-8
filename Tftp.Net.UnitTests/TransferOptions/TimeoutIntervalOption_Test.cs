// <copyright file="TimeoutIntervalOption_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using Tftp.Net.Commands;
using Tftp.Net.Transfer;

namespace Tftp.Net.UnitTests.TransferOptions;

[TestFixture]
internal class TimeoutIntervalOption_Test
{
    private TransferOptionSet options;

    [Test]
    public void AcceptsValidTimeout()
    {
        Parse(new TransferOption("timeout", "10"));
        Assert.Multiple(() =>
        {
            Assert.That(options.IncludesTimeoutOption, Is.True);
            Assert.That(options.Timeout, Is.EqualTo(10));
        });
    }

    [Test]
    public void AcceptsMinTimeout()
    {
        Parse(new TransferOption("timeout", "1"));
        Assert.Multiple(() =>
        {
            Assert.That(options.IncludesTimeoutOption, Is.True);
            Assert.That(options.Timeout, Is.EqualTo(1));
        });
    }

    [Test]
    public void AcceptsMaxTimeout()
    {
        Parse(new TransferOption("timeout", "255"));
        Assert.Multiple(() =>
        {
            Assert.That(options.IncludesTimeoutOption, Is.True);
            Assert.That(options.Timeout, Is.EqualTo(255));
        });
    }

    [Test]
    public void RejectsTimeoutTooLow()
    {
        Parse(new TransferOption("timeout", "0"));
        Assert.Multiple(() =>
        {
            Assert.That(options.IncludesTimeoutOption, Is.False);
            Assert.That(options.Timeout, Is.EqualTo(5));
        });
    }

    [Test]
    public void RejectsTimeoutTooHigh()
    {
        Parse(new TransferOption("timeout", "256"));
        Assert.Multiple(() =>
        {
            Assert.That(options.IncludesTimeoutOption, Is.False);
            Assert.That(options.Timeout, Is.EqualTo(5));
        });
    }

    [Test]
    public void RejectsNonIntegerTimeout()
    {
        Parse(new TransferOption("timeout", "blub"));
        Assert.Multiple(() =>
        {
            Assert.That(options.IncludesTimeoutOption, Is.False);
            Assert.That(options.Timeout, Is.EqualTo(5));
        });
    }

    private void Parse(TransferOption option)
    {
        options = new TransferOptionSet(new TransferOption[] { option });
    }
}
