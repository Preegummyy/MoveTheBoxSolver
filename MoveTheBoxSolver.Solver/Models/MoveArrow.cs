﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MoveTheBoxSolver.Solver.Models
{
    public class MoveArrow
    {
        public MoveMode Move { get; set; }
        public BoxIndex StartIndex { get; set; }
        public BoxType ToMoveBoxType { get; set; }
        public BoxType FromMoveBoxType { get; set; }
    }

    public class HumanMoveArrow
    {
        public HumanMoveMode Move { get; set; }
        public BoxIndex StartIndex { get; set; }
        public BoxType ToMoveBoxType { get; set; }
        public BoxType FromMoveBoxType { get; set; }
    }


    #region MoveMode
    public enum MoveMode
    {
        MoveRight = 1,
        MoveUp = 2
    }

    public enum HumanMoveMode
    {
        Right = 1,
        Left = 2,
        Up = 3,
        Down = 4
    }
    
    #endregion
}
