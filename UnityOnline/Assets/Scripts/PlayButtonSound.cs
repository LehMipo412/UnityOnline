using UnityEngine;
using UnityEngine.Audio;

public class PlayButtonSound : MonoBehaviour
{
    [SerializeField] AudioSource AuS;
    [SerializeField] AudioClip Auc;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound()
    {   if(!AuS.isPlaying)
        AuS.PlayOneShot(Auc);
    }
}
