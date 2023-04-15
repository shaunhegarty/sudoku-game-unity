using System;
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
        public List<Region> Blocks => regions;

        public Game()
        {
            // base constructor, goes away if not already defined
        }

        public Game(List<List<int>> numbers)
        {
            prenumbers = numbers;
            numbersPreloaded = true;
        }

        public static Game DefaultGame()
        {
            List<List<int>> numbers = new List<List<int>>() {
                new List<int>() { 0, 0, 9, 2, 0, 8, 0, 0, 7 },
                new List<int>() { 2, 8, 0, 9, 0, 7, 4, 3, 6 },
                new List<int>() { 0, 3, 7, 5, 0, 0, 2, 9, 8 },
                new List<int>() { 0, 4, 3, 6, 0, 1, 0, 0, 0 },
                new List<int>() { 8, 7, 0, 0, 5, 2, 0, 0, 9 },
                new List<int>() { 0, 2, 0, 8, 7, 0, 1, 5, 0 },
                new List<int>() { 7, 1, 8, 3, 0, 0, 9, 0, 5 },
                new List<int>() { 3, 0, 0, 7, 8, 0, 0, 0, 1 },
                new List<int>() { 0, 5, 2, 1, 4, 0, 0, 8, 3 },
            };

            Game game = new Game(numbers);
            return game;
        }

        public static Game InsaneGame()
        {
            /// Insane #190
            
            List<List<int>> numbers = new List<List<int>>() {
                new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 4 },
                new List<int>() { 0, 0, 9, 0, 7, 5, 0, 8, 0 },
                new List<int>() { 0, 0, 5, 0, 0, 0, 0, 0, 0 },
                new List<int>() { 0, 0, 2, 0, 0, 0, 0, 6, 0 },
                new List<int>() { 0, 0, 0, 5, 0, 0, 2, 0, 0 },
                new List<int>() { 0, 0, 0, 0, 0, 0, 1, 0, 0 },
                new List<int>() { 0, 0, 0, 0, 0, 0, 7, 0, 0 },
                new List<int>() { 5, 0, 0, 0, 6, 0, 0, 0, 8 },
                new List<int>() { 0, 0, 7, 0, 5, 0, 0, 0, 2 },
            };

            Game game = new Game(numbers);
            return game;
        }

        public static Game VeryHardGame()
        {
            /// Very Hard #202

            List<List<int>> numbers = new List<List<int>>() {
                new List<int>() { 0, 0, 1, 7, 0, 3, 0, 5, 0 },
                new List<int>() { 0, 0, 0, 0, 0, 9, 0, 7, 0 },
                new List<int>() { 0, 0, 0, 0, 0, 0, 3, 0, 8 },
                new List<int>() { 2, 0, 0, 0, 0, 0, 1, 0, 0 },
                new List<int>() { 0, 7, 0, 0, 0, 0, 4, 0, 0 },
                new List<int>() { 4, 0, 3, 0, 0, 0, 0, 0, 0 },
                new List<int>() { 0, 0, 0, 0, 0, 7, 0, 9, 0 },
                new List<int>() { 5, 6, 0, 3, 0, 2, 0, 0, 0 },
                new List<int>() { 0, 0, 8, 0, 0, 1, 5, 0, 4 },
            };

            Game game = new Game(numbers);
            return game;
        }

        public void BuildBoard(int boardSize = 9)
        {
            // Regions in Sudoku are typically the 9 3x3 squares. 
            // Region Index can be identified based on index position on the board, square knows its own region
            for(int i = 0; i < boardSize; i++)
            {
                Blocks.Add(new Region());
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
                    Blocks[square.RegionIndex].AddSquare(square);
                    
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

        public Region GetBlock(int regionIndex)
        {
            return Blocks[regionIndex];
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

        public static Position New(int newX, int newY)
        {
            return new Position(newX, newY);
        }
        public override string ToString()
        {
            return Square.LabelFromPosition(this);
        }
    }

    public struct Solution
    {
        public int Number;
        public Position Index;
        public string Explanation;
        public Difficulty Difficulty;


        public Solution(int number, Position index, string explanation, Difficulty difficulty)
        {
            Number = number;
            Index = index;
            Explanation = explanation;
            Difficulty = difficulty;
        }
    }

    public enum Difficulty
    {
        None, Easy, Medium, Hard
    }
    public class Solver
    {
        private Game game;
        public List<Solution> solutions = new List<Solution>();
        private Dictionary<Position, List<Solution>> solutionsByIndex = new Dictionary<Position, List<Solution>>();

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
            return IsSquareCollectionValid(game.GetBlock(regionIndex).Squares, out invalidPositions);
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
            return game.GetBlock(region).IsNumberInRegion(number);
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
            if (number == 0 || ValidateNumberForSquare(x, y, number))
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

        public HashSet<int> GetAllowedNumbersForSquare(int row, int column)
        {
            return Game.GetSquare(row, column).AllowedNumbers;
        }


        private HashSet<int> DetermineAllowedNumbersForSquare(Position position)
        {
            return DetermineAllowedNumbersForSquare(position.x, position.y);
        }

        private HashSet<int> DetermineAllowedNumbersForSquare(int row, int col)
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
            var allowed = DetermineAllowedNumbersForSquare(x, y);
            if(allowed.Count == 1)
            {
                SetNumber(x, y, allowed.ElementAt(0));
            }            
        }

        public void SolveBoard()
        {
            solutions = new List<Solution>();
            HashSet<Solution> simpleSolvable = new HashSet<Solution>();
            HashSet<Solution> mediumSolvable = new HashSet<Solution>();
            // First Pass, identify candidates based on current row, column and region placement, 
            IEnumerable<Square> emptySquares = from square in game.AllSquares() where square.Number == 0 select square;


            foreach (Square square in emptySquares)
            {
                square.ClearAllowedNumbers();

                var allowed = DetermineAllowedNumbersForSquare(square.Index);
                square.AllowNumbers(allowed);

                if (allowed.Count == 1)
                {
                    Solution solution = new Solution(
                        number: square.AllowedNumbers.ElementAt(0),
                        index: square.Index,
                        explanation: $"{square.Index} has sole candidate number: {square.AllowedNumbers.ElementAt(0)}",
                        difficulty: Difficulty.Easy);
                    simpleSolvable.Add(solution);
                }
            }

            // Methods to remove candidates
            // https://www.kristanix.com/sudokuepic/sudoku-solving-techniques.php

            TrimCandidatesUsingBlockRowInteraction();
            /*var position = Position.New(0, 0);
            var allowedNumbers = GetAllowedNumbersForSquare(position);
            Console.WriteLine($"Allowed for {position}: {string.Join(" ", allowedNumbers)} (After Block-Row)");            */

            TrimCandidatesUsingBlockBlockInteraction();
            /* allowedNumbers = GetAllowedNumbersForSquare(position);
            Console.WriteLine($"Allowed for {position}: {string.Join(" ", allowedNumbers)} (After Block-Block)");*/            

            SolutionsForUniqueCandidates(simpleSolvable, mediumSolvable);

            // PrintSolutions();
            MapSolutionByIndex();

        }

        private void SolutionsForUniqueCandidates(HashSet<Solution> simpleSolvable, HashSet<Solution> mediumSolvable)
        {
            // Second Pass, identify if square is only square within a region which may permit a certain number
            // get empty squares in row (i.e. where the number is 0)
            // for each unsolved number, count the number of squares for which it is allowed, if it is only 1, then the square is solved
            foreach (Region region in game.Rows)
            {
                mediumSolvable.UnionWith(UniqueCandidatesForRegion(region, additionalInfo: "Row"));
            }

            foreach (Region region in game.Columns)
            {
                mediumSolvable.UnionWith(UniqueCandidatesForRegion(region, additionalInfo: "Column"));
            }

            foreach (Region region in game.Blocks)
            {
                mediumSolvable.UnionWith(UniqueCandidatesForRegion(region, additionalInfo: "Region"));
            }

            solutions.AddRange(simpleSolvable);
            solutions.AddRange(mediumSolvable);
        }

        private void TrimCandidatesUsingBlockRowInteraction()
        {
            // Block and Column/Row interaction
            // Find out if candidates within a block are limited to a single row or column
            foreach (Region block in game.Blocks)
            {
                var solvedNumbers = from square in block.Squares where square.Number > 0 select square.Number;
                var remainingSquares = from square in block.Squares where square.Number == 0 select square;
                for(int i = 1; i <= block.Squares.Count; i++)
                {
                    if(solvedNumbers.Contains(i))
                    {
                        continue;
                    }

                    var columnsForCandidate = new HashSet<int>();
                    var rowsForCandidate = new HashSet<int>();
                    foreach (Square square in remainingSquares)
                    {
                        if (square.AllowedNumbers.Contains(i))
                        {
                            columnsForCandidate.Add(square.Column);
                            rowsForCandidate.Add(square.Row);
                        }
                    }

                    if (columnsForCandidate.Count == 1)
                    {
                        var column = game.GetColumn(columnsForCandidate.ElementAt(0));
                        // Remove candidate from squares not in region
                        var remainingColumnSquares = from square in column where !remainingSquares.Contains(square) select square;
                        foreach (Square square in remainingColumnSquares)
                        {
                            square.DisallowNumber(i);
                        }
                    }

                    if (rowsForCandidate.Count == 1)
                    {
                        var row = game.GetRow(rowsForCandidate.ElementAt(0));
                        // Remove candidate from squares not in region
                        var remainingRowSquares = from square in row where !remainingSquares.Contains(square) select square;
                        foreach (Square square in remainingRowSquares)
                        {
                            square.DisallowNumber(i);
                        }
                    }
                }
            }
        }

        private void TrimCandidatesUsingBlockBlockInteraction()
        {
            // Block/Block interaction
            // For blocks in a row, if a number's candidate locations span only two rows within two blocks, those two rows can be excluded from the third

            // Let's start with the first row of blocks
            // And within that let's just test the first two blocks
            void TrimInteractionForGroup(int firstBlock, int secondBlock, int thirdBlock, bool useRows = true)
            {
                // does the candidate number span only two rows here?
                var firstBlockSquares = from square in game.GetBlock(firstBlock).Squares
                                        where square.Number == 0
                                        select square;
                var secondBlockSquares = from square in game.GetBlock(secondBlock).Squares
                                         where square.Number == 0
                                         select square;
                var thirdBlockSquares = from square in game.GetBlock(thirdBlock).Squares
                                         where square.Number == 0
                                         select square;

                for (int candidate = 1; candidate <= 9; candidate++)
                {
                    var linesForCandidate = new HashSet<int>();
                    var blocksForCandidate = new HashSet<int>();
                    foreach (Square square in firstBlockSquares)
                    {
                        if (square.AllowedNumbers.Contains(candidate))
                        {
                            linesForCandidate.Add(useRows ? square.Row : square.Column);
                            blocksForCandidate.Add(square.RegionIndex);
                        }
                    }

                    foreach (Square square in secondBlockSquares)
                    {
                        if (square.AllowedNumbers.Contains(candidate))
                        {
                            linesForCandidate.Add(useRows ? square.Row : square.Column);
                            blocksForCandidate.Add(square.RegionIndex);
                        }
                    }

                    if (linesForCandidate.Count == 2 && blocksForCandidate.Count == 2)
                    {
                        foreach (int line in linesForCandidate)
                        {
                            var lineSquares = useRows ? game.GetRow(line) : game.GetColumn(line);
                            // Remove candidate from squares not in region
                            var remainingRowSquares = from square in lineSquares
                                                      where square.Number == 0 && thirdBlockSquares.Contains(square)
                                                      select square;
                            foreach (Square square in remainingRowSquares)
                            {
                                square.DisallowNumber(candidate);
                            }
                        }
                    }
                }


            }
            
            for (int blockRow = 0; blockRow < 3; blockRow ++)
            {
                // Magic Numbers for Rows :(
                int block1 = blockRow * 3;
                int block2 = blockRow * 3 + 1;
                int block3 = blockRow * 3 + 2;

                // Who needs a permutation function anyway
                TrimInteractionForGroup(block1, block2, block3);                
                TrimInteractionForGroup(block1, block3, block2);
                TrimInteractionForGroup(block2, block3, block1);
            }

            for (int blockColumn = 0; blockColumn < 3; blockColumn++)
            {
                // Magic Numbers for Columns :(
                int block1 = blockColumn;
                int block2 = blockColumn + 3;
                int block3 = blockColumn + 6;

                // Who needs a permutation function anyway
                TrimInteractionForGroup(block1, block2, block3, useRows: false);
                TrimInteractionForGroup(block1, block3, block2, useRows: false);
                TrimInteractionForGroup(block2, block3, block1, useRows: false);
            }
        }

        private void MapSolutionByIndex()
        {
            solutionsByIndex = new Dictionary<Position, List<Solution>>();
            var mappedSolutions = from solution in solutions
                                  group solution by solution.Index into sGroup                                  
                                  select sGroup;
            foreach(var indexGroup in mappedSolutions)
            {
                List<Solution> indexedSolutions = new List<Solution>();
                foreach(var solution in indexGroup.OrderBy(s => s.Difficulty))
                {
                    indexedSolutions.Add(solution);
                }
                solutionsByIndex.Add(indexGroup.Key, indexedSolutions);
            }
        }

        public List<Solution> GetSolutionsByIndex(Position index)
        {
            solutionsByIndex.TryGetValue(index, out List<Solution> output);
            return output;
        }

        public Solution GetBestAvailableSolution(Position index)
        {
            var solutions = GetSolutionsByIndex(index);
            if (solutions != null)
            {
                return solutions[0];
            }
            return new Solution();
        }

        public int GetSolvedNumberForIndex(Position index)
        {
            var solutions = GetSolutionsByIndex(index);
            if(solutions != null)
            {
                return GetSolutionsByIndex(index)[0].Number;
            }
            return 0;
            
        }

        public int GetSolvedNumberForIndex(int row, int col)
        {
            return GetSolvedNumberForIndex(new Position(row, col));
        }

        public void PrintSolutions()
        {
            Console.WriteLine("\nSolutions:\n");
            foreach (Solution solution in solutions.OrderBy(x => x.Index.ToString()))
            {
                Console.WriteLine(solution.Explanation);
            }
        }

        private HashSet<Solution> UniqueCandidatesForRegion(Region region, string additionalInfo = "")
        {
            var mediumSolvable = new HashSet<Solution>();
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
                    Solution solution = new Solution(
                        number: number, 
                        index: square.Index, 
                        explanation: $"{square.Index} is the unique candidate for {number} ({additionalInfo})",
                        difficulty: Difficulty.Medium);
                    mediumSolvable.Add(solution);
                    // Console.WriteLine(solution.Explanation);
                }
            }

            return mediumSolvable;

        }

        public Game Game { get => game; set => game = value; }
    }
}
