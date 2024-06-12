using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tftp.Net;

internal class InvalidMessageError : ITftpTransferError
{
    public override string ToString()
    {
        return "An unexpected message was received during a transfer.";
    }
}
