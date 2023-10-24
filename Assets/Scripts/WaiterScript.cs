using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class WaiterScript : MonoBehaviour
{
    public CurrentState currentState = CurrentState.Free;
    AIPath aiPath;
    AIDestinationSetter aiDestinationSetter;
    public float x = 0.1f;

    public GameObject tableCustomerSits;

    private void Start()
    {
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
    }
    public enum CurrentState
    {
        Free,
        GotTheOrder,
        CarryingFood,
        ClearingTable,
    }


    public IEnumerator ClearTable()
    {

        TableScript table = tableCustomerSits.GetComponent<TableScript>();
        Transform a = table.transform.GetChild(0);
        aiDestinationSetter.target = a.transform;
        aiPath.enabled = true;

        if (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > .3f) { 

            while (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > x)
            {
                yield return null;
            }
        
        }

        aiDestinationSetter.target = null;
        aiPath.enabled = false;


        currentState = CurrentState.ClearingTable;

        yield return new WaitForSeconds(1.5f);

        currentState = CurrentState.Free;
        table.rd.color = Color.white;
        table.isDirty = false;
    }

    public IEnumerator GetTheOrder()
    {
        TableScript table = tableCustomerSits.GetComponent<TableScript>();
        Transform a = table.transform.GetChild(0);
        aiDestinationSetter.target = a.transform;
        aiPath.enabled = true;

        if (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > .3f)
        {

            while (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > x)
            {
                yield return null;
            }

        }

        aiDestinationSetter.target = null;
        aiPath.enabled = false;

        currentState = CurrentState.GotTheOrder;

    }

    public IEnumerator GetTheFood()
    {
        yield return null;

    }

    public IEnumerator ServeTheFood()
    {
        yield return null;

    }



}

