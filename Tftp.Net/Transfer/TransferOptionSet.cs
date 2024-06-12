// <copyright file="TransferOptionSet.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Tftp.Net.Transfer;

internal class TransferOptionSet
{
    public const int DefaultBlockSize = 512;
    public const int DefaultTimeoutSeconds = 5;

    public TransferOptionSet(IEnumerable<TransferOption> options)
    {
        IncludesBlockSizeOption = IncludesTimeoutOption = IncludesTransferSizeOption = false;

        foreach (TransferOption option in options)
        {
            Parse(option);
        }
    }

    private TransferOptionSet()
    {
    }

    public int BlockSize { get; set; } = DefaultBlockSize;

    public bool IncludesBlockSizeOption { get; set; } = false;

    public bool IncludesTimeoutOption { get; set; } = false;

    public bool IncludesTransferSizeOption { get; set; } = false;

    public int Timeout { get; set; } = DefaultTimeoutSeconds;

    public long TransferSize { get; set; } = 0;

    public static TransferOptionSet NewDefaultSet()
    {
        return new TransferOptionSet() { IncludesBlockSizeOption = true, IncludesTimeoutOption = true, IncludesTransferSizeOption = true };
    }

    public static TransferOptionSet NewEmptySet()
    {
        return new TransferOptionSet() { IncludesBlockSizeOption = false, IncludesTimeoutOption = false, IncludesTransferSizeOption = false };
    }

    public List<TransferOption> ToOptionList()
    {
        var result = new List<TransferOption>();

        if (IncludesBlockSizeOption)
        {
            result.Add(new TransferOption("blksize", BlockSize.ToString()));
        }

        if (IncludesTimeoutOption)
        {
            result.Add(new TransferOption("timeout", Timeout.ToString()));
        }

        if (IncludesTransferSizeOption)
        {
            result.Add(new TransferOption("tsize", TransferSize.ToString()));
        }

        return result;
    }

    private void Parse(TransferOption option)
    {
        switch (option.Name)
        {
            case "blksize":
                IncludesBlockSizeOption = ParseBlockSizeOption(option.Value);
                break;

            case "timeout":
                IncludesTimeoutOption = ParseTimeoutOption(option.Value);
                break;

            case "tsize":
                IncludesTransferSizeOption = ParseTransferSizeOption(option.Value);
                break;
        }
    }

    private bool ParseBlockSizeOption(string value)
    {
        if (!int.TryParse(value, out var blockSize))
        {
            return false;
        }

        // Only accept block sizes in the range [8, 65464]
        if (blockSize < 8 || blockSize > 65464)
        {
            return false;
        }

        BlockSize = blockSize;
        return true;
    }

    private bool ParseTimeoutOption(string value)
    {
        if (!int.TryParse(value, out var timeout))
        {
            return false;
        }

        // Only accept timeouts in the range [1, 255]
        if (timeout < 1 || timeout > 255)
        {
            return false;
        }

        Timeout = timeout;
        return true;
    }

    private bool ParseTransferSizeOption(string value)
    {
        if (long.TryParse(value, out var txSize) && txSize >= 0)
        {
            TransferSize = txSize;
            return true;
        }

        return false;
    }
}
