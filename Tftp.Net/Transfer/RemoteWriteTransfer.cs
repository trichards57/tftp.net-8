// <copyright file="RemoteWriteTransfer.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Tftp.Net.Channel;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer;

internal class RemoteWriteTransfer : TftpTransfer
{
    public RemoteWriteTransfer(ITransferChannel connection, string filename)
        : base(connection, filename, new StartOutgoingWrite())
    {
    }
}
