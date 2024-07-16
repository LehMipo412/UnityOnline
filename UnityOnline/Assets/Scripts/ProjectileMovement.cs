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

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
            transform.Translate(Vector3.forward * (Time.deltaTime * speed));

        timer -= Time.deltaTime;

        if(timer <=0)
            PhotonNetwork.Destroy(gameObject);

    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag(PlayerTag))
    //     {
    //         PlayerController playerController = other.GetComponent<PlayerController>();
    //         if (playerController.photonView.Owner.ActorNumber == photonView.Owner.ActorNumber)
    //             return;
    //         
    //         if (photonView.IsMine)
    //         {
    //             //run login that affect other players! only the projectile owner should do that
    //             PhotonNetwork.Destroy(gameObject);
    //         }
    //     }
    // }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        meshRenderer.material = projectileColors[(int)instantiationData[0]];
    }

    [PunRPC]
    public void DisableRenderer()
    {
        meshRenderer.enabled = false;
    }
}
