using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MoneyHandler : MonoBehaviour
{
    public JSONHandler json;
    public TextMeshProUGUI text;
    public GameObject astar;

    private void Start()
    {
        StartCoroutine(Delay(.1f));

        CustomerScript.OnMoneyChange += ShowMoney;
        StartCoroutine(DelayFor(1.5f));
    }



    private void ShowMoney()
    {
        StartCoroutine(Delay(.5f));
    }


    private IEnumerator Delay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        text.text = "Money: " + json.GetMoney().ToString();
    }

    private IEnumerator DelayFor(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        astar.SetActive(true);
    }
}
