using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ChatManagerScript : MonoBehaviourPun
{
    public PhotonView playerView;
    public TMP_InputField playerInputField;
    public TMP_InputField textShower;

    public void ButtonChatUpdate()
    {
        playerView.RPC(nameof(UpdateChat),RpcTarget.All, playerInputField.text);
    }
    [PunRPC]
    public void ConnectAndSendChatMessage(PhotonMessageInfo messageInfo)
    {
        Debug.Log(messageInfo.Sender + "Sent a Message");

        

        messageInfo.photonView.RPC(nameof(UpdateChat), messageInfo.Sender, messageInfo.Sender.ToString(), playerInputField.text);
    }

    [PunRPC]
    public void UpdateChat(string msg, PhotonMessageInfo info)
    {
        textShower.text +=(info.Sender.NickName+": "+ msg +"\n");
        playerInputField.text = "";
    }
}
