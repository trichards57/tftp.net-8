// <copyright file="TransferOptionParser.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.IO;

namespace Tftp.Net.Commands;

internal static class TransferOptionParser
{
    public static List<TransferOption> Parse(TftpStreamReader reader)
    {
        var options = new List<TransferOption>();

        while (true)
        {
            string name;

            try
            {
                name = reader.ReadNullTerminatedString();
            }
            catch (IOException)
            {
                name = string.Empty;
            }

            if (name.Length == 0)
            {
                break;
            }

            var value = reader.ReadNullTerminatedString();
            options.Add(new TransferOption(name, value));
        }

        return options;
    }
}
