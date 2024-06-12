// <copyright file="TftpParserException.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tftp.Net.Commands;

/// <summary>
/// Represents an error that occurs when parsing a TFTP message.
/// </summary>
public class TftpParserException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TftpParserException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TftpParserException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TftpParserException"/> class.
    /// </summary>
    /// <param name="e">The inner exception.</param>
    public TftpParserException(Exception e)
        : base("Error while parsing message.", e)
    {
    }
}
