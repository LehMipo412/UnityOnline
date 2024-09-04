using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

public class GameStateSaver : MonoBehaviour
{
    private const string SAVE_FILE_NAME = "/MCSave.dat";
    public List<int> takenChampionIndexesList;
    public List<PlayerSaveCapsule> playersStatsInfo;
    public List<PlayerController> playersList;


   public static GameStateSaver Instance { get; private set; }


    private void Awake()
    {


        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            Debug.Log("GameStateServer is up!");
        }
    }

   public void SavePlayerHP(int playerIndex)
    {
        playersStatsInfo[playerIndex].HP = playersList[playerIndex].HP;
        SaveToJson(playerIndex);
    }

    public void SavePlayerScore(int playerIndex)
    {
        playersStatsInfo[playerIndex].Score = playersList[playerIndex].score;
        SaveToJson(playerIndex);
    }

    public void SavePlayerKnockBackPrecentage(int playerIndex)
    {
        playersStatsInfo[playerIndex].KnockBackPrecentage = playersList[playerIndex].knockbackPrecentage;
        SaveToJson(playerIndex);
    }

    public void LoadGameState()
    {
        LoadFromJson();
        for (int i = 0; i < playersList.Count; i++)
        {
            playersList[i].HP = playersStatsInfo[i].HP;
            playersList[i].knockbackPrecentage = playersStatsInfo[i].KnockBackPrecentage;
            playersList[i].score = playersStatsInfo[i].Score;
        }

        for (int i = 0; i < takenChampionIndexesList.Count; i++)
        {
            ChampSelectManger.Instance.champsButtons[takenChampionIndexesList[i]].interactable = false;
        }
        

        

    }

    public void SaveTakenIndexToJson()
    {
        string jsonString = JsonUtility.ToJson(takenChampionIndexesList, true);

        File.WriteAllText(Application.persistentDataPath + SAVE_FILE_NAME, jsonString);
    }
    public void SaveToJson(int index)
    {
        string jsonString = JsonUtility.ToJson(playersStatsInfo[index], true);

        File.WriteAllText(Application.persistentDataPath + SAVE_FILE_NAME, jsonString);


        jsonString = JsonUtility.ToJson(playersList[index], true);
        File.WriteAllText(Application.persistentDataPath + SAVE_FILE_NAME, jsonString);

    }

    private void LoadFromJson()
    {
        string jsonString = File.ReadAllText(Application.persistentDataPath + SAVE_FILE_NAME);
        playersStatsInfo = JsonUtility.FromJson<List<PlayerSaveCapsule>>(jsonString);
        takenChampionIndexesList = JsonUtility.FromJson<List<int>>(jsonString);
        playersList = JsonUtility.FromJson<List<PlayerController>>(jsonString);



    }


}
