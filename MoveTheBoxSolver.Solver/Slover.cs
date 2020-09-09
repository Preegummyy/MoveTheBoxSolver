using MoveTheBoxSolver.Solver.Models;
using System;
using System.Collections.Generic;

namespace MoveTheBoxSolver.Solver
{
    public class Slover
    {
        #region Constuctor
        public Slover()
        {

        }
        #endregion

        #region Public Method
        public MoveArrow[] Solve(PuzzleTable puzzle, int moveLimit)
        {
            PuzzleTable[] TablesStack = new PuzzleTable[moveLimit];
            MoveArrow[] Solution = new MoveArrow[moveLimit];
            int[] IndexOfLastmove = new int[moveLimit];
            List<MoveArrow> MoveList = GetMoveList(puzzle);
            int MovelistCount = MoveList.Count;

            for (int i = 0; i < IndexOfLastmove.Length; i++)
            {
                IndexOfLastmove[i] = 0;
            }

            for (int i = 0; i < TablesStack.Length; i++)
            {
                TablesStack[i] = new PuzzleTable(puzzle.PuzzleWeight, puzzle.PuzzleHeight);
            }

            for (int MoveRound = 0; MoveRound < moveLimit; MoveRound++)
            {
                var MoveDirectionIndex = IndexOfLastmove[MoveRound];
                var MoveDirection = MoveList[MoveDirectionIndex];
                TablesStack[MoveRound] = Move(MoveRound, MoveDirection, puzzle, TablesStack);
                IndexOfLastmove[MoveRound]++;
                if (IndexOfLastmove[MoveRound] >= MovelistCount - 1)
                {
                    //รอบ แรก
                    if (MoveRound <= 0)
                    {
                        return null;
                    }
                    IndexOfLastmove[MoveRound] = 0;
                    TablesStack[MoveRound].SetPuzzle(TablesStack[MoveRound - 1]);
                    MoveRound = MoveRound - 2;
                }

                //รอบสุดท้าย
                if (MoveRound >= moveLimit - 1)
                {
                    if (TablesStack[MoveRound].IsSuccess)
                    {
                        for (int i = 0; i < Solution.Length; i++)
                        {
                            Solution[i] = MoveList[IndexOfLastmove[i] - 1];
                        }
                        return Solution;
                    }
                    else
                    {
                        if (MoveRound <= 0)
                        {
                            TablesStack[MoveRound].SetPuzzle(puzzle);
                        }
                        else
                        {
                            TablesStack[MoveRound].SetPuzzle(TablesStack[MoveRound - 1]);
                        }
                        MoveRound--;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Private Method
        private PuzzleTable Move(int round, MoveArrow moveArrow, PuzzleTable puzzle, PuzzleTable[] tablesStack)
        {
            var TempTable = new PuzzleTable(puzzle.PuzzleWeight, puzzle.PuzzleHeight);

            if (round > 0)
            {
                TempTable.SetPuzzle(tablesStack[round - 1]);
            }
            else
            {
                TempTable.SetPuzzle(puzzle);
            }

            TempTable.Move(moveArrow.StartIndex.Index_X, moveArrow.StartIndex.Index_Y, moveArrow.Move);
            return TempTable;
        }

        private List<MoveArrow> GetMoveList(PuzzleTable puzzle)
        {
            var Movelist = new List<MoveArrow>();

            for (int x = 0; x < puzzle.PuzzleWeight - 1; x++)
            {
                for (int y = 0; y < puzzle.PuzzleHeight - 1; y++)
                {
                    for (int moveMode = 1; moveMode <= 2; moveMode++)
                    {
                        Movelist.Add(new MoveArrow() { Move = (MoveMode)moveMode, StartIndex = new BoxIndex() { Index_X = x, Index_Y = y } });
                    }
                }
            }

            return Movelist;
        }
        #endregion


    }
}
