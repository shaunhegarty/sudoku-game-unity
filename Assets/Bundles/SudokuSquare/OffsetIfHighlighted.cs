using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetIfHighlighted : MonoBehaviour
{

    public Vector3 offset = Vector3.zero;
    public float offsetSpeed = 1;
    public Vector3 BasePosition { get; private set; }

    public bool highlighted = false;

    // Start is called before the first frame update
    void Start()
    {
        BasePosition = transform.position;     
    }

    // Update is called once per frame
    void Update()
    {
        if(highlighted)
        {
            Offset();
        }
        else
        {
            ReturnToBase();
        }
    }

    private void OnMouseEnter()
    {
        highlighted = true;
    }


    private void OnMouseExit()
    {
        highlighted = false;
    }

    private void Offset()
    {

        if (transform.position.y < BasePosition.y + offset.y)
        {
            transform.position = transform.position + new Vector3(0, offsetSpeed * Time.deltaTime, 0);
        }
                        
        
    }

    private void ReturnToBase()
    {
        if (transform.position.y > BasePosition.y)
        {
            transform.position = transform.position - new Vector3(0, offsetSpeed * Time.deltaTime, 0);
        }
    }
}
