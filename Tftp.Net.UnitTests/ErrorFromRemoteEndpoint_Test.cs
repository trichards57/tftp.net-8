// <copyright file="ErrorFromRemoteEndpoint_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;

namespace Tftp.Net.UnitTests;

[TestFixture]
internal class ErrorFromRemoteEndpoint_Test
{
    [Test]
    public void CanBeCreatedWithValidValues()
    {
        var error = new TftpErrorPacket(123, "Test Message");
        Assert.Multiple(() =>
        {
            Assert.That(error.ErrorCode, Is.EqualTo(123));
            Assert.That(error.ErrorMessage, Is.EqualTo("Test Message"));
        });
    }

    [Test]
    public void RejectsEmptyMessage()
    {
        Assert.That(() => new TftpErrorPacket(123, string.Empty), Throws.ArgumentException);
    }

    [Test]
    public void RejectsNullMessage()
    {
        Assert.That(() => new TftpErrorPacket(123, null), Throws.ArgumentNullException);
    }
}
