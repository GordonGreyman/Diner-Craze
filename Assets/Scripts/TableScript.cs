using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableScript : MonoBehaviour
{
    public bool isOccupied = false;
    public bool isDirty = false;
    public bool waiterHandles = false;
    public GameObject sittingCustomer;
    public CustomerScript customer;

    public SpriteRenderer rd;

    private void Start()
    {
        rd = GetComponent<SpriteRenderer>();
    }


}
