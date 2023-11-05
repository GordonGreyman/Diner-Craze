using UnityEngine;

public class FoodScript : MonoBehaviour
{
    public GameObject chefThatPrepared;
    public int type;
    public int cost;

    private void Start()
    {
        type = chefThatPrepared.GetComponent<ChefScript>().orderID;
        AssignFoodData();
    }
    private void AssignFoodData()
    {
        switch (type)
        {
            case 0:
                cost = 10;
                break;

            case 1:
                cost = 50;
                break;

            default:
                cost = 0;
                break;
        }
    }

}
