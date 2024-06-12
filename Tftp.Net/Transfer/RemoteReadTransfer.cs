// <copyright file="RemoteReadTransfer.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Tftp.Net.Channel;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer;

internal class RemoteReadTransfer : TftpTransfer
{
    public RemoteReadTransfer(ITransferChannel connection, string filename)
        : base(connection, filename, new StartOutgoingRead())
    {
    }

    public override long ExpectedSize
    {
        get { return base.ExpectedSize; }
        set { throw new NotSupportedException("You cannot set the expected size of a file that is remotely transferred to this system."); }
    }
}
