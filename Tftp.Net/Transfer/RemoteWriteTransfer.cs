// <copyright file="RemoteWriteTransfer.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging;
using Tftp.Net.Channel;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer;

internal class RemoteWriteTransfer(ITransferChannel connection, string filename, ILogger logger) 
    : TftpTransfer(connection, filename, new StartOutgoingWrite(logger), logger)
{
}
