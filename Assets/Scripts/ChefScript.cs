using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefScript : MonoBehaviour
{
    public CurrentState currentState = CurrentState.Free;
    public int tableID;
    public int orderID;
    public GameObject[] foodTypes;
    public GameObject originPointForPlates;

    public List<GameObject> platePoints = new List<GameObject>();


    public enum CurrentState
    {
        Free,
        CookingFood,
    }

    public IEnumerator PrepareFood()
    {
        currentState = CurrentState.CookingFood;
        yield return new WaitForSeconds(2);
        currentState = CurrentState.Free;

        for(int i = 0; i < platePoints.Count; i++)
        {
            if (platePoints[i] == null)
            {
                GameObject food = Instantiate(foodTypes[0], new Vector3(originPointForPlates.transform.position.x + i, originPointForPlates.transform.position.y, originPointForPlates.transform.position.z), Quaternion.identity);
                food.GetComponent<FoodScript>().chefThatPrepared = transform.gameObject;
                platePoints[i] = food; 
                break;
            }
        }
    }
}
