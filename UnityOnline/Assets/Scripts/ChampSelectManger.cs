using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
//using UnityEngine.UIElements;

public class ChampSelectManger : MonoBehaviourPun
{
    [SerializeField] Button[] champsButtons;
    [SerializeField] Canvas champSelectCanvas;
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] MultiplayerGameManager currentMultiplayerManager;
    [SerializeField] PhotonView ChampSelectManagerPhotonView;
    [SerializeField] TMP_Text winnerText;
    private int livingPlayersCounter;
    public static bool isPaused = false;
    private List<PhotonView> alivePlayersList = new List<PhotonView>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [PunRPC]
    public void AddLivingPkayer()
    {
        
        livingPlayersCounter++;
    }
    [PunRPC]
    public void RemoveLivingPkayer()
    {
        livingPlayersCounter--;

        if(livingPlayersCounter <= 1)
        {
            gameOverCanvas.gameObject.SetActive(true);
            //alivePlayersList.Remove(playerPhotonView);
            //alivePlayersList.Sort();
            GameObject winnerPV = GameObject.FindGameObjectWithTag("Player");
            alivePlayersList.Add(winnerPV.GetComponent<PhotonView>()) ;
            winnerText.text = $"The winner is: {alivePlayersList[0].Owner.NickName}!";
            isPaused = true;
        }
    }
    void Start()
    {
        
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
    public void ChampSelectedForEveryone(int index)
    {
        champsButtons[index].interactable = false;
        photonView.RPC(nameof(AddLivingPkayer), RpcTarget.All);
        Debug.Log("Added Player");


    }

    [PunRPC]
    public void ChampSelectedForMe(int index)
    {
        ChampSelectManagerPhotonView.RPC(nameof(ChampSelectedForEveryone), RpcTarget.All, index);
        champSelectCanvas.gameObject.SetActive(false);


        currentMultiplayerManager.SpawnPlayer(currentMultiplayerManager.GetRandomSpawnPoint(), index);
        Debug.Log("MovedScene");
    }
    // Update is called once per frame
    
}
