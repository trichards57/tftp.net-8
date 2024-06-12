// <copyright file="TftpStreamWriter_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using System.IO;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class TftpStreamWriter_Test
{
    private MemoryStream ms;
    private TftpStreamWriter tested;

    [SetUp]
    public void Setup()
    {
        ms = new MemoryStream();
        tested = new TftpStreamWriter(ms);
    }

    [Test]
    public void WritesArrays()
    {
        tested.WriteBytes([3, 4, 5]);

        Assert.That(ms.Length, Is.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(ms.GetBuffer()[0], Is.EqualTo(3));
            Assert.That(ms.GetBuffer()[1], Is.EqualTo(4));
            Assert.That(ms.GetBuffer()[2], Is.EqualTo(5));
        });
    }

    [Test]
    public void WritesShorts()
    {
        tested.WriteUInt16(0x0102);
        tested.WriteUInt16(0x0304);

        Assert.That(ms.Length, Is.EqualTo(4));
        Assert.Multiple(() =>
        {
            Assert.That(ms.GetBuffer()[0], Is.EqualTo(1));
            Assert.That(ms.GetBuffer()[1], Is.EqualTo(2));
            Assert.That(ms.GetBuffer()[2], Is.EqualTo(3));
            Assert.That(ms.GetBuffer()[3], Is.EqualTo(4));
        });
    }

    [Test]
    public void WritesSingleBytes()
    {
        tested.WriteByte(1);
        tested.WriteByte(2);
        tested.WriteByte(3);

        Assert.That(ms.Length, Is.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(ms.GetBuffer()[0], Is.EqualTo(1));
            Assert.That(ms.GetBuffer()[1], Is.EqualTo(2));
            Assert.That(ms.GetBuffer()[2], Is.EqualTo(3));
        });
    }
}
