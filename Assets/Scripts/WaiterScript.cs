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
    public FoodScript food;

    public int orderOfCustomer;
    public int carriedFoodType;
    public int costOfFood;

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
        if (!WaiterIsPresent())
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
                    if (aiPath.desiredVelocity.x <= -0.1f)
                    {
                        transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                    else if (aiPath.desiredVelocity.x >= 0.1f)
                    {
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                    yield return null;
                }

            }

            aiDestinationSetter.target = null;
            aiPath.enabled = false;
            table.waiterHandles = false;
            transform.SetParent(table.transform);


            currentState = previousState;
            text.text = currentState.ToString();

            if (table.isDirty && table.isOccupied)
            {
                table.customer.PayAndLeave();
            }
            performingAnAction = false;
        }
    }

    public IEnumerator ClearTable()
    {
        if (!WaiterIsPresent())
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
                    if (aiPath.desiredVelocity.x <= -0.1f)
                    {
                        transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                    else if (aiPath.desiredVelocity.x >= 0.1f)
                    {
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                    yield return null;
                }

            }

            aiDestinationSetter.target = null;
            aiPath.enabled = false;

            currentState = CurrentState.ClearingTable;
            text.text = currentState.ToString();
            transform.SetParent(table.transform);


            yield return new WaitForSeconds(1.5f);

            table.rd.color = Color.white;
            table.isDirty = false;
            table.waiterHandles = false;

            currentState = previousState;
            text.text = currentState.ToString();
            performingAnAction = false;
        }
    }

    public IEnumerator GetTheOrder()
    {
        if (!WaiterIsPresent())
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

            orderOfCustomer = table.customer.order;
            currentState = CurrentState.GotTheOrder;
            text.text = currentState.ToString();
            transform.SetParent(table.transform);

            performingAnAction = false;
        }
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

        chef.orderID = orderOfCustomer;
        chef.currentState = ChefScript.CurrentState.CookingFood;
        StartCoroutine(chef.PrepareFood());

        currentState = CurrentState.Free;
        text.text = currentState.ToString();
        transform.parent = null;


        performingAnAction = false;
    }

    public IEnumerator GetTheFood()
    {
        performingAnAction = true;

        currentState = CurrentState.Walking;
        text.text = currentState.ToString();

        Transform destination = food.transform.GetChild(0);
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

        carriedFoodType = food.type;
        costOfFood = food.cost;
        Destroy(food.gameObject);
        currentState = CurrentState.CarryingFood;
        text.text = currentState.ToString();
        transform.parent = null;

        performingAnAction = false;
    }

    public IEnumerator ServeTheFood()
    {
        if (!WaiterIsPresent() && table.expectedOrder == carriedFoodType)
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

            table.customer.paymentCost = costOfFood;
            StartCoroutine(table.customer.Eat());
            table.waiterHandles = false;

            currentState = CurrentState.Free;
            text.text = currentState.ToString();
            transform.SetParent(table.transform);

            performingAnAction = false;
        }
    }


    private bool WaiterIsPresent()
    {
        for (int i = 0; i < table.transform.childCount; i++)
        {
            Transform child = table.transform.GetChild(i);
            if (child.name.Contains("Waiter") && child != transform)
            {
                return true;
            }
        }
        return false;
    }
}

