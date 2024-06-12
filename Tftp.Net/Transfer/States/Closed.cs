// <copyright file="Closed.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States;

internal class Closed(ILogger logger) : BaseState
{
    private readonly ILogger logger = logger;

    public override void OnStateEnter()
    {
        logger.StateEntered(nameof(Closed));
        Context.Dispose();
    }
}
