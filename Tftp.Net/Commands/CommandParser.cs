// <copyright file="CommandParser.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;

namespace Tftp.Net.Commands;

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
            var reader = new TftpStreamReader(new MemoryStream(message));

            var opcode = reader.ReadUInt16();

            return opcode switch
            {
                ReadRequest.OpCode => ReadRequest.ReadFromStream(reader),
                WriteRequest.OpCode => WriteRequest.ReadFromStream(reader),
                Data.OpCode => Data.ReadFromStream(reader),
                Acknowledgement.OpCode => Acknowledgement.ReadFromStream(reader),
                Error.OpCode => Error.ReadFromStream(reader),
                OptionAcknowledgement.OpCode => OptionAcknowledgement.ReadFromStream(reader),
                _ => throw new TftpParserException("Invalid opcode"),
            };
        }
        catch (Exception e) when (e is not TftpParserException)
        {
            throw new TftpParserException(e);
        }
    }
}
