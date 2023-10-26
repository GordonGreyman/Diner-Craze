using System.Collections;
using UnityEngine;
using Pathfinding;
using TMPro;

public class WaiterScript : MonoBehaviour
{
    public CurrentState currentState = CurrentState.Free;
    AIPath aiPath;
    AIDestinationSetter aiDestinationSetter;
    public bool performingAnAction = false;
    private float stopDistance = 0.2f;
    private float checkDistance = 1f;
    public TextMeshProUGUI text;
    public TableScript table;
    public ChefScript chef;

    private void Start()
    {
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        
    }
    public enum CurrentState
    {
        Free,
        Walking,
        GotTheOrder,
        CarryingFood,
        ClearingTable,
    }

    public IEnumerator WalkWithoutAction()
    {
        performingAnAction = true;

        CurrentState previousState = currentState;
        currentState = CurrentState.Walking;
        text.text = currentState.ToString();

        table.waiterHandles = true;
        Transform destination = table.transform.GetChild(0);
        aiDestinationSetter.target = destination.transform;
        aiPath.enabled = true;

        if (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > checkDistance)
        {

            while (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > stopDistance)
            {
                yield return null;
            }

        }

        aiDestinationSetter.target = null;
        aiPath.enabled = false;
        table.waiterHandles = false;

        currentState = previousState;
        text.text = currentState.ToString();
        performingAnAction = false;
    }
    public IEnumerator ClearTable()
    {
        performingAnAction = true;

        CurrentState previousState = currentState;
        currentState = CurrentState.Walking;
        text.text = currentState.ToString();



        table.waiterHandles = true;
        Transform destination = table.transform.GetChild(0);
        aiDestinationSetter.target = destination.transform;
        aiPath.enabled = true;

        if (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > checkDistance) { 

            while (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > stopDistance)
            {
                yield return null;
            }
        
        }

        aiDestinationSetter.target = null;
        aiPath.enabled = false;
        table.waiterHandles = false;


        currentState = CurrentState.ClearingTable;
        text.text = currentState.ToString();


        yield return new WaitForSeconds(1.5f);

        table.rd.color = Color.white;
        table.isDirty = false;
        table.waiterHandles = false;

        currentState = previousState;
        text.text = currentState.ToString();
        performingAnAction = false;
    }

    public IEnumerator GetTheOrder()
    {
        performingAnAction = true;

        currentState = CurrentState.Walking;
        text.text = currentState.ToString();


        table.waiterHandles = true;

        Transform destination = table.transform.GetChild(0);
        aiDestinationSetter.target = destination.transform;
        aiPath.enabled = true;

        if (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > checkDistance)
        {

            while (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > stopDistance)
            {
                yield return null;
            }

        }

        aiDestinationSetter.target = null;
        aiPath.enabled = false;

        table.customer.currentState = CustomerScript.CurrentState.isWaitingToReceiveTheOrder;
        table.customer.rd.color = Color.cyan;
        table.customer.text.text = table.customer.currentState.ToString();
        table.waiterHandles = false;

        currentState = CurrentState.GotTheOrder;
        text.text = currentState.ToString();
        performingAnAction = false;

    }


    public IEnumerator GiveTheOrderToChef()
    {
        performingAnAction = true;

        currentState = CurrentState.Walking;
        text.text = currentState.ToString();

        Transform destination = chef.transform.GetChild(0);
        aiDestinationSetter.target = destination.transform;
        aiPath.enabled = true;

        if (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > checkDistance)
        {

            while (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > stopDistance)
            {
                yield return null;
            }

        }

        aiDestinationSetter.target = null;
        aiPath.enabled = false;

        chef.currentState = ChefScript.CurrentState.CookingFood;
        StartCoroutine(chef.PrepareFood());

        currentState = CurrentState.Free;
        text.text = currentState.ToString();
        performingAnAction = false;
    }
    public IEnumerator GetTheFood()
    {
        performingAnAction = true;

        currentState = CurrentState.Walking;
        text.text = currentState.ToString();

        Transform destination = chef.transform.GetChild(0);
        aiDestinationSetter.target = destination.transform;
        aiPath.enabled = true;

        if (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > checkDistance)
        {

            while (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > stopDistance)
            {
                yield return null;
            }

        }

        aiDestinationSetter.target = null;
        aiPath.enabled = false;

        chef.currentState = ChefScript.CurrentState.Free;


        currentState = CurrentState.CarryingFood;
        text.text = currentState.ToString();
        performingAnAction = false;
    }

    public IEnumerator ServeTheFood()
    {
        performingAnAction = true;
        currentState = CurrentState.Walking;
        text.text = currentState.ToString();


        table.waiterHandles = true;

        Transform destination = table.transform.GetChild(0);
        aiDestinationSetter.target = destination.transform;
        aiPath.enabled = true;

        if (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > checkDistance)
        {

            while (Vector3.Distance(transform.position, aiDestinationSetter.target.transform.position) > stopDistance)
            {
                yield return null;
            }

        }

        aiDestinationSetter.target = null;
        aiPath.enabled = false;

        StartCoroutine(table.customer.Eat());
        table.waiterHandles = false;

        currentState = CurrentState.Free;
        text.text = currentState.ToString();
        performingAnAction = false;
    }



}

