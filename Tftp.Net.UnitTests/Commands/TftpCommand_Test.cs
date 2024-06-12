// <copyright file="TftpCommand_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class TftpCommand_Test
{
    [Test]
    public void CreateAck()
    {
        var command = new Acknowledgement(100);
        Assert.That(command.BlockNumber, Is.EqualTo(100));
    }

    [Test]
    public void CreateData()
    {
        var data = new byte[] { 1, 2, 3 };
        var command = new Data(150, data);
        Assert.Multiple(() =>
        {
            Assert.That(command.BlockNumber, Is.EqualTo(150));
            Assert.That(data, Is.EqualTo(command.Bytes));
        });
    }

    [Test]
    public void CreateError()
    {
        var command = new Error(123, "Hallo Welt");
        Assert.Multiple(() =>
        {
            Assert.That(command.ErrorCode, Is.EqualTo(123));
            Assert.That(command.Message, Is.EqualTo("Hallo Welt"));
        });
    }

    [Test]
    public void CreateReadRequest()
    {
        var command = new ReadRequest(@"C:\bla\blub.txt", TftpTransferMode.octet, null);
        Assert.Multiple(() =>
        {
            Assert.That(command.Filename, Is.EqualTo(@"C:\bla\blub.txt"));
            Assert.That(command.Mode, Is.EqualTo(TftpTransferMode.octet));
        });
    }

    [Test]
    public void CreateWriteRequest()
    {
        var command = new WriteRequest(@"C:\bla\blub.txt", TftpTransferMode.octet, null);
        Assert.Multiple(() =>
        {
            Assert.That(command.Filename, Is.EqualTo(@"C:\bla\blub.txt"));
            Assert.That(command.Mode, Is.EqualTo(TftpTransferMode.octet));
        });
    }
}
