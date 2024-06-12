// <copyright file="TransferOption.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tftp.Net;

/// <summary>
/// A single transfer options according to RFC2347.
/// </summary>
public readonly record struct TransferOption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransferOption"/> struct.
    /// </summary>
    /// <param name="name">The name of the option.</param>
    /// <param name="value">The value of the option.</param>
    /// <param name="isAcknowledged">Indicates if the option has been acknowledged.</param>
    public TransferOption(string name, string value, bool isAcknowledged = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(value);

        Name = name;
        Value = value;
        IsAcknowledged = isAcknowledged;
    }

    /// <summary>
    /// Gets a value indicating whether the option has been acknowledged by the remote party.
    /// </summary>
    public bool IsAcknowledged { get; init; }

    /// <summary>
    /// Gets the name of the option.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the value of the option.
    /// </summary>
    public string Value { get; init; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Name + "=" + Value;
    }
}
