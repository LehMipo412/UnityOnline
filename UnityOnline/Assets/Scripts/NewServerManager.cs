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

    [Header("DropDowns")]
    [SerializeField] private TMP_Dropdown _lobbiesDropDown;
    [SerializeField] private TMP_Dropdown _roomsDropDown;

    [Header("Inputs")]
    [SerializeField] private TMP_InputField _roomNameInput;
    [SerializeField] TMP_InputField nicknameEditorInputField;

    [Header("GameObjects")]
    [SerializeField] private GameObject _lobbySection;
    [SerializeField] private GameObject _roomsSection;
    [SerializeField] private GameObject _leaveRoomButton;
    [SerializeField] private GameObject _outsideRoom;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _roomStatusObj;


    [Header("Text")]
    [SerializeField] private Slider _playerAmmountSlider;

    [Header("Others")]
    List<RoomInfo> MyRoomList;
    private bool _hasLeftRoom = false;
    private bool isInRoom = false;
    private int RoomCount = 0;

    #region Awake/Start

    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
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

    public void UpdateSlider()
    {
        _playerAmmount.text = _playerAmmountSlider.value.ToString();
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void JoinRoom()
    {
        if (_roomsDropDown.options.Count > 0)
        {
            PhotonNetwork.JoinRoom(_roomsDropDown.options[_roomsDropDown.value].text.Substring(0, _roomsDropDown.options[_roomsDropDown.value].text.IndexOf(':')));
        }
       
    }
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (int)_playerAmmountSlider.value;
        roomOptions.EmptyRoomTtl = 30000;
        //roomOptions.PlayerTtl = 30000;
        PhotonNetwork.CreateRoom(_roomNameInput.text, roomOptions, TypedLobby.Default);

    }
    public void JoinRandomRoom()
    {
        Debug.Log("Room count is " + RoomCount);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.EmptyRoomTtl = 30000;
        if (RoomCount == 0)
        {
            PhotonNetwork.JoinRandomOrCreateRoom(null, 20, MatchmakingMode.FillRoom, TypedLobby.Default, null, "Default", roomOptions);
            return;
        }

        PhotonNetwork.JoinRandomRoom();

    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby(new TypedLobby(_lobbiesDropDown.options[(int)_lobbiesDropDown.value].text, LobbyType.Default));
    }
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("CurrentMainGameScene");
        }

    }
    public void EditNickname()
    {
        PhotonNetwork.NickName = nicknameEditorInputField.text;
    }

    #endregion

    #region callbacks

    public override void OnConnectedToMaster()
    {
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
        Debug.Log("Joined room " + PhotonNetwork.CurrentRoom.Name);
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
        RoomCount = roomList.Count;
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
                RoomCount = roomList.Count;
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

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("Error number " + returnCode + "Error Message " + message);
    }


    #endregion
}
