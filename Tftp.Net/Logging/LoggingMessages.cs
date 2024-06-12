// <copyright file="LoggingMessages.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using System;
using Tftp.Net.Logging;

namespace Tftp.Net.Trace;

internal static partial class LoggingMessages
{
    [LoggerMessage(EventId = LoggingCodes.DataBlockReceived, Level = LogLevel.Information, Message = "Data block {blockNumber} of {filename} received.")]
    public static partial void DataBlockReceived(this ILogger logger, ushort blockNumber, string filename);

    [LoggerMessage(EventId = LoggingCodes.DataBlockResent, Level = LogLevel.Information, Message = "Data block {blockNumber} of {filename} received again.")]
    public static partial void DataBlockResent(this ILogger logger, ushort blockNumber, string filename);

    [LoggerMessage(EventId = LoggingCodes.StateCancelled, Level = LogLevel.Information, Message = "State {state} cancelled : {code} {reason}.")]
    public static partial void StateCancelled(this ILogger logger, string state, ushort code, string reason);

    [LoggerMessage(EventId = LoggingCodes.StateDataReceived, Level = LogLevel.Information, Message = "State {state} received data.")]
    public static partial void StateDataReceived(this ILogger logger, string state);

    [LoggerMessage(EventId = LoggingCodes.StateCommandReceived, Level = LogLevel.Information, Message = "State {state} received a command.")]
    public static partial void StateCommandReceived(this ILogger logger, string state);

    [LoggerMessage(EventId = LoggingCodes.StateAcknowledged, Level = LogLevel.Information, Message = "State {state} received acknowledgement for {blockNumber}.")]
    public static partial void StateAcknowledged(this ILogger logger, string state, ushort blockNumber);

    [LoggerMessage(EventId = LoggingCodes.StateEntered, Level = LogLevel.Information, Message = "State {state} entered.")]
    public static partial void StateEntered(this ILogger logger, string state);

    [LoggerMessage(EventId = LoggingCodes.StateStarted, Level = LogLevel.Information, Message = "State {state} started.")]
    public static partial void StateStarted(this ILogger logger, string state);

    [LoggerMessage(EventId = LoggingCodes.StateError, Level = LogLevel.Error, Message = "State {state} received error : {code} {message}.")]
    public static partial void StateError(this ILogger logger, string state, ushort code, string message);

    [LoggerMessage(EventId = LoggingCodes.StateTimer, Level = LogLevel.Information, Message = "State {state} received timer tick.")]
    public static partial void StateTimer(this ILogger logger, string state);

    [LoggerMessage(EventId = LoggingCodes.TimedOut, Level = LogLevel.Error, Message = "A request timed out.")]
    public static partial void TimedOut(this ILogger logger);

    [LoggerMessage(EventId = LoggingCodes.SuppressedUnhandledException, Level = LogLevel.Error, Message = "An unhandled exception was thrown and suppressed.")]
    public static partial void SuppressedUnhandledException(this ILogger logger, Exception exception);
}
