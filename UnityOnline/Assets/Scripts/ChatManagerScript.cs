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
    public Canvas chatCanvas;
    private int chatColorIndex;
    public static bool isChatting;
    


   
    public void ButtonChatUpdate()
    {
        playerView.RPC(nameof(UpdateChat),RpcTarget.All, playerInputField.text, chatColorIndex);
        playerInputField.text = "";
    }
  
    [PunRPC]
    public void UpdateChat(string msg, int colorIndex, PhotonMessageInfo info)
    {

        string wantedColor = "";
        
        if (colorIndex == 0)
        {
            wantedColor = "black";
           
        }
        if (colorIndex == 1)
        {
            wantedColor = "yellow";
            
        }
        if (colorIndex == 2)
        {
            wantedColor = "red";
            
        }
        textShower.text +=($"<color={wantedColor}>{info.Sender.NickName}: {msg}</color> \n");
        
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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatCanvas.gameObject.activeSelf)
            {
                chatCanvas.gameObject.SetActive(false);
                isChatting = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                chatCanvas.gameObject.SetActive(true);
                isChatting = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}
