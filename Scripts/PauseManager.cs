using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;

    [SerializeField]
    private Canvas confirm;

    [SerializeField]
    private AudioClip alert, back, general;

    private bool isEnabled = false;
    public bool IsEnabled { get { return isEnabled; } }

    public AudioMixerSnapshot paused, unpaused;

    private AudioSource self;

    private Coroutine expand, contract;

    void Start()
    {
        if (!confirm)
            confirm = transform.Find("Confirm Canvas").GetComponent<Canvas>();
        self = GetComponent<AudioSource>();
        Return(false);
    }

    void OnEnable()
    {
        GameManager.Pause += HandlePause;
    }

    void OnDisable()
    {
        GameManager.Pause -= HandlePause;
    }

    public void Mute()
    {
        //
    }

    public void AttemptRestart()
    {
        if (self && alert)
            self.PlayOneShot(alert);
        confirm.enabled = true;
    }

    public void Return(bool playSound=true)
    {
        if(playSound)
            self.PlayOneShot(back);
        confirm.enabled = false;
    }

    public void Restart()
    {
        GameManager.Instance.ObliterateSaves();
    }
	
    public void Exit()
    {
        GameManager.Pause(false);
        GameManager.Instance.LoadMainMenu();
    }

    public void Quit()
    {
        GameManager.Instance.Exit();
    }

    public void HandlePause(bool toggle)
    {
        

        if (toggle)
        {
            if (contract != null)
                StopCoroutine(contract);
            expand = StartCoroutine(Expand());
            paused.TransitionTo(0);
        }
        else
        {
            if (expand != null)
                StopCoroutine(expand);
            contract = StartCoroutine(Contract());
            unpaused.TransitionTo(0);
        }
        //GetComponent<RectTransform>().localScale = toggle ? Vector3.one : Vector3.zero;
        
    }

    IEnumerator Expand()
    {
        RectTransform self = GetComponent<RectTransform>();
        self.localScale = Vector3.zero;
        float coefficient = Time.unscaledDeltaTime * speed;

        while (self.localScale.x < 1.2f)
        {
            self.localScale += new Vector3(coefficient, coefficient, 1);
            yield return null; 
        }

        while (self.localScale.x > 1)
        {
            self.localScale -= new Vector3(coefficient, coefficient, 1);
            yield return null;
        }

        self.localScale = Vector3.one;
    }

    IEnumerator Contract()
    {
        RectTransform self = GetComponent<RectTransform>();
        float coefficient = Time.unscaledDeltaTime * speed * 4;
        while (self.localScale.x > 0)
        {
            self.localScale -= new Vector3(coefficient, coefficient, 1);
            yield return null;
        }
        self.localScale = Vector3.zero;
    }
}
