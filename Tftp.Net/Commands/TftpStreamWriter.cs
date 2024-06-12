// <copyright file="TftpStreamWriter.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;

namespace Tftp.Net.Commands;

internal class TftpStreamWriter(Stream stream, bool disposeStream = false) : IDisposable
{
    private readonly BinaryWriter writer = new(stream);
    private readonly bool disposeStream = disposeStream;
    private bool disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void WriteUInt16(ushort value)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        writer.Write((byte)(value >> 8));
        writer.Write((byte)(value & 0xFF));
    }

    public void WriteByte(byte b)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        writer.Write(b);
    }

    public void WriteBytes(Span<byte> data)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        writer.Write(data);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        if (disposing && disposeStream)
        {
            writer.Dispose();
        }

        disposed = true;
    }
}
