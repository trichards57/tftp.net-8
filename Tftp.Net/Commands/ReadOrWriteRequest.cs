// <copyright file="ReadOrWriteRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Text;

namespace Tftp.Net.Commands;

internal abstract class ReadOrWriteRequest(ushort opCode, string filename, TftpTransferMode mode, IEnumerable<TransferOption> options)
{
    private readonly ushort opCode = opCode;

    public string Filename { get; private set; } = filename;

    public TftpTransferMode Mode { get; private set; } = mode;

    public IEnumerable<TransferOption> Options { get; private set; } = options;

    public void WriteToStream(TftpStreamWriter writer)
    {
        writer.WriteUInt16(opCode);
        writer.WriteBytes(Encoding.ASCII.GetBytes(Filename));
        writer.WriteByte(0);
        writer.WriteBytes(Encoding.ASCII.GetBytes(Mode.ToString()));
        writer.WriteByte(0);

        if (Options != null)
        {
            foreach (var option in Options)
            {
                writer.WriteBytes(Encoding.ASCII.GetBytes(option.Name));
                writer.WriteByte(0);
                writer.WriteBytes(Encoding.ASCII.GetBytes(option.Value));
                writer.WriteByte(0);
            }
        }
    }
}
