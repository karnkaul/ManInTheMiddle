using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Definitions;

public class ControlsManager : MonoBehaviour
{
    public Button[] buttons;
    public AudioClip[] buttonClips;
    public AudioClip pauseClip;

    private AudioSource self;

    float PlaySFX(bool pause = false)
    {
        float delay = 0;
        if (!self)
            self = GetComponent<AudioSource>();
        if (self)
        {
            AudioClip toPlay = buttonClips[Random.Range(0, buttonClips.Length)];
            if (pause)
                toPlay = pauseClip;
            self.PlayOneShot(toPlay);
            delay = toPlay.length;
        }
        return delay;
    }

    IEnumerator PlaySFXAndCallGM(Choice choice, bool pause=false)
    {
        yield return new WaitForSeconds(PlaySFX());
        GameManager.Instance.PlayerChose(choice);
    }

    public void Left()
    {
        StartCoroutine(PlaySFXAndCallGM(Choice.Left));
        //GameManager.Instance.PlayerChose(Choice.Left);
    }

    

    public void Right()
    {
        StartCoroutine(PlaySFXAndCallGM(Choice.Right));
        //GameManager.Instance.PlayerChose(Choice.Right);
    }

    public void Centre()
    {
        if (!self)
            self = GetComponent<AudioSource>();
        if (self)
            self.PlayOneShot(pauseClip);
        GameManager.Instance.Centre();
    }

    public void Toggle(bool enable)
    {
        //Button[] buttons = GetComponentsInChildren<Button>();
        //Image[] images = GetComponentsInChildren<Image>();
        foreach (Button button in buttons)
        {
            button.interactable = enable;
            Image image = button.GetComponent<Image>();
            Color color = image.color;
            image.color = new Color(color.r, color.g, color.b, enable ? 1 : 0);
        }
    }
}
