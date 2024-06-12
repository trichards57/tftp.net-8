// <copyright file="LocalWriteTransfer.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using Tftp.Net.Channel;

namespace Tftp.Net.Transfer;

internal sealed class LocalWriteTransfer(ITransferChannel connection) : ITftpTransfer
{
    private readonly ITransferChannel connection = connection;

    public event TftpErrorHandler OnError;

    public event TftpEventHandler OnFinished;

    public event TftpProgressHandler OnProgress;

    public BlockCounterWrapAround BlockCounterWrapping { get; set; }

    public int BlockSize { get; set; }

    public long ExpectedSize { get; set; }

    public string Filename { get; }

    public int RetryCount { get; set; }

    public TimeSpan RetryTimeout { get; set; }

    public TftpTransferMode TransferMode { get; set; }

    public object UserContext { get; set; }

    public void Cancel(TftpErrorPacket reason)
    {
    }

    public void Dispose()
    {
    }

    public void Start(Stream data)
    {
        connection.Send(new Error { ErrorCode = 2, Message = "Write-Access is not supported." });
    }
}
