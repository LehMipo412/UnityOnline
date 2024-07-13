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
    private const string boostBoxPrefabName = "Prefabs\\RoomObjects\\BoostBox";
    private const string damageBoxPrefabNName = "Prefabs\\RoomObjects\\DamageBox";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < spawnPointsArray.Length; i++)
        {
            if(i % 2 == 0)
            {
                PhotonNetwork.InstantiateRoomObject(boostBoxPrefabName, spawnPointsArray[i].position, Quaternion.identity);
            }
            if (i % 2 == 1)
            {
                PhotonNetwork.InstantiateRoomObject(damageBoxPrefabNName, spawnPointsArray[i].position, Quaternion.identity);
            }
        }
    }

   
}
