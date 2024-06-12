// <copyright file="TransferOption_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using System;
using Tftp.Net.Commands;
using Xunit;

namespace Tftp.Net.UnitTests.TransferOptions;

public class TransferOption_Test
{
    [Fact]
    public void AcceptsEmptyValue()
    {
        var func = () => new TransferOption("Test", string.Empty);

        func.Should().NotThrow();
    }

    [Fact]
    public void CanBeCreatedWithValidNameAndValue()
    {
        var option = new TransferOption("Test", "Hallo Welt");
        option.Name.Should().Be("Test");
        option.Value.Should().Be("Hallo Welt");
        option.IsAcknowledged.Should().BeFalse();
    }

    [Fact]
    public void RejectsInvalidName1()
    {
        var func = () => new TransferOption(string.Empty, "Hallo Welt");

        func.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RejectsInvalidName2()
    {
        var func = () => new TransferOption(null, "Hallo Welt");

        func.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RejectsInvalidValue()
    {
        var func = () => new TransferOption("Test", null);

        func.Should().Throw<ArgumentNullException>();
    }
}
