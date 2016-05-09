using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    [Header("Enqueue")]
    //public bool autoSwap = false;
    public AudioClip enqueue;
    [Range(0, 5)]
    public float crossfadeTime = 1;

    //void OnLevelWasLoaded(int level)
    //{
    //    if (autoSwap)
    //    {
    //        Debug.Log("autoswap:" + autoSwap);
    //        Swap();
    //    }
    //}

    public void Swap()
    {
        AudioSource prev = GetComponent<AudioSource>();
        AudioSource curr = gameObject.AddComponent<AudioSource>();

        curr.playOnAwake = true;
        curr.loop = true;
        curr.volume = 0;
        curr.outputAudioMixerGroup = prev.outputAudioMixerGroup;

        StartCoroutine(_Swap(prev, curr));
    }

    IEnumerator _Swap(AudioSource prev, AudioSource curr)
    {
        curr.clip = enqueue;
        curr.Play();
        float increment = 1 / crossfadeTime;

        while (curr.volume < 1)
        {
            float multiplier = Time.deltaTime * increment;
            curr.volume += multiplier;
            prev.volume -= (prev.volume <= 0) ? 0 : multiplier;
            yield return null;
        }
        curr.volume = 1;
        prev.volume = 0;
        Destroy(prev);
    }
}
