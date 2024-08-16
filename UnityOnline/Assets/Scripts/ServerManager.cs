using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using Photon.Pun.UtilityScripts;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private UnityEngine.UI.Button[] ButtonsMainMenu;

    [SerializeField] private TMP_InputField LobbyWanted;
    [SerializeField] private TextMeshProUGUI debugPhoton;
    [SerializeField] private TextMeshProUGUI RoomInfo;
    [SerializeField] private TMP_InputField nicknameEditorInputField;

    private const string LobbyDefultName = "Our First Lobby";
    private const string LobbySecondName = "Our Second Lobby";

    private void InitialButtons()
    {
        for (int i = 0; i < ButtonsMainMenu.Length; i++)
        {
            ButtonsMainMenu[i].enabled = false;

            if (i != 0 && i < ButtonsMainMenu.Length)
            {
                ButtonsMainMenu[i].gameObject.SetActive(false);


            }
            if (i == 6)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    ButtonsMainMenu[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void Awake()
    {
        InitialButtons();
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We succefully connected to photon");
        base.OnConnectedToMaster();

        if (ButtonsMainMenu.Length > 0)
        {
            ButtonsMainMenu[0].enabled = true;
        }
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    //To join the lobby
    public void JoinLobby()
    {
        Debug.Log($"LobbyDefultName: {LobbyDefultName}, LobbySecondName: {LobbySecondName}, LobbyWanted.text: {LobbyWanted.text}");

        if (LobbySecondName == LobbyWanted.text)
        {
            Debug.Log("Joining LobbySecondName...");
            PhotonNetwork.JoinLobby(new TypedLobby(LobbySecondName, LobbyType.Default));
        }

        else
        {
            Debug.Log("Joining LobbyDefultName...");
            PhotonNetwork.JoinLobby(new TypedLobby(LobbyDefultName, LobbyType.Default));
        }
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log($"We successfully joined the lobby {PhotonNetwork.CurrentLobby.Name}!");
        ActiveRooms();
    }

    public void ActiveRooms()
    {
        if (PhotonNetwork.CurrentLobby.Name == LobbySecondName)
        {
            for (int i = 1; i < ButtonsMainMenu.Length; i++)
            {
                ButtonsMainMenu[i].enabled = true;
                ButtonsMainMenu[0].enabled = false;

                if (i != 2 && i != 5 && i != 4 && i < ButtonsMainMenu.Length)
                {
                    ButtonsMainMenu[i].gameObject.SetActive(true);
                }
            }
        }

        else
        {
            for (int i = 1; i < ButtonsMainMenu.Length; i++)
            {
                ButtonsMainMenu[i].enabled = true;
                ButtonsMainMenu[0].enabled = false;

                if (i != 1 && i != 4 && i != 5 && i < ButtonsMainMenu.Length)
                {
                    ButtonsMainMenu[i].gameObject.SetActive(true);
                }
            }
        }
    }

    //To join a room
    public void CreateAndJoinRoom(int maxPlayers)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayers,
        };
        string roomName;

        if (PhotonNetwork.CurrentLobby.Name == LobbyDefultName)
        {
            roomName = "Room 1";
        }
        else if (PhotonNetwork.CurrentLobby.Name == LobbySecondName)
        {
            roomName = "Room 2";
        }
        else
        {
            Debug.LogError("Unexpected lobby name: " + PhotonNetwork.CurrentLobby.Name);
            return;
        }

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public void DeActivateButtons()
    {
        for (int i = 0; i < ButtonsMainMenu.Length; i++)
        {
            if (ButtonsMainMenu[i].tag == "Rooms")
            {
                ButtonsMainMenu[i].gameObject.SetActive(false);
            }

            if (PhotonNetwork.CurrentLobby.Name == LobbySecondName)
            {
                ButtonsMainMenu[4].gameObject.SetActive(true);
            }

            else if (PhotonNetwork.CurrentLobby.Name == LobbyDefultName)
            {
                ButtonsMainMenu[5].gameObject.SetActive(true);
            }
            if (i == 6)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    ButtonsMainMenu[i].gameObject.SetActive(true);
                }
            }
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        Debug.Log("Room created successfully!");
    }

    public void JoinRandomRoom()
    {
        //PhotonNetwork.JoinRandomRoom();
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We successfully joined the room " + PhotonNetwork.CurrentRoom);

        Room room = PhotonNetwork.CurrentRoom;
        if (RoomInfo != null && PhotonNetwork.CurrentRoom != null)
        {
            RoomInfo.text = $"Room: {room.Name} - Players: {room.PlayerCount + " / " + room.MaxPlayers}";
        }

        DeActivateButtons();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        foreach (RoomInfo roomInfo in roomList)
        {
            Debug.Log(roomInfo.Name);
            Debug.Log(roomInfo.PlayerCount);
        }
    }

    //Leave Room
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();

        if (PhotonNetwork.CurrentLobby.Name == LobbySecondName)
        {
            ButtonsMainMenu[4].gameObject.SetActive(false);
        }

        else if (PhotonNetwork.CurrentLobby.Name == LobbyDefultName)
        {
            ButtonsMainMenu[5].gameObject.SetActive(false);

        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        if (RoomInfo != null && PhotonNetwork.CurrentRoom == null)
        {
            RoomInfo.text = "Left Room";
        }

        ActiveRooms();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        

        Room room = PhotonNetwork.CurrentRoom;
        if (RoomInfo != null && PhotonNetwork.CurrentRoom != null)
        {
            RoomInfo.text = $"Room: {room.Name} - Players: {room.PlayerCount + " / " + room.MaxPlayers}";
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        Room room = PhotonNetwork.CurrentRoom;
        
        if (RoomInfo != null && PhotonNetwork.CurrentRoom != null)
        {
            RoomInfo.text = $"Room: {room.Name} - Players: {room.PlayerCount + " / " + room.MaxPlayers}";
        }
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
        Debug.Log("The name of this Client is:" + PhotonNetwork.NickName);
    }
    public void Update()
    {
        debugPhoton.text = PhotonNetwork.NetworkClientState.ToString();
    }
}