using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float countdownDuration = 6.0f;  
    public TextMeshProUGUI countdownText;
    public GameObject closedSign;

    public delegate void TimesUpEventHandler();
    public static event TimesUpEventHandler TimesUp;

    private void Start()
    {
        StartTimer();
        TimesUp += SummonClosedSign;
    }

    private void OnDestroy()
    {
        TimesUp -= SummonClosedSign;
    }
    private IEnumerator Countdown()
    {
        float currentTime = countdownDuration;
        UpdateTimerDisplay(currentTime);

        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            currentTime -= 1.0f;

            UpdateTimerDisplay(currentTime);

        }

        TimesUp?.Invoke();
    }

    private void UpdateTimerDisplay(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartTimer()
    {
        StartCoroutine(Countdown());
    }

    private void SummonClosedSign()
    {
        closedSign.SetActive(true);
    }
}