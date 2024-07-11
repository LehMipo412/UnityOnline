using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class ChampSelectManger : MonoBehaviourPun
{
    [SerializeField] Button[] champsButtons;
    [SerializeField] Canvas champSelectCanvas;
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] MultiplayerGameManager currentMultiplayerManager;
    [SerializeField] PhotonView ChampSelectManagerPhotonView;
    private int livingPlayersCounter;
    public static bool isPaused = false;
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
            isPaused = true;
        }
    }
    void Start()
    {
        
    }
    [PunRPC]    
    public void ChampSelectedForEveryone(int index)
    {
        champsButtons[index].interactable = false;
        AddLivingPkayer();


    }

    [PunRPC]
    public void ChampSelectedForMe(int index)
    {
        ChampSelectManagerPhotonView.RPC(nameof(ChampSelectedForEveryone), RpcTarget.All, index);
        champSelectCanvas.gameObject.SetActive(false);


        currentMultiplayerManager.SpawnPlayer(currentMultiplayerManager.GetRandomSpawnPoint());
        Debug.Log("MovedScene");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
