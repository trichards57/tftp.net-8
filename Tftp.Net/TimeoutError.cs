// <copyright file="TimeoutError.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tftp.Net;

/// <summary>
/// An error raised if <see cref="ITftpTransfer.RetryTimeout" /> has been exceeded more than <see cref="ITftpTransfer.RetryCount" /> times in a row.
/// </summary>
public class TimeoutError(TimeSpan retryTimeout, int retryCount) : ITftpTransferError
{
    private readonly int retryCount = retryCount;
    private readonly TimeSpan retryTimeout = retryTimeout;

    /// <inheritdoc/>
    public override string ToString()
    {
        return "Timeout error. RetryTimeout (" + retryTimeout + ") violated more than " + retryCount + " times in a row.";
    }
}
