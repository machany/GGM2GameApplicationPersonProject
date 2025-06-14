using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Work.Scripts.Executors
{
    public struct ProgramCounterMoveInformation
    {
        public int moveProgramCounterValue;

        public ProgramCounterMoveInformation Init(int moveCommandValue)
        {
            this.moveProgramCounterValue = moveCommandValue;
            return this;
        }
    }
}
