// <copyright file="Error.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Text;

namespace Tftp.Net;

internal class Error(ushort errorCode, string message) : ITftpCommand
{
    public const ushort OpCode = 5;

    public ushort ErrorCode { get; private set; } = errorCode;

    public string Message { get; private set; } = message;

    public void Visit(ITftpCommandVisitor visitor)
    {
        visitor.OnError(this);
    }

    public void WriteToStream(TftpStreamWriter writer)
    {
        writer.WriteUInt16(OpCode);
        writer.WriteUInt16(ErrorCode);
        writer.WriteBytes(Encoding.ASCII.GetBytes(Message));
        writer.WriteByte(0);
    }
}
