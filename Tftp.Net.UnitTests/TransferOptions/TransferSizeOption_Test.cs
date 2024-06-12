// <copyright file="TransferSizeOption_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Tftp.Net.Commands;
using Tftp.Net.Transfer;
using Xunit;

namespace Tftp.Net.UnitTests.TransferOptions;

public class TransferSizeOption_Test
{
    private TransferOptionSet options;

    [Fact]
    public void ReadsTransferSize()
    {
        Parse(new TransferOption("tsize", "0"));
        options.IncludesTransferSizeOption.Should().BeTrue();
        options.TransferSize.Should().Be(0);
    }

    [Fact]
    public void RejectsNegativeTransferSize()
    {
        Parse(new TransferOption("tsize", "-1"));
        options.IncludesTransferSizeOption.Should().BeFalse();
        options.TransferSize.Should().Be(0);
    }

    [Fact]
    public void RejectsNonIntegerTransferSize()
    {
        Parse(new TransferOption("tsize", "abc"));
        options.IncludesTransferSizeOption.Should().BeFalse();
        options.TransferSize.Should().Be(0);
    }

    private void Parse(TransferOption option)
    {
        options = new TransferOptionSet([option]);
    }
}
