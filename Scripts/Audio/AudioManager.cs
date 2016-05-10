using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

    void Awake()
    {
        
        if (instance == null)
            instance = this;
        if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
