using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip unrelatedSound;
    private AudioSource audioScource;

    // Start is called before the first frame update
    void Start()
    {
        //correctSound = Resources.Load<AudioClip>("correct1");
        //incorrectSound = Resources.Load<AudioClip>("incorrect1");
        audioScource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCorrectSound()
    {
        audioScource.PlayOneShot(correctSound);
    }

    public void PlayIncorrectSound()
    {
        audioScource.PlayOneShot(incorrectSound);
    }
    public void PlayUnrelatedSound()
    {
        audioScource.PlayOneShot(unrelatedSound);
    }
}
