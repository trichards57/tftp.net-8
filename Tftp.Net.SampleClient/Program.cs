// <copyright file="Program.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Threading;

namespace Tftp.Net.SampleClient;

internal static class Program
{
    private static readonly AutoResetEvent TransferFinishedEvent = new(false);

    private static void Main()
    {
        // Setup a TftpClient instance
        var client = new TftpClient("localhost");

        // Prepare a simple transfer (GET test.dat)
        var transfer = client.Download("EUPL-EN.pdf");

        // Capture the events that may happen during the transfer
        transfer.OnProgress += Transfer_OnProgress;
        transfer.OnFinished += Transfer_OnFinished;
        transfer.OnError += Transfer_OnError;

        // Start the transfer and write the data that we're downloading into a memory stream
        Stream stream = new MemoryStream();
        transfer.Start(stream);

        // Wait for the transfer to finish
        TransferFinishedEvent.WaitOne();
        Console.ReadKey();
    }

    private static void Transfer_OnError(ITftpTransfer transfer, ITftpTransferError error)
    {
        Console.WriteLine("Transfer failed: " + error);
        TransferFinishedEvent.Set();
    }

    private static void Transfer_OnFinished(ITftpTransfer transfer)
    {
        Console.WriteLine("Transfer succeeded.");
        TransferFinishedEvent.Set();
    }

    private static void Transfer_OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
    {
        Console.WriteLine("Transfer running. Progress: " + progress);
    }
}
