using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

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
                text.text = "Money: " + GetMoney().ToString();

                Debug.Log("Game data loaded.");
            }
            else
            {
                gameData = new GameData(1, 0, 3, 1);
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
        gameData = new GameData(1, 0, 3, 1);
        SaveGameData();

        SceneManager.LoadScene("SampleScene");


    }



    public void CheckTableCount()
    {
        string tagToFind = "Table";

        GameObject[] tables = GameObject.FindGameObjectsWithTag(tagToFind);

        gameData.tableCount = tables.Length;
    }


    public IEnumerator delay()
    {

        CheckTableCount();
        yield return new WaitForSecondsRealtime(1f);
        EnableTables();
    }
    public void EnableTables()
    {
        for (int i = 0; i < gameData.tableCount; i++)
        {
            tables[i].SetActive(true);
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



    public void AddTable()
    {
        gameData.tableCount += 1;
    }



    public int GetLevel()
    {
        return gameData.level;
    }
    public void NextLevel()
    {
        gameData.level++;
    }



    public float GetPrestige()
    {
        return gameData.prestige;
    }
    public void AddPrestige(float addedPrestige)
    {
        gameData.prestige += addedPrestige;
    }

 
}