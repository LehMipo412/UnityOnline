using Photon.Pun;
using UnityEngine;

public class ProjectileMovement : MonoBehaviourPun
{
    [SerializeField] private float speed = 20;
    private float timer = 3;

    public GameObject visualPanel;

    void Update()
    {
        if (photonView.IsMine)
        {
            transform.Translate(Vector3.forward * (Time.deltaTime * speed));
        }

        timer -= Time.deltaTime;

        if (timer <= 0 && photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }

    }
}
