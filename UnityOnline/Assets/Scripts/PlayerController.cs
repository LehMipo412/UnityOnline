using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;


public class PlayerController : MonoBehaviourPun
{
    [Header("Const Strings")]
    private const string ProjectileTag = "Projectile";
    private const string BoostBoxTag = "BoostBox";
    private const string DamageBoxTag = "DamageBox";
    private const string RecievedamageRPC = "RecieveDamage";

    [Header("Strings")]
    private string ProjectilePrefabName = "Prefabs\\Projectile";

    [Header("Damage And Movement")]
    [SerializeField] private Transform projectileSpawnTransform;
    [SerializeField] public float damage;
    [SerializeField] public float knockbackPrecentage;
    [SerializeField] private float speed = 10;
    [SerializeField] private float jumpModifier;
    [SerializeField] private bool isOnLAva = false;
    [SerializeField] private bool canJump = true;
    [SerializeField] private float lavatimer = 1f;

    [Header("Network And Physics")]
    [SerializeField] public Rigidbody playerRB;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private ChampSelectManger _champSelectManger;
    [SerializeField] private GameObject strikeZone;
    [SerializeField] private float animationOffset;

    [Header("Camera Helpers")]
    public Transform neckIndicator;

    [Header("AIChange")]
    public bool isSupposedToBeControlledByAI = false;
    private float AIChangeTimerTimer = 5f;
    private float AIRandomDiraction;
    

    [Header("HP And Score")]
    public float maxHP = 200;
    public float HP = 200;
    public int score = 0;
    public EnemyHPbar playerHpBar;
    public EnemyHPbar SelfHPBar;

    private void Start()
    {
        if (photonView.IsMine)
        {
            playerHpBar.HpCanvas.SetActive(false);
            playerHpBar = SelfHPBar;
        }
        else
        {
            SelfHPBar.HpCanvas.SetActive(false);
        }
       
        
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("Begginer"))
        {
            damage = 1.5f;
        }

        _champSelectManger = ChampSelectManger.Instance;

        switch (playerRB.mass)
        {
            case 2f:
                ProjectilePrefabName = "Prefabs\\KunaiPrefab";
                jumpModifier = 4f;
                break;
            case 1f:
                ProjectilePrefabName = "Prefabs\\NewArrowPrefab";
                jumpModifier = 2f;
                break;
            case 1.2f:
                ProjectilePrefabName = "Prefabs\\FistPrefab";
                jumpModifier = 2.4f;
                break;
            case 1.5f:
                ProjectilePrefabName = "Prefabs\\SlashPrefab";
                jumpModifier = 3f;
                break;
            case 1.7f:
                ProjectilePrefabName = "Prefabs\\PencilProjectile";
                jumpModifier = 3.4f;
                break;

        }

        AIRandomDiraction = Random.Range(0, 4);
    }

    [PunRPC]
    public void GetKnockedBack(Vector3 hitDitraction,float additionalKBPrec)
    {
        playerRB.AddForce(hitDitraction * (3 * knockbackPrecentage*0.8f) * -1*2*1.2f, ForceMode.Impulse);
        knockbackPrecentage += additionalKBPrec;
        playerRB.AddForce(Vector3.up * (1 * knockbackPrecentage/3) * 2, ForceMode.Impulse);
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (!ChampSelectManger.isPaused)
            {
                if (!ChatManagerScript.IsChatting)
                {
                   
                    if (!_photonView.IsMine)
                        return;
                    if (!isSupposedToBeControlledByAI)
                    { 
                        if (Input.GetKey(KeyCode.W))
                        {
                            playerAnimator.SetBool("IsRunning", true);
                            
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
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            if (canJump)
                            {
                                Jump();
                            }
                        }

                        if (!Input.anyKey)
                    {

                        playerAnimator.SetBool("IsRunning", false);
                        
                    }
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
                                
                                transform.Translate(Vector3.forward * (Time.deltaTime * speed));
                            }
                            if (AIRandomDiraction == 1)
                            {
                                playerAnimator.SetBool("IsRunning", true);
                                
                                transform.Translate(Vector3.left * (Time.deltaTime * speed));
                            }
                            if (AIRandomDiraction == 2)
                            {
                                playerAnimator.SetBool("IsRunning", true);
                               
                                transform.Translate(Vector3.back * (Time.deltaTime * speed));
                            }
                            if (AIRandomDiraction == 3)
                            {
                                playerAnimator.SetBool("IsRunning", true);
                               
                                transform.Translate(Vector3.right * (Time.deltaTime * speed));
                            }
                        }
                    }
                    if(isOnLAva)
                    {
                        lavatimer -= Time.deltaTime; 

                        if(lavatimer <= 0)
                        {
                            photonView.RPC(RecievedamageRPC, RpcTarget.All, 10, PhotonNetwork.LocalPlayer.NickName);
                            lavatimer = 1f;
                        }
                    }
                }
            }
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Lava")) ;
        {
            isOnLAva = false;
        }
    }

    public void Jump()
    {
        playerRB.AddForce(Vector3.up * jumpModifier *4, ForceMode.Impulse  ) ;
        canJump = false;
        Debug.LogWarning("JUMP!");
    }

    public void StrikeEnvelope()
    {
        Debug.Log("Strike!");
        photonView.RPC(nameof(StrikeFunc), RpcTarget.All);
    }
    [PunRPC]
    public void TakeDamage(string hitterName)
    {
        if(HP <= 0)
        {
            
            if (photonView.IsMine)
            {
               
                _champSelectManger.photonView.RPC(nameof(_champSelectManger.RemoveLivingPkayer), RpcTarget.All);
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if(player.NickName == hitterName)
                    {
                        if(hitterName == PhotonNetwork.LocalPlayer.NickName)
                        {
                            if ((string)player.CustomProperties["Kills"] == null)
                            {
                                int currentkils = -1;
                                player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Kills", currentkils.ToString() } });
                                Debug.LogWarning("The player got null as the property kills");
                            }
                            else
                            {

                                int currentkils = int.Parse((string)player.CustomProperties["Kills"]);
                                currentkils--;
                                player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Kills", currentkils.ToString() } });
                                Debug.LogWarning("Hey hey! you got a kill! current kills:" + currentkils);
                            }
                        }
                        Debug.LogWarning( "The Player Who Hitted You: "+hitterName);
                        Debug.LogWarning("The killing player current score: " + (string)player.CustomProperties["Kills"]);
                        if ((string)player.CustomProperties["Kills"] == null)
                        {
                           int currentkils = -1;
                            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Kills", currentkils.ToString() } });
                            Debug.LogWarning("The player got null as the property kills");
                        }
                        else
                        {
                            
                           
                           int  currentkils = int.Parse((string)player.CustomProperties["Kills"]);
                            currentkils++;
                            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Kills", currentkils.ToString() } });
                            Debug.LogWarning("Hey hey! you got a kill! current kills:" + currentkils);
                        }
                       
                    }
                }
                
                Debug.Log("Players Remaining: " + _champSelectManger.livingPlayersCounter);
                StartCoroutine(DestroyDelay(0.2f, gameObject));
            }
            else
            {
                SelfHPBar.gameObject.SetActive(false);
            }
        }
        else
        {
            if (HP >0)
            {
                playerHpBar.ChangeHPbarPercent(HP);
            }
        }
    
    }

    [PunRPC]
    //this is not working, annoying :(
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
            isOnLAva = true;
            Debug.LogWarning("The Floor Is Lava!");
            canJump = true;
        }
        if (collision.gameObject.CompareTag("Floor"))
        {
            canJump = true;
        }
    }

    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Lava")) ;
        {
            isOnLAva = false;
        }
    }

    [PunRPC]
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ProjectileTag))
        {
            ProjectileMovement otherProjectile = other.gameObject.GetComponent<ProjectileMovement>();
            Debug.Log("player Got hurt!");

            if (otherProjectile.photonView.Owner.ActorNumber == photonView.Owner.ActorNumber)
                return;

            if (otherProjectile.photonView.IsMine)
            {
                photonView.RPC(RecievedamageRPC, RpcTarget.All, 10, otherProjectile.gameObject.GetPhotonView().Owner.NickName);
                Debug.LogWarning("hitting player: " + otherProjectile.gameObject.GetPhotonView().Owner.NickName);

               
                StartCoroutine(DestroyDelay(1f, otherProjectile.gameObject));
               
                
                
            }
            

            otherProjectile.visualPanel.SetActive(false);
            
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
    private void RecieveDamage(int damageAmount, string hitterNickName)
    {
        HP -= damageAmount;
        Debug.Log("Hp left is " + HP);
        TakeDamage(hitterNickName);
     
     
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

    [PunRPC]
    public void SwitchFromPlayerToAI(int leftplayerID, PhotonMessageInfo info)
    {
        Debug.LogWarning("leftplayerID Player Id is: " + leftplayerID);
        Debug.LogWarning("creator ID: " + photonView.CreatorActorNr);
        if (leftplayerID == photonView.CreatorActorNr)
        {
            Debug.Log("Meep Morp, ZEET!");
            isSupposedToBeControlledByAI = true;
        }
    }
}