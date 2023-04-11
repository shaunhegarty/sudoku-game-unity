using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardManager : MonoBehaviour
{
    public SudokuSquare squarePrefab;
    public float spacing = 0.1f;
    public float innerSpacing = 0.05f;

    public SudokuSquare activeSquare = null;

    public TMP_Dropdown numberDropdown;

    private readonly List<List<SudokuSquare>> sudokuSquares = new();
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.RegisterBoard(this);

        int size = 9;
        BoxCollider squareCollider = squarePrefab.GetComponent<BoxCollider>();
        float xAdjustment;
        float yAdjustment;
        for (int i = 0; i < size; i++)
        {
            List<SudokuSquare> row = new();
            for(int j = 0; j < size; j++)
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
                
                row.Add(square);
            }
            sudokuSquares.Add(row);
        }
    }

    public void SetActiveSquare(SudokuSquare square)
    {
        activeSquare = square;
    }

    public void SetActiveSquareNumber(int number)
    {
        activeSquare.SetNumber(number);
    }



}
