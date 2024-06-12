// <copyright file="SimpleTimer_Test.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Tftp.Net.Transfer;

namespace Tftp.Net.UnitTests.Transfer;

[TestFixture]
internal class SimpleTimer_Test
{
    [Test]
    public void ImmediateTimeout()
    {
        var timer = new SimpleTimer(new TimeSpan(0));
        Assert.That(timer.IsTimeout(), Is.True);
    }

    [Test]
    public void RestartingResetsTimeout()
    {
        var timer = new SimpleTimer(new TimeSpan(100));
        Assert.That(timer.IsTimeout(), Is.False);
        Task.Delay(200).Wait();
        Assert.That(timer.IsTimeout(), Is.True);
        timer.Restart();
        Assert.That(timer.IsTimeout(), Is.False);
    }

    [Test]
    public void TimesOutWhenTimeoutIsReached()
    {
        var timer = new SimpleTimer(new TimeSpan(100));
        Assert.That(timer.IsTimeout(), Is.False);
        Task.Delay(200).Wait();
        Assert.That(timer.IsTimeout(), Is.True);
    }
}
