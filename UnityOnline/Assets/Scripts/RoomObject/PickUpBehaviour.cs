using UnityEngine;
using Photon.Pun;

public class PickUpBehaviour : MonoBehaviourPun
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.localScale = new Vector3(1,1,1);
            transform.eulerAngles = new Vector3(0, 0, 0);
            ChampSelectManger.Instance.currentPickupTransform = transform;
            ChampSelectManger.Instance._pickupCollectedEventScript.pickupedEvent.Invoke();
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }

    }
    
}
