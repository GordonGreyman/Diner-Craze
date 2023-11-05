using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefScript : MonoBehaviour
{
    public CurrentState currentState = CurrentState.Free;
    public GameObject originPointForPlates;
    public List<GameObject> platePoints = new List<GameObject>();
    public int orderID;

    private MenuScript menuScript;

    private void Start()
    {
        menuScript = FindObjectOfType<MenuScript>();
    }

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
                GameObject food = Instantiate(menuScript.menu[orderID], new Vector3(originPointForPlates.transform.position.x + i * 1.2f, originPointForPlates.transform.position.y, originPointForPlates.transform.position.z), Quaternion.identity);
                food.GetComponent<FoodScript>().chefThatPrepared = transform.gameObject;
                platePoints[i] = food; 
                break;
            }
        }
    }
}
