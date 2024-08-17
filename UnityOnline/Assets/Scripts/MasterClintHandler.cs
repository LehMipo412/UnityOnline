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
    private bool didLeftRoom;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("youAreMaster");
            isReallyMasterClient = true;
            ChangeNextPlayer();
        }
       
       
    }

    public void LeaveTheRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
       // Debug.LogWarning("MasterLeft");
        
        //if (isReallyMasterClient)
        //{
        //    Debug.LogWarning("MasterLeft");
        //    CustomChangeMasterClient();
        //}
        //else
        //{
        //    Debug.LogWarning("This Player Is Not Master");
        //}

        base.OnLeftRoom();
    }
    public void Update()
    {
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
       
        if(otherPlayer.IsMasterClient)
        {
            Debug.LogWarning("Was Master Client");
        }
    }

    public void CustomChangeMasterClient()
    {
        //foreach (var player in PhotonNetwork.PlayerList)
        //{
        //    if (PhotonNetwork.PlayerList.ToListPooled().IndexOf(player) != PhotonNetwork.PlayerList.ToListPooled().IndexOf(PhotonNetwork.MasterClient))
        //    {
        //        PhotonNetwork.SetMasterClient(player);
        //        break;
        //    }
        //}
        photonView.RPC(nameof(TellEveryOneThatMasteClientChanged), RpcTarget.All);
       
        


    }

    [PunRPC]
    public void TellEveryOneThatMasteClientChanged()
    {
        MasterChangerText.text = $"The New Master Client IS: {PhotonNetwork.MasterClient.NickName}";
        Debug.LogWarning("Master Client Changed");
       // ChangeNextPlayer();
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
           // nextMasterClient = PhotonNetwork.PlayerList[playerIndex + 1];
        }
    }

    public void GiveTheMasterCrown()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (PhotonNetwork.PlayerList.ToListPooled().IndexOf(player) != PhotonNetwork.PlayerList.ToListPooled().IndexOf(PhotonNetwork.MasterClient))
            {
                PhotonNetwork.SetMasterClient(player);
                break;
            }
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
        //newMasterClient = nextMasterClient;
        Debug.Log("new Master here");
        base.OnMasterClientSwitched(newMasterClient);
        if (isReallyMasterClient)
        {
            Debug.LogWarning("MasterLeft");
            CustomChangeMasterClient();
        }
        else
        {
            Debug.LogWarning("This Player Is Not Master");
        }
        photonView.RPC(nameof(ShowEveryoneMasterChanged), RpcTarget.All);
        if(newMasterClient.UserId == PhotonNetwork.LocalPlayer.UserId)
        {
            isReallyMasterClient = true;
        }
        
        
        
    }

     
}
