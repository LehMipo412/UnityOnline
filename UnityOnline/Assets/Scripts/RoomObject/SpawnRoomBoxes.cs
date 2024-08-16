using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;



public class SpawnRoomBoxes : MonoBehaviourPun
{
    public Transform[] spawnPointsArray;
    private const string HealthPickUpName = "Prefabs\\HealthPickUp";
    private const string SpeedBoostPickupName = "Prefabs\\SpeedPickUp";
    public List<Transform> takenPlacesList = new List<Transform>();
    private float timer = 3f;
    private Transform currentSpawnPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //for (int i = 0; i < spawnPointsArray.Length; i++)
        //{
        //    if(i % 2 == 0)
        //    {
        //        PhotonNetwork.InstantiateRoomObject(HealthPickUpName, spawnPointsArray[i].position, Quaternion.identity);
        //    }
        //    if (i % 2 == 1)
        //    {
        //        PhotonNetwork.InstantiateRoomObject(SpeedBoostPickupName, spawnPointsArray[i].position, Quaternion.identity);
        //    }
        //}
        SetNextSpawnPoint();
    }

    void Update()
    {
        if (photonView.Owner == PhotonNetwork.MasterClient)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                string pickupToSpawn = ChooseRandomPickupAndLocation();
                if (pickupToSpawn != "")
                {
                    PhotonNetwork.InstantiateRoomObject(pickupToSpawn, currentSpawnPoint.position, Quaternion.identity);
                    Debug.Log($"{spawnPointsArray.Length}, {takenPlacesList.Count}");
                    Debug.Log("Pickup Spawned");
                    photonView.RPC(nameof( SetNextSpawnPoint), RpcTarget.All);
                    timer = 3f;
                }
                else
                {
                    timer = 3f;
                    photonView.RPC(nameof(SetNextSpawnPoint), RpcTarget.All);
                    return;
                }
            }
           
        }
        
    }
    public string ChooseRandomPickupAndLocation()
    {
        if(currentSpawnPoint == default)
        {
            return "";
        }

        

        int randomPickup = Random.Range(0, 2);

       

        if(randomPickup== 0)
        {
          
            return HealthPickUpName;
        }
        else
        {
            
            return SpeedBoostPickupName;
        }


        
    }

    [PunRPC]
    public void SetNextSpawnPoint()
    {
        int randomIndex = Random.Range(0, spawnPointsArray.Length);

        if (takenPlacesList.Count == spawnPointsArray.Length)
        {
            Debug.Log("No Available Spawn Points");
            currentSpawnPoint = default;
            return;
        }
        currentSpawnPoint = spawnPointsArray[randomIndex];
        takenPlacesList.Add(spawnPointsArray[randomIndex]);


        while (takenPlacesList.Contains(currentSpawnPoint))
        {
           

            randomIndex = Random.Range(0, spawnPointsArray.Length);
            takenPlacesList.Add(spawnPointsArray[randomIndex]);
            currentSpawnPoint = spawnPointsArray[randomIndex];
           
        }
    }


}
