using System.Collections;
using UnityEngine;
using TMPro;
public class MoneyHandler : MonoBehaviour
{
    public JSONHandler json;
    public TextMeshProUGUI text;

    private void Start()
    {
        StartCoroutine(Delay(.1f));
        CustomerScript.OnMoneyChange += ShowMoney;
    }

    private void OnDestroy()
    {
        CustomerScript.OnMoneyChange -= ShowMoney;

    }
    private void ShowMoney()
    {
        StartCoroutine(Delay(.5f));
    }

    private IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        text.text = "Money: " + json.GetMoney().ToString();
    }
}
