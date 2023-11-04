using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelScript : MonoBehaviour
{
    public CustomerSpawner customerSpawner;
    public float levelSatisfaction;
    private float maxSatisfactionPossible;

    public Image satisfactionBar;
    public Image thresholdMarker; 
    public float thresholdPercentage = 0.25f; 

    void Start()
    {
        CustomerScript.OnMoneyChange += UpdateSatisfaction;
        satisfactionBar.fillAmount = 0;

        float barHeight = satisfactionBar.rectTransform.rect.height;
        float markerPosition = thresholdPercentage * barHeight;
        thresholdMarker.rectTransform.anchoredPosition = new Vector2(-thresholdMarker.rectTransform.rect.width *0.75f, markerPosition - (barHeight / 2));
    }

    private void OnDestroy()
    {
        CustomerScript.OnMoneyChange -= UpdateSatisfaction;
    }
    private void UpdateSatisfaction()
    {
        maxSatisfactionPossible = customerSpawner.handledCustomerCount * 5;
        float targetFillAmount = levelSatisfaction / maxSatisfactionPossible;
        float lerpDuration = 0.5f; 

        if (levelSatisfaction / maxSatisfactionPossible < thresholdPercentage)
        {
            satisfactionBar.color = Color.red;
        }
        else
        {
            satisfactionBar.color = Color.green;
        }

        if (customerSpawner.dayEnded)
        {
            CustomerScript[] remainingCustomers = FindObjectsOfType<CustomerScript>();

            if (remainingCustomers.Length == 1)
            {
                Debug.Log("Level Satisfaction: " + (levelSatisfaction / maxSatisfactionPossible).ToString("F1"));
            }
        }

        if (Mathf.Approximately(satisfactionBar.fillAmount, targetFillAmount) == false)
        {
            StartCoroutine(LerpFillAmount(targetFillAmount, lerpDuration));
        }
    }

    private IEnumerator LerpFillAmount(float targetFillAmount, float duration)
    {
        float startTime = Time.time;
        float initialFillAmount = satisfactionBar.fillAmount;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            satisfactionBar.fillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, t);
            yield return null;
        }

        satisfactionBar.fillAmount = targetFillAmount;
    }
}