// <copyright file="ReceivedError.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class ReceivedError(ITftpTransferError error, ILogger logger) : BaseState
{
    private readonly ILogger logger = logger;
    private readonly ITftpTransferError error = error;

    public ReceivedError(Error error, ILogger logger)
        : this(new TftpErrorPacket(error.ErrorCode, GetNonEmptyErrorMessage(error)), logger)
    {
    }

    public override void OnStateEnter()
    {
        logger.StateError(nameof(ReceivedError), 0, error.ToString());
        Context.RaiseOnError(error);
        Context.SetState(new Closed(logger));
    }

    private static string GetNonEmptyErrorMessage(Error error)
    {
        return string.IsNullOrEmpty(error.Message) ? "(No error message provided)" : error.Message;
    }
}
