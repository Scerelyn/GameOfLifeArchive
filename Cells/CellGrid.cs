using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife.Cells
{
    /// <summary>
    /// Handles grid-based logic on cell objects, as well as storing the grid of cells
    /// </summary>
    class CellGrid
    {
        /// <summary>
        /// The rule string that the cell grid follows for behavior. The default string follows Conway's Game of Life rules
        /// </summary>
        public string RuleString { get; set; } = "B3/S23";
        /// <summary>
        /// The array of ints whose ints are the number of living neighboring cells are required for survival ie: not dying
        /// </summary>
        private int[] surviveNums;
        /// <summary>
        /// The array of ints whose ints are the number of living neighboring cells are required for being born ie: made into a living cell
        /// </summary>
        private int[] birthNums; 
        /// <summary>
        /// The Cell 2D array that this CellGrid object stores
        /// </summary>
        public Cell[,] Grid; //arrays can be indexed as x,y 

        /// <summary>
        /// Constructor for the CellGrid, in x by y dimensions
        /// </summary>
        /// <param name="x">The width of the grid</param>
        /// <param name="y">The height of the grid</param>
        public CellGrid(int x, int y, string rule)
        {
            RuleString = rule;
            ParseRuleString();
            Grid = new Cell[x,y];
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    Grid[i, j] = new Cell();
                }
            }
        }

        /// <summary>
        /// Gives the amount of alive cells at a give x,y cell position
        /// </summary>
        /// <param name="x">The x coordinate of the cell to check around</param>
        /// <param name="y">The y coordinate of the cell to check around</param>
        /// <param name="griddy">The specific cell grid to use</param>
        /// <returns>The amount of alive cell surrounding the given cell location</returns>
        public int CheckAround(int x, int y, Cell[,] griddy)
        {
            int alive = 0;
            //Console.Write($"x:{x} y:{y}");
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++) //for looping surrounding points, i and j are shifts/translations, not direct points
                {
                    if (!( (x+i < 0) || (x+i >= Grid.GetLength(0)) || (y+j < 0) || (y+j >= Grid.GetLength(1)) )) //checking out of bounds conditions
                    {
                    
                        //Console.WriteLine("ok!");
                        if (!(i == j && i == 0)) //0,0 means the point is itself rather than surrounding
                        {
                            alive += griddy[x + i, y + j].IsAlive ? 1 : 0;
                        }
                    
                    }
                }
            }
            //Console.WriteLine(" : " + alive);
            return alive;
        }

        /// <summary>
        /// Performs one step or tick on the grid using Conway's Game of Life ruleset
        /// </summary>
        public void Step()
        {
            Cell[,] copy = CopyGrid();
            for (int i = 0; i < copy.GetLength(0); i++)
            {
                for (int j = 0; j < copy.GetLength(1); j++)
                {
                    int around = CheckAround(i, j, copy);
                    if (birthNums.Contains(around))
                    {
                        Grid[i, j].IsAlive = true;
                    }
                    else if (!surviveNums.Contains(around))
                    {
                        Grid[i, j].IsAlive = false;
                    }
                }
            }
            //Console.WriteLine("step done");
        }

        /// <summary>
        /// Makes an entirely new copy of this CellGrid instance of Grid
        /// </summary>
        /// <returns>A copy of this instance of CellGrid's Cell array Grid</returns>
        public Cell[,] CopyGrid()
        {
            Cell[,] copy = new Cell[Grid.GetLength(0), Grid.GetLength(1)];
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    copy[i, j] = new Cell() { IsAlive = Grid[i, j].IsAlive };
                }
            }
            return copy;
        }

        /// <summary>
        /// Randomzies the IsAlive values of the Cells in Grid
        /// </summary>
        /// <param name="weight">The percentage probability of Cells being alive</param>
        public void Randomize(double weight)
        {
            Random r = new Random();
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    Grid[i, j].IsAlive = false;
                    if (r.NextDouble() < weight)
                    {
                        Grid[i, j].IsAlive = true;
                    }
                }
            }
        }
        /// <summary>
        /// Clears the grid, making all cells dead
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    Grid[i, j].IsAlive = false;
                }
            }
        }
        /// <summary>
        /// Parses the set RuleString
        /// </summary>
        /// <returns>A boolean if the parsing succeeds or not. On failure, the rulestring and the birth and survive int arrays default to conway's game of life rules</returns>
        private bool ParseRuleString()
        {
            string[] BSSplit = RuleString.Split('/');
            if (BSSplit.Length != 2) // '/' parsing failure
            {
                birthNums = new int[] { 3 };
                surviveNums = new int[] { 2, 3 };
                RuleString = "B3/S23"; //default to a functional rule
                return false;
            }
            else
            {
                BSSplit[0] = BSSplit[0].Substring(1);
                BSSplit[1] = BSSplit[1].Substring(1);
                birthNums = new int[BSSplit[0].Length];
                surviveNums = new int[BSSplit[1].Length];
                for (int i = 0; i < BSSplit[0].Length || i < BSSplit[1].Length; i++)
                {
                    if (i < BSSplit[0].Length)
                    {
                        bool success = int.TryParse(BSSplit[0][i] + "", out birthNums[i]);
                        if (!success) //number parsing failure
                        {
                            birthNums = new int[] { 3 };
                            surviveNums = new int[] { 2, 3 };
                            RuleString = "B3/S23";
                            return false;
                        }
                    }
                    if (i < BSSplit[1].Length)
                    {
                        bool success = int.TryParse(BSSplit[1][i] + "", out surviveNums[i]);
                        if (!success) //number parsing failure
                        {
                            birthNums = new int[] { 3 };
                            surviveNums = new int[] { 2, 3 };
                            RuleString = "B3/S23";
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}
