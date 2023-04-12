using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku;

namespace SudokuUnitTest
{
    [TestClass]
    public class SudokuTest
    {

        [TestMethod]
        public void TestRegionIndex()
        {
            Game game = new Game();
            game.BuildBoard();
            Assert.AreEqual(game.GetSquare(0, 0).RegionIndex, 0);
            Assert.AreEqual(game.GetSquare(2, 2).RegionIndex, 0);
            Assert.AreEqual(game.GetSquare(3, 2).RegionIndex, 3);
            Assert.AreEqual(game.GetSquare(3, 5).RegionIndex, 4);
            Assert.AreEqual(game.GetSquare(6, 5).RegionIndex, 7);
            Assert.AreEqual(game.GetSquare(6, 8).RegionIndex, 8);
        }

        [TestMethod]
        public void TestSquareIsInCorrectRegion()
        {
            Game game = new Game();
            game.BuildBoard();

            Square squareToTest = game.GetSquare(0, 0);
            Region squareRegion = game.GetRegion(squareToTest.RegionIndex);
            Assert.IsTrue(squareRegion.IsSquareInRegion(squareToTest));

            Region other = game.GetRegion(8);
            Assert.IsFalse(other.IsSquareInRegion(squareToTest));
        }

        [TestMethod]
        public void TestSquareValidation()
        {
            Game game = new Game();
            game.BuildBoard();

            Solver solver = new Solver(game);

            game.SetNumber(0, 0, 3);
            game.SetNumber(0, 1, 4);
            game.SetNumber(0, 2, 5);
            game.SetNumber(3, 3, 6);

            Console.WriteLine(game.ToString());

            Assert.IsFalse(solver.ValidateNumberForSquare(3, 0, 3));
            Assert.IsFalse(solver.ValidateNumberForSquare(0, 3, 3));
            Assert.IsFalse(solver.ValidateNumberForSquare(0, 0, 6));
            Assert.IsTrue(solver.ValidateNumberForSquare(1, 1, 6));
            Assert.IsTrue(solver.ValidateNumberForSquare(3, 4, 3));
        }

        [TestMethod]
        public void TestCorrectSolutionIsValid()
        {
            List<List<int>> numbers = new List<List<int>>() {
                new List<int>() { 9, 1, 7, 8, 3, 6, 4, 2, 5 },
                new List<int>() { 5, 2, 6, 7, 1, 4, 3, 8, 9 },
                new List<int>() { 3, 4, 8, 9, 2, 5, 7, 1, 6 },
                new List<int>() { 6, 5, 9, 2, 7, 1, 8, 4, 3 },
                new List<int>() { 8, 3, 2, 6, 4, 9, 5, 7, 1 },
                new List<int>() { 4, 7, 1, 5, 8, 3, 6, 9, 2 },
                new List<int>() { 7, 6, 5, 1, 9, 8, 2, 3, 4 },
                new List<int>() { 1, 8, 3, 4, 6, 2, 9, 5, 7 },
                new List<int>() { 2, 9, 4, 3, 5, 7, 1, 6, 8 },
            };

            Game game = new Game(numbers);
            game.BuildBoard();

            Solver solver = new Solver(game);

            Console.WriteLine(game.ToString());
            bool isGameValid = solver.IsGameValid(out HashSet<Position> invalids);
            Console.WriteLine(String.Join("\n", invalids));
            Assert.IsTrue(isGameValid);                        
        }

        [TestMethod]
        public void TestIncorrectSolutionIsInvalid()
        {
            List<List<int>> numbers = new List<List<int>>() {
                new List<int>() { 9, 1, 7, 8, 3, 6, 4, 2, 5 },
                new List<int>() { 5, 2, 6, 7, 1, 4, 3, 8, 9 },
                new List<int>() { 3, 4, 8, 9, 2, 5, 7, 1, 6 },
                new List<int>() { 6, 5, 9, 2, 7, 1, 8, 4, 3 },
                new List<int>() { 8, 3, 2, 2, 4, 9, 5, 7, 1 },
                new List<int>() { 4, 7, 1, 5, 8, 3, 6, 9, 2 },
                new List<int>() { 7, 6, 5, 1, 9, 8, 2, 3, 4 },
                new List<int>() { 1, 8, 3, 4, 6, 2, 9, 5, 7 },
                new List<int>() { 2, 9, 4, 3, 5, 7, 1, 6, 8 },
            };

            Game game = new Game(numbers);
            game.BuildBoard();

            Solver solver = new Solver(game);

            Console.WriteLine(game.ToString());
            bool isGameValid = solver.IsGameValid(out HashSet<Position> invalids);
            Console.WriteLine(String.Join("\n", invalids));
            Assert.IsFalse(isGameValid);
            Assert.IsTrue(invalids.Count == 1);

            Assert.AreEqual("D5", Square.LabelFromPosition(invalids.ToArray()[0]));
        }

        [TestMethod]
        public void TestAllowedNumbersForSquare()
        {
            Game game = new Game();
            game.BuildBoard();

            Solver solver = new Solver(game);

            game.SetNumber(0, 0, 3);
            game.SetNumber(0, 1, 4);
            game.SetNumber(0, 2, 5);
            game.SetNumber(3, 3, 6);

            Console.WriteLine(game.ToString());

            IEnumerable<int> allowedNumbers = solver.GetAllowedNumbersForSquare(0, 3);
            Assert.IsTrue(allowedNumbers.Contains(2));
            Assert.IsFalse(allowedNumbers.Contains(3));
        }
    }
}
