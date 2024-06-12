using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tftp.Net.Transfer.StateMachine;

internal enum Trigger
{
    Start = 1,
    Cancel = 2,
    OptionsReceived = 3,
    Close = 4,
    Acknowledge = 5,
    Error = 6,
}
