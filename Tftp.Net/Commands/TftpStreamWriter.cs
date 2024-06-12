// <copyright file="TftpStreamWriter.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;

namespace Tftp.Net;

internal class TftpStreamWriter
{
    private readonly Stream stream;

    public TftpStreamWriter(Stream stream)
    {
        this.stream = stream;
    }

    public void WriteUInt16(ushort value)
    {
        stream.WriteByte((byte)(value >> 8));
        stream.WriteByte((byte)(value & 0xFF));
    }

    public void WriteByte(byte b)
    {
        stream.WriteByte(b);
    }

    public void WriteBytes(byte[] data)
    {
        stream.Write(data, 0, data.Length);
    }
}
