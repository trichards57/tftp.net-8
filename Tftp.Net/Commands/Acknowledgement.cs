// <copyright file="Acknowledgement.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net.Commands;

internal readonly record struct Acknowledgement : ITftpCommand
{
    public const ushort OpCode = 4;

    public ushort BlockNumber { get; init; }

    public static Acknowledgement ReadFromStream(TftpStreamReader reader)
    {
        var blockNumber = reader.ReadUInt16();
        return new Acknowledgement { BlockNumber = blockNumber };
    }

    public void WriteToStream(TftpStreamWriter writer)
    {
        writer.WriteUInt16(OpCode);
        writer.WriteUInt16(BlockNumber);
    }
}
