// <copyright file="TftpCommandParserAndSerializer_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using System;
using System.IO;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class TftpCommandParserAndSerializer_Test
{
    [Test]
    public void ParsesAck()
    {
        var original = new Acknowledgement { BlockNumber = 10 };

        var parsed = (Acknowledgement)CommandParser.Parse(Serialize(original));
        Assert.That(parsed.BlockNumber, Is.EqualTo(original.BlockNumber));
    }

    [Test]
    public void ParsesData()
    {
        byte[] data = [12, 15, 19, 0, 4];
        var original = new Data { BlockNumber = 123, Bytes = data };

        Data parsed = (Data)CommandParser.Parse(Serialize(original));
        Assert.Multiple(() =>
        {
            Assert.That(parsed.BlockNumber, Is.EqualTo(original.BlockNumber));
            Assert.That(parsed.Bytes, Has.Length.EqualTo(original.Bytes.Length));
        });

        for (int i = 0; i < original.Bytes.Length; i++)
        {
            Assert.That(parsed.Bytes[i], Is.EqualTo(original.Bytes[i]));
        }
    }

    [Test]
    public void ParsesError()
    {
        var original = new Error { ErrorCode = 15, Message = "Hallo Welt" };

        var parsed = (Error)CommandParser.Parse(Serialize(original));
        Assert.Multiple(() =>
        {
            Assert.That(parsed.ErrorCode, Is.EqualTo(original.ErrorCode));
            Assert.That(parsed.Message, Is.EqualTo(original.Message));
        });
    }

    [Test]
    public void ParsesReadRequest()
    {
        var original = new ReadRequest("Hallo Welt.txt", TftpTransferMode.netascii, null);

        var parsed = (ReadRequest)CommandParser.Parse(Serialize(original));
        Assert.Multiple(() =>
        {
            Assert.That(parsed.Filename, Is.EqualTo(original.Filename));
            Assert.That(parsed.Mode, Is.EqualTo(original.Mode));
        });
    }

    [Test]
    public void ParsesWriteRequest()
    {
        var original = new WriteRequest("Hallo Welt.txt", TftpTransferMode.netascii, null);

        var parsed = (WriteRequest)CommandParser.Parse(Serialize(original));
        Assert.Multiple(() =>
        {
            Assert.That(parsed.Filename, Is.EqualTo(original.Filename));
            Assert.That(parsed.Mode, Is.EqualTo(original.Mode));
        });
    }

    private static byte[] Serialize(ITftpCommand command)
    {
        using var stream = new MemoryStream();
        var writer = new TftpStreamWriter(stream);
        command.WriteToStream(writer);
        byte[] commandAsBytes = stream.GetBuffer();
        Array.Resize(ref commandAsBytes, (int)stream.Length);
        return commandAsBytes;
    }
}
