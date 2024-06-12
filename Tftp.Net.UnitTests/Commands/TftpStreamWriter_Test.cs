// <copyright file="TftpStreamWriter_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using System.IO;
using Tftp.Net.Commands;
using Xunit;

namespace Tftp.Net.UnitTests;

public class TftpStreamWriter_Test
{
    private readonly MemoryStream ms;
    private readonly TftpStreamWriter tested;

    public TftpStreamWriter_Test()
    {
        ms = new MemoryStream();
        tested = new TftpStreamWriter(ms);
    }

    [Fact]
    public void WritesArrays()
    {
        var expected = new byte[] { 3, 4, 5 };

        tested.WriteBytes([3, 4, 5]);

        var actual = ms.ToArray();

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void WritesShorts()
    {
        var expected = new byte[] { 1, 2, 3, 4 };

        tested.WriteUInt16(0x0102);
        tested.WriteUInt16(0x0304);

        var actual = ms.ToArray();

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void WritesSingleBytes()
    {
        var expected = new byte[] { 1, 2, 3 };

        tested.WriteByte(1);
        tested.WriteByte(2);
        tested.WriteByte(3);

        var actual = ms.ToArray();

        actual.Should().BeEquivalentTo(expected);
    }
}
