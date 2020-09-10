using System;
using System.Collections.Generic;
using System.Text;

namespace MoveTheBoxSolver.Solver.Models
{
    public class MoveArrow
    {
        public MoveMode Move { get; set; }
        public BoxIndex StartIndex { get; set; }
        public BoxType MoveBoxType { get; set; }
    }

    #region MoveMode
    public enum MoveMode
    {
        MoveRight = 1,
        MoveUp = 2
    }
    #endregion
}
