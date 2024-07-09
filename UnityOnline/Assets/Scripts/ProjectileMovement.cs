using Photon.Pun;
using UnityEngine;

public class ProjectileMovement : MonoBehaviourPun, IPunInstantiateMagicCallback
{

    [SerializeField] Rigidbody projectileRB;
    [SerializeField] float speed = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        projectileRB.AddForce(Vector3.forward * (Time.deltaTime * speed));
    }

    // Update is called once per frame
    void Update()
    {
       
        


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController.photonView.Owner.ActorNumber == photonView.Owner.ActorNumber)
                return;

            if (photonView.IsMine)
            {
                //run login that affect other players! only the projectile owner should do that
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
