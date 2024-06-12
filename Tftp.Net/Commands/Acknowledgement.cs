// <copyright file="Acknowledgement.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net;

internal class Acknowledgement(ushort blockNumber) : ITftpCommand
{
    public const ushort OpCode = 4;

    public ushort BlockNumber { get; private set; } = blockNumber;

    public void Visit(ITftpCommandVisitor visitor)
    {
        visitor.OnAcknowledgement(this);
    }

    public void WriteToStream(TftpStreamWriter writer)
    {
        writer.WriteUInt16(OpCode);
        writer.WriteUInt16(BlockNumber);
    }
}
