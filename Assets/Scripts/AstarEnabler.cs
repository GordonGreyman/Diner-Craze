using System.Collections;
using UnityEngine;

public class AstarEnabler : MonoBehaviour
{
    public GameObject astar;

    void Start()
    {
        StartCoroutine(EnableAstar());
    }



    private IEnumerator EnableAstar()
    {
        yield return new WaitForSeconds(1.5f);
        astar.SetActive(true);
    }
}
