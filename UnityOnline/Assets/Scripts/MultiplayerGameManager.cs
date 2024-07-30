using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.Cinemachine;

public class MultiplayerGameManager : MonoBehaviourPun
{
    private const string NinjaPlayerPathName = "Prefabs\\NinjaPlayer";
    private const string WomanPlayerPathName = "Prefabs\\WomanPrefab";
    private const string CyborgPlayerPathName = "Prefabs\\CyborgPlayer";
    private const string PiratePlayerPathName = "Prefabs\\WomanPiratePlayer";
    public CinemachineCamera playerFollowerCamera;


    [Header("Spawn Points")]
    [SerializeField] private bool randomizeSpawnPoint;

    [SerializeField] private SpawnPoint[] randomSpawnPoints;

    [SerializeField] private SpawnPoint defaultSpawnPoint;

    private GameObject selectedPlayer;



    private void Start()
    {
      
    }

  public  SpawnPoint GetRandomSpawnPoint()
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
        
        if (index == 0)
        {
            targetSpawnPoint.Take();
            selectedPlayer = PhotonNetwork.Instantiate(NinjaPlayerPathName,
                targetSpawnPoint.transform.position, targetSpawnPoint.transform.rotation);
        }
        if (index == 1)
        {
            targetSpawnPoint.Take();
            selectedPlayer = PhotonNetwork.Instantiate(WomanPlayerPathName,
                targetSpawnPoint.transform.position, targetSpawnPoint.transform.rotation);
        }
        if (index == 2)
        {
            targetSpawnPoint.Take();
            selectedPlayer = PhotonNetwork.Instantiate(CyborgPlayerPathName,
                targetSpawnPoint.transform.position, targetSpawnPoint.transform.rotation);
        }
        if (index == 3)
        {
            targetSpawnPoint.Take();
            selectedPlayer = PhotonNetwork.Instantiate(PiratePlayerPathName,
                targetSpawnPoint.transform.position, targetSpawnPoint.transform.rotation);
        }
        playerFollowerCamera.Target.TrackingTarget = selectedPlayer.GetComponent<PlayerController>().neckIndicator;
        //playerFollowerCamera.Follow = selectedPlayer.GetComponent<PlayerController>().neckIndicator;
        //playerFollowerCamera.LookAt= selectedPlayer.GetComponent<PlayerController>().mouseIndicator; ;
        //Cursor.lockState = CursorLockMode.Locked;
    }
    [PunRPC]
    private void SetSpawnPoint(SpawnPoint spawnPoint)
    {
        Debug.Log("Recieved spawn point is " + spawnPoint);
    }
    [PunRPC]
    private void ClientIsReady(PhotonMessageInfo messageInfo)
    {
        Debug.Log(messageInfo.Sender + " Is ready");
        SpawnPoint randomSpawnPoint = GetRandomSpawnPoint();
        randomSpawnPoint.Take();

        messageInfo.photonView.RPC(nameof(SetSpawnPoint), messageInfo.Sender, randomSpawnPoint);
    }


}