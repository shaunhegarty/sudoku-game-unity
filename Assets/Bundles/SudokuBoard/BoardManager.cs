using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardManager : MonoBehaviour
{
    // Game Components
    public SudokuSquare squarePrefab;
    private BoxCollider squareCollider;
    public TMP_Dropdown numberDropdown;

    // Position Settings
    public float spacing = 0.1f;
    public float innerSpacing = 0.05f;
    private int boardSize = 9;

    // Game State
    private Sudoku.Game internalGame;
    private Sudoku.Solver internalSolver;
    public SudokuSquare activeSquare = null;    
    private readonly List<List<SudokuSquare>> sudokuSquares = new();

    
    void Start()
    {
        GameManager.Instance.RegisterBoard(this);       
        squareCollider = squarePrefab.GetComponent<BoxCollider>();
        
        SetupInternalBoard();
        SetupBoard();
    }

    private void SetupInternalBoard()
    {
        internalGame = Sudoku.Game.DefaultGame();
        internalGame.BuildBoard();

        internalSolver = new(internalGame);
        internalSolver.SolveBoard();
    }

    private void SetupBoard()
    {
        float xAdjustment;
        float yAdjustment;
        for (int i = 0; i < boardSize; i++)
        {
            List<SudokuSquare> row = new();
            for (int j = 0; j < boardSize; j++)
            {
                xAdjustment = i % 3 == 0 ? innerSpacing : 0;
                yAdjustment = j % 3 == 0 ? innerSpacing : 0;
                var square = Instantiate(
                    original: squarePrefab,
                    position: squarePrefab.transform.position + new Vector3(
                        x: i * (squareCollider.size.x + spacing) + xAdjustment,
                        y: 0,
                        z: j * (squareCollider.size.z + spacing) + yAdjustment
                    ),
                    rotation: squarePrefab.transform.rotation);
                square.transform.SetParent(transform);

                square.SetIndex(i, j);
                var index = new Sudoku.Position(i, j);
                square.SetNumber(internalGame.GetNumber(index));
                var solutions = internalSolver.GetSolutionsByIndex(index);
                if (square.Number == 0 && (solutions != null && solutions.Count > 0))
                {
                    square.difficulty = SudokuSquare.SolveDifficulty.Medium;
                };
                square.gameObject.name = $"Square {square.Label}";

                row.Add(square);
            }
            sudokuSquares.Add(row);
        }
    }

    public void SetActiveSquare(SudokuSquare square)
    {
        if(activeSquare != null)
        {
            activeSquare.Deselect();
        }        
        activeSquare = square;
        activeSquare.Select();
    }

    public void SetActiveSquareNumber(int number)
    {
        activeSquare.SetNumber(number);
    }



}
