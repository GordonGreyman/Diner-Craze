using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerScript : MonoBehaviour
{
    public CurrentState currentState = CurrentState.None;

    public enum CurrentState
    {
        None,
        isStanding,
        isThinking,
        isWaitingToGiveOrders,
        isWaitingToReceiveTheOrder,
        isWaitingToPay,
        isLeaving,
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.name + " " + currentState);
    }



    public IEnumerator Think()
    {
        currentState = CurrentState.isThinking;

        float seconds = Random.Range(3,6);
        yield return new WaitForSeconds(seconds);

        currentState = CurrentState.isWaitingToGiveOrders;
    }
}
