using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
public class InputHandler : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject activeTouchedObject = null;
    public SelectedPerson selectedPerson = SelectedPerson.None;
    public GameObject waiterObj;
    public GameObject customerObj;
    public GameObject chefObj;
    public LayerMask foodLayer;
    public LayerMask waiterLayer;
    public enum SelectedPerson
    {
        None,
        Chef,
        Waiter,
        Customer,
    }


    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                HandleTouchStart(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                HandleTouchEnd(touch.position);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            HandleTouchStart(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HandleTouchEnd(Input.mousePosition);
        }
    }

    private void HandleTouchStart(Vector2 inputPosition)
    {
        Vector2 touchPosition = mainCamera.ScreenToWorldPoint(inputPosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(touchPosition, Vector2.zero);

        if (hits.Length > 1)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Food") || hit.collider.CompareTag("Waiter"))
                {
                    activeTouchedObject = hit.collider.gameObject;
                    return; 
                }
            }
        }

        foreach (var hit in hits)
        {
            activeTouchedObject = hit.collider.gameObject;
            return; 
        }

        StartCoroutine(ReturnToWaiter());
    }

    private void HandleTouchEnd(Vector2 inputPosition)
    {
        if (activeTouchedObject != null && activeTouchedObject.CompareTag("Food"))
        {
            Vector2 touchPosition = mainCamera.ScreenToWorldPoint(inputPosition);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero, Mathf.Infinity, foodLayer);

            if (hit.collider != null && hit.collider.gameObject == activeTouchedObject)
            {
                if (selectedPerson == SelectedPerson.Waiter && waiterObj.GetComponent<WaiterScript>().currentState == WaiterScript.CurrentState.Free)
                {
                    FoodScript food = activeTouchedObject.GetComponent<FoodScript>();
                    ChefScript chef = food.chefThatPrepared.GetComponent<ChefScript>();
                    WaiterScript waiter = waiterObj.GetComponent<WaiterScript>();

                    for (int i = 0; i < chef.platePoints.Count; i++)
                    {

                        if (food.gameObject == chef.platePoints[i])
                        {
                            chef.platePoints[i] = null;
                            waiter.food = food;
                            StartCoroutine(waiter.GetTheFood());
                        }
                    }
                    return;
                }
            }
        }

        if (activeTouchedObject != null && activeTouchedObject.CompareTag("Waiter"))
        {
            Vector2 touchPosition = mainCamera.ScreenToWorldPoint(inputPosition);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero, Mathf.Infinity, waiterLayer); ;

            if (hit.collider != null && hit.collider.gameObject == activeTouchedObject)
            {
                if (waiterObj != null)
                {
                    waiterObj.transform.GetChild(1).GetComponent<Light2D>().enabled = false;
                }
                if (customerObj != null)
                {
                    customerObj.transform.GetChild(1).GetComponent<Light2D>().enabled = false;
                }

                ClearPeople();
                selectedPerson = SelectedPerson.Waiter;
                waiterObj = activeTouchedObject;

                waiterObj.transform.GetChild(1).GetComponent<Light2D>().enabled = true;


                var tables = FindObjectsOfType<TableScript>();

                foreach (TableScript table in tables)
                {
                    for (int i = 0; i < table.transform.childCount; i++)
                    {
                        if (table.transform.GetChild(i).name.Contains("Highlight"))
                        {
                            table.transform.GetChild(i).gameObject.SetActive(false);
                        }
                    }

                    if (waiterObj.GetComponent<WaiterScript>().currentState == WaiterScript.CurrentState.CarryingFood)
                    {

                        if (table.expectedOrder == waiterObj.GetComponent<WaiterScript>().carriedFoodType)
                        {

                            for(int i = 0; i < table.transform.childCount; i++)
                            {
                                if (table.transform.GetChild(i).name.Contains("Highlight") && table.customer != null && table.customer.currentState == CustomerScript.CurrentState.isWaitingToReceiveTheOrder)
                                {
                                    table.transform.GetChild(i).gameObject.SetActive(true);
                                }                          
                            }
                        }
                    }
                }
                return;
            }
        }


        if (activeTouchedObject != null)
        {
            Vector2 touchPosition = mainCamera.ScreenToWorldPoint(inputPosition);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == activeTouchedObject)
            {


                if (activeTouchedObject.CompareTag("Table"))
                {
                    TableScript table = activeTouchedObject.GetComponent<TableScript>();

                    if (selectedPerson == SelectedPerson.Customer && customerObj.GetComponent<CustomerScript>().currentState == CustomerScript.CurrentState.isStanding && !table.isDirty && !table.isOccupied)
                    {
                        customerObj.transform.SetParent(table.transform);
                        for(int i = 0; i< table.transform.childCount; i++)
                        {
                            if (table.transform.GetChild(i).name.Contains("Customer"))
                            {
                                table.sittingCustomer = table.transform.GetChild(i).gameObject;
                                break;
                            }
                        }
                        table.customer = table.sittingCustomer.GetComponent<CustomerScript>();

                        StartCoroutine(table.customer.SitAndThink());

                        StartCoroutine(ReturnToWaiter());


                    }
                    else if (selectedPerson == SelectedPerson.Customer && (table.isOccupied || table.isDirty))
                    {
                        StartCoroutine(ReturnToWaiter());
                    }
                     


                    // WAITER'S TABLE BEHAVIOR STARTS

                    if (selectedPerson == SelectedPerson.Waiter)
                    {
                        WaiterScript waiter = waiterObj.GetComponent<WaiterScript>();
                        if (!waiter.performingAnAction)
                            waiter.table = table;

                        if (table.isDirty && !table.isOccupied && !table.waiterHandles && waiter.currentState != WaiterScript.CurrentState.Walking && waiter.currentState != WaiterScript.CurrentState.ClearingTable)
                        {
                            StartCoroutine(waiter.ClearTable());
                        }
                        else if (!table.isOccupied && !table.waiterHandles && waiter.currentState != WaiterScript.CurrentState.Walking && waiter.currentState != WaiterScript.CurrentState.ClearingTable)
                        {
                            StartCoroutine(waiter.WalkWithoutAction());
                        }

                        if (table.isOccupied && !table.waiterHandles)
                        {



                            if (table.customer.currentState == CustomerScript.CurrentState.isWaitingToGiveOrders && waiter.currentState == WaiterScript.CurrentState.Free)
                            {
                                // Waiter gets the order
                                StartCoroutine(waiter.GetTheOrder());
                            }
                            else if (table.customer.currentState == CustomerScript.CurrentState.isWaitingToReceiveTheOrder && waiter.currentState == WaiterScript.CurrentState.CarryingFood)
                            {
                                // Waiter delivers food
                                StartCoroutine(waiter.ServeTheFood());
                            }
                            else if (!waiter.performingAnAction)
                            {
                                StartCoroutine(waiter.WalkWithoutAction()); //also collects payment and gets the order
                            }
                        }
                    }
                            // WAITER'S TABLE BEHAVIOR ENDS
                }



                else if (activeTouchedObject.CompareTag("Chef"))
                {
                    if (selectedPerson == SelectedPerson.Waiter)
                    {

                        chefObj = activeTouchedObject;
                        ChefScript chef = chefObj.GetComponent<ChefScript>();
                        WaiterScript waiter = waiterObj.GetComponent<WaiterScript>();

                        if (chef.currentState == ChefScript.CurrentState.Free && waiter.currentState == WaiterScript.CurrentState.GotTheOrder)
                        {


                            waiter.chef = chefObj.GetComponent<ChefScript>();
                            StartCoroutine(waiter.GiveTheOrderToChef());


                        }


                        chefObj = null;

                    }
                    else
                    {
                        StartCoroutine(ReturnToWaiter());
                    }
                }


                else if (activeTouchedObject.CompareTag("Customer"))
                {
                    if(customerObj != null)
                    {
                        customerObj.transform.GetChild(1).GetComponent<Light2D>().enabled = false;                   //Take the already highlightened customer and de-highlight it
                    }

                    selectedPerson = SelectedPerson.Customer;
                    customerObj = activeTouchedObject;
                    customerObj.transform.GetChild(1).GetComponent<Light2D>().enabled = true;                        //Highlight the current customer and de-highlight the last waiter


                    if (waiterObj != null)
                    {
                        waiterObj.transform.GetChild(1).GetComponent<Light2D>().enabled = false;
                    }

                    DisableTableHighlight();
                }
            }

            activeTouchedObject = null;
        }
    }

    private void ClearPeople()
    {
        customerObj = null;
        waiterObj = null;
        chefObj = null;
    }

    private IEnumerator ReturnToWaiter()
    {
        selectedPerson = SelectedPerson.None;

        if(customerObj != null)
        {
            customerObj.transform.GetChild(1).GetComponent<Light2D>().enabled = false;
            customerObj = null;
        }

        yield return new WaitForSeconds(0.1f);

        if(waiterObj != null)
        {
            selectedPerson = SelectedPerson.Waiter;
            waiterObj.transform.GetChild(1).GetComponent<Light2D>().enabled = true;

            var tables = FindObjectsOfType<TableScript>();

            foreach (TableScript table in tables)
            {

                if (waiterObj.GetComponent<WaiterScript>().currentState == WaiterScript.CurrentState.CarryingFood)
                {

                    if (table.expectedOrder == waiterObj.GetComponent<WaiterScript>().carriedFoodType)
                    {
                        for (int i = 0; i < table.transform.childCount; i++)
                        {
                            if (table.transform.GetChild(i).name.Contains("Highlight") && table.customer != null && table.customer.currentState == CustomerScript.CurrentState.isWaitingToReceiveTheOrder)
                            {
                                table.transform.GetChild(i).gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
    }

    private void DisableTableHighlight()
    {
        var tables = FindObjectsOfType<TableScript>();

        foreach (TableScript table in tables)
        {
            for (int i = 0; i < table.transform.childCount; i++)
            {
                if (table.transform.GetChild(i).name.Contains("Highlight"))
                {
                    table.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}