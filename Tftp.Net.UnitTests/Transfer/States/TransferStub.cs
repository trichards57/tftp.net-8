// <copyright file="TransferStub.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using Tftp.Net.Transfer;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests;

internal sealed class TransferStub : TftpTransfer
{
    public TransferStub(MemoryStream stream)
        : base(new ChannelStub(), "dummy.txt", new Uninitialized(), NullLogger.Instance)
    {
        InputOutputStream = stream;
        HadNetworkTimeout = false;
        OnError += new TftpErrorHandler(TransferStub_OnError);
    }

    public TransferStub()
        : this(null)
    {
    }

    public bool HadNetworkTimeout { get; set; }

    public List<ITftpCommand> SentCommands => Channel.SentCommands;

    public new ITransferState State => base.State;

    private ChannelStub Channel => (ChannelStub)Connection;

    public bool CommandWasSent(Type commandType)
    {
        return SentCommands.Exists(x => x.GetType().IsAssignableFrom(commandType));
    }

    public void OnCommand(ITftpCommand command)
    {
        State.OnCommand(command, GetConnection().RemoteEndpoint);
    }

    public void OnTimer()
    {
        State.OnTimer();
    }

    protected override void Dispose(bool disposing)
    {
        // Don't dispose the input/output stream during unit tests
        InputOutputStream = null;
    }

    private void TransferStub_OnError(ITftpTransfer transfer, ITftpTransferError error)
    {
        if (error is TimeoutError)
        {
            HadNetworkTimeout = true;
        }
    }

    private class Uninitialized : BaseState
    {
    }
}
