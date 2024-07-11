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
    private int chatColorIndex;
    


   
    public void ButtonChatUpdate()
    {
        playerView.RPC(nameof(UpdateChat),RpcTarget.All, playerInputField.text, chatColorIndex);
    }
  
    [PunRPC]
    public void UpdateChat(string msg, int colorIndex, PhotonMessageInfo info)
    {

        
        
        if (colorIndex == 0)
        {
            textShower.textComponent.color = Color.black;
           
        }
        if (colorIndex == 1)
        {
            textShower.textComponent.color = Color.yellow;
            
        }
        if (colorIndex == 2)
        {
            textShower.textComponent.color = Color.red;
            
        }
        textShower.text +=(info.Sender.NickName+": "+ msg +"\n");
        playerInputField.text = "";
    }

    public void UpdateColor(int index)
    {
        if(index == 0)
        {
            playerInputField.textComponent.color = Color.black ;
            chatColorIndex = 0;
        }
        if (index == 1)
        {
            playerInputField.textComponent.color = Color.yellow;
            chatColorIndex = 1;
        }
        if (index == 2)
        {
            playerInputField.textComponent.color = Color.red;
            chatColorIndex = 2;
        }
    }
}
