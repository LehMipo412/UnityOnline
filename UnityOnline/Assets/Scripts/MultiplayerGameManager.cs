using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class MultiplayerGameManager : MonoBehaviourPun
{
    private const string PlayerPrefabName = "Prefabs\\Player";

    [Header("Spawn Points")]
    [SerializeField] private bool randomizeSpawnPoint;

    [SerializeField] private SpawnPoint[] randomSpawnPoints;

    [SerializeField] private SpawnPoint defaultSpawnPoint;

    

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
    public void SpawnPlayer(SpawnPoint targetSpawnPoint)
    {
        targetSpawnPoint.Take();
        PhotonNetwork.Instantiate(PlayerPrefabName,
            targetSpawnPoint.transform.position, targetSpawnPoint.transform.rotation);
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