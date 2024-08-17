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
    private int[] spooped = {1,2 };
    public Player nextMasterClient;

    private void Awake()
    {
        ChangeNextPlayer();
    }
    public override void OnLeftRoom()
    {
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CustomChangeMasterClient();
        }
    }

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
        Debug.Log("Master Client Changed");
        ChangeNextPlayer();
    }

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
        
    }

}
