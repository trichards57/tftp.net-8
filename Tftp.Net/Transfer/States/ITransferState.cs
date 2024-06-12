// <copyright file="ITransferState.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Net;

namespace Tftp.Net.Transfer.States;

/// <summary>
/// Represents a state in the TFTP transfer state machine.
/// </summary>
internal interface ITransferState
{
    /// <summary>
    /// Gets or sets the transfer context.
    /// </summary>
    TftpTransfer Context { get; set; }

    /// <summary>
    /// Called when the transfer is cancelled.
    /// </summary>
    /// <param name="reason">The reason for the cancellation.</param>
    void OnCancel(TftpErrorPacket reason);

    /// <summary>
    /// Called when a command is received.
    /// </summary>
    /// <param name="command">The received command.</param>
    /// <param name="endpoint">The endpoint the command was received from.</param>
    void OnCommand(ITftpCommand command, IPEndPoint endpoint);

    /// <summary>
    /// Called when the transfer is started.
    /// </summary>
    void OnStart();

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    void OnStateEnter();

    /// <summary>
    /// Called regularly while the state is active.
    /// </summary>
    void OnTimer();
}
