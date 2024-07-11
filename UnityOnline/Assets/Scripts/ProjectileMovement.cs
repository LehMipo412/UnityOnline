using Photon.Pun;
using UnityEngine;

public class ProjectileMovement : MonoBehaviourPun, IPunInstantiateMagicCallback
{

    [SerializeField] Rigidbody projectileRB;
    [SerializeField] float speed = 20f;
    private float lifeSpan;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lifeSpan = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (photonView.IsMine)
            transform.Translate(Vector3.forward * (Time.deltaTime * speed));
        lifeSpan -= Time.deltaTime;

        if(lifeSpan <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }


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
                playerController.TakeDamage();
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
