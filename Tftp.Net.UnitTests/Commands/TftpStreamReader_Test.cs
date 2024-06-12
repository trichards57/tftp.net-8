// <copyright file="TftpStreamReader_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using System.IO;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class TftpStreamReader_Test
{
    private readonly byte[] data = [0x00, 0x01, 0x02, 0x03];
    private TftpStreamReader tested;

    [Test]
    public void ReadsIntoArraysWithPerfectSize()
    {
        byte[] bytes = tested.ReadBytes(4);
        Assert.That(bytes, Has.Length.EqualTo(4));
        Assert.Multiple(() =>
        {
            Assert.That(bytes[0], Is.EqualTo(0x00));
            Assert.That(bytes[1], Is.EqualTo(0x01));
            Assert.That(bytes[2], Is.EqualTo(0x02));
            Assert.That(bytes[3], Is.EqualTo(0x03));
        });
    }

    [Test]
    public void ReadsIntoLargerArrays()
    {
        byte[] bytes = tested.ReadBytes(10);
        Assert.That(bytes, Has.Length.EqualTo(4));
        Assert.Multiple(() =>
        {
            Assert.That(bytes[0], Is.EqualTo(0x00));
            Assert.That(bytes[1], Is.EqualTo(0x01));
            Assert.That(bytes[2], Is.EqualTo(0x02));
            Assert.That(bytes[3], Is.EqualTo(0x03));
        });
    }

    [Test]
    public void ReadsIntoSmallerArrays()
    {
        byte[] bytes = tested.ReadBytes(2);
        Assert.That(bytes, Has.Length.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(bytes[0], Is.EqualTo(0x00));
            Assert.That(bytes[1], Is.EqualTo(0x01));
        });
    }

    [Test]
    public void ReadsShorts()
    {
        Assert.That(tested.ReadUInt16(), Is.EqualTo(0x0001));
        Assert.That(tested.ReadUInt16(), Is.EqualTo(0x0203));
    }

    [Test]
    public void ReadsSingleBytes()
    {
        Assert.That(tested.ReadByte(), Is.EqualTo(0x00));
        Assert.That(tested.ReadByte(), Is.EqualTo(0x01));
        Assert.That(tested.ReadByte(), Is.EqualTo(0x02));
        Assert.That(tested.ReadByte(), Is.EqualTo(0x03));
    }

    [SetUp]
    public void Setup()
    {
        var ms = new MemoryStream(data);
        tested = new TftpStreamReader(ms);
    }
}
