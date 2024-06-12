// <copyright file="ReadRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Tftp.Net;

internal class ReadRequest(string filename, TftpTransferMode mode, IEnumerable<TransferOption> options)
    : ReadOrWriteRequest(OpCode, filename, mode, options), ITftpCommand
{
    public const ushort OpCode = 1;

    public void Visit(ITftpCommandVisitor visitor)
    {
        visitor.OnReadRequest(this);
    }
}
