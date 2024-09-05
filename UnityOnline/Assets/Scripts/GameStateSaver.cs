using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Photon.Pun;
using Newtonsoft.Json;


public class GameStateSaver : MonoBehaviour
{
    private const string SAVE_FILE_NAME = "/MCSave.dat";
    public List<int> takenChampionIndexesList = new List<int>();
    public List<PlayerSaveCapsule> playersStatsInfo = new List<PlayerSaveCapsule>();
    public List<PlayerController> playersList = new List<PlayerController>();
    public string currentJsonString ;


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
        //takenChampionIndexesList = new List<int>();
        //playersStatsInfo = new List<PlayerSaveCapsule>();
        //playersList = new List<PlayerController>();
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

   [PunRPC]
    public void LoadGameState(string masterJson, PhotonMessageInfo info)
    {
        LoadFromJson(masterJson, info);
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
        currentJsonString = jsonString;

    }

    [PunRPC]
    public void LoadFromJson(string MasterJson, PhotonMessageInfo info)
    {
        string jsonString = MasterJson;
        playersStatsInfo = JsonUtility.FromJson<List<PlayerSaveCapsule>>(jsonString);
        takenChampionIndexesList = JsonUtility.FromJson<List<int>>(jsonString);
        //takenChampionIndexesList.Add(1);
        playersList = JsonUtility.FromJson<List<PlayerController>>(jsonString);
       // Debug.Log("StateLoaded! your first taken index is+: " + takenChampionIndexesList[0]);



    }
    public void LoadForMaster()
    {
         string jsonString = File.ReadAllText(Application.persistentDataPath + SAVE_FILE_NAME);
    }
    
    [PunRPC]
    public void giveInfoToPeasents(PhotonMessageInfo info)
    {
        gameObject.GetPhotonView().RPC(nameof(LoadGameState), info.Sender, currentJsonString);
    }


}
