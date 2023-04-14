using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChangeOnHighlight : MonoBehaviour
{
    private Material baseMaterial;
    private Renderer baseRenderer;
    public GameObject targetObject;

    public Material hoverMaterial;
    public Material selectMaterial;

    public Material notYetSolveMaterial;
    public Material easySolveMaterial;
    public Material mediumSolveMaterial;
    private Material currentMaterial;

    private bool isSelected = false;
    private Sudoku.Difficulty lastDifficulty = Sudoku.Difficulty.None;
    private SudokuSquare square;

    // Start is called before the first frame update
    void Start()
    {
        baseRenderer = targetObject.GetComponent<Renderer>();
        baseMaterial = baseRenderer.material;
        currentMaterial = baseMaterial;
        square = GetComponent<SudokuSquare>();
        SetMaterialByDifficulty(square.difficulty);
    }

    private void OnMouseEnter()
    {
        if(!isSelected)
        {
            baseRenderer.material = hoverMaterial;
        }
    }

    private void OnMouseExit()
    {
        if(!isSelected)
        {
            baseRenderer.material = currentMaterial;
        }
    }

    public void OnSelect()
    {
        if(!isSelected)
        {
            isSelected = true;
            baseRenderer.material = selectMaterial;
        }
    }

    public void OnDeselect()
    {
        if(isSelected)
        {
            isSelected = false;
            baseRenderer.material = currentMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(lastDifficulty != square.difficulty)
        {
            lastDifficulty = square.difficulty;
            SetMaterialByDifficulty(square.difficulty);
        }
    }

    public void SetMaterialByDifficulty(Sudoku.Difficulty difficulty)
    {
        if (difficulty == Sudoku.Difficulty.None)
        {
            if(square.Number == 0)
            {
                currentMaterial = notYetSolveMaterial;
            } else
            {
                currentMaterial = baseMaterial;
            }            
        }
        else if (difficulty == Sudoku.Difficulty.Easy)
        {
            currentMaterial = easySolveMaterial;
        }
        else if (difficulty == Sudoku.Difficulty.Medium)
        {
            currentMaterial = mediumSolveMaterial;
        } else
        {
            currentMaterial = notYetSolveMaterial;
        }
        baseRenderer.material = currentMaterial;
    }
}
