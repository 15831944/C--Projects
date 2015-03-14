using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.LookAheadPath
{
    internal interface IPathBase
    {
        void ResetRelativeLocation();
        void AssertBoxLocation();
        void SetRelativeLocation(ColoBox boxToPlace);
        Point? UpdateMachineState(ColoBox boxToPlace);
    }
}
