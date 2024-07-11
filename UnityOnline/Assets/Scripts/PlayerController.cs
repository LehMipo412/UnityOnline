using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class PlayerController : MonoBehaviourPun
{
    private const string ProjectileTag = "Projectile";
    private const string ProjectilePrefabName = "Prefabs\\Projectile";
    private const string RecievedamageRPC = "RecieveDamage";
    [SerializeField] private Transform projectileSpawnTransform;
    [SerializeField] private float speed = 10;
    [SerializeField] private PhotonView _photonView;
    private Vector3 raycastPos;
    private Camera cachedCamera;
    private int HP = 30;

    private void Start()
    {
        cachedCamera = Camera.main;
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            Ray ray = cachedCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // hit.point contains the world position where the ray hit.
                raycastPos = hit.point;
            }
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
            if (Input.GetKeyDown(KeyCode.Mouse0))
                Shoot();


            Vector3 directionToFace = raycastPos - gameObject.transform.position;
            Quaternion lookAtRotation = Quaternion.LookRotation(directionToFace);
            Vector3 eulerRotation = lookAtRotation.eulerAngles;
            eulerRotation.x = 0;
            eulerRotation.z = 0;
            transform.eulerAngles = eulerRotation;
        }
    }
    [PunRPC]
    public void TakeDamage()
    {
        if(HP <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ProjectileTag))
        {
            ProjectileMovement otherProjectile = other.GetComponent<ProjectileMovement>();

            if (otherProjectile.photonView.Owner.ActorNumber == photonView.Owner.ActorNumber)
                return;

            if (otherProjectile.photonView.IsMine)
            {
                //run login that affect other players! only the projectile owner should do that
                StartCoroutine(DestroyDelay(5f, otherProjectile.gameObject));
                photonView.RPC(RecievedamageRPC, RpcTarget.All, 10);
                TakeDamage();
            }

            otherProjectile.visualPanel.SetActive(false);
            //add bool for projectile hit
        }
    }

    IEnumerator DestroyDelay(float delay, GameObject otherObject)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(otherObject);
    }

    public void Shoot()
    {
        GameObject projectile = PhotonNetwork.Instantiate(ProjectilePrefabName,
            projectileSpawnTransform.position, projectileSpawnTransform.rotation, 0,
            new object[] {});
    }
}