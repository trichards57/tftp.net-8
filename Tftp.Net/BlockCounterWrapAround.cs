// <copyright file="BlockCounterWrapAround.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net;

/// <summary>
/// Represents the behaviour when the block counter reaches the maximum value.
/// </summary>
public enum BlockCounterWrapAround
{
    /// <summary>
    /// The block counter will wrap around to zero.
    /// </summary>
    ToZero,

    /// <summary>
    /// The block counter will wrap around to one.
    /// </summary>
    ToOne,
}
