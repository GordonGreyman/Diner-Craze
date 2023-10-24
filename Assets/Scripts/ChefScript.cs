using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefScript : MonoBehaviour
{
    public CurrentState currentState = CurrentState.Free;


    public enum CurrentState
    {
        Free,
        CookingFood,
        PreparedTheFood,
    }

    public IEnumerator PrepareFood()
    {
        Debug.Log("someone cooked here");
        yield return new WaitForSeconds(1);
        currentState = CurrentState.PreparedTheFood;
    }
}
