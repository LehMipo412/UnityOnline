using System;
using System.Net.Mime;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    private const string ProjectilePrefabName = "Prefabs\\Projectile";
    [SerializeField] private Transform projectileSpawnTransform;
    [SerializeField] private float speed = 10;
    [SerializeField] private PhotonView _photonView;
    private Vector3 raycastPos;
    private Camera cachedCamera;
    private int HP;

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
        HP -= 10;
        if(HP <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }    
    public void Shoot()
    {
        GameObject projectile = PhotonNetwork.Instantiate(ProjectilePrefabName,
            projectileSpawnTransform.position, projectileSpawnTransform.rotation, 0,
            new object[] {});
    }
}