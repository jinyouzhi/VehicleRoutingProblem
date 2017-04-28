using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRoutingProblem
{
    abstract public class GABase
    {
        abstract internal void run(Form1 form1);
        internal abstract void initialize(Form1 form1);
    }
}
