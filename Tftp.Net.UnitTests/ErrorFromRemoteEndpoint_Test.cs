// <copyright file="ErrorFromRemoteEndpoint_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using System;
using Xunit;

namespace Tftp.Net.UnitTests;

public class ErrorFromRemoteEndpoint_Test
{
    [Fact]
    public void CanBeCreatedWithValidValues()
    {
        var error = new TftpErrorPacket(123, "Test Message");
        error.ErrorCode.Should().Be(123);
        error.ErrorMessage.Should().Be("Test Message");
    }

    [Fact]
    public void RejectsEmptyMessage()
    {
        var func = () => new TftpErrorPacket(123, string.Empty);

        func.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RejectsNullMessage()
    {
        var func = () => new TftpErrorPacket(123, null);

        func.Should().Throw<ArgumentNullException>();
    }
}
