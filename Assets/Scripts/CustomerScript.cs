using System.Collections;
using UnityEngine;
using TMPro;

public class CustomerScript : MonoBehaviour
{
    public CurrentState currentState = CurrentState.isStanding;

    public SpriteRenderer rd;
    public Canvas canvas;
    public TextMeshProUGUI text;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;


    public int customerType;
    public int order;
    public int paymentCost;

    public float currentHappiness;
    public int happiness = 5;           //Quit(0)-Mad(20)-Angry(40)-Neutral(60)-Pleased(80)-Excellent(100)
    public float maxHappiness;

    public float tipPossibility;
    public int thinkTime;

    public JSONHandler json;
    public TableScript tableCustomerSits;
    public CustomerSpawner customerSpawner;
    public LevelScript level;
    public MenuScript menuScript;

    public delegate void MoneyChangedHandler();
    public static event MoneyChangedHandler OnMoneyChange;
    private void Start()
    {
        customerSpawner = FindObjectOfType<CustomerSpawner>();
        menuScript = FindObjectOfType<MenuScript>();
        level = FindObjectOfType<LevelScript>();
        json = FindObjectOfType<JSONHandler>();
        rd = GetComponent<SpriteRenderer>();
        canvas = transform.GetChild(0).GetComponent<Canvas>();

        AssignCustomerData();
        StartCoroutine(DecreaseMoodOverTime());
    }
    public enum CurrentState
    {
        isStanding,
        isWalkingtoTable,
        isThinking,
        isWaitingToGiveOrders,
        isWaitingToReceiveTheOrder,
        isEating,
        isWaitingToPay,
        isPaying,
    }

    public IEnumerator SitAndThink()
    {
        transform.tag = "Player";
        tableCustomerSits = transform.parent.GetComponent<TableScript>();
        currentState = CurrentState.isWalkingtoTable;
        tableCustomerSits.isOccupied = true;
        GameObject a = GameObject.FindGameObjectWithTag("Player");

        for (int i = 0; i < customerSpawner.activeCustomers.Count; i++)
        {
            if (customerSpawner.activeCustomers[i] == a)
            {
                customerSpawner.activeCustomers[i] = null;
                break;
            }
        }
        transform.tag = "Untagged";
        StartCoroutine(customerSpawner.SwipeDown());

        float moveSpeed = 5.0f;
        Vector3 targetPosition = new Vector3(tableCustomerSits.transform.position.x, tableCustomerSits.transform.position.y + 1.5f, tableCustomerSits.transform.position.z);

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float step = moveSpeed * Time.deltaTime;

            transform.position = transform.position + moveDirection * step;
            yield return null;
        }

        transform.position = targetPosition;

        currentState = CurrentState.isThinking;
        rd.color = Color.green;

        float seconds = thinkTime;
        yield return new WaitForSeconds(seconds);

        order = Random.Range(0, menuScript.menu.Count);
        tableCustomerSits.expectedOrder = order;
        currentState = CurrentState.isWaitingToGiveOrders;

        rd.color = Color.yellow;
        text.text = order.ToString();

    }


    public IEnumerator Eat()
    {
        currentState = CurrentState.isEating;
        rd.color = Color.red;

        float seconds = Random.Range(3, 6);
        yield return new WaitForSeconds(seconds);

        currentState = CurrentState.isWaitingToPay;

        rd.color = Color.blue;

        tableCustomerSits.rd.color = Color.black;
        tableCustomerSits.isDirty = true;
    }


    public IEnumerator PayAndLeave()
    {
        currentState = CurrentState.isPaying;
    
        int tip = 0;
        switch (happiness)
        {


            case 1:
                if (Random.Range(0f, 1f) < tipPossibility - 0.25f)
                {
                    tip = Mathf.Max(0, (paymentCost / 10) - 4);
                    json.AddMoney(paymentCost + tip);
                }
                else
                {
                    json.AddMoney(paymentCost);
                }

                level.levelSatisfaction++;

                break;


            case 2:
                if (Random.Range(0f, 1f) < tipPossibility - 0.20f)
                {
                    tip = Mathf.Max(0, (paymentCost / 10) - 3);
                    json.AddMoney(paymentCost + tip);
                }
                else
                {
                    json.AddMoney(paymentCost);
                }

                level.levelSatisfaction += 2;
                break;


            case 3:
                                                                                             //TODO distinguish payment, tip, and customer type  
                if (Random.Range(0f, 1f) < tipPossibility - 0.15f)
                {
                    tip = Mathf.Max(0, (paymentCost / 10) - 2);
                    json.AddMoney(paymentCost + tip);
                }
                else
                {
                    json.AddMoney(paymentCost);
                }
                level.levelSatisfaction += 3;
                break;


            case 4:

                if (Random.Range(0f, 1f) < tipPossibility - 0.5f)
                {
                    tip = paymentCost / 10;
                    json.AddMoney(paymentCost + tip);
                }
                else
                {
                    json.AddMoney(paymentCost);
                }
                level.levelSatisfaction += 4;
                break;


            case 5:
                if (Random.Range(0f, 1f) < tipPossibility)
                {
                    tip = paymentCost / 10 + 2;
                    json.AddMoney(paymentCost + tip);
                }
                else
                {
                    json.AddMoney(paymentCost);
                }
                level.levelSatisfaction += 5;
                break;
            default:
                break;
        }
        customerSpawner.handledCustomerCount++;
        OnMoneyChange?.Invoke();

        var moneyText = Instantiate(text2, transform.position, Quaternion.identity, canvas.transform);
       

        if (tip > 0 )
        {
            moneyText.text = paymentCost.ToString();
            moneyText.transform.GetComponent<Animation>().Play();

            yield return new WaitForSeconds(1f);

            var tipText = Instantiate(text3, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), Quaternion.identity, canvas.transform);
            tipText.text = tip.ToString();
            tipText.transform.GetComponent<Animation>().Play();

        }
        else
        {
            moneyText.text = paymentCost.ToString();
            moneyText.transform.GetComponent<Animation>().Play();
        }
        


        yield return new WaitForSeconds(1.5f);

        tableCustomerSits.isOccupied = false;
        Destroy(transform.gameObject);


    }




    private IEnumerator DecreaseMoodOverTime()
    {

        currentHappiness = maxHappiness;
        while (currentHappiness > 0)
        {

            if (currentState == CurrentState.isStanding ||
                currentState == CurrentState.isWaitingToGiveOrders ||
                currentState == CurrentState.isWaitingToReceiveTheOrder ||
                currentState == CurrentState.isWaitingToPay)
            {
                yield return new WaitForSeconds(2);
                currentHappiness -= 2;

                if (currentHappiness / maxHappiness >= .8f)
                {
                    happiness = 5;
                }
                else if (currentHappiness / maxHappiness >= .6f)
                {
                    happiness = 4;
                }
                else if (currentHappiness / maxHappiness >= .4f)
                {
                    happiness = 3;
                }
                else if (currentHappiness / maxHappiness >= .2f)
                {
                    happiness = 2;
                }
                else if (currentHappiness / maxHappiness > .0f)
                {
                    happiness = 1;
                }
                else
                {
                    happiness = 0;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private void AssignCustomerData()
    {
        switch (customerType)
        {
            case 1:
                maxHappiness = 100;
                tipPossibility = 0.5f;
                thinkTime = 2;
                break;
            case 2:
                maxHappiness = 120;
                tipPossibility = 0.7f;
                thinkTime = 4;
                break;
            default:
                maxHappiness = 100;
                tipPossibility = 0.5f;
                thinkTime = 2;
                break;
        }
    }
}
