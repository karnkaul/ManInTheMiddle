using UnityEngine;
using UnityEngine.SceneManagement;
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

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public static PageManager currentPM;

    private bool controlsEnabled = false;

    void Awake()
    {
        // First run
        if (instance == null)
        {
            // Not required any more, it is now a seamless loop using default constructor and GameState.Reset()
            // Ready previous page for first PageManager.Start() 
            //Page zero;
            //zero.number = 0;
            //zero.playerChoice = previousChoice;

            //GameState.previousPage = zero;
            //GameState.PushBackPreviousPage();
            
            // Make singleton
            instance = this;
        }
        if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        
    }

	void Start ()
    {
        
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
	}

    public bool ToggleControls(bool enable)
    {
        //string doing = enable ? "Enabling" : "Disabling";
        //Debug.Log(doing + " controls.");
        controlsEnabled = enable;
        FindObjectOfType<ControlsManager>().Toggle(enable);
        return controlsEnabled;
    }

    public void PlayerChose(Choice playerChoice)
    {
        Debug.Log("Player choice: " + playerChoice);
        if (currentPM)
            currentPM.LoadNext(playerChoice);
    }
}
