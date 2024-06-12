// <copyright file="StateWithNetworkTimeout.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class StateWithNetworkTimeout : BaseState
{
    private ITftpCommand lastCommand;
    private int retriesUsed = 0;
    private SimpleTimer timer;

    /// <inheritdoc/>
    public override void OnStateEnter()
    {
        timer = new SimpleTimer(Context.RetryTimeout);
    }

    /// <inheritdoc/>
    public override void OnTimer()
    {
        if (timer.IsTimeout())
        {
            TftpTrace.Trace("Network timeout.", Context);
            timer.Restart();

            if (retriesUsed++ >= Context.RetryCount)
            {
                ITftpTransferError error = new TimeoutError(Context.RetryTimeout, Context.RetryCount);
                Context.SetState(new ReceivedError(error));
            }
            else
            {
                HandleTimeout();
            }
        }
    }

    protected void ResetTimeout()
    {
        timer.Restart();
        retriesUsed = 0;
    }

    protected void SendAndRepeat(ITftpCommand command)
    {
        Context.GetConnection().Send(command);
        lastCommand = command;
        ResetTimeout();
    }

    private void HandleTimeout()
    {
        if (lastCommand != null)
        {
            Context.GetConnection().Send(lastCommand);
        }
    }
}
