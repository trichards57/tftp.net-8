// <copyright file="TftpClientServer_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class TftpClientServer_Test
{
    private readonly byte[] demoData = [1, 2, 3];
    private bool transferHasFinished = false;

    [Test]
    public async Task ClientsReadsFromServer()
    {
        using var server = new TftpServer(new IPEndPoint(IPAddress.Loopback, 69));
        server.OnReadRequest += new TftpServerEventHandler(Server_OnReadRequest);
        server.Start();

        var client = new TftpClient(new IPEndPoint(IPAddress.Loopback, 69));
        using var transfer = client.Download("Demo File");
        var ms = new MemoryStream();
        transfer.OnFinished += new TftpEventHandler(Transfer_OnFinished);
        transfer.Start(ms);

        await Task.Delay(500);
        Assert.That(transferHasFinished, Is.True);
    }

    private void Server_OnReadRequest(ITftpTransfer transfer, EndPoint client)
    {
        transfer.Start(new MemoryStream(demoData));
    }

    private void Transfer_OnFinished(ITftpTransfer transfer)
    {
        transferHasFinished = true;
    }
}
