using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Definitions;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    [Header("Previous Page")]
    [Tooltip("Will only work if Load Checkpoint is false")]
    private Choice previousChoice = Choice.None;

    [SerializeField]
    [Tooltip("Deselect to control previous page's choices via above")]
    private bool loadCheckpoint = false;
    public bool LoadCheckpoint { get { return loadCheckpoint; } }

    [Header("Controls")]
    [SerializeField]
    [Tooltip("If checked, interface will enable controls once flushing text is completed")]
    private bool interfaceControl = true;
    public bool InterfaceControl { get { return interfaceControl; } }
    [SerializeField]
    private float timeScaler = 3;
    public float TimeScaler { get { return timeScaler; } }

    [SerializeField]
    [Tooltip("Timeout to force-enable controls")]
    private int autoEnable = 10;
    public int Autoenable { get { return autoEnable; } }

    [Header("Debug")]
    [SerializeField]
    private DebugLevel debugLevel = 0;
    public DebugLevel DebugLevel { get { return debugLevel; } }

    //[SerializeField]
    //private bool resetGameState = false;

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public static event Definitions.Toggle Pause;
    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } }

    public static PageManager currentPM;

    private bool controlsEnabled = false, buffer;

    void OnEnable()
    {
        Pause += HandlePause;
    }

    void OnDisable()
    {
        Pause -= HandlePause;
    }

    void Awake()
    {
        // First run
        if (instance == null)
        {
            // Ready previous page for first PageManager.Start() 
            if (!LoadCheckpoint)
            {
                Page zero;
                zero.number = 0;
                zero.playerChoice = previousChoice;

                GameState.previousPage = zero;
                GameState.PushBackPreviousPage();
            }
            // Make singleton
            instance = this;

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
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

        StartCoroutine(PMPreload());
    }

    void OnLevelWasLoaded(int level)
    {
        StartCoroutine(PMPreload());
    }

    IEnumerator PMPreload()
    {
        if (debugLevel >= DebugLevel.Notify)
            Debug.Log("GM requesting PM to start preloading in 0.5s");
        yield return new WaitForSeconds(0.5f);
        currentPM.StartPreloading();
    }
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Exit();
        //if (resetGameState)
        //{
        //    GameState.Reset();
        //    resetGameState = false;
        //}

	}

    public void Exit()
    {
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

    }

    void HandlePause(bool toggle)
    {
        if (toggle)
        {
            buffer = controlsEnabled;
            ToggleControls(false);
        }
        else
            ToggleControls(buffer);
        
        Time.timeScale = toggle ? 0 : 1;
        isPaused = toggle;
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
        //ObliterateSaves();
        Pause(!isPaused);
    }

    public void ObliterateSaves()
    {
        Persistor.Clear();
        SceneManager.LoadScene(0);
        Pause(false);
    }
}
