// <copyright file="OptionAcknowledgement.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Tftp.Net;

internal class OptionAcknowledgement(IEnumerable<TransferOption> options) : ITftpCommand
{
    public const ushort OpCode = 6;

    public IEnumerable<TransferOption> Options { get; } = options;

    public void Visit(ITftpCommandVisitor visitor)
    {
        visitor.OnOptionAcknowledgement(this);
    }
}
