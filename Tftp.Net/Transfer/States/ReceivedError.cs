// <copyright file="ReceivedError.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class ReceivedError(ITftpTransferError error) : BaseState
{
    private readonly ITftpTransferError error = error;

    public ReceivedError(Error error)
        : this(new TftpErrorPacket(error.ErrorCode, GetNonEmptyErrorMessage(error)))
    {
    }

    public override void OnStateEnter()
    {
        TftpTrace.Trace("Received error: " + error, Context);
        Context.RaiseOnError(error);
        Context.SetState(new Closed());
    }

    private static string GetNonEmptyErrorMessage(Error error)
    {
        return string.IsNullOrEmpty(error.Message) ? "(No error message provided)" : error.Message;
    }
}
