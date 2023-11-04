using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelScript : MonoBehaviour
{
    public CustomerSpawner customerSpawner;
    public float levelSatisfaction;
    public Image satisfactionBar;

    private float maxSatisfactionPossible;

    void Start()
    {
        CustomerScript.OnMoneyChange += UpdateSatisfaction;
    }

    private void OnDestroy()
    {
        CustomerScript.OnMoneyChange -= UpdateSatisfaction;
    }
    private void UpdateSatisfaction()
    {
        maxSatisfactionPossible = customerSpawner.totalCustomerCount * 5;
        satisfactionBar.fillAmount = levelSatisfaction / maxSatisfactionPossible;

        if (customerSpawner.dayEnded)
        {
            CustomerScript[] remainingCustomers = FindObjectsOfType<CustomerScript>();

            if (remainingCustomers.Length == 1)
            {
                Debug.Log("Level Satisfaction: " + (levelSatisfaction / maxSatisfactionPossible).ToString("F1"));
            }
        }
    }
}