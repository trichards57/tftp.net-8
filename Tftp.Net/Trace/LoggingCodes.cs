// <copyright file="LoggingCodes.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net.Trace;

internal static class LoggingCodes
{
    public const int StateCancelled = 1001;
    public const int StateDataReceived = 1002;
    public const int StateError = 1003;
    public const int StateEntered = 1004;
    public const int StateAcknowledged = 1005;
    public const int StateCommandReceived = 1006;
    public const int StateStarted = 1007;
    public const int StateTimer = 1008;
    public const int DataBlockReceived = 1101;
    public const int DataBlockResent = 1102;
    public const int TimedOut = 1200;
    public const int SuppressedUnhandledException = 2000;
}
