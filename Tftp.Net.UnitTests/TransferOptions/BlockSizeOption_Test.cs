// <copyright file="BlockSizeOption_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using Tftp.Net.Commands;
using Tftp.Net.Transfer;

namespace Tftp.Net.UnitTests.TransferOptions;

[TestFixture]
internal class BlockSizeOption_Test
{
    private TransferOptionSet options;

    [Test]
    public void AcceptsMaxBlocksize()
    {
        Parse(new TransferOption("blksize", "65464"));
        Assert.That(options.IncludesBlockSizeOption, Is.True);

        Parse(new TransferOption("blksize", "65465"));
        Assert.That(options.IncludesBlockSizeOption, Is.False);
    }

    [Test]
    public void AcceptsMinBlocksize()
    {
        Parse(new TransferOption("blksize", "8"));
        Assert.That(options.IncludesBlockSizeOption, Is.True);

        Parse(new TransferOption("blksize", "7"));
        Assert.That(options.IncludesBlockSizeOption, Is.False);
    }

    [Test]
    public void AcceptsRegularOption()
    {
        Parse(new TransferOption("blksize", "16"));
        Assert.Multiple(() =>
        {
            Assert.That(options.IncludesBlockSizeOption, Is.True);
            Assert.That(options.BlockSize, Is.EqualTo(16));
        });
    }

    [Test]
    public void IgnoresInvalidValue()
    {
        Parse(new TransferOption("blksize", "not-a-number"));
        Assert.Multiple(() =>
        {
            Assert.That(options.BlockSize, Is.EqualTo(512));
            Assert.That(options.IncludesBlockSizeOption, Is.False);
        });
    }

    [Test]
    public void IgnoresUnknownOption()
    {
        Parse(new TransferOption("blub", "16"));
        Assert.Multiple(() =>
        {
            Assert.That(options.BlockSize, Is.EqualTo(512));
            Assert.That(options.IncludesBlockSizeOption, Is.False);
        });
    }

    private void Parse(TransferOption option)
    {
        options = new TransferOptionSet([option]);
    }
}
