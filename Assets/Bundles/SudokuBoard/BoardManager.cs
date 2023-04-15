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
    public TMP_Text infoText;

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
        internalGame = Sudoku.Game.VeryHardGame();
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
                square.gameObject.name = $"Square {square.Label}";

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
        Sudoku.Position position = Vector2Position(square.Index);
        var internalSquare = internalGame.GetSquare(position);
        var number = internalSquare.Number;
        square.SetNumber(number);
        square.SetInternalSquare(internalSquare);
        

        if (number != 0 && fixValue)
        {
            Destroy(square.GetComponent<BoxCollider>());
            square.fixedNumber = true;
        }

        var solution = internalSolver.GetBestAvailableSolution(position);
        square.SetSolution(solution);
        if (square.Number == 0)
        {
            square.difficulty = solution.Number != 0 ? solution.Difficulty : Sudoku.Difficulty.None;                        
        };
    }

    private readonly KeyCode[] alphaNums =
    {
         KeyCode.Alpha0,
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
    };

    private readonly KeyCode[] keypadNums =
{
         KeyCode.Keypad0,
         KeyCode.Keypad1,
         KeyCode.Keypad2,
         KeyCode.Keypad3,
         KeyCode.Keypad4,
         KeyCode.Keypad5,
         KeyCode.Keypad6,
         KeyCode.Keypad7,
         KeyCode.Keypad8,
         KeyCode.Keypad9,
    };

    private void Update()
    {
        if(activeSquare != null)
        {
            for (int i = 0; i < alphaNums.Length; i++)
            {
                if (Input.GetKeyDown(alphaNums[i]) || Input.GetKeyDown(keypadNums[i]))
                {
                    SetActiveSquareNumber(i);
                }
            }
        }
        
    }

    private void UpdateBoard()
    {  
        var allSquares = from row in sudokuSquares 
                         from square in row
                         where !square.fixedNumber
                         select square;
        foreach(SudokuSquare square in allSquares)
        {
            MatchSquareToInternal(square);
            
        }
        ShowWhereNumberIsAllowed(10);
        Debug.Log(internalGame);
    }

    public void ShowWhereNumberIsAllowed(int number)
    {
        var allSquares = from row in sudokuSquares
                         from square in row
                         select square;
        foreach(SudokuSquare square in allSquares)
        {
            square.HidePossibleNumber();
        }

        var availableSquares = from row in sudokuSquares
                               from square in row
                               where !square.fixedNumber && square.Number == 0
                               select square;
        foreach (SudokuSquare square in availableSquares)
        {
            square.ShowPossibleNumber(number);            
        }
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
        if(number == 0 || internalSolver.ValidateNumberForSquare(position.x, position.y, number))
        {
            internalSolver.SetNumber(activeSquare.Index.x, activeSquare.Index.y, number);
            internalSolver.SolveBoard();            
            activeSquare.SetNumber(number);
            Deselect();
            UpdateBoard();            
        }        
    }

    public void UpdateInfoText(string text)
    {
        infoText.text = text;
    }



}
