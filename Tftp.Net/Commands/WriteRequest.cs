// <copyright file="WriteRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace Tftp.Net.Commands;

internal class WriteRequest(string filename, TftpTransferMode mode, IEnumerable<TransferOption> options)
    : ReadOrWriteRequest(OpCode, filename, mode, options), ITftpCommand
{
    public const ushort OpCode = 2;

    public static WriteRequest ReadFromStream(TftpStreamReader reader)
    {
        var filename = reader.ReadNullTerminatedString();
        var modeStr = reader.ReadNullTerminatedString();
        var validMode = Enum.TryParse<TftpTransferMode>(modeStr, true, out var mode);

        if (!validMode)
        {
            throw new TftpParserException($"Unknown mode type: {modeStr}");
        }

        var options = TransferOptionParser.Parse(reader);

        return new WriteRequest(filename, mode, options);
    }

    public void Visit(ITftpCommandVisitor visitor)
    {
        visitor.OnWriteRequest(this);
    }
}
