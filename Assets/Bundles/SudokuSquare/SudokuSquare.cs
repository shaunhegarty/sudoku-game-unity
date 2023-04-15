using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using UnityEngine;
using TMPro;

public class SudokuSquare : MonoBehaviour
{
    public TMP_Text textMesh;
    public TMP_Text indexMesh;
    public TMP_Text tempMesh;
    public int Number;
    public bool fixedNumber = false;
    public Vector2Int Index { get; private set; }
    public Sudoku.Difficulty difficulty = Sudoku.Difficulty.None;
    private Sudoku.Square internalSquare;
    private Sudoku.Solution solution;

    public string Label { get
        {
            return LabelFromPosition(Index);
        } 
    }

    public MaterialChangeOnHighlight highlighter;

    private void Start()
    {
        UpdateIndexText();
        Debug.Log($"{textMesh}");
        highlighter = GetComponent<MaterialChangeOnHighlight>();
    }

    // Update is called once per frame
    void Update()
    {
       UpdateNumberText();     
    }

    public void ShowPossibleNumber(int number)
    {
        //if(internalSquare.AllowedNumbers.Contains(number)) {
            tempMesh.gameObject.SetActive(true);
            StringBuilder sb = new("", 50);
            var allowedNumbers = internalSquare.AllowedNumbers.ToArray();
            for(int i = 0; i < internalSquare.AllowedNumbers.Count; i++)
            {
                if (i > 0 && i % 3 == 0)
                {
                    sb.Append('\n');
                }
                sb.Append(allowedNumbers[i].ToString());
                
            }
            tempMesh.text = $"<mspace=16.00>{sb}</mspace>";            
        //}
        
    }

    public void HidePossibleNumber()
    {
        tempMesh.gameObject.SetActive(false);        
    }

    public string InfoText()
    {
        return $"Square {Label}\n" +
            $"Allowed Numbers: \n{String.Join(", ", internalSquare.AllowedNumbers)}\n" +
            $"Solution: {solution.Number}\n" +
            $"Difficulty: {solution.Difficulty}\n" +
            $"Explanation: {solution.Explanation}";
    }

    public void SetInternalSquare(Sudoku.Square square)
    {
        internalSquare = square;
    }

    public void SetSolution(Sudoku.Solution _solution)
    {
        solution = _solution;
    }

    private void OnMouseDown()
    {
        GameManager.Instance.Board.SetActiveSquare(this);            
    }

    public void SetNumber(int num)
    {
        Number = num;        
    }

    public void SetIndex(int x, int y)
    {
        Index = new(x, y);
    }

    public void UpdateNumberText()
    {
        textMesh.text = Number > 0 ? $"{Number}": "";
    }

    public void UpdateIndexText()
    {        
        indexMesh.text = Label;
    }    

    public static string LabelFromPosition(Vector2Int pos)
    {
        return $"{(char)('A' + pos.y)}{pos.x + 1}";
    }

    public void Select()
    {
        highlighter.OnSelect();
    }

    public void Deselect()
    {
        highlighter.OnDeselect();
    }
}
