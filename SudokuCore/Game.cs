﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    public class Game
    {
        readonly List<List<Square>> squares = new List<List<Square>>();
        readonly List<Region> regions = new List<Region>();
        readonly List<Region> columns = new List<Region>();
        readonly List<Region> rows = new List<Region>();

        readonly bool numbersPreloaded = false;
        readonly List<List<int>> prenumbers;

        public List<List<Square>> Squares => squares;

        public List<Region> Rows => rows;
        public List<Region> Columns => columns;
        public List<Region> Regions => regions;

        public Game()
        {
            // base constructor, goes away if not already defined
        }

        public Game(List<List<int>> numbers)
        {
            prenumbers = numbers;
            numbersPreloaded = true;
        }

        public void BuildBoard(int boardSize = 9)
        {
            // Regions in Sudoku are typically the 9 3x3 squares. 
            // Region Index can be identified based on index position on the board, square knows its own region
            for(int i = 0; i < boardSize; i++)
            {
                Regions.Add(new Region());
            }

            for(int i = 0; i < boardSize; i++)
            {
                List<Square> row = new List<Square>();
                for (int j = 0; j < boardSize; j++)
                {
                    Square square;
                    if (numbersPreloaded)
                    {
                        square = new Square(i, j, prenumbers[i][j]);
                    } else
                    {
                        square = new Square(i, j);
                    }
                    
                    row.Add(square);
                    Regions[square.RegionIndex].AddSquare(square);
                    
                }
                Rows.Add(new Region(row));
                Squares.Add(row);
            }
            
            // Set up Columns for convenience later
            for(int i = 0; i < boardSize; i++)
            {
                List<Square> column = new List<Square>();
                foreach (List<Square> row in Squares)
                {
                    column.Add(row[i]);
                }                
                Columns.Add(new Region(column));
            }
            // Console.WriteLine(this);
        }

        public Region GetRegion(int regionIndex)
        {
            return Regions[regionIndex];
        }

        public override string ToString()
        {
            string output = "";
            IEnumerable<string> yLabels = from square in Squares[0] select Square.YLabel(square.Index.y);
            IEnumerable<string> underLabels = from square in Squares[0] select "-";
            output += $"  | {String.Join(" ", yLabels)}";
            output += $"\n--+-{String.Join("-", underLabels)}";
            for (int i = 0; i < Squares.Count; i++)
            {
                List<Square> row = Squares[i];
                output += $"\n{Square.XLabel(i)} | {String.Join(" ", row)}";
            }
            return output;
        }

        public Square GetSquare(Position pos)
        {
            return Squares[pos.x][pos.y];
        }

        public Square GetSquare(int x, int y)
        {
            return Squares[x][y];
        }

        public int GetNumber(Position pos)
        {
            return GetSquare(pos).Number;
        }

        public void SetNumber(int x, int y, int number)
        {
            GetSquare(x, y).Number = number;            
        }

        public List<Square> GetRow(int row)
        {
            return Squares[row];
        }

        public List<Square> GetColumn(int colIndex)
        {
            return Columns[colIndex].Squares;
        }

        public IEnumerable<Square> AllSquares()
        {
            var allSquares = from row in squares
                             from square in row
                             select square;
            return allSquares;
        }
        public static void Main()
        {
            Game game = new Game();
            game.BuildBoard();
            Console.WriteLine("Hello World");
        }

    }

    public class Square
    {
        public int Number { get; set; }
        public Position Index { get; private set; }
        public int Row => Index.x;
        public int Column => Index.y;

        public int RegionIndex { get
            {
                return 3 * (Index.x / 3) + Index.y / 3; // These are ints so x / 3, y / 3 gets floored
            } 
        }        
        public HashSet<int> AllowedNumbers { get; private set; } = new HashSet<int>();

        public Square(int x, int y)
        {
            Index = new Position(x, y);
        }

        public Square(int x, int y, int number)
        {
            Index = new Position(x, y);
            Number = number;
        }

        public void ClearAllowedNumbers()
        {
            AllowedNumbers = new HashSet<int>();
        }

        public void AllowNumber(int number)
        {
            AllowedNumbers.Add(number);
        }

        public void AllowNumbers(IEnumerable<int> numbers)
        {
            AllowedNumbers.UnionWith(numbers);
        }

        public void DisallowNumber(int number)
        {
            AllowedNumbers.Remove(number);
        }

        public void DisallowNumbers(IEnumerable<int> numbers)
        {
            AllowedNumbers.ExceptWith(numbers);
        }

        public override string ToString()
        {
            return $"{Number}";
        }

        public static string LabelFromPosition(Position pos)
        {
            return LabelFromPosition(pos.x, pos.y);
        }

        public static string LabelFromPosition(int x, int y)
        {
            return $"{YLabel(y)}{XLabel(x)}";
        }

        public static string XLabel(int x)
        {
            return $"{x + 1}";
        }

        public static string YLabel(int y)
        {
            return $"{(char)('A' + y)}";
        }

    }

    /// <summary>
    /// <c>Region</c> is an area of a Sudoku board in which numbers may not be duplicated.
    /// </summary>
    public class Region
    {
        readonly List<Square> squares;
        public List<Square> Squares { get { return squares;  } }

        public Region()
        {
            squares = new List<Square>();
        }

        public Region(List<Square> regionSquares)
        {
            squares = regionSquares;
        }

        public void AddSquare(Square square)
        {
            squares.Add(square);
        }

        public bool IsNumberInRegion(int number)
        {
            foreach(Square square in squares)
            {
                if(square.Number == number)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsSquareInRegion(Square square)
        {
            return squares.Contains(square);
        }

        public bool IsRegionValid()
        {
            HashSet<int> numbers = new HashSet<int>();
            foreach(Square square in squares)
            {
                if(numbers.Contains(square.Number))
                {
                    return false;
                } else
                {
                    numbers.Add(square.Number);
                }                
            }
            return true;
        }
    }

    public struct Position {
        public readonly int x;
        public readonly int y;

        public Position(int newX, int newY)
        {
            x = newX;
            y = newY;
        }

        public override string ToString()
        {
            return Square.LabelFromPosition(this);
        }
    }

    public class Solver
    {
        private Game game;

        public Solver(Game gameToSolve)
        {
            Game = gameToSolve;
        }


        public bool IsColumnValid(int colIndex, out IEnumerable<Position> invalidPositions, bool requireComplete = false)
        {
            return IsSquareCollectionValid(game.GetColumn(colIndex), out invalidPositions);
        }

        public bool IsRowValid(int rowIndex, out IEnumerable<Position> invalidPositions, bool requireComplete = false)
        {
            return IsSquareCollectionValid(game.GetRow(rowIndex), out invalidPositions);
        }

        public bool IsRegionValid(int regionIndex, out IEnumerable<Position> invalidPositions, bool requireComplete = false)
        {
            return IsSquareCollectionValid(game.GetRegion(regionIndex).Squares, out invalidPositions);
        }

        public bool IsSquareCollectionValid(List<Square> squaresToTest, out IEnumerable<Position> invalidPositions, bool requireComplete = false)
        {
            var invalids = new HashSet<Position>();
            var numbers = new HashSet<int>();

            foreach (Square square in squaresToTest)
            {
                if(!requireComplete && square.Number == 0)
                {
                    continue;
                }

                if (numbers.Contains(square.Number))
                {                    
                    invalids.Add(square.Index);                    
                }
                else
                {
                    numbers.Add(square.Number);
                }
            }
            invalidPositions = invalids;
            return invalids.Count == 0;
        }

        public bool IsGameValid(out HashSet<Position> invalidPositions, bool requireComplete = false)
        {
            invalidPositions = new HashSet<Position>();
            // check the rows
            for (int i = 0; i < game.Squares.Count; i++)
            {
                IsRowValid(i, out IEnumerable<Position> invalids, requireComplete);
                invalidPositions.UnionWith(invalids);

                IsColumnValid(i, out invalids, requireComplete);
                invalidPositions.UnionWith(invalids);

                IsRegionValid(i, out invalids, requireComplete);
                invalidPositions.UnionWith(invalids);
            }

            return invalidPositions.Count == 0;
        }

        public bool IsNumberInRegion(int region, int number)
        {
            return game.GetRegion(region).IsNumberInRegion(number);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="number"></param>
        /// <returns><c>true</c> if the square is empty and the number can be placed there</returns>
        public bool ValidateNumberForSquare(int x, int y, int number)
        {
            Square square = game.GetSquare(x, y);
            if (square.Number > 0)
            {
                return false;
            }
            return !IsNumberInRow(x, number) 
                && !IsNumberInColumn(y, number) 
                && !IsNumberInRegion(game.GetSquare(x, y).RegionIndex, number);
        }

        public void SetNumber(int x, int y, int number)
        {
            if (ValidateNumberForSquare(x, y, number))
            {
                game.GetSquare(x, y).Number = number;
            }
            else
            {
                Console.WriteLine($"That number isn't valid at position {Square.LabelFromPosition(x, y)}");
            }

        }

        public bool IsNumberInRow(int row, int number)
        {
            foreach (Square square in game.GetRow(row))
            {
                if (square.Number == number)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsNumberInColumn(int column, int number)
        {
            foreach (Square square in game.GetColumn(column))
            {
                if (square.Number == number)
                {
                    return true;
                }
            }
            return false;
        }

        public HashSet<int> GetAllowedNumbersForSquare(Position position)
        {
            return GetAllowedNumbersForSquare(position.x, position.y);
        }

        public HashSet<int> GetAllowedNumbersForSquare(int row, int col)
        {
            HashSet<int> allowed = new HashSet<int>();
            HashSet<int> allowedByRow = new HashSet<int>();
            HashSet<int> allowedByColumn = new HashSet<int>();
            HashSet<int> allowedByRegion = new HashSet<int>();

            int regionIndex = game.GetSquare(row, col).RegionIndex;
            // strategy 1 - allowed by row
            for (int number = 1; number <= game.Squares.Count; number++)
            {
                if(!IsNumberInRow(row, number))
                {
                    allowedByRow.Add(number);
                }

                if (!IsNumberInColumn(col, number))
                {
                    allowedByColumn.Add(number);
                }

                if (!IsNumberInRegion(regionIndex, number))
                {
                    allowedByRegion.Add(number);
                }
            }
            allowed.UnionWith(allowedByRow);
            allowed.IntersectWith(allowedByColumn);
            allowed.IntersectWith(allowedByRegion);
            return allowed;
        }

        public void SolveSquare(int x, int y)
        {
            var allowed = GetAllowedNumbersForSquare(x, y);
            if(allowed.Count == 1)
            {
                SetNumber(x, y, allowed.ElementAt(0));
            }            
        }

        public void SolveBoard()
        {
            List<Square> simpleSolvable = new List<Square>();
            HashSet<Square> mediumSolvable = new HashSet<Square>();
            // First Pass, identify possible numbers based on current row, column and region placement, 
            IEnumerable<Square> emptySquares = from square in game.AllSquares() where square.Number == 0 select square;

            Console.WriteLine("\nEasy Solves:\n");
            foreach (Square square in emptySquares)
            {
                square.ClearAllowedNumbers();

                var allowed = GetAllowedNumbersForSquare(square.Index);
                square.AllowNumbers(allowed);

                if(allowed.Count == 1)
                {
                    simpleSolvable.Add(square);
                    Console.WriteLine($"{square.Index} is solvable: value should be {square.AllowedNumbers.ElementAt(0)}");
                }
            }

            Console.WriteLine("\nMedium Solves:");
            // Second Pass, identify if square is only square within a region which may permit a certain number
            // get empty squares in row (i.e. where the number is 0)
            // for each unsolved number, count the number of squares for which it is allowed, if it is only 1, then the square is solved
            Console.WriteLine("\nBy Row:");
            foreach (Region region in game.Rows)
            {
                mediumSolvable.UnionWith(SolveRegion(region));
            }

            Console.WriteLine("\nBy Column:");
            foreach (Region region in game.Columns)
            {
                // TODO store correct number somewhere to be applied later
                mediumSolvable.UnionWith(SolveRegion(region));
            }

            Console.WriteLine("\nBy Region:");
            foreach (Region region in game.Regions)
            {
                // TODO store correct number somewhere to be applied later
                mediumSolvable.UnionWith(SolveRegion(region));
            }
        }

        private HashSet<Square> SolveRegion(Region region)
        {
            var mediumSolvable = new HashSet<Square>();
            var rowSquares = from rowSquare in region.Squares where rowSquare.Number == 0 select rowSquare;
            for (int number = 1; number <= region.Squares.Count; number++)
            {
                var squareList = new List<Square>();
                foreach (Square rowSquare in rowSquares)
                {
                    if (rowSquare.AllowedNumbers.Contains(number))
                    {
                        squareList.Add(rowSquare);
                    }
                }
                if (squareList.Count == 1)
                {
                    var square = squareList[0];
                    mediumSolvable.Add(square);
                    Console.WriteLine($"{square.Index} is solvable: value should be {number}");
                }
            }

            return mediumSolvable;

        }

        public Game Game { get => game; set => game = value; }
    }
}
