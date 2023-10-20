using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableScript : MonoBehaviour
{
    public bool isOccupied = false;
    public bool isDirty = false;
    public GameObject sittingCustomer;
    public CustomerScript customerScript;

    private SpriteRenderer rd;

    private void Start()
    {
        rd = GetComponent<SpriteRenderer>();
    }



    public void CustomersLeaving()
    {
        rd.color = Color.black;
        customerScript.currentState = CustomerScript.CurrentState.isLeaving;
        isDirty = true;
        isOccupied = false;
    }

    public void WaiterCleaned()
    {
        rd.color = Color.white;

    }
}
