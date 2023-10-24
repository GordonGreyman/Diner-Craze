using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomerScript : MonoBehaviour
{
    public CurrentState currentState = CurrentState.isStanding;
    public TableScript tableCustomerSits;
    public SpriteRenderer rd;
    public TextMeshProUGUI text;

    private void Start()
    {
        rd = GetComponent<SpriteRenderer>();
    }
    public enum CurrentState
    {
        None,
        isStanding,
        isThinking,
        isWaitingToGiveOrders,
        isWaitingToReceiveTheOrder,
        isEating,
        isWaitingToPay,
    }

    private void Update()
    {
        Debug.Log(currentState);
    }

    public IEnumerator SitAndThink()
    {
        tableCustomerSits = transform.parent.GetComponent<TableScript>();
        currentState = CurrentState.isThinking;
        tableCustomerSits.isOccupied = true;

        float moveSpeed = 5.0f;
        Vector3 targetPosition = new Vector3(tableCustomerSits.transform.position.x, tableCustomerSits.transform.position.y + 1, tableCustomerSits.transform.position.z);

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float step = moveSpeed * Time.deltaTime;

            transform.position = transform.position + moveDirection * step;
            yield return null;
        }

        transform.position = targetPosition;

        rd.color = Color.green;
        text.text = currentState.ToString();

        float seconds = Random.Range(3, 6);
        yield return new WaitForSeconds(seconds);

        currentState = CurrentState.isWaitingToGiveOrders;

        rd.color = Color.yellow;
        text.text = currentState.ToString();

    }


    public IEnumerator Eat()
    {
        currentState = CurrentState.isEating;
        rd.color = Color.red;
        text.text = currentState.ToString();


        float seconds = Random.Range(3, 6);
        yield return new WaitForSeconds(seconds);

        currentState = CurrentState.isWaitingToPay;

        rd.color = Color.blue;
        text.text = currentState.ToString();

        tableCustomerSits.rd.color = Color.black;
        tableCustomerSits.isDirty = true;
    }


    public void PayAndLeave()
    {
        Debug.Log("al abimmm");
        rd.color = Color.white;
        text.text = currentState.ToString();

        Transform customerAsChild = tableCustomerSits.transform.GetChild(1);
        customerAsChild.parent = null;
        tableCustomerSits.isOccupied = false;
        currentState = CustomerScript.CurrentState.None;
       
    }
}
