using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private readonly int boardSize = 9;

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
                square.gameObject.name = $"Square {square.Label}";
                square.SetIndex(i, j);

                MatchSquareToInternal(square, fixValue: true);

                row.Add(square);               
            }
            sudokuSquares.Add(row);
        }
    }
    private static Sudoku.Position Vector2Position(Vector2Int v)
    {
        return new Sudoku.Position(v.x, v.y);
    }
    private void MatchSquareToInternal(SudokuSquare square, bool fixValue = false)
    {
        Debug.Log($"Updating {square}");
        Sudoku.Position position = Vector2Position(square.Index);
        var number = internalGame.GetNumber(position);
        square.SetNumber(number);

        if (number != 0 && fixValue)
        {
            Destroy(square.GetComponent<BoxCollider>());
            square.fixedNumber = true;
        }

        var solution = internalSolver.GetBestAvailableSolution(position);
        if (square.Number == 0 && solution.Number != 0)
        {
            square.difficulty = solution.Difficulty;
        };
    }

    private void UpdateBoard()
    {
        Debug.Log("Updating Board");
        var allSquares = from row in sudokuSquares 
                         from square in row
                         where !square.fixedNumber
                         select square;
        foreach(SudokuSquare square in allSquares)
        {
            MatchSquareToInternal(square);
            
        }
        Debug.Log(internalGame);
    }

    public void SetActiveSquare(SudokuSquare square)
    {
        bool sameSquare = false;
        
        if(activeSquare != null)
        {
            sameSquare = activeSquare.Equals(square);
            Deselect();        
                        
        }      
        if(!sameSquare)
        {
            activeSquare = square;
            activeSquare.Select();
        }
        
    }

    private void Deselect()
    {
        activeSquare.Deselect();
        numberDropdown.value = 0;
        activeSquare = null;
    }

    public void SetActiveSquareNumber(int number)
    {
        var position = Vector2Position(activeSquare.Index);
        if(internalSolver.ValidateNumberForSquare(position.x, position.y, number))
        {
            internalSolver.SetNumber(activeSquare.Index.x, activeSquare.Index.y, number);
            internalSolver.SolveBoard();            
            activeSquare.SetNumber(number);
            Deselect();
            UpdateBoard();
            
        }
        
    }



}
