// <copyright file="TftpStreamReader_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using System.IO;
using Xunit;

namespace Tftp.Net.UnitTests;

public class TftpStreamReader_Test
{
    private readonly byte[] data = [0x00, 0x01, 0x02, 0x03];
    private readonly TftpStreamReader tested;

    public TftpStreamReader_Test()
    {
        var ms = new MemoryStream(data);
        tested = new TftpStreamReader(ms);
    }

    [Fact]
    public void ReadsIntoArraysWithPerfectSize()
    {
        var expected = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        byte[] actual = tested.ReadBytes(4);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadsIntoLargerArrays()
    {
        var expected = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        byte[] actual = tested.ReadBytes(10);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadsIntoSmallerArrays()
    {
        var expected = new byte[] { 0x00, 0x01 };
        byte[] actual = tested.ReadBytes(2);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadsShorts()
    {
        ushort expected1 = 0x0001;
        ushort expected2 = 0x0203;

        var actual1 = tested.ReadUInt16();
        var actual2 = tested.ReadUInt16();

        actual1.Should().Be(expected1);
        actual2.Should().Be(expected2);
    }

    [Fact]
    public void ReadsSingleBytes()
    {
        byte expected1 = 0x00;
        byte expected2 = 0x01;
        byte expected3 = 0x02;
        byte expected4 = 0x03;

        var actual1 = tested.ReadByte();
        var actual2 = tested.ReadByte();
        var actual3 = tested.ReadByte();
        var actual4 = tested.ReadByte();

        actual1.Should().Be(expected1);
        actual2.Should().Be(expected2);
        actual3.Should().Be(expected3);
        actual4.Should().Be(expected4);
    }
}
