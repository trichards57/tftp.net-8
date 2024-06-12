// <copyright file="TransferOption.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace Tftp.Net;

/// <summary>
/// A single transfer options according to RFC2347.
/// </summary>
public sealed class TransferOption : IEquatable<TransferOption>
{
    internal TransferOption(string name, string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(value);

        Name = name;
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether the option has been acknowledged by the remote party.
    /// </summary>
    public bool IsAcknowledged { get; internal set; }

    /// <summary>
    /// Gets the name of the option.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets or sets the value of the option.
    /// </summary>
    public string Value { get; set; }

    public static bool operator !=(TransferOption left, TransferOption right)
    {
        return !(left == right);
    }

    public static bool operator ==(TransferOption left, TransferOption right)
    {
        return EqualityComparer<TransferOption>.Default.Equals(left, right);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        return Equals(obj as TransferOption);
    }

    /// <inheritdoc/>
    public bool Equals(TransferOption other)
    {
        return other is not null &&
               IsAcknowledged == other.IsAcknowledged &&
               Name == other.Name &&
               Value == other.Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(IsAcknowledged, Name, Value);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Name + "=" + Value;
    }
}
