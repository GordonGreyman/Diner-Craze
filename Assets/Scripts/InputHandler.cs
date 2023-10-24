using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject activeTouchedObject = null;
    public SelectedPerson selectedPerson = SelectedPerson.None;
    public GameObject waiterObj;
    public GameObject customerObj;
    public GameObject chefObj;

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
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

        if (hit.collider != null)
        {
            activeTouchedObject = hit.collider.gameObject;

        }
        else
        {
            selectedPerson = SelectedPerson.None;
            ClearPeople();
        }
    }

    private void HandleTouchEnd(Vector2 inputPosition)
    {
        if (activeTouchedObject != null)
        {
            Vector2 touchPosition = mainCamera.ScreenToWorldPoint(inputPosition);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == activeTouchedObject)
            {


                if (activeTouchedObject.CompareTag("Chef"))
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
                        else if(chef.currentState == ChefScript.CurrentState.PreparedTheFood && waiter.currentState == WaiterScript.CurrentState.Free)
                        {

                            StartCoroutine(waiter.GetTheFood());
                            ClearPeople();
                            selectedPerson = SelectedPerson.None;
                        }
                        else
                        {
                            selectedPerson = SelectedPerson.None;
                            ClearPeople();
                        }

                    }
                    else
                    {
                        ClearPeople();
                        selectedPerson = SelectedPerson.Chef;

                    }
                }


                else if (activeTouchedObject.CompareTag("Waiter"))
                {
                    ClearPeople();
                    selectedPerson = SelectedPerson.Waiter;
                    waiterObj = activeTouchedObject;

                }


                else if (activeTouchedObject.CompareTag("Table"))
                {
                    TableScript table = activeTouchedObject.GetComponent<TableScript>();

                    if (selectedPerson == SelectedPerson.Customer && customerObj.GetComponent<CustomerScript>().currentState == CustomerScript.CurrentState.isStanding && !table.isDirty && !table.isOccupied)
                    {
                        customerObj.transform.SetParent(table.transform);
                        table.sittingCustomer = table.transform.GetChild(1).gameObject;
                        table.customer = table.sittingCustomer.GetComponent<CustomerScript>();


                        StartCoroutine(table.customer.SitAndThink());

                        
                    }
                    else if (selectedPerson == SelectedPerson.Customer && table.isOccupied)
                    {
                        Debug.Log("Table is occupied");
                    }
                    else if (selectedPerson == SelectedPerson.Customer && table.isDirty)
                    {
                        Debug.Log("Table is dirty");
                    }




                    // WAITER'S TABLE BEHAVIOR STARTS

                    if (selectedPerson == SelectedPerson.Waiter)
                    {
                        WaiterScript waiter = waiterObj.GetComponent<WaiterScript>();
                        waiter.table = table;

                        if (table.isDirty && !table.isOccupied && !table.waiterHandles && waiter.currentState != WaiterScript.CurrentState.Walking && waiter.currentState != WaiterScript.CurrentState.ClearingTable)
                        {
                            StartCoroutine(waiter.ClearTable());
                        }
                        else if (!table.isOccupied && waiter.currentState != WaiterScript.CurrentState.Walking && waiter.currentState != WaiterScript.CurrentState.ClearingTable)
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
                            else if (table.customer.currentState == CustomerScript.CurrentState.isWaitingToPay)
                            {
                                // Waiter collects payment
                                table.customer.PayAndLeave();
                            }
                            else if (waiter.currentState != WaiterScript.CurrentState.Walking && waiter.currentState != WaiterScript.CurrentState.ClearingTable)
                            {
                                StartCoroutine(waiter.WalkWithoutAction());
                            }
                        }
                    }

                    selectedPerson = SelectedPerson.None;
                    ClearPeople();
                    table = null;
                }

                                // WAITER'S TABLE    BEHAVIOR ENDS




                else if (activeTouchedObject.CompareTag("Customer"))
                {
                    ClearPeople();
                    selectedPerson = SelectedPerson.Customer;
                    customerObj = activeTouchedObject;
                    
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
}