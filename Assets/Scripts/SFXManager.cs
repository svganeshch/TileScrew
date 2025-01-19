using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public AudioClip tilePickSound;
    public AudioClip tileIceCrackSound;
    public AudioClip tileBlockedSound;
    public AudioClip screwsMatchedSound;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayTilePickSound()
    {
        audioSource.PlayOneShot(tilePickSound);
    }

    public void PlayTileIceCrackSound()
    {
        audioSource.PlayOneShot(tileIceCrackSound);
    }

    public void PlayTileBlockedSound()
    {
        audioSource.PlayOneShot(tileBlockedSound);
    }

    public void PlayScrewsMatchedSound()
    {
        audioSource.PlayOneShot(screwsMatchedSound);
    }
}
