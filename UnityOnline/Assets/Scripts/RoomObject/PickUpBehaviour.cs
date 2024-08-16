using UnityEngine;
using Photon.Pun;


public class PickUpBehaviour : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.localScale = new Vector3(1,1,1);
            transform.eulerAngles = new Vector3(0, 0, 0);
            Debug.LogWarning(transform.localScale);
            ChampSelectManger.Instance.currentPickupTransform = transform;
            ChampSelectManger.Instance._pickupCollectedEventScript.pickupedEvent.Invoke();



            PhotonNetwork.Destroy(gameObject);
        }

    }
    
}
