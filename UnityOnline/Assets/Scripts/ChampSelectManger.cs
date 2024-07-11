using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class ChampSelectManger : MonoBehaviourPun
{
    [SerializeField] Button[] champsButtons;
    [SerializeField] Canvas champSelectCanvas;
    [SerializeField] MultiplayerGameManager currentMultiplayerManager;
    [SerializeField] PhotonView ChampSelectManagerPhotonView;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    [PunRPC]    
    public void ChampSelectedForEveryone(int index)
    {
        champsButtons[index].interactable = false;


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
