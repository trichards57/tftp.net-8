// <copyright file="Closed.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net.Transfer.States;

internal class Closed : BaseState
{
    public override void OnStateEnter()
    {
        Context.Dispose();
    }
}
