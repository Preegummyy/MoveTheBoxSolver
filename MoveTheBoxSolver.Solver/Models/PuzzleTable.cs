using System;
using System.Collections.Generic;
using System.Text;

namespace MoveTheBoxSolver.Solver.Models
{
    public class PuzzleTable
    {
        #region Private Properties
        private Box[,] Boxes;
        #endregion

        #region Public Properties
        public int PuzzleWeight { get; }
        public int PuzzleHeight { get; }
        public bool IsSuccess
        {
            get
            {
                for (int x = 0; x < PuzzleWeight; x++)
                {
                    for (int y = 0; y < PuzzleHeight; y++)
                    {
                        if (this.Boxes[x, y].Type != BoxType.Empty)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        #endregion

        #region Constuctor
        public PuzzleTable(int weight, int height)
        {
            this.Boxes = new Box[weight + 1, height + 1];
            this.PuzzleWeight = weight;
            this.PuzzleHeight = height;
        }
        #endregion

        #region Public Method
        public void SetPuzzle(PuzzleTable table)
        {
            for (int x = 0; x < PuzzleWeight; x++)
            {
                for (int y = 0; y < PuzzleHeight; y++)
                {
                    this.Boxes[x, y] = table.Boxes[x, y].Clone();
                }
            }
        }

        public void Move(int index_x, int index_y, MoveMode mode)
        {
            switch (mode)
            {
                case MoveMode.MoveRight:
                    moveRight(index_x, index_y);
                    break;
                case MoveMode.MoveUp:
                    moveUp(index_x, index_y);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Private Method
        private void moveUp(int index_x, int index_y)
        {
            var Temp = Boxes[index_x, index_y].Clone();
            Boxes[index_x, index_y] = Boxes[index_x, index_y + 1].Clone();
            Boxes[index_x, index_y + 1] = Temp.Clone();
            fall();
            check();
        }

        private void moveRight(int index_x, int index_y)
        {
            var Temp = Boxes[index_x, index_y].Clone();
            Boxes[index_x, index_y] = Boxes[index_x + 1, index_y].Clone();
            Boxes[index_x + 1, index_y] = Temp.Clone();
            fall();
            check();
        }

        private void fall()
        {
            for (int x = 0; x < PuzzleWeight; x++)
            {
                for (int y = 0; y < PuzzleHeight; y++)
                {
                    if (Boxes[x, y].Type != BoxType.Empty && y != 0 && Boxes[x, y - 1].Type == BoxType.Empty)
                    {
                        for (int i = y - 1; i < PuzzleHeight - 1; i++)
                        {
                            Boxes[x, i] = Boxes[x, i + 1].Clone();
                        }
                        y = 0;
                    }
                }
            }
        }

        private void check()
        {
            List<BoxIndex> RemoveList = new List<BoxIndex>();
            for (int x = 0; x < PuzzleWeight; x++)
            {
                var RemoveListCol = checkCol(x);
                foreach (var item in RemoveListCol)
                {
                    RemoveList.Add(item);
                }
            }
            for (int y = 0; y < PuzzleHeight; y++)
            {
                var RemoveListRow = checkRow(y);
                foreach (var item in RemoveListRow)
                {
                    RemoveList.Add(item);
                }
            }
            remove(RemoveList);

        }

        private List<BoxIndex> checkCol(int index_x)
        {
            int Count = 1;
            BoxType LastFound = Boxes[index_x, 0].Type;
            List<BoxIndex> RemoveList = new List<BoxIndex>();
            bool IsChange = false;
            for (int y = 1; y < PuzzleHeight + 1; y++)
            {
                if (LastFound == Boxes[index_x, y].Type)
                {
                    Count++;
                }
                else
                {
                    LastFound = Boxes[index_x, y].Type;
                    IsChange = true;
                }
                if (IsChange)
                {
                    if (Boxes[index_x, y - 1].Type != BoxType.Empty)
                    {
                        if (Count >= 3)
                        {
                            for (int i = 0; i < Count; i++)
                            {
                                BoxIndex RemoveIndex = new BoxIndex { Index_X = index_x, Index_Y = y - i - 1 };
                                RemoveList.Add(RemoveIndex);
                            }
                        }
                    }
                    Count = 1;
                    IsChange = false;
                }
            }
            return RemoveList;
        }

        private List<BoxIndex> checkRow(int index_y)
        {
            int Count = 1;
            BoxType LastFound = Boxes[0, index_y].Type;
            List<BoxIndex> RemoveList = new List<BoxIndex>();
            bool IsChange = false;
            for (int x = 1; x < PuzzleWeight + 1; x++)
            {
                if (LastFound == Boxes[x, index_y].Type)
                {
                    Count++;
                }
                else
                {
                    LastFound = Boxes[x, index_y].Type;
                    IsChange = true;
                }
                if (IsChange)
                {
                    if (Boxes[x - 1, index_y].Type != BoxType.Empty)
                    {
                        if (Count >= 3)
                        {
                            for (int i = 0; i < Count; i++)
                            {
                                BoxIndex RemoveIndex = new BoxIndex { Index_X = x - i - 1, Index_Y = index_y };
                                RemoveList.Add(RemoveIndex);
                            }
                        }
                    }
                    Count = 1;
                    IsChange = false;
                }
            }
            return RemoveList;
        }

        private void remove(List<BoxIndex> removeList)
        {
            foreach (var item in removeList)
            {
                Boxes[item.Index_X, item.Index_Y] = new Box() { Type = BoxType.Empty };
            }
            if (removeList.Count != 0)
            {
                fall();
                check();
            }
        }
        #endregion
    }
}
