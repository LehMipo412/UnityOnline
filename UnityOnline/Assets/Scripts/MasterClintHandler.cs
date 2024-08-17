using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using Photon.Pun.UtilityScripts;

public class MasterClintHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] Canvas masterAnnouncer;
    [SerializeField] TMP_Text MasterChangerText;
    public Player nextMasterClient;
    private bool isReallyMasterClient;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("youAreMaster");
            isReallyMasterClient = true;
            ChangeNextPlayer();
        }
       
       
    }

    public override void OnLeftRoom()
    {
        Debug.LogWarning("MasterLeft");
       
        if (isReallyMasterClient)
        {
            Debug.LogWarning("MasterLeft");
            CustomChangeMasterClient();
        }
        else
        {
            Debug.LogWarning("This Player Is Not Master");
        }
            base.OnLeftRoom();

    }
//    public override void OnPlayerLeftRoom(Player otherPlayer)
//{
        
//    }

    public void CustomChangeMasterClient()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if(PhotonNetwork.PlayerList.ToListPooled().IndexOf(player) != PhotonNetwork.PlayerList.ToListPooled().IndexOf(PhotonNetwork.MasterClient))
            {
                PhotonNetwork.SetMasterClient(player);
                break;
            }
        }
        photonView.RPC(nameof(TellEveryOneThatMasteClientChanged), RpcTarget.All);
       
        


    }

    [PunRPC]
    public void TellEveryOneThatMasteClientChanged()
    {
        MasterChangerText.text = $"The New Master Client IS: {PhotonNetwork.MasterClient.NickName}";
        Debug.LogWarning("Master Client Changed");
        ChangeNextPlayer();
    }
    [PunRPC]
    public void ShowEveryoneMasterChanged()
    {
        StartCoroutine(AnnounceMasterChange());
    }


    public void ChangeNextPlayer()
    {
        int playerIndex = PhotonNetwork.PlayerList.ToListPooled().IndexOf((PhotonNetwork.LocalPlayer));
        if (PhotonNetwork.PlayerList[playerIndex + 1] != null)
        {
            nextMasterClient = PhotonNetwork.PlayerList[playerIndex + 1];
        }
    }

    IEnumerator AnnounceMasterChange()
    {
        masterAnnouncer.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        masterAnnouncer.gameObject.SetActive(false);

    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        newMasterClient = nextMasterClient;
        base.OnMasterClientSwitched(newMasterClient);
        photonView.RPC(nameof(ShowEveryoneMasterChanged), RpcTarget.All);
        if(newMasterClient.UserId == PhotonNetwork.LocalPlayer.UserId)
        {
            isReallyMasterClient = true;
        }
        
        
        
    }

     
}
