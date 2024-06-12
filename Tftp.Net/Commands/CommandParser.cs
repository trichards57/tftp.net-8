// <copyright file="CommandParser.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tftp.Net;

/// <summary>
/// Parses a ITftpCommand.
/// </summary>
internal static class CommandParser
{
    /// <summary>
    /// Parses an ITftpCommand from the given byte array. If the byte array cannot be parsed for some reason, a TftpParserException is thrown.
    /// </summary>
    public static ITftpCommand Parse(byte[] message)
    {
        try
        {
            return ParseInternal(message);
        }
        catch (TftpParserException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new TftpParserException(e);
        }
    }

    private static Acknowledgement ParseAcknowledgement(TftpStreamReader reader)
    {
        ushort blockNumber = reader.ReadUInt16();
        return new Acknowledgement(blockNumber);
    }

    private static Data ParseData(TftpStreamReader reader)
    {
        ushort blockNumber = reader.ReadUInt16();
        byte[] data = reader.ReadBytes(10000);
        return new Data(blockNumber, data);
    }

    private static Error ParseError(TftpStreamReader reader)
    {
        ushort errorCode = reader.ReadUInt16();
        string message = ParseNullTerminatedString(reader);
        return new Error(errorCode, message);
    }

    private static ITftpCommand ParseInternal(byte[] message)
    {
        var reader = new TftpStreamReader(new MemoryStream(message));

        ushort opcode = reader.ReadUInt16();
        return opcode switch
        {
            ReadRequest.OpCode => ParseReadRequest(reader),
            WriteRequest.OpCode => ParseWriteRequest(reader),
            Data.OpCode => ParseData(reader),
            Acknowledgement.OpCode => ParseAcknowledgement(reader),
            Error.OpCode => ParseError(reader),
            OptionAcknowledgement.OpCode => ParseOptionAcknowledgement(reader),
            _ => throw new TftpParserException("Invalid opcode"),
        };
    }

    private static TftpTransferMode ParseModeType(string mode)
    {
        mode = mode.ToLowerInvariant();

        return mode switch
        {
            "netascii" => TftpTransferMode.netascii,
            "mail" => TftpTransferMode.mail,
            "octet" => TftpTransferMode.octet,
            _ => throw new TftpParserException($"Unknown mode type: {mode}"),
        };
    }

    private static string ParseNullTerminatedString(TftpStreamReader reader)
    {
        byte b;
        var str = new StringBuilder();
        while ((b = reader.ReadByte()) > 0)
        {
            str.Append((char)b);
        }

        return str.ToString();
    }

    private static OptionAcknowledgement ParseOptionAcknowledgement(TftpStreamReader reader)
    {
        IEnumerable<TransferOption> options = ParseTransferOptions(reader);
        return new OptionAcknowledgement(options);
    }

    private static ReadRequest ParseReadRequest(TftpStreamReader reader)
    {
        string filename = ParseNullTerminatedString(reader);
        TftpTransferMode mode = ParseModeType(ParseNullTerminatedString(reader));
        IEnumerable<TransferOption> options = ParseTransferOptions(reader);
        return new ReadRequest(filename, mode, options);
    }

    private static List<TransferOption> ParseTransferOptions(TftpStreamReader reader)
    {
        var options = new List<TransferOption>();

        while (true)
        {
            string name;

            try
            {
                name = ParseNullTerminatedString(reader);
            }
            catch (IOException)
            {
                name = string.Empty;
            }

            if (name.Length == 0)
            {
                break;
            }

            string value = ParseNullTerminatedString(reader);
            options.Add(new TransferOption(name, value));
        }

        return options;
    }

    private static WriteRequest ParseWriteRequest(TftpStreamReader reader)
    {
        string filename = ParseNullTerminatedString(reader);
        TftpTransferMode mode = ParseModeType(ParseNullTerminatedString(reader));
        IEnumerable<TransferOption> options = ParseTransferOptions(reader);
        return new WriteRequest(filename, mode, options);
    }
}
