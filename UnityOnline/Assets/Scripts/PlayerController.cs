using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class PlayerController : MonoBehaviourPun
{
    private const string ProjectileTag = "Projectile";
    private const string BoostBoxTag = "BoostBox";
    private const string DamageBoxTag = "DamageBox";
    private const string ProjectilePrefabName = "Prefabs\\Projectile";
    private const string RecievedamageRPC = "RecieveDamage";
    [SerializeField] private Transform projectileSpawnTransform;
    [SerializeField] private float speed = 10;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private PhotonView champSelectPhotonView;
    [SerializeField] private ChampSelectManger _champSelectManger;
    private Vector3 raycastPos;
    private Camera cachedCamera;
    private int HP = 200;

    private void Start()
    {
        cachedCamera = Camera.main;
        champSelectPhotonView = GameObject.Find("ChampSelectManagerGO").GetComponent<PhotonView>();
        _champSelectManger = GameObject.Find("ChampSelectManagerGO").GetComponent<ChampSelectManger>();
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (!ChampSelectManger.isPaused)
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
                {
                    playerAnimator.SetBool("IsRunning", true);
                    Debug.Log("runing");
                    transform.Translate(Vector3.forward * (Time.deltaTime * speed));
                }
                if (Input.GetKey(KeyCode.A))
                {
                    playerAnimator.SetBool("IsRunning", true);
                    transform.Translate(Vector3.left * (Time.deltaTime * speed));
                }

                if (Input.GetKey(KeyCode.S))
                {
                    playerAnimator.SetBool("IsRunning", true);
                    transform.Translate(Vector3.back * (Time.deltaTime * speed));
                }

                if (Input.GetKey(KeyCode.D))
                {
                    playerAnimator.SetBool("IsRunning", true);
                    transform.Translate(Vector3.right * (Time.deltaTime * speed));
                }
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    Shoot();

                if(!Input.anyKey)
                {
                    playerAnimator.SetBool("IsRunning", false);
                    Debug.Log("Idle");
                }
                Vector3 directionToFace = raycastPos - gameObject.transform.position;
                Quaternion lookAtRotation = Quaternion.LookRotation(directionToFace);
                Vector3 eulerRotation = lookAtRotation.eulerAngles;
                eulerRotation.x = 0;
                eulerRotation.z = 0;
                transform.eulerAngles = eulerRotation;
            }
        }
    }
    [PunRPC]
    public void TakeDamage()
    {
        if(HP == 0)
        {
            
            if (photonView.IsMine)
            {
                champSelectPhotonView.RPC(nameof(_champSelectManger.RemoveLivingPkayer), RpcTarget.All);
                Debug.Log("Players Remaining: " + _champSelectManger.livingPlayersCounter);
                StartCoroutine(DestroyDelay(2f, gameObject));
            }
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
                StartCoroutine(DestroyDelay(1f, otherProjectile.gameObject));
                photonView.RPC(RecievedamageRPC, RpcTarget.All, 10);
                
            }

            otherProjectile.visualPanel.SetActive(false);
            //add bool for projectile hit
        }

        if (other.CompareTag(BoostBoxTag))
        {
            playerRB.AddForce(Vector3.up * 20, ForceMode.Impulse);
        }
        if (other.CompareTag(DamageBoxTag))
        {
            photonView.RPC(RecievedamageRPC, RpcTarget.All, 10);
        }
    }
    [PunRPC]
    private void RecieveDamage(int damageAmount)
    {
        HP -= damageAmount;
        Debug.Log("Hp left is " + HP);
        TakeDamage();
    }
    IEnumerator DestroyDelay(float delay, GameObject otherObject)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(otherObject);
    }

    public void Shoot()
    {
        Debug.Log("fire in the hole");
        GameObject projectile = PhotonNetwork.Instantiate(ProjectilePrefabName,
            projectileSpawnTransform.position, projectileSpawnTransform.rotation, 0,
            new object[] {});
    }
}