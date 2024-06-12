// <copyright file="TftpTransferProgress.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net;

/// <summary>
/// Represents a progress report from a transfer.
/// </summary>
/// <param name="transferred">The amount of data that has been transferred.</param>
/// <param name="total">The total amount of data, or 0 if not known.</param>
public class TftpTransferProgress(long transferred, long total)
{
    /// <summary>
    /// Gets the total number of bytes being transferred, or 0 if not known.
    /// </summary>
    public long TotalBytes { get; } = total;

    /// <summary>
    /// Gets the number of bytes that have already been transferred.
    /// </summary>
    public long TransferredBytes { get; } = transferred;

    /// <inheritdoc/>
    public override string ToString()
    {
        if (TotalBytes > 0)
        {
            return ((TransferredBytes * 100L) / TotalBytes) + "% completed";
        }
        else
        {
            return TransferredBytes + " bytes transferred";
        }
    }
}
