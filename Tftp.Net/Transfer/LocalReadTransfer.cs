// <copyright file="LocalReadTransfer.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Tftp.Net.Channel;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer;

internal class LocalReadTransfer(ITransferChannel connection, string filename, IEnumerable<TransferOption> options) : TftpTransfer(connection, filename, new StartIncomingRead(options))
{
    public override int BlockSize
    {
        get => base.BlockSize;
        set => throw new NotSupportedException("For incoming transfers, the block size is determined by the client.");
    }

    public override TimeSpan RetryTimeout
    {
        get => base.RetryTimeout;
        set => throw new NotSupportedException("For incoming transfers, the retry timeout is determined by the client.");
    }

    public override TftpTransferMode TransferMode
    {
        get => base.TransferMode;
        set => throw new NotSupportedException("Cannot change the transfer mode for incoming transfers. The transfer mode is determined by the client.");
    }
}
