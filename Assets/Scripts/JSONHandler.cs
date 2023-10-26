using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class JSONHandler : MonoBehaviour
{
    public GameData gameData;
    public TextMeshProUGUI text;
    public GameObject[] tables = new GameObject[10];

    private string savePath;

    private void Start()
    {
        StartCoroutine(delay());
        savePath = Application.persistentDataPath + "/savefile.json";
        LoadGameData();
    }

    public void SaveGameData()
    {
        try
        {
            string jsonString = JsonUtility.ToJson(gameData);
            File.WriteAllText(savePath, jsonString);
            Debug.Log("Game data saved.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save game data: " + e.Message);
        }
    }

    public void LoadGameData()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string readJson = File.ReadAllText(savePath);
                gameData = JsonUtility.FromJson<GameData>(readJson);
                Debug.Log("Game data loaded.");
            }
            else
            {
                gameData = new GameData(1, 0,3);
                SaveGameData();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load game data: " + e.Message);
        }
    }

    public void ResetGameData()
    {
        gameData = new GameData(1, 0, 3);
        SaveGameData();
    }



    public void CheckTableCount()
    {
        string tagToFind = "Table";

        GameObject[] tables = GameObject.FindGameObjectsWithTag(tagToFind);

        gameData.tableCount = tables.Length;
    }


    public IEnumerator delay()
    {
        yield return new WaitForSecondsRealtime(.5f);
        EnableTables();
    }
    public void EnableTables()
    {
        Debug.Log("outside");

        for (int i = 0; i < gameData.tableCount; i++)
        {
            Debug.Log("inside: " + i);
            tables[i].SetActive(true);
            Debug.Log("after activation: " + i);

        }
    }
    public int GetMoney()
    {
        return gameData.money;
    }

    public void SetMoney(int moneyToSet)
    {
        gameData.money = moneyToSet;
    }
    public void AddMoney(int moneyToAdd)
    {
        gameData.money += moneyToAdd;
    }

    public void RemoveMoney(int moneyToRemove)
    {
        gameData.money -= moneyToRemove;
    }

    public void NextLevel()
    {
        gameData.level++;
    }

 
}