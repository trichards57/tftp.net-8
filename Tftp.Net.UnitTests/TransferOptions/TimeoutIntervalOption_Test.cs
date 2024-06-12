// <copyright file="TimeoutIntervalOption_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Tftp.Net.Commands;
using Tftp.Net.Transfer;
using Xunit;

namespace Tftp.Net.UnitTests.TransferOptions;

public class TimeoutIntervalOption_Test
{
    private TransferOptionSet options;

    [Fact]
    public void AcceptsValidTimeout()
    {
        options = new TransferOptionSet([new TransferOption("timeout", "10")]);

        options.IncludesTimeoutOption.Should().BeTrue();
        options.Timeout.Should().Be(10);
    }

    [Fact]
    public void AcceptsMinTimeout()
    {
        options = new TransferOptionSet([new TransferOption("timeout", "1")]);

        options.IncludesTimeoutOption.Should().BeTrue();
        options.Timeout.Should().Be(1);
    }

    [Fact]
    public void AcceptsMaxTimeout()
    {
        options = new TransferOptionSet([new TransferOption("timeout", "255")]);

        options.IncludesTimeoutOption.Should().BeTrue();
        options.Timeout.Should().Be(255);
    }

    [Fact]
    public void RejectsTimeoutTooLow()
    {
        options = new TransferOptionSet([new TransferOption("timeout", "0")]);

        options.IncludesTimeoutOption.Should().BeFalse();
        options.Timeout.Should().Be(5);
    }

    [Fact]
    public void RejectsTimeoutTooHigh()
    {
        options = new TransferOptionSet([new TransferOption("timeout", "256")]);

        options.IncludesTimeoutOption.Should().BeFalse();
        options.Timeout.Should().Be(5);
    }

    [Fact]
    public void RejectsNonIntegerTimeout()
    {
        options = new TransferOptionSet([new TransferOption("timeout", "blub")]);

        options.IncludesTimeoutOption.Should().BeFalse();
        options.Timeout.Should().Be(5);
    }
}
