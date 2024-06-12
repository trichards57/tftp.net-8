// <copyright file="TftpCommand_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Tftp.Net.Commands;
using Xunit;

namespace Tftp.Net.UnitTests;

public class TftpCommand_Test
{
    [Fact]
    public void CreateReadRequest()
    {
        var command = new ReadRequest(@"C:\bla\blub.txt", TftpTransferMode.octet, null);
        command.Filename.Should().Be(@"C:\bla\blub.txt");
        command.Mode.Should().Be(TftpTransferMode.octet);
    }

    [Fact]
    public void CreateWriteRequest()
    {
        var command = new WriteRequest(@"C:\bla\blub.txt", TftpTransferMode.octet, null);
        command.Filename.Should().Be(@"C:\bla\blub.txt");
        command.Mode.Should().Be(TftpTransferMode.octet);
    }
}
