using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField]
    private CanvasGroup pauseFader;

    [SerializeField]
    private AudioMixerSnapshot mute, unmute;
    [SerializeField]
    private Text muteText;

    private static bool isEnabled = false, isMuted = false;
    public bool IsEnabled { get { return isEnabled; } }
    public bool IsMuted { get { return isMuted; } }

    public AudioMixerSnapshot paused, unpaused;

    private AudioSource self;

    private Coroutine expand, contract;

    void Start()
    {
        if (!confirm)
            confirm = transform.Find("Confirm Canvas").GetComponent<Canvas>();
        self = GetComponent<AudioSource>();
        Return(false);

        if (isMuted)
            muteText.text = "Unmute";
        else
            muteText.text = "Mute";
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
        if (!isMuted)
        {
            if (muteText)
                muteText.text = "Unmute";
            mute.TransitionTo(0);
            isMuted = true;
        }
        else
        {
            if (muteText)
                muteText.text = "Mute";
            unmute.TransitionTo(0);
            isMuted = false;
        }
    }

    IEnumerator Unmute()
    {
        if (muteText)
            muteText.text = "Mute";
        Time.timeScale = 1;
        float x = 0;
        unmute.TransitionTo(2);
        unmute.TransitionTo(0.5f);
        while (x < 0.5f)
        {
            x += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 0;
        isMuted = false;
    }

    public void AttemptRestart()
    {
        if (self && alert)
            self.PlayOneShot(alert);
        confirm.enabled = true;
    }

    public void Return(bool playSound=true)
    {
        if(playSound && self && back)
            self.PlayOneShot(back);
        confirm.enabled = false;
    }

    public void Restart()
    {
        GameManager.Pause(false);
        GameManager.Instance.ObliterateSaves();
    }
	
    public void Exit()
    {
        StartCoroutine(_Exit());
    }

    IEnumerator _Exit()
    {
        GameManager.Pause(false);
        if (self && general)
        {
            self.PlayOneShot(general);
            yield return new WaitForSeconds(general.length);
        }
        yield return null;
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
            if(!isMuted)
                paused.TransitionTo(0);
        }
        else
        {
            if (expand != null)
                StopCoroutine(expand);
            contract = StartCoroutine(Contract());
            if (!isMuted)
                unpaused.TransitionTo(0);
        }
    }

    IEnumerator Expand()
    {
        RectTransform self = GetComponent<RectTransform>();
        self.localScale = Vector3.zero;
        float coefficient;

        while (self.localScale.x < 1.2f)
        {
            coefficient = Time.unscaledDeltaTime * speed;
            self.localScale += new Vector3(coefficient, coefficient, 1);
            if (pauseFader)
                pauseFader.alpha += 0.03f;
            yield return null; 
        }

        while (self.localScale.x > 1)
        {
            coefficient = Time.unscaledDeltaTime * speed;
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
            if (pauseFader)
                pauseFader.alpha -= 0.03f;
            yield return null;
        }
        if (pauseFader)
            pauseFader.alpha = 0;
        self.localScale = Vector3.zero;
    }
}
