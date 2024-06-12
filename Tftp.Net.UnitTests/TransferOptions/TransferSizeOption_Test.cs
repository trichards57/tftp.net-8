// <copyright file="TransferSizeOption_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using Tftp.Net.Commands;
using Tftp.Net.Transfer;

namespace Tftp.Net.UnitTests.TransferOptions;

[TestFixture]
internal class TransferSizeOption_Test
{
    private TransferOptionSet options;

    [Test]
    public void ReadsTransferSize()
    {
        Parse(new TransferOption("tsize", "0"));
        Assert.Multiple(() =>
        {
            Assert.That(options.IncludesTransferSizeOption, Is.True);
            Assert.That(options.TransferSize, Is.EqualTo(0));
        });
    }

    [Test]
    public void RejectsNegativeTransferSize()
    {
        Parse(new TransferOption("tsize", "-1"));
        Assert.That(options.IncludesTransferSizeOption, Is.False);
    }

    [Test]
    public void RejectsNonIntegerTransferSize()
    {
        Parse(new TransferOption("tsize", "abc"));
        Assert.That(options.IncludesTransferSizeOption, Is.False);
    }

    private void Parse(TransferOption option)
    {
        options = new TransferOptionSet([option]);
    }
}
