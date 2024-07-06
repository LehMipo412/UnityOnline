using System;
using System.Net.Mime;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private float speed = 10;
    [SerializeField] private PhotonView _photonView;
    private void Update()
    {
        if (!_photonView.IsMine)
            return;

        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.forward * (Time.deltaTime * speed));
        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * (Time.deltaTime * speed));
        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.back * (Time.deltaTime * speed));
        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * (Time.deltaTime * speed));
    }
}