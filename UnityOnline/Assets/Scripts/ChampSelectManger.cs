using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using ExitGames.Client.Photon;


public class ChampSelectManger : MonoBehaviourPunCallbacks
{
    [SerializeField] public Button[] champsButtons;
    [SerializeField] Canvas champSelectCanvas;
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] MultiplayerGameManager currentMultiplayerManager;
    [SerializeField] TMP_Text winnerText;
    public PickupCollectedEventScript _pickupCollectedEventScript;
    public int livingPlayersCounter;
    public static bool isPaused = false;
    public Transform currentPickupTransform;
    

    private List<PhotonView> alivePlayersList = new List<PhotonView>();

    public static ChampSelectManger Instance { get; private set; }

    private void Awake()
    {
        

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    

    [PunRPC]
    public void AddLivingPkayer()
    {
        
        livingPlayersCounter++;
    }

    [PunRPC]
    public void FinishGame()
    {
        StartCoroutine(WaitAndFinishGame());
    }

    [PunRPC]
    System.Collections.IEnumerator WaitAndFinishGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        yield return new WaitForSeconds(1);
        Debug.LogWarning("Game Is Supposed to finish");
        gameOverCanvas.gameObject.SetActive(true);

        GameObject winnerPV = GameObject.FindGameObjectWithTag("Player");
        alivePlayersList.Add(winnerPV.GetComponent<PhotonView>());
        var winnerPlayerController = winnerPV.GetComponent<PlayerController>();
        winnerPlayerController.SelfHPBar.hpCanvas.gameObject.SetActive(false);
       
        winnerText.text = $"The winner is: {alivePlayersList[0].Owner.NickName}! \n Scores: \n";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            winnerText.text += player.NickName + ": " + (string)player.CustomProperties["Kills"] + "\n";
        }
        isPaused = true;
    }


    [PunRPC]
    public void RemoveLivingPkayer()
    {
        livingPlayersCounter--;
        Debug.LogWarning("A player died");
        
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {

        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        Debug.LogWarning("A Player killed a man");
        
      
        if (livingPlayersCounter == 1)
        {
            Debug.LogWarning("OI! finish game!");
            photonView.RPC(nameof(FinishGame), RpcTarget.All);
        }
          
    }
   

    public void ExitGame()
    {
        Application.Quit();
    }

   
    public void ReturnToMainMenu()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            
            PhotonNetwork.LoadLevel("MainMenu");
        }
    }
    [PunRPC]
    public void DefenetlyLeaveRoom()
    {

        PhotonNetwork.LeaveRoom();
        Debug.LogWarning("Player LeftRoom");
    }
    public void LeaveCurrentRoomAfterGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;

            photonView.RPC(nameof(DefenetlyLeaveRoom), RpcTarget.Others);
            StartCoroutine(WaitAndLeaveRooomMaster());
        }
    }

    System.Collections.IEnumerator WaitAndLeaveRooomMaster()
    {
       
        yield return new WaitForSeconds(2f);
        PhotonNetwork.LeaveRoom();
        ReturnToMainMenu();
    }


    [PunRPC]
    public void DefenetlyLeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
    }
    public void LeaveCurrentLobbyAfterGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(DefenetlyLeaveLobby), RpcTarget.Others);
            PhotonNetwork.LeaveLobby();
            ReturnToMainMenu();
        }
    }
    [PunRPC]    
    public void ChampSelectedForEveryone(int index)
    {
        champsButtons[index].interactable = false;
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(AddLivingPkayer), RpcTarget.All);
        }
        Debug.Log("Added Player: " + livingPlayersCounter);


    }

    [PunRPC]
    public void ChampSelectedForMe(int index)
    {
            photonView.RPC(nameof(ChampSelectedForEveryone), RpcTarget.All, index);
           
        
        champSelectCanvas.gameObject.SetActive(false);


        currentMultiplayerManager.SpawnPlayer(currentMultiplayerManager.GetRandomSpawnPoint(), index);
        Debug.Log("MovedScene");
    }
   

   
}
