// <copyright file="TftpCommandParserAndSerializer_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using System.IO;
using Xunit;

namespace Tftp.Net.UnitTests;

public class TftpCommandParserAndSerializer_Test
{
    [Fact]
    public void ParsesAck()
    {
        var original = new Acknowledgement { BlockNumber = 10 };

        var parsed = (Acknowledgement)CommandParser.Parse(Serialize(original));

        parsed.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void ParsesData()
    {
        byte[] data = [12, 15, 19, 0, 4];
        var original = new Data { BlockNumber = 123, Bytes = data };

        Data parsed = (Data)CommandParser.Parse(Serialize(original));

        parsed.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void ParsesError()
    {
        var original = new Error { ErrorCode = 15, Message = "Hallo Welt" };

        var parsed = (Error)CommandParser.Parse(Serialize(original));

        parsed.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void ParsesReadRequest()
    {
        var original = new ReadRequest("Hallo Welt.txt", TftpTransferMode.netascii, null);

        var parsed = (ReadRequest)CommandParser.Parse(Serialize(original));

        parsed.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void ParsesWriteRequest()
    {
        var original = new WriteRequest("Hallo Welt.txt", TftpTransferMode.netascii, null);

        var parsed = (WriteRequest)CommandParser.Parse(Serialize(original));

        parsed.Should().BeEquivalentTo(original);
    }

    private static byte[] Serialize(ITftpCommand command)
    {
        using var stream = new MemoryStream();
        var writer = new TftpStreamWriter(stream);
        command.WriteToStream(writer);
        return stream.ToArray();
    }
}
