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
    [Header ("Const Strings")]
    private const string HealthPickUpName = "Prefabs\\HealthPickUp";
    private const string SpeedBoostPickupName = "Prefabs\\SpeedPickUp";
    [Header("Transforms")]
    public Transform[] spawnPointsArray;
    public Transform[] takenPlacesList;
    private Transform currentSpawnPoint;
    [Header("Others")]
    private int takenIndex = 0;
    private int loopTimes = 0;
    private float timer = 3f;


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
            currentSpawnPoint = default;
            return;
        }

        int randomIndex = Random.Range(0, spawnPointsArray.Length);
      


        currentSpawnPoint = spawnPointsArray[randomIndex];
        if (ContainTransform(takenPlacesList, currentSpawnPoint))
        {
            ApplyNewLocation();
        }
       
        takenPlacesList[takenIndex] = spawnPointsArray[randomIndex];
      
        takenIndex++;
    }

    public bool ContainTransform(Transform[] containingArray, Transform isContained)
    {
        foreach (Transform location in containingArray)
        {
            
            if (isContained.position == location.position)
            {
                
                return true;
            }
        }
        
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
        int placeindex = 0;
        for (int i = 0; i < takenPlacesList.Length; i++)
        {
            if (transformInArray.position == takenPlacesList[i].position)
            {
                return i;
            }
            placeindex++;
        }
        Debug.LogError("You did not find the pick up location: i= " +placeindex);
        return 0;
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
