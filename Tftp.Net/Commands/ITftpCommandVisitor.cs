// <copyright file="ITftpCommandVisitor.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net.Commands;

/// <summary>
/// Interface for a visitor pattern for TFTP commands.
/// </summary>
internal interface ITftpCommandVisitor
{
    /// <summary>
    /// Called when an acknowledgement command is received.
    /// </summary>
    /// <param name="command">The received command.</param>
    void OnAcknowledgement(Acknowledgement command);

    /// <summary>
    /// Called when data is received.
    /// </summary>
    /// <param name="command">The received command.</param>
    void OnData(Data command);

    /// <summary>
    /// Called when an error is received.
    /// </summary>
    /// <param name="command">The received command.</param>
    void OnError(Error command);

    /// <summary>
    /// Called when an option acknowledgement is received.
    /// </summary>
    /// <param name="command">The received command.</param>
    void OnOptionAcknowledgement(OptionAcknowledgement command);

    /// <summary>
    /// Called when a read request is received.
    /// </summary>
    /// <param name="command">The received command.</param>
    void OnReadRequest(ReadRequest command);

    /// <summary>
    /// Called when a write request is received.
    /// </summary>
    /// <param name="command">The received command.</param>
    void OnWriteRequest(WriteRequest command);
}
