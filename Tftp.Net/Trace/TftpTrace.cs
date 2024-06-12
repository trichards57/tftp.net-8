// <copyright file="TftpTrace.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net.Trace;

/// <summary>
/// Class that controls all tracing in the TFTP module.
/// </summary>
public static class TftpTrace
{
    static TftpTrace()
    {
        Enabled = false;
    }

    /// <summary>
    /// Set this property to <see langword="false" /> to disable tracing.
    /// </summary>
    public static bool Enabled { get; set; }

    internal static void Trace(string message, ITftpTransfer transfer)
    {
        if (!Enabled)
        {
            return;
        }

        System.Diagnostics.Trace.WriteLine(message, transfer.ToString());
    }
}
