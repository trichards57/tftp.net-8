// <copyright file="TransferOption_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;

namespace Tftp.Net.UnitTests.TransferOptions;

[TestFixture]
internal class TransferOption_Test
{
    [Test]
    public void AcceptsEmptyValue()
    {
        Assert.That(() => new TransferOption("Test", string.Empty), Throws.Nothing);
    }

    [Test]
    public void CanBeCreatedWithValidNameAndValue()
    {
        var option = new TransferOption("Test", "Hallo Welt");
        Assert.Multiple(() =>
        {
            Assert.That(option.Name, Is.EqualTo("Test"));
            Assert.That(option.Value, Is.EqualTo("Hallo Welt"));
            Assert.That(option.IsAcknowledged, Is.False);
        });
    }

    [Test]
    public void RejectsInvalidName1()
    {
        Assert.That(() => new TransferOption(string.Empty, "Hallo Welt"), Throws.ArgumentException);
    }

    [Test]
    public void RejectsInvalidName2()
    {
        Assert.That(() => new TransferOption(null, "Hallo Welt"), Throws.ArgumentNullException);
    }

    [Test]
    public void RejectsInvalidValue()
    {
        Assert.That(() => new TransferOption("Test", null), Throws.ArgumentNullException);
    }
}
