﻿// <copyright file="TftpStreamReader.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Text;

namespace Tftp.Net.Commands;

internal class TftpStreamReader(Stream stream, bool disposeStream = false) : IDisposable
{
    private readonly BinaryReader reader = new(stream);
    private readonly bool disposeStream = disposeStream;
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

    public string ReadNullTerminatedString()
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        var builder = new StringBuilder();

        while (true)
        {
            var b = reader.ReadChar();

            if (b == '\0')
            {
                break;
            }

            builder.Append(b);
        }

        return builder.ToString();
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

        if (disposing && disposeStream)
        {
            reader.Dispose();
        }

        disposed = true;
    }
}
