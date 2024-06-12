// <copyright file="Program.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Net;

namespace Tftp.Net.SampleServer;

internal static class Program
{
    private static string serverDirectory;

    private static void CancelTransfer(ITftpTransfer transfer, TftpErrorPacket reason)
    {
        OutputTransferStatus(transfer, "Cancelling transfer: " + reason.ErrorMessage);
        transfer.Cancel(reason);
    }

    private static void Main()
    {
        serverDirectory = Environment.CurrentDirectory;

        Console.WriteLine("Running TFTP server for directory: " + serverDirectory);
        Console.WriteLine();
        Console.WriteLine("Press any key to close the server.");

        using var server = new TftpServer();
        server.OnReadRequest += Server_OnReadRequest;
        server.OnWriteRequest += Server_OnWriteRequest;
        server.Start();
        Console.Read();
    }

    private static void OutputTransferStatus(ITftpTransfer transfer, string message)
    {
        Console.WriteLine("[" + transfer.Filename + "] " + message);
    }

    private static void Server_OnReadRequest(ITftpTransfer transfer, EndPoint client)
    {
        var path = Path.Combine(serverDirectory, transfer.Filename);
        var file = new FileInfo(path);

        // Is the file within the server directory?
        if (!file.FullName.StartsWith(serverDirectory, StringComparison.InvariantCultureIgnoreCase))
        {
            CancelTransfer(transfer, TftpErrorPacket.AccessViolation);
        }
        else if (!file.Exists)
        {
            CancelTransfer(transfer, TftpErrorPacket.FileNotFound);
        }
        else
        {
            OutputTransferStatus(transfer, "Accepting request from " + client);
            StartTransfer(transfer, new FileStream(file.FullName, FileMode.Open, FileAccess.Read));
        }
    }

    private static void Server_OnWriteRequest(ITftpTransfer transfer, EndPoint client)
    {
        var file = Path.Combine(serverDirectory, transfer.Filename);

        if (File.Exists(file))
        {
            CancelTransfer(transfer, TftpErrorPacket.FileAlreadyExists);
        }
        else
        {
            OutputTransferStatus(transfer, "Accepting write request from " + client);
            StartTransfer(transfer, new FileStream(file, FileMode.CreateNew));
        }
    }

    private static void StartTransfer(ITftpTransfer transfer, Stream stream)
    {
        transfer.OnProgress += Transfer_OnProgress;
        transfer.OnError += Transfer_OnError;
        transfer.OnFinished += Transfer_OnFinished;
        transfer.Start(stream);
    }

    private static void Transfer_OnError(ITftpTransfer transfer, ITftpTransferError error)
    {
        OutputTransferStatus(transfer, "Error: " + error);
    }

    private static void Transfer_OnFinished(ITftpTransfer transfer)
    {
        OutputTransferStatus(transfer, "Finished");
    }

    private static void Transfer_OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
    {
        OutputTransferStatus(transfer, "Progress " + progress);
    }
}
