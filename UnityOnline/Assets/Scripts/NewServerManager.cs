using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class NewServerManager : MonoBehaviourPunCallbacks
{[Header("Texts")]
    [SerializeField] private TMP_Text _roomStatus;
    [SerializeField] private TMP_Text _playerAmmount;

    [Header("DropDowns")]
    [SerializeField] private TMP_Dropdown _lobbiesDropDown;
    [SerializeField] private TMP_Dropdown _roomsDropDown;

    [Header("Inputs")]
    [SerializeField] private TMP_InputField _roomNameInput;

    [Header("GameObjects")]
    [SerializeField] private GameObject _lobbySection;
    [SerializeField] private GameObject _roomsSection;
    [SerializeField] private GameObject _leaveRoomButton;
    [SerializeField] private GameObject _outsideRoom;
    [SerializeField] private GameObject _startButton;


    [Header("Text")]
    [SerializeField] private Slider _playerAmmountSlider;

    [Header("Others")]
    private bool _hasLeftRoom = false;

    #region Awake/Start

    public void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public void Update()
    {
        _roomStatus.text = PhotonNetwork.NetworkClientState.ToString();
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
            PhotonNetwork.JoinRoom(_roomsDropDown.options[_roomsDropDown.value].text.Substring(0, _roomsDropDown.options[_roomsDropDown.value].text.IndexOf(':')));
    }
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (int)_playerAmmountSlider.value;
        roomOptions.EmptyRoomTtl = 30000;
        PhotonNetwork.CreateRoom(_roomNameInput.text, roomOptions, PhotonNetwork.CurrentLobby);

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
    private string RoomInfo()
    {
        return PhotonNetwork.CurrentRoom.Name.ToString() + ":" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    #endregion

    #region callbacks

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        if(!_hasLeftRoom)
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
        _roomsDropDown.options.Add(new TMP_Dropdown.OptionData(RoomInfo()));
        Debug.Log("Created Room : " + PhotonNetwork.CurrentRoom.Name);
        _leaveRoomButton.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        _leaveRoomButton.SetActive(true);
        _startButton.SetActive(true);
        _outsideRoom.SetActive(false);
        Debug.Log("Joined room " + PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        base.OnLeftRoom();
        _leaveRoomButton.SetActive(false);
        _startButton.SetActive(false);
        _outsideRoom.SetActive(true);
        _hasLeftRoom = true;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        Debug.Log("Room list updated");

        //PhotonNetwork.CurrentRoom.Name + ":" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        if (roomList.Count > 0)
        {
            //_roomsDropDown.options[i].text.Substring(0, _roomsDropDown.options[i].text.IndexOf(':')) + ":" + roomList[i].PlayerCount + "/" + roomList[i].MaxPlayers
            Debug.Log("Room list counter "  + roomList.Count);
            for (int i = 0; i < roomList.Count; i++)
            {
                if (_roomsDropDown.options[i].text == "" && _roomsDropDown.options[i].text != roomList[i].Name + ":" + roomList[i].PlayerCount + "/" + roomList[i].MaxPlayers)
                {
                    _roomsDropDown.options.Add(new TMP_Dropdown.OptionData(roomList[i].Name + ":" + roomList[i].PlayerCount + "/" + roomList[i].MaxPlayers));
                }
                else Debug.Log("Nothing happened");
            }
          
                for (int i = 0; i < roomList.Count; i++)
            {/*.Substring(_roomsDropDown.options[_roomsDropDown.value].text.IndexOf(':'), _roomsDropDown.options[_roomsDropDown.value].text.Length - 1);*/
                //_roomsDropDown.options[i].text = _roomsDropDown.options[i].text.Substring(0,_roomsDropDown.options[i].text.IndexOf(':'))+ ":" + roomList[i].PlayerCount+"/"+ roomList[i].MaxPlayers;
                if (roomList[i].RemovedFromList)
                { 
                    Debug.Log("Room is closed" + roomList[i].Name);
                    _roomsDropDown.options.RemoveAt(i);
                    roomList.RemoveAt(i);
                }
            }
        }
    }

    //Fails

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("Error number "+returnCode + "Error Message "+ message);
    }


    #endregion
}
