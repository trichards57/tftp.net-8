// <copyright file="Error.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text;

namespace Tftp.Net.Commands;

internal readonly record struct Error : ITftpCommand
{
    public const ushort OpCode = 5;

    public ushort ErrorCode { get; init; }

    public string Message { get; init; }

    public static Error ReadFromStream(TftpStreamReader reader)
    {
        var errorCode = reader.ReadUInt16();
        var message = reader.ReadNullTerminatedString();
        return new Error { ErrorCode = errorCode, Message = message };
    }

    public void WriteToStream(TftpStreamWriter writer)
    {
        writer.WriteUInt16(OpCode);
        writer.WriteUInt16(ErrorCode);
        writer.WriteBytes(Encoding.ASCII.GetBytes(Message));
        writer.WriteByte(0);
    }
}
