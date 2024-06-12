// <copyright file="SimpleTimer.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tftp.Net.Transfer;

/// <summary>
/// Simple implementation of a timer.
/// </summary>
internal class SimpleTimer
{
    private readonly TimeSpan timeout;
    private DateTime nextTimeout;

    public SimpleTimer(TimeSpan timeout)
    {
        this.timeout = timeout;
        Restart();
    }

    public bool IsTimeout()
    {
        return DateTime.Now >= nextTimeout;
    }

    public void Restart()
    {
        nextTimeout = DateTime.Now.Add(timeout);
    }
}
