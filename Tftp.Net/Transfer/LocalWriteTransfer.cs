// <copyright file="LocalWriteTransfer.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Tftp.Net.Channel;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer;

internal class LocalWriteTransfer(ITransferChannel connection, string filename, IEnumerable<TransferOption> options, ILogger logger) 
    : TftpTransfer(connection, filename, new StartIncomingWrite(options, logger), logger)
{
    public override TftpTransferMode TransferMode
    {
        get { return base.TransferMode; }
        set { throw new NotSupportedException("Cannot change the transfer mode for incoming transfers. The transfer mode is determined by the client."); }
    }

    public override int BlockSize
    {
        get { return base.BlockSize; }
        set { throw new NotSupportedException("For incoming transfers, the blocksize is determined by the client."); }
    }

    public override TimeSpan RetryTimeout
    {
        get { return base.RetryTimeout; }
        set { throw new NotSupportedException("For incoming transfers, the retry timeout is determined by the client."); }
    }

    public override long ExpectedSize
    {
        get { return base.ExpectedSize; }
        set { throw new NotSupportedException("You cannot set the expected size of a file that is remotely transferred to this system."); }
    }
}
