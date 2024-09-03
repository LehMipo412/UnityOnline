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
    private string ProjectilePrefabName = "Prefabs\\Projectile";
    private const string RecievedamageRPC = "RecieveDamage";
    [SerializeField] private Transform projectileSpawnTransform;
    [SerializeField] public float damage;
    [SerializeField] private float knockbackPrecentage;
    [SerializeField] private float speed = 10;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private ChampSelectManger _champSelectManger;
    [SerializeField] private GameObject strikeZone;
    [SerializeField] private float animationOffset;
    public Transform neckIndicator;
    public Transform mouseIndicator;
    public bool isSupposedToBeControlledByAI = false;
    private float AIChangeTimerTimer = 5f;
    private float AIRandomDiraction;

    private Vector3 raycastPos;
   // private Camera cachedCamera;
    private int HP = 200;

    private void Start()
    {
        //cachedCamera = Camera.main;
        
        _champSelectManger = ChampSelectManger.Instance;
        if(playerRB.mass ==2f)
        {
            ProjectilePrefabName = "Prefabs\\KunaiPrefab";
        
        }
        if (playerRB.mass == 1f)
        {
            ProjectilePrefabName = "Prefabs\\NewArrowPrefab";

        }
        if (playerRB.mass == 1.2f)
        {
            ProjectilePrefabName = "Prefabs\\FistPrefab";

        }
        if (playerRB.mass == 1.5f)
        {
            ProjectilePrefabName = "Prefabs\\SlashPrefab";

        }
        if (playerRB.mass == 1.7f)
        {
            ProjectilePrefabName = "Prefabs\\PencilProjectile";

        }
        AIRandomDiraction = Random.Range(0, 4);
    }

    [PunRPC]
    public void GetKnockedBack(Vector3 hitDitraction,float additionalKBPrec)
    {
     
        playerRB.AddForce(hitDitraction * (1 * knockbackPrecentage*0.8f) * -1*2*1.2f, ForceMode.Impulse);
        knockbackPrecentage += additionalKBPrec;
        playerRB.AddForce(Vector3.up * (1 * knockbackPrecentage/3) * 2, ForceMode.Impulse);
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (!ChampSelectManger.isPaused)
            {
                if (!ChatManagerScript.isChatting)
                {
                    //Ray ray = cachedCamera.ScreenPointToRay(Input.mousePosition);
                    //RaycastHit hit;
                    //if (Physics.Raycast(ray, out hit))
                    //{
                    //    // hit.point contains the world position where the ray hit.
                    //    raycastPos = hit.point;
                    //}
                    if (!_photonView.IsMine)
                        return;
                    if (!isSupposedToBeControlledByAI)
                    { 
                        if (Input.GetKey(KeyCode.W))
                        {
                            playerAnimator.SetBool("IsRunning", true);
                            // Debug.Log("runing");
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
                    {
                        Shoot();
                    }
                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        StrikeEnvelope();
                    }

                    if (!Input.anyKey)
                    {

                        playerAnimator.SetBool("IsRunning", false);
                        //  Debug.Log("Idle");
                    }
                    //Vector3 directionToFace = raycastPos - gameObject.transform.position;
                    //Quaternion lookAtRotation = Quaternion.LookRotation(directionToFace);
                    //Vector3 eulerRotation = lookAtRotation.eulerAngles;
                    //eulerRotation.x = 0;
                    //eulerRotation.z = 0;
                    //transform.eulerAngles = eulerRotation;
                }
                    else
                    {
                        AIChangeTimerTimer -= Time.deltaTime;
                        if(AIChangeTimerTimer <=0)
                        {
                            StrikeEnvelope();
                            AIChangeTimerTimer = 5f;
                            AIRandomDiraction = Random.Range(0, 4);
                        }
                        else
                        {
                            if (AIRandomDiraction == 0)
                            {
                                playerAnimator.SetBool("IsRunning", true);
                                // Debug.Log("runing");
                                transform.Translate(Vector3.forward * (Time.deltaTime * speed));
                            }
                            if (AIRandomDiraction == 1)
                            {
                                playerAnimator.SetBool("IsRunning", true);
                                // Debug.Log("runing");
                                transform.Translate(Vector3.left * (Time.deltaTime * speed));
                            }
                            if (AIRandomDiraction == 2)
                            {
                                playerAnimator.SetBool("IsRunning", true);
                                // Debug.Log("runing");
                                transform.Translate(Vector3.back * (Time.deltaTime * speed));
                            }
                            if (AIRandomDiraction == 3)
                            {
                                playerAnimator.SetBool("IsRunning", true);
                                // Debug.Log("runing");
                                transform.Translate(Vector3.right * (Time.deltaTime * speed));
                            }
                        }
                    }
                }
            }
        }
    }

    public void StrikeEnvelope()
    {
        Debug.Log("Strike!");
        photonView.RPC(nameof(StrikeFunc), RpcTarget.All);
    }
    [PunRPC]
    public void TakeDamage()
    {
        if(HP == 0)
        {
            
            if (photonView.IsMine)
            {
                _champSelectManger.photonView.RPC(nameof(_champSelectManger.RemoveLivingPkayer), RpcTarget.All);
                Debug.Log("Players Remaining: " + _champSelectManger.livingPlayersCounter);
                StartCoroutine(DestroyDelay(2f, gameObject));
            }
        }
    
    }

    [PunRPC]
    public void DisableProjectileMesh(ProjectileMovement otherProjectile)
    {
        if (photonView.IsMine)
        {

            otherProjectile.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }


    [PunRPC]
    public void StrikeFunc()
    {
        StartCoroutine(Strike());
    }

    [PunRPC]
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ProjectileTag))
        {
            ProjectileMovement otherProjectile = other.GetComponent<ProjectileMovement>();
            Debug.Log("player Got hurt!");

            if (otherProjectile.photonView.Owner.ActorNumber == photonView.Owner.ActorNumber)
                return;

            if (otherProjectile.photonView.IsMine)
            {
                photonView.RPC(RecievedamageRPC, RpcTarget.All, 10);

                photonView.RPC(nameof(DisableProjectileMesh), RpcTarget.All, otherProjectile);
                
                //run login that affect other players! only the projectile owner should do that
                StartCoroutine(DestroyDelay(1f, otherProjectile.gameObject));
               
                
                
            }
            

            otherProjectile.visualPanel.SetActive(false);
            //add bool for projectile hit
        }
        if (other.CompareTag("Strike"))
        {
            if (photonView.IsMine)
            {
                Debug.Log("A Player got knocked away!");
                PlayerController strikingActor = other.GetComponentInParent<PlayerController>();
                photonView.RPC(nameof(GetKnockedBack), RpcTarget.All, other.transform.localPosition, strikingActor.damage);
            }
        }
            if (other.CompareTag(BoostBoxTag))
        {
            playerRB.AddForce(Vector3.up * 20, ForceMode.Impulse);
        }
        if (other.CompareTag(DamageBoxTag))
        {
            photonView.RPC(RecievedamageRPC, RpcTarget.All, 10);
        }
        if(other.CompareTag("HealthPickup"))
        {
            knockbackPrecentage -= 2;
        }
        if (other.CompareTag("SpeedPickUp"))
        {
            Debug.LogWarning("I AM SPEED");
          //  StartCoroutine(DestroyDelay(0.2f, other.gameObject));
            StartCoroutine(GetSpeedBoost());
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

    IEnumerator GetSpeedBoost()
    {
        speed += 5;
        yield return new WaitForSeconds(5f);
        speed -= 5;
    }

    public void Shoot()
    {
        Debug.Log("fire in the hole");
        GameObject projectile = PhotonNetwork.Instantiate(ProjectilePrefabName,
            projectileSpawnTransform.position, projectileSpawnTransform.rotation, 0,
            new object[] {});
    }

    IEnumerator Strike()
    {

        playerAnimator.SetBool("IsStriking", true);
        strikeZone.SetActive(true);

        float CRAL = playerAnimator.GetCurrentAnimatorClipInfo(0).Length;
        Math.Clamp(CRAL,0.26f,CRAL);
        yield return new WaitForSeconds(CRAL-animationOffset);

        strikeZone.SetActive(false);
        playerAnimator.SetBool("IsStriking", false);
    }

    
}