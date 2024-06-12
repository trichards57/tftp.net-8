using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tftp.Net.Transfer.StateMachine;

internal enum State
{
    Start = 1,
    Cancelled = 2,
    Sending = 3,
    SendOptionAcknowledgement = 4,
    Closed = 5,
    ReceivedError = 6,
}
