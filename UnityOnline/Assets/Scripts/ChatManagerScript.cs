using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ChatManagerScript : MonoBehaviourPun
{
    [SerializeField] private TMP_InputField _playerInputField;
    [SerializeField] private TMP_InputField _textShower;
    [SerializeField] private PhotonView _playerView;
    [SerializeField] private Canvas _chatCanvas;
    public static bool IsChatting;
    private int _chatColorIndex;
    public void ButtonChatUpdate()
    {
        _playerView.RPC(nameof(UpdateChat),RpcTarget.All, _playerInputField.text, _chatColorIndex);
        _playerInputField.text = "";
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
        _textShower.text +=($"<color={wantedColor}>{info.Sender.NickName}: {msg}</color> \n");
        
    }

    public void UpdateColor(int index)
    {
        if(index == 0)
        {
            _playerInputField.textComponent.color = Color.black ;
            _chatColorIndex = 0;
        }
        if (index == 1)
        {
            _playerInputField.textComponent.color = Color.yellow;
            _chatColorIndex = 1;
        }
        if (index == 2)
        {
            _playerInputField.textComponent.color = Color.red;
            _chatColorIndex = 2;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (_chatCanvas.gameObject.activeSelf)
            {
                _chatCanvas.gameObject.SetActive(false);
                IsChatting = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                _chatCanvas.gameObject.SetActive(true);
                IsChatting = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}
