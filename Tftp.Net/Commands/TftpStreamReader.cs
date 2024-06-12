// <copyright file="TftpStreamReader.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;

namespace Tftp.Net;

internal class TftpStreamReader(Stream stream) : IDisposable
{
    private readonly BinaryReader reader = new(stream);
    private bool disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public byte ReadByte()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        return reader.ReadByte();
    }

    public byte[] ReadBytes(int maxBytes)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        return reader.ReadBytes(maxBytes);
    }

    public ushort ReadUInt16()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        var byte1 = reader.ReadByte();
        var byte2 = reader.ReadByte();
        return (ushort)(byte1 << 8 | byte2);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        if (disposing)
        {
            reader.Dispose();
        }

        disposed = true;
    }
}
