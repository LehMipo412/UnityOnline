using UnityEngine;

public class PlayButtonSound : MonoBehaviour
{
    [SerializeField] AudioSource AuS;
    [SerializeField] AudioClip Auc;
    public void PlaySound()
    {   
      if(!AuS.isPlaying) AuS.PlayOneShot(Auc);
    }
}
