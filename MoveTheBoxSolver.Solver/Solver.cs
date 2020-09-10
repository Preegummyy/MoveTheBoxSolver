using MoveTheBoxSolver.Solver.Models;
using System;
using System.Collections.Generic;

namespace MoveTheBoxSolver.Solver
{
    public class Solver
    {
        #region Constuctor
        public Solver()
        {

        }
        #endregion

        #region Public Method
        public List<HumanMoveArrow> Solve(PuzzleTable puzzle, int moveLimit)
        {
            var solution = this.solve(puzzle, moveLimit);
            if (solution != null)
            {
                var newSolution = new List<HumanMoveArrow>();
                foreach (var item in solution)
                {
                    //Empty Move
                    if (item.FromMoveBoxType == BoxType.Empty && item.ToMoveBoxType == BoxType.Empty)
                    {
                        continue;
                    }

                    //FromMoveBoxType Empty
                    if (item.FromMoveBoxType == BoxType.Empty)
                    {
                        //transform to human
                        HumanMoveArrow humanMoveArrow = new HumanMoveArrow()
                        {
                            StartIndex = item.StartIndex,
                            FromMoveBoxType = item.ToMoveBoxType,
                            ToMoveBoxType = item.FromMoveBoxType
                        };

                        switch (item.Move)
                        {
                            case MoveMode.MoveRight:
                                humanMoveArrow.Move = HumanMoveMode.Left;
                                humanMoveArrow.StartIndex.Index_X = item.StartIndex.Index_X + 1;
                                break;
                            case MoveMode.MoveUp:
                                humanMoveArrow.Move = HumanMoveMode.Down;
                                humanMoveArrow.StartIndex.Index_Y = item.StartIndex.Index_Y + 1;
                                break;
                            default:
                                break;
                        }
                        newSolution.Add(humanMoveArrow);
                    }
                    else
                    {
                        HumanMoveArrow humanMoveArrow = new HumanMoveArrow()
                        {
                            StartIndex = item.StartIndex,
                            FromMoveBoxType = item.FromMoveBoxType,
                            ToMoveBoxType = item.ToMoveBoxType
                        };

                        switch (item.Move)
                        {
                            case MoveMode.MoveRight:
                                humanMoveArrow.Move = HumanMoveMode.Right;
                                break;
                            case MoveMode.MoveUp:
                                humanMoveArrow.Move = HumanMoveMode.Up;
                                break;
                            default:
                                break;
                        }
                        newSolution.Add(humanMoveArrow);
                    }
                }
                return newSolution;
            }
            else
            {
                return null;
            }
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

        public MoveArrow[] solve(PuzzleTable puzzle, int moveLimit)
        {
            puzzle.Fall();

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
                            if (i == 0)
                            {
                                Solution[i].FromMoveBoxType = puzzle.GetBoxsType(Solution[i].StartIndex.Index_X, Solution[i].StartIndex.Index_Y);
                                switch (Solution[i].Move)
                                {
                                    case MoveMode.MoveRight:
                                        Solution[i].ToMoveBoxType = puzzle.GetBoxsType(Solution[i].StartIndex.Index_X + 1, Solution[i].StartIndex.Index_Y);
                                        break;
                                    case MoveMode.MoveUp:
                                        Solution[i].ToMoveBoxType = puzzle.GetBoxsType(Solution[i].StartIndex.Index_X, Solution[i].StartIndex.Index_Y + 1);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                Solution[i].FromMoveBoxType = TablesStack[i - 1].GetBoxsType(Solution[i].StartIndex.Index_X, Solution[i].StartIndex.Index_Y);
                                switch (Solution[i].Move)
                                {
                                    case MoveMode.MoveRight:
                                        Solution[i].ToMoveBoxType = TablesStack[i - 1].GetBoxsType(Solution[i].StartIndex.Index_X + 1, Solution[i].StartIndex.Index_Y);
                                        break;
                                    case MoveMode.MoveUp:
                                        Solution[i].ToMoveBoxType = TablesStack[i - 1].GetBoxsType(Solution[i].StartIndex.Index_X, Solution[i].StartIndex.Index_Y + 1);
                                        break;
                                    default:
                                        break;
                                }
                            }
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


    }
}
