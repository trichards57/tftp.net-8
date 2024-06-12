// <copyright file="ITftpCommand.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net.Commands;

/// <summary>
/// Represents a TFTP Command.
/// </summary>
internal interface ITftpCommand
{
    /// <summary>
    /// Called to write the command to the stream.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    void WriteToStream(TftpStreamWriter writer);
}
