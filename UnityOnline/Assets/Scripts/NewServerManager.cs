using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class NewServerManager : MonoBehaviourPunCallbacks
{
    [Header("Texts")]
    [SerializeField] private TMP_Text _gameStatus;
    [SerializeField] private TMP_Text _playerAmmount;
    [SerializeField] private TMP_Text _roomStatus;
    [SerializeField] private TMP_Text _errorOutput;

    [Header("DropDowns")]
    [SerializeField] private TMP_Dropdown _lobbiesDropDown;
    [SerializeField] private TMP_Dropdown _roomsDropDown;
    [SerializeField] private TMP_Dropdown _createRoomMode;
    [SerializeField] private TMP_Dropdown _joinRoomProperty;

    [Header("Inputs")]
    [SerializeField] private TMP_InputField _roomNameInput;
    [SerializeField] TMP_InputField nicknameEditorInputField;

    [Header("GameObjects")]
    [SerializeField] private GameObject _lobbySection;
    [SerializeField] private GameObject _roomsSection;
    [SerializeField] private GameObject _outsideRoom;
    [SerializeField] private GameObject _roomStatusObj;

    [Header("Buttons")]
    [SerializeField] private Button _rejoinButton;
    [SerializeField] private GameObject _leaveRoomButton;
    [SerializeField] private GameObject _startButton;

    [Header("Sliders")]
    [SerializeField] private Slider _playerAmmountSlider;

    [Header("Others")]
    List<RoomInfo> MyRoomList;
    private bool _hasLeftRoom = false;
    private bool isInRoom = false;
    private bool _shouldRefresh = false;
    private int _roomsCount = 0;
    private const string Diff = "D";

    #region Awake/Start

    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        UpdateSlider();
        MyRoomList = new List<RoomInfo>();
    }
    public void Update()
    {
        _gameStatus.text = PhotonNetwork.NetworkClientState.ToString();
        if (isInRoom)
        {
            _roomStatus.text = "Room Name : " + PhotonNetwork.CurrentRoom.Name + " - "
                  + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + " Players";
        }
        if (_roomsDropDown.options.Count == 0)
        {
            _roomsDropDown.captionText.text = "";
        }
    }

    #endregion

    #region Buttons

    public void LeaveLobbyButt()
    {
        PhotonNetwork.LeaveLobby();
      
    }
    public void Refresh()
    {
        _shouldRefresh = true;
        PhotonNetwork.LeaveLobby();
    }

    public void UpdateSlider()
    {
        _playerAmmount.text = _playerAmmountSlider.value.ToString();
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void RejoinRoomEvent()
    {
        PhotonNetwork.RejoinRoom(_roomsDropDown.options[_roomsDropDown.value].text.Split(':')[0]);
    }
    public void JoinRoom()
    {
        if (_roomsDropDown.options.Count > 0)
        {
            PhotonNetwork.JoinRoom(_roomsDropDown.options[_roomsDropDown.value].text.Substring(0, _roomsDropDown.options[_roomsDropDown.value].text.IndexOf(':')));
        }
        else _errorOutput.text = "Error Output: There are no rooms to join";


    }
    public void CreateRoom()
    {
        RoomOptions roomOptions = new()
        {
            MaxPlayers = (int)_playerAmmountSlider.value,
            EmptyRoomTtl = 30000,
            PlayerTtl = 30000,
            CustomRoomPropertiesForLobby = new string[] { Diff },
            CustomRoomProperties = new Hashtable() { { Diff, _createRoomMode.options[_createRoomMode.value].text } }
        };
           
        PhotonNetwork.CreateRoom(_roomNameInput.text, roomOptions, TypedLobby.Default);
    }
    public void JoinRandomRoom()
    {
        if (_roomsCount <= 0)
        {
            RoomOptions roomOptions = new();
            roomOptions.EmptyRoomTtl = 30000;
            roomOptions.PlayerTtl = 30000;
            roomOptions.MaxPlayers = 3;
            roomOptions.CustomRoomProperties = new Hashtable() { { Diff, _joinRoomProperty.options[_joinRoomProperty.value].text } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { Diff };
            PhotonNetwork.JoinRandomOrCreateRoom(null,0,MatchmakingMode.RandomMatching, PhotonNetwork.CurrentLobby, null, "Default", roomOptions);
            return;
        }

        PhotonNetwork.JoinRandomRoom(new Hashtable() { { Diff, _joinRoomProperty.options[_joinRoomProperty.value].text } }, 0,MatchmakingMode.RandomMatching,PhotonNetwork.CurrentLobby,null);
        
    }

   
    public void JoinLobby()
    {
        if (nicknameEditorInputField.text.Length > 0)
        {
            PhotonNetwork.JoinLobby(new TypedLobby(_lobbiesDropDown.options[(int)_lobbiesDropDown.value].text, LobbyType.Default));
        }
        else
        {
            Debug.Log("You need to have a nick name");
            _errorOutput.text = "Error Output: You need to have a nick name ";
            StartCoroutine("NotifyPlayerAboutNickname");
        }
        
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient /*&& PhotonNetwork.CurrentRoom.PlayerCount >= 3*/)
        {
            PhotonNetwork.LoadLevel("CurrentMainGameScene");
        }
        else
        {
            Debug.Log("Not Enough players: Minimum 3");
            _errorOutput.text = "Error Output: Not Enough players: Minimum 3 ";
        } 

    }
    public void EditNickname()
    {
        PhotonNetwork.NickName = nicknameEditorInputField.text;
    }

    #endregion

    #region callbacks

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        if (_shouldRefresh)
        {
            PhotonNetwork.JoinLobby(new TypedLobby(_lobbiesDropDown.options[(int)_lobbiesDropDown.value].text, LobbyType.Default));
            _shouldRefresh = false;
        }

        _roomsSection.SetActive(false);
        _lobbySection.SetActive(true);
        _roomsDropDown.options.Clear();
        MyRoomList.Clear();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log(_hasLeftRoom + " " + _shouldRefresh);

        base.OnConnectedToMaster();
        if (!_hasLeftRoom)
        {
        
            Debug.Log("Connected To Master Succesfully");
        }
        else
        {
            PhotonNetwork.JoinLobby(new TypedLobby(_lobbiesDropDown.options[(int)_lobbiesDropDown.value].text, LobbyType.Default));
            _hasLeftRoom = false;
        }
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined Lobby : " + PhotonNetwork.CurrentLobby.Name);
        _lobbySection.SetActive(false);
        _roomsSection.SetActive(true);
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Created Room : " + PhotonNetwork.CurrentRoom.Name);
        _leaveRoomButton.SetActive(true);
    }
    public override void OnJoinedRoom()
    { 
        base.OnJoinedRoom();
        _leaveRoomButton.SetActive(true);
        _startButton.SetActive(true);
        _roomStatusObj.SetActive(true);
        _outsideRoom.SetActive(false);
        isInRoom = true;
        Debug.Log("Joined room " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.CustomProperties);
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        base.OnLeftRoom();
        _outsideRoom.SetActive(true);
        _leaveRoomButton.SetActive(false);
        _startButton.SetActive(false);
        _roomStatusObj.SetActive(false);
        _hasLeftRoom = true;
        isInRoom = false;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        _roomsCount = roomList.Count;
        base.OnRoomListUpdate(roomList);
        Debug.Log("Room list updated");


        if (roomList.Count > 0)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                Debug.Log(roomList[i].Name + ":" + roomList[i].PlayerCount + "/" + roomList[i].MaxPlayers);

                if (roomList[i].IsOpen && roomList[i].IsVisible && !MyRoomList.Contains(roomList[i]))
                {
                    _roomsDropDown.options.Add(new TMP_Dropdown.OptionData((roomList[i].Name + ":" + roomList[i].PlayerCount + "/" + roomList[i].MaxPlayers)));
                    MyRoomList.Add(roomList[i]);
                }

                for (int j = 0; j < _roomsDropDown.options.Count; j++)
                {
                    string st = _roomsDropDown.options[j].text.Split(':')[0];
                    if (st == roomList[i].Name)
                    {
                        _roomsDropDown.options[j].text = roomList[i].Name + ":" + roomList[i].PlayerCount + "/" + roomList[i].MaxPlayers;
                    }
                }
            }
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                _roomsCount = roomList.Count;
                Debug.Log("Room is closed " + roomList[i].Name);

                //Remove from dropdown
                for (int j = 0; j < _roomsDropDown.options.Count; j++)
                {
                    string st = _roomsDropDown.options[j].text.Split(':')[0];
                    if (st == roomList[i].Name)
                    {
                        _roomsDropDown.options.RemoveAt(j);
                    }
                }
               
                //List for Duplicates
                for (int j = 0; j < MyRoomList.Count; j++)
                {
                    if (MyRoomList[j].Name == roomList[i].Name)
                    {
                        MyRoomList.RemoveAt(j);
                    }
                }
                //roomList.RemoveAt(i);
                
            }
        }
    }


    //Fails , not everything impelemented yet

   
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Error number " + returnCode + "Error Message " + message);
        _errorOutput.text = "Error Output : " + message;
        PhotonNetwork.JoinLobby(new TypedLobby(_lobbiesDropDown.options[(int)_lobbiesDropDown.value].text, LobbyType.Default));
        if (returnCode ==32749)
        {
            StartCoroutine("NotifyPlayerAboutRejoin");
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("Error number " + returnCode + "Error Message " + message);
        _errorOutput.text = "Error Output :" + message + ". Maybe try  rejoining";
        PhotonNetwork.JoinLobby(new TypedLobby(_lobbiesDropDown.options[(int)_lobbiesDropDown.value].text, LobbyType.Default));
        if (returnCode == 32760 && _roomsCount > 0)
        {
            StartCoroutine("NotifyPlayerAboutRejoin");
            return;
        }
    }


    #endregion

    #region Misc
    System.Collections.IEnumerator NotifyPlayerAboutRejoin()
    {
        _rejoinButton.image.color = Color.cyan;
        yield return new WaitForSeconds(0.5f);
        _rejoinButton.image.color = Color.white;
    }
    System.Collections.IEnumerator NotifyPlayerAboutNickname()
    {
        nicknameEditorInputField.image.color = Color.grey;
        yield return new WaitForSeconds(0.5f);
        nicknameEditorInputField.image.color = Color.white;
    }

    #endregion
}
