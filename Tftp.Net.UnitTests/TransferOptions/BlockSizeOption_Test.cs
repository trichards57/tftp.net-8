// <copyright file="BlockSizeOption_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Tftp.Net.Commands;
using Tftp.Net.Transfer;
using Xunit;

namespace Tftp.Net.UnitTests.TransferOptions;

public class BlockSizeOption_Test
{
    private TransferOptionSet options;

    [Fact]
    public void AcceptsMaxBlocksize()
    {
        Parse(new TransferOption("blksize", "65464"));
        options.IncludesBlockSizeOption.Should().BeTrue();

        Parse(new TransferOption("blksize", "65465"));
        options.IncludesBlockSizeOption.Should().BeFalse();
    }

    [Fact]
    public void AcceptsMinBlocksize()
    {
        Parse(new TransferOption("blksize", "8"));
        options.IncludesBlockSizeOption.Should().BeTrue();

        Parse(new TransferOption("blksize", "7"));
        options.IncludesBlockSizeOption.Should().BeFalse();
    }

    [Fact]
    public void AcceptsRegularOption()
    {
        Parse(new TransferOption("blksize", "16"));
        options.IncludesBlockSizeOption.Should().BeTrue();
        options.BlockSize.Should().Be(16);
    }

    [Fact]
    public void IgnoresInvalidValue()
    {
        Parse(new TransferOption("blksize", "not-a-number"));
        options.IncludesBlockSizeOption.Should().BeFalse();
        options.BlockSize.Should().Be(512);
    }

    [Fact]
    public void IgnoresUnknownOption()
    {
        Parse(new TransferOption("blub", "16"));
        options.IncludesBlockSizeOption.Should().BeFalse();
        options.BlockSize.Should().Be(512);
    }

    private void Parse(TransferOption option)
    {
        options = new TransferOptionSet([option]);
    }
}
