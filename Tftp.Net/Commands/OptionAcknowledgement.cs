// <copyright file="OptionAcknowledgement.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Text;

namespace Tftp.Net;

internal class OptionAcknowledgement(IEnumerable<TransferOption> options) : ITftpCommand
{
    public const ushort OpCode = 6;

    public IEnumerable<TransferOption> Options { get; } = options;

    public static OptionAcknowledgement ReadFromStream(TftpStreamReader reader)
    {
        var options = TransferOptionParser.Parse(reader);
        return new OptionAcknowledgement(options);
    }

    public void Visit(ITftpCommandVisitor visitor)
    {
        visitor.OnOptionAcknowledgement(this);
    }

    public void WriteToStream(TftpStreamWriter writer)
    {
        writer.WriteUInt16(OpCode);

        foreach (var option in Options)
        {
            writer.WriteBytes(Encoding.ASCII.GetBytes(option.Name));
            writer.WriteByte(0);
            writer.WriteBytes(Encoding.ASCII.GetBytes(option.Value));
            writer.WriteByte(0);
        }
    }
}
