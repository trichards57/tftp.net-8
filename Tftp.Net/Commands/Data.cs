// <copyright file="Data.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net;

internal class Data(ushort blockNumber, byte[] data) : ITftpCommand
{
    public const ushort OpCode = 3;

    public ushort BlockNumber { get; private set; } = blockNumber;

    public byte[] Bytes { get; private set; } = data;

    public static Data ReadFromStream(TftpStreamReader reader)
    {
        var blockNumber = reader.ReadUInt16();
        var data = reader.ReadBytes(10000);
        return new Data(blockNumber, data);
    }

    public void Visit(ITftpCommandVisitor visitor)
    {
        visitor.OnData(this);
    }

    public void WriteToStream(TftpStreamWriter writer)
    {
        writer.WriteUInt16(OpCode);
        writer.WriteUInt16(BlockNumber);
        writer.WriteBytes(Bytes);
    }
}
