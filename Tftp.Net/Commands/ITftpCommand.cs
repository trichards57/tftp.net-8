// <copyright file="ITftpCommand.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net;

/// <summary>
/// Represents a TFTP Command.
/// </summary>
internal interface ITftpCommand
{
    /// <summary>
    /// Called to raise hte appropriate event on the visitor.
    /// </summary>
    /// <param name="visitor">The command visitor.</param>
    void Visit(ITftpCommandVisitor visitor);
}
