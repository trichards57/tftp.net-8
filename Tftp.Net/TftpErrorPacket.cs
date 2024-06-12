// <copyright file="TftpErrorPacket.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tftp.Net;

/// <summary>
/// Represents an error that has been sent by the remote part in a TFTP Error Packet.
/// </summary>
public class TftpErrorPacket : ITftpTransferError
{
    /// <summary>
    /// An error that indicates that the file path was not accessible.
    /// </summary>
    public static readonly TftpErrorPacket AccessViolation = new(2, "Access violation");

    /// <summary>
    /// An error that indicates that the disk is full or the allocation was exceeded.
    /// </summary>
    public static readonly TftpErrorPacket DiskFull = new(3, "Disk full or allocation exceeded");

    /// <summary>
    /// An error that indicates that the file already exists.
    /// </summary>
    public static readonly TftpErrorPacket FileAlreadyExists = new(6, "File already exists");

    /// <summary>
    /// An error that indicates that the file was not found.
    /// </summary>
    public static readonly TftpErrorPacket FileNotFound = new(1, "File not found");

    /// <summary>
    /// An error that indicates that an illegal operation was performed.
    /// </summary>
    public static readonly TftpErrorPacket IllegalOperation = new(4, "Illegal TFTP operation");

    /// <summary>
    /// An error that indicates that the user does not exist.
    /// </summary>
    public static readonly TftpErrorPacket NoSuchUser = new(7, "No such user");

    /// <summary>
    /// An error that indicates that the transfer ID was unknown.
    /// </summary>
    public static readonly TftpErrorPacket UnknownTransferId = new(5, "Unknown transfer ID");

    /// <summary>
    /// Initializes a new instance of the <see cref="TftpErrorPacket"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="errorMessage">The error message.</param>
    public TftpErrorPacket(ushort errorCode, string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Gets the error code that was sent from the other party.
    /// </summary>
    public ushort ErrorCode { get; private set; }

    /// <summary>
    /// Gets the error description that was sent by the other party.
    /// </summary>
    public string ErrorMessage { get; private set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return ErrorCode + " - " + ErrorMessage;
    }
}
