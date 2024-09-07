using System;
using System.Collections.Generic;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.Cinemachine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class MultiplayerGameManager : MonoBehaviourPun
{
    [Header("Const Strings")]
    private const string NinjaPlayerPathName = "Prefabs\\NinjaPlayer";
    private const string WomanPlayerPathName = "Prefabs\\WomanPrefab";
    private const string CyborgPlayerPathName = "Prefabs\\CyborgPlayer";
    private const string PiratePlayerPathName = "Prefabs\\WomanPiratePlayer";
    private const string BarPlayerPathName = "Prefabs\\BarWarriorPlayer";

    [Header ("CinemaMachine")]
    public CinemachineCamera playerFollowerCamera;

    [Header("Spawn Points")]
    [SerializeField] private bool randomizeSpawnPoint;
    [SerializeField] private SpawnPoint[] randomSpawnPoints;
    [SerializeField] private SpawnPoint defaultSpawnPoint;

    [Header ("GameObjects")]
    private GameObject selectedPlayer;

    private void Start()
    {
        if (!PhotonNetwork.LocalPlayer.HasRejoined)
        {
            PhotonNetwork.LocalPlayer.CustomProperties = (new ExitGames.Client.Photon.Hashtable() { { "Kills", "0" } }); 
            Debug.LogWarning(PhotonNetwork.LocalPlayer.CustomProperties.ToString());
        }   
    }

    [PunRPC]
    public SpawnPoint GetRandomSpawnPoint()
    {
        List<SpawnPoint> availableSpawnPoints = new List<SpawnPoint>();

        foreach (var spawnPoint in randomSpawnPoints)
        {
            if (!spawnPoint.IsTaken)
            {
                availableSpawnPoints.Add(spawnPoint);
            }
        }

        if (availableSpawnPoints.Count == 0)
        {
            Debug.LogError("All spawn points are taken!");
        }

        int index = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
        return availableSpawnPoints[index];
    }
    public void SpawnPlayer(SpawnPoint targetSpawnPoint, int index)
    {
        switch (index)
        {
            case 0:
                targetSpawnPoint.Take();
                selectedPlayer = PhotonNetwork.Instantiate(NinjaPlayerPathName,
                    targetSpawnPoint.transform.position, targetSpawnPoint.transform.rotation);
                break;
            case 1:
                targetSpawnPoint.Take();
                selectedPlayer = PhotonNetwork.Instantiate(WomanPlayerPathName,
                    targetSpawnPoint.transform.position, targetSpawnPoint.transform.rotation);
                break;
            case 2:
                targetSpawnPoint.Take();
                selectedPlayer = PhotonNetwork.Instantiate(CyborgPlayerPathName,
                    targetSpawnPoint.transform.position, targetSpawnPoint.transform.rotation);
                break;
            case 3:
                targetSpawnPoint.Take();
                selectedPlayer = PhotonNetwork.Instantiate(PiratePlayerPathName,
                    targetSpawnPoint.transform.position, targetSpawnPoint.transform.rotation);
                break;
            case 4:
                targetSpawnPoint.Take();
                selectedPlayer = PhotonNetwork.Instantiate(BarPlayerPathName,
                    targetSpawnPoint.transform.position, targetSpawnPoint.transform.rotation);
                break;
            default:
                break;
        }

        playerFollowerCamera.Target.TrackingTarget = selectedPlayer.GetComponent<PlayerController>().neckIndicator;
    }
   
    [PunRPC]
    private void ClientIsReady(PhotonMessageInfo messageInfo)
    {
        Debug.Log(messageInfo.Sender + " Is ready");
        SpawnPoint randomSpawnPoint = GetRandomSpawnPoint();
        randomSpawnPoint.Take();
    }
    [PunRPC]
    public void TellPlayerToSwitchToAI(int leftPlayerId)
    {
        var currentPlayerController = selectedPlayer.GetComponent<PlayerController>();
    
        Debug.Log("going to ai thingy");
      
        Debug.LogWarning("creator actor: " + currentPlayerController.photonView.CreatorActorNr);
        Debug.LogWarning("Mass Of Your Supposed player: "+currentPlayerController.playerRB.mass);
      

        if (currentPlayerController.photonView.Owner.IsMasterClient)
        {
           currentPlayerController.photonView.RPC(nameof(currentPlayerController.SwitchFromPlayerToAI), RpcTarget.All, leftPlayerId);
        }
    }
}