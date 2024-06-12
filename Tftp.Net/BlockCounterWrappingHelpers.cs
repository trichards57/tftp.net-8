// <copyright file="BlockCounterWrappingHelpers.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tftp.Net;

internal static class BlockCounterWrappingHelpers
{
    private const ushort LastAvailableBlockNumber = 65535;

    public static ushort CalculateNextBlockNumber(this BlockCounterWrapAround wrapping, ushort previousBlockNumber)
    {
        if (previousBlockNumber == LastAvailableBlockNumber)
        {
            return wrapping == BlockCounterWrapAround.ToZero ? (ushort)0 : (ushort)1;
        }

        return (ushort)(previousBlockNumber + 1);
    }
}
