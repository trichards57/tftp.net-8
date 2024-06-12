// <copyright file="ITftpTransferError.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net;

/// <summary>
/// Base class for all errors that may be passed to <see cref="ITftpTransfer.OnError"/>.
/// </summary>
public interface ITftpTransferError
{
    /// <summary>
    /// Converts the error to a string.
    /// </summary>
    /// <returns>The error as a string.</returns>
    string ToString();
}
