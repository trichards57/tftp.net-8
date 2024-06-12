// <copyright file="CommandSerializer.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;
using System.Text;

namespace Tftp.Net;

/// <summary>
/// Serializes an ITftpCommand into a stream of bytes.
/// </summary>
internal static class CommandSerializer
{
    public static void Serialize(ITftpCommand command, Stream stream)
    {
        var visitor = new CommandComposerVisitor(stream);
        command.Visit(visitor);
    }

    private sealed class CommandComposerVisitor(Stream stream) : ITftpCommandVisitor
    {
        private readonly TftpStreamWriter writer = new(stream);

        public void OnAcknowledgement(Acknowledgement command)
        {
            writer.WriteUInt16(Acknowledgement.OpCode);
            writer.WriteUInt16(command.BlockNumber);
        }

        public void OnData(Data command)
        {
            writer.WriteUInt16(Data.OpCode);
            writer.WriteUInt16(command.BlockNumber);
            writer.WriteBytes(command.Bytes);
        }

        public void OnError(Error command)
        {
            writer.WriteUInt16(Error.OpCode);
            writer.WriteUInt16(command.ErrorCode);
            writer.WriteBytes(Encoding.ASCII.GetBytes(command.Message));
            writer.WriteByte(0);
        }

        public void OnOptionAcknowledgement(OptionAcknowledgement command)
        {
            writer.WriteUInt16(OptionAcknowledgement.OpCode);

            foreach (var option in command.Options)
            {
                writer.WriteBytes(Encoding.ASCII.GetBytes(option.Name));
                writer.WriteByte(0);
                writer.WriteBytes(Encoding.ASCII.GetBytes(option.Value));
                writer.WriteByte(0);
            }
        }

        public void OnReadRequest(ReadRequest command)
        {
            writer.WriteUInt16(ReadRequest.OpCode);
            OnReadOrWriteRequest(command);
        }

        public void OnWriteRequest(WriteRequest command)
        {
            writer.WriteUInt16(WriteRequest.OpCode);
            OnReadOrWriteRequest(command);
        }

        private void OnReadOrWriteRequest(ReadOrWriteRequest command)
        {
            writer.WriteBytes(Encoding.ASCII.GetBytes(command.Filename));
            writer.WriteByte(0);
            writer.WriteBytes(Encoding.ASCII.GetBytes(command.Mode.ToString()));
            writer.WriteByte(0);

            if (command.Options != null)
            {
                foreach (var option in command.Options)
                {
                    writer.WriteBytes(Encoding.ASCII.GetBytes(option.Name));
                    writer.WriteByte(0);
                    writer.WriteBytes(Encoding.ASCII.GetBytes(option.Value));
                    writer.WriteByte(0);
                }
            }
        }
    }
}
