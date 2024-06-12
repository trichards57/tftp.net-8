// <copyright file="NetworkError.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tftp.Net;

/// <summary>
/// Network errors (i.e. socket exceptions) are represented by this class.
/// </summary>
public class NetworkError(Exception exception) : ITftpTransferError
{
    /// <summary>
    /// Gets the exception that was thrown.
    /// </summary>
    public Exception Exception { get; } = exception;

    /// <inheritdoc/>
    public override string ToString()
    {
        return Exception.ToString();
    }
}
