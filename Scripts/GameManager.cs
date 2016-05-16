using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using Definitions;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Choice previousChoice = Choice.None;

    [SerializeField]
    private int autoEnableTimeout = 10;
    public int AutoEnableTimeout { get { return autoEnableTimeout; } }

    [SerializeField]
    private DebugLevel debugLevel = 0;
    public DebugLevel DebugLevel { get { return debugLevel; } }

    [SerializeField]
    private bool loadCheckpoint = false;
    public bool LoadCheckpoint { get { return loadCheckpoint; } }

    [SerializeField]
    private bool resetGameState = false;

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public static PageManager currentPM;

    private bool controlsEnabled = false;

    void Awake()
    {
        // First run
        if (instance == null)
        {
            // Ready previous page for first PageManager.Start() 
            Page zero;
            zero.number = 0;
            zero.playerChoice = previousChoice;

            GameState.previousPage = zero;
            GameState.PushBackPreviousPage();
            
            // Make singleton
            instance = this;
        }
        if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);


    }

	void Start ()
    {
        // Load last checkpoint
        if (loadCheckpoint)
        {
            Checkpoint checkpoint = Persistor.Load();
            if (checkpoint != null)
                SceneManager.LoadScene(checkpoint.sceneName);
            else
                Debug.Log("Checkpoint file not found.");
        }
    }

    void OnLevelWasLoaded(int level)
    {
        StartCoroutine(PMPreload());
    }

    IEnumerator PMPreload()
    {
        yield return new WaitForSeconds(0.5f);
        currentPM.StartPreloading();
    }
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        if (resetGameState)
        {
            GameState.Reset();
            resetGameState = false;
        }
	}

    public bool ToggleControls(bool enable)
    {
        controlsEnabled = enable;
        FindObjectOfType<ControlsManager>().Toggle(enable);
        return controlsEnabled;
    }

    public void PlayerChose(Choice playerChoice)
    {
        if (debugLevel >= DebugLevel.Verbose)
            Debug.Log("Player choice: " + playerChoice);

        if (currentPM)
            currentPM.LoadNext(playerChoice);
    }

    public void Centre()
    {
        // Obliterate saves
        Persistor.Clear();
    }
}
