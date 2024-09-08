using UnityEngine;
using Photon.Pun;
using TMPro;


public class ChatManagerScript : MonoBehaviourPun
{
    [SerializeField] private TMP_InputField _playerInputField;
    [SerializeField] private TMP_InputField _textShower;
    [SerializeField] private PhotonView _playerView;
    [SerializeField] private Canvas _chatCanvas;

    private int _chatColorIndex;

    public static bool IsChatting;
    

    public void ButtonChatUpdate()
    {
        _playerView.RPC(nameof(UpdateChat),RpcTarget.All, _playerInputField.text, _chatColorIndex);
        _playerInputField.text = "";
    }
  
    [PunRPC]
    public void UpdateChat(string msg, int colorIndex, PhotonMessageInfo info)
    {

        string wantedColor = "";

        switch (colorIndex)
        {
            case 0:
                wantedColor = "black";
                break;
            case 1:
                wantedColor = "yellow";
                break;
            case 2:
                wantedColor = "red";
                break;
            default:
                break;
        }

        _textShower.text +=($"<color={wantedColor}>{info.Sender.NickName}: {msg}</color> \n");  
    }

    public void UpdateColor(int index)
    {
        switch (index)
        {
            case 0:
                _playerInputField.textComponent.color = Color.black;
                _chatColorIndex = 0;
                break;
            case 1:
                _playerInputField.textComponent.color = Color.yellow;
                _chatColorIndex = 1;
                break;
            case 2:
                _playerInputField.textComponent.color = Color.red;
                _chatColorIndex = 2;
                break;
            default:
                break;
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
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                _chatCanvas.gameObject.SetActive(true);
                IsChatting = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
