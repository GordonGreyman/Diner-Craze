using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public JSONHandler json;
    public GameObject[] customerType;
    public List<GameObject> activeCustomers = new List<GameObject>();
    public GameObject spawnPoint;
    public bool isSwiping = false;

    private float yOffset = 1.0f; // Adjust this value for the desired Y offset

    void Start()
    {
        StartCoroutine(SpawnCustomers());
    }

    private IEnumerator SpawnCustomers()
    {
        yield return new WaitForSecondsRealtime(.1f);

        while (true)
        {
            bool hasEmptyPlace = false;

            for (int i = 0; i < activeCustomers.Count; i++)
            {
                if (activeCustomers[i] == null)
                {
                    hasEmptyPlace = true;
                    break;
                }
            }

            if (hasEmptyPlace && !isSwiping)
            {
                float wait = 0.5f / json.GetPrestige();
                yield return new WaitForSecondsRealtime(wait);

                for (int i = 0; i < activeCustomers.Count; i++)
                {
                    if (activeCustomers[i] == null)
                    {
                        Vector3 spawnPosition = spawnPoint.transform.position + new Vector3(0, yOffset * i, 0);
                        GameObject instantiatedCustomer = Instantiate(customerType[0], spawnPosition, Quaternion.identity);
                        activeCustomers[i] = instantiatedCustomer;
                        float r = Random.Range(0f, 1f);
                        float g = Random.Range(0f, 1f);
                        float b = Random.Range(0f, 1f);

                        // Create a random color
                        Color randomColor = new Color(r, g, b);
                        activeCustomers[i].GetComponent<SpriteRenderer>().color = randomColor;
                        break;
                    }
                }
            }
            else
            {
                yield return new WaitForSeconds(1.0f);
            }
        }
    }

    public IEnumerator SwipeDown()
    {
        isSwiping = true;
        for (int i = 0; i < activeCustomers.Count - 1; i++)
        {
            if (activeCustomers[i] == null && activeCustomers[i + 1] != null)
            {
                activeCustomers[i] = activeCustomers[i + 1];
                activeCustomers[i + 1] = null;
                Vector3 targetPosition = spawnPoint.transform.position + new Vector3(0, yOffset * i, 0);
                StartCoroutine(MoveCustomer(activeCustomers[i], targetPosition));
                yield return new WaitForSeconds(.5f);

            }
        }
        isSwiping = false;
    }

    private IEnumerator MoveCustomer(GameObject customer, Vector3 targetPosition)
    {
        float startTime = Time.time;
        Vector3 initialPosition = customer.transform.position;

        while (Time.time - startTime < .5f)
        {
            float normalizedTime = (Time.time - startTime) / .5f;
            customer.transform.position = Vector3.Lerp(initialPosition, targetPosition, normalizedTime);
            yield return null;
        }

        // Ensure the final position is exactly the target position
        customer.transform.position = targetPosition;

    }
}