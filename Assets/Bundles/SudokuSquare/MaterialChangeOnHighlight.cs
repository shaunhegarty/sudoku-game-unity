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

    private bool isSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        baseRenderer = targetObject.GetComponent<Renderer>();
        baseMaterial = baseRenderer.material;
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
            baseRenderer.material = baseMaterial;
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
            baseRenderer.material = baseMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
