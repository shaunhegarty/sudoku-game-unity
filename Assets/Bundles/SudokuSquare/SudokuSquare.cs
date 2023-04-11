using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SudokuSquare : MonoBehaviour
{
    public TMP_Text textMesh;
    public TMP_Text indexMesh;
    public int Number;
    public Vector2Int Index { get; private set; }
    public string Label { get
        {
            return LabelFromPosition(Index);
        } 
    }

    private void Start()
    {
        UpdateIndexText();
        Debug.Log($"{textMesh}");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNumberText();
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
}
