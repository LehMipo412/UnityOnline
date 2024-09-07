using System;
using Photon.Pun;
using UnityEngine;

public class ProjectileMovement : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    private const string PlayerTag = "Player";

    [SerializeField] private Material[] projectileColors;
    [SerializeField] public MeshRenderer meshRenderer;
    public GameObject visualPanel;
    [SerializeField] private float speed = 20;
    private float timer = 3;

    void Update()
    {
        if (photonView.IsMine)
            transform.Translate(Vector3.forward * (Time.deltaTime * speed));

        timer -= Time.deltaTime;

        if(timer <=0)
            PhotonNetwork.Destroy(gameObject);

    }

   
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("Projectile indeed spawned");
    }

    [PunRPC]
    public void DisableRenderer()
    {
        meshRenderer.enabled = false;
    }
}
