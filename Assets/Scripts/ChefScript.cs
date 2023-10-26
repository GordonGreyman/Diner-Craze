using System.Collections;
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
        yield return new WaitForSeconds(2);
        currentState = CurrentState.PreparedTheFood;
    }
}
