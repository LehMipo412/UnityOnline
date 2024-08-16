using System;
using System.Collections;
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
    public Transform[] takenPlacesList;
    int takenIndex = 0;
    private float timer = 3f;
    private int loopTimes = 0;
    private Transform currentSpawnPoint;


    void Start()
    {

        ChampSelectManger.Instance._pickupCollectedEventScript.pickupedEvent.AddListener(TellEveryoneYouPickedUpBoost);

       
        if ((photonView.Owner == PhotonNetwork.MasterClient))
         {
            photonView.RPC(nameof(SetNextSpawnPoint), RpcTarget.All);
        }
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

                    PhotonNetwork.InstantiateRoomObject(pickupToSpawn, currentSpawnPoint.position, currentSpawnPoint.rotation);
                    
                    Debug.Log($"{spawnPointsArray.Length}, {takenIndex}");
                    Debug.Log("Pickup Spawned");
                    timer = 3f;
                    photonView.RPC(nameof(SetNextSpawnPoint), RpcTarget.All);

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
        if (currentSpawnPoint == default)
        {
            return "";
        }



        int randomPickup = Random.Range(0, 2);



        if (randomPickup == 0)
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
        loopTimes = 1;
        if (takenIndex == spawnPointsArray.Length)
        {
            Debug.Log("No Available Spawn Points");
            currentSpawnPoint = default;
            return;
        }

        int randomIndex = Random.Range(0, spawnPointsArray.Length);
       // Debug.Log(randomIndex);


        currentSpawnPoint = spawnPointsArray[randomIndex];
        if (ContainTransform(takenPlacesList, currentSpawnPoint))
        {
            ApplyNewLocation();
        }
        Debug.Log($"It Found New Location After {loopTimes} Times");



        //while (ContainTransform(takenPlacesList, currentSpawnPoint))
        //{
        //    //looptime++;
        //    //Debug.Log(looptime);

        //    randomIndex = Random.Range(0, spawnPointsArray.Length);
        //    takenPlacesList[takenIndex] = (spawnPointsArray[randomIndex]);
        //    currentSpawnPoint = spawnPointsArray[randomIndex];

        //}
        takenPlacesList[takenIndex] = spawnPointsArray[randomIndex];
        Debug.Log("Spawn Point Added");
        takenIndex++;
    }

    public bool ContainTransform(Transform[] containingArray, Transform isContained)
    {
        foreach (Transform location in containingArray)
        {
            //  Debug.Log($"Locations comparer: {isContained} In Compare to {location}");
            if (isContained.position == location.position)
            {
                // Debug.Log("Found Your Location, FBI On the way");
                return true;
            }
        }
        // Debug.Log("Location is not taken");
        return false;
    }
    public void ApplyNewLocation()
    {
        if (loopTimes <= 6000)
        {
            loopTimes++;
            if (ContainTransform(takenPlacesList, currentSpawnPoint))
            {


                var randomIndex = Random.Range(0, spawnPointsArray.Length);
                takenPlacesList[takenIndex] = (spawnPointsArray[randomIndex]);
                currentSpawnPoint = spawnPointsArray[randomIndex];
                ApplyNewLocation();
            }
        }
    }
    public int GetIndexOfTransformInArray(Transform transformInArray)
    {
        for (int i = 0; i < takenPlacesList.Length; i++)
        {
            if (transformInArray.position == takenPlacesList[i].position)
            {
                return i;
            }
        }
        return -1;
    }
    [PunRPC]
    public void TellEveryoneYouPickedUpBoost()
    {
        photonView.RPC(nameof(GetPickupTransformFromGM), RpcTarget.All);
    }
    [PunRPC]
    public void GetPickupTransformFromGM()
    {
        Debug.Log("This pickup was taken!");
        PickUpWasTaken(ChampSelectManger.Instance.currentPickupTransform);
    }
    public void PickUpWasTaken(Transform pickupTransform)
    {
        Debug.LogWarning("The Index That Supposed To Be Deleted Is: "+GetIndexOfTransformInArray(  pickupTransform));
        takenPlacesList[GetIndexOfTransformInArray(pickupTransform)] = transform;
        takenIndex--;
    }
}
