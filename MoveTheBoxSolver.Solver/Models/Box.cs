using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace MoveTheBoxSolver.Solver.Models
{
    public class Box
    {
        public BoxType Type { get; set; }

        public Box Clone()
        {
            return new Box() { Type = this.Type };
        }

    }

    public enum BoxType
    {
        Empty = 0,
        BrownWood = 1,
        RedWood = 2,
        GreenWood = 3,
        BrownLeather = 4,
        BrownPaper = 5,
        BlueSteel = 6,
    }
}
