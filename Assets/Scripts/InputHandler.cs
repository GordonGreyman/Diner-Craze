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
                    ClearPeople();
                    selectedPerson = SelectedPerson.Chef;
                    chefObj = activeTouchedObject;
                }
                else if (activeTouchedObject.CompareTag("Waiter"))
                {
                    ClearPeople();
                    selectedPerson = SelectedPerson.Waiter;
                    waiterObj = activeTouchedObject;

                }
                else if (activeTouchedObject.CompareTag("Table"))
                {
                    var table = activeTouchedObject.GetComponent<TableScript>();
                    Debug.Log("clicked table is " + table.gameObject.name);
                    CustomerScript customer = null;
                    WaiterScript waiter = null;

                    if (customerObj != null) customer = customerObj.GetComponent<CustomerScript>();

                    if (waiterObj != null) waiter = waiterObj.GetComponent<WaiterScript>();

                    if (selectedPerson == SelectedPerson.Customer && !table.isDirty && !table.isOccupied)
                    {
                        table.sittingCustomer = customer.gameObject;
                        table.customerScript = table.sittingCustomer.GetComponent<CustomerScript>();
                        StartCoroutine(table.customerScript.Think());
                        table.isOccupied = true;
                        
                    }
                    else if (selectedPerson == SelectedPerson.Customer && table.isOccupied)
                    {
                        Debug.Log("Table is occupied");
                    }
                    else if (selectedPerson == SelectedPerson.Customer && table.isDirty)
                    {
                        Debug.Log("Table is dirty");
                    }


                    if (selectedPerson == SelectedPerson.Waiter && table.isDirty)
                    {

                        //Clean code

                        table.WaiterCleaned();
                        table.isDirty = false;
                    }
                    else if (selectedPerson == SelectedPerson.Waiter && table.customerScript != null && table.customerScript.currentState == CustomerScript.CurrentState.isWaitingToGiveOrders)
                    {
                        Debug.Log("took the order");

                        table.customerScript.currentState = CustomerScript.CurrentState.isWaitingToReceiveTheOrder;
                    }
                    else if (selectedPerson == SelectedPerson.Waiter && table.customerScript != null && table.customerScript.currentState == CustomerScript.CurrentState.isWaitingToReceiveTheOrder)
                    {
                        table.CustomersLeaving();
                    }
                    else if (selectedPerson == SelectedPerson.Waiter && table.customerScript != null && table.customerScript.currentState == CustomerScript.CurrentState.isWaitingToPay)
                    {
                        //Get the payment
                        
                    }

                    selectedPerson = SelectedPerson.None;
                    ClearPeople();
                    table = null;
                }
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