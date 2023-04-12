using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Sudoku
{
    public class Game
    {
        readonly List<List<Square>> squares = new List<List<Square>>();
        readonly List<Region> regions = new List<Region>();
        readonly bool numbersPreloaded = false;
        readonly List<List<int>> prenumbers;

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
                regions.Add(new Region());
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
                    regions[square.RegionIndex].AddSquare(square);
                    
                }
                squares.Add(row);
            }
            
            // Console.WriteLine(this);
        }

        public Region GetRegion(int regionIndex)
        {
            return regions[regionIndex];
        }

        public override string ToString()
        {
            string output = "";
            IEnumerable<string> yLabels = from square in squares[0] select Square.YLabel(square.Index.y);
            IEnumerable<string> underLabels = from square in squares[0] select "-";
            output += $"  | {String.Join(" ", yLabels)}";
            output += $"\n--+-{String.Join("-", underLabels)}";
            for (int i = 0; i < squares.Count; i++)
            {
                List<Square> row = squares[i];
                output += $"\n{Square.XLabel(i)} | {String.Join(" ", row)}";
            }
            foreach (List<Square> row in squares)
            {
                
            }
            return output;
        }

        public Square GetSquare(Position pos)
        {
            return squares[pos.x][pos.y];
        }

        public Square GetSquare(int x, int y)
        {
            return squares[x][y];
        }

        public int GetNumber(Position pos)
        {
            return GetSquare(pos).Number;
        }

        public bool IsNumberInRow(int row, int number)
        {
            foreach (Square square in squares[row])
            {
                if(square.Number == number)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsNumberInColumn(int column, int number)
        {
            foreach(Square square in GetColumn(column))
            {
                if (square.Number == number) {
                    return true;
                }
            }
            return false;
        }

        public List<Square> GetRow(int row)
        {
            return squares[row];
        }

        public List<Square> GetColumn(int colIndex)
        {
            List<Square> column = new List<Square>();
            foreach(List<Square> row in squares)
            {
                column.Add(row[colIndex]);
            }
            return column;
        }

        public bool IsColumnValid(int colIndex, out List<Position> invalidPositions)
        {
            return IsSquareCollectionValid(GetColumn(colIndex), out invalidPositions);
        }

        public bool IsRowValid(int rowIndex, out List<Position> invalidPositions)
        {
            return IsSquareCollectionValid(GetRow(rowIndex), out invalidPositions);
        }

        public bool IsRegionValid(int regionIndex, out List<Position> invalidPositions)
        {
            return IsSquareCollectionValid(GetRegion(regionIndex).Squares, out invalidPositions);
        }

        public bool IsSquareCollectionValid(List<Square> squaresToTest, out List<Position> invalidPositions)
        {
            invalidPositions = new List<Position>();
            HashSet<int> numbers = new HashSet<int>();

            foreach (Square square in squaresToTest)
            {
                if (numbers.Contains(square.Number))
                {
                    invalidPositions.Add(square.Index);
                    return false;
                }
                else
                {
                    numbers.Add(square.Number);
                }
            }
            return true;
        }

        public bool IsGameValid(out HashSet<Position> invalidPositions)
        {
            invalidPositions = new HashSet<Position>();
            // check the rows
            for(int i = 0; i < squares.Count; i++)
            {
                IsRowValid(i, out List<Position> invalids);
                invalidPositions.UnionWith(invalids);

                IsColumnValid(i, out invalids);
                invalidPositions.UnionWith(invalids);

                IsRegionValid(i, out invalids);
                invalidPositions.UnionWith(invalids);
            }

            return invalidPositions.Count == 0;
        }

        public bool IsNumberInRegion(int region, int number)
        {
            return GetRegion(region).IsNumberInRegion(number);
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
            Square square = GetSquare(x, y);
            if(square.Number > 0)
            {
                return false;
            }
            return !IsNumberInRow(x, number) && !IsNumberInColumn(y, number) && !IsNumberInRegion(GetSquare(x, y).RegionIndex, number);
        }

        public void SetNumber(int x, int y, int number)
        {
            if(ValidateNumberForSquare(x, y, number))
            {
                GetSquare(x, y).Number = number;
            } else
            {
                Console.WriteLine($"That number isn't valid at position {Square.LabelFromPosition(x, y)}");
            }
            
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
        public int RegionIndex { get
            {
                return 3 * (Index.x / 3) + Index.y / 3; // These are ints so x / 3, y / 3 gets floored
            } 
        }

        public Square(int x, int y)
        {
            Index = new Position(x, y);
        }

        public Square(int x, int y, int number)
        {
            Index = new Position(x, y);
            Number = number;
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
}
