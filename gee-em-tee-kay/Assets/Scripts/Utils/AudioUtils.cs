using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioUtils 
{

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime) 
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) 
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }

        audioSource.Stop();
    }
    
    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime, float maxVolume = 1) 
    {
        audioSource.Play();
        audioSource.volume = 0f;

        while (audioSource.volume < maxVolume) 
        {
            audioSource.volume += Time.deltaTime / FadeTime;
            yield return null;
        }

        audioSource.volume = maxVolume;
    }

}