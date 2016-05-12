using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scroller : MonoBehaviour
{
    [TextArea(4, 10)]
    public string content_text;
    [Range(0.1f, 2.0f)]
    public float doubleTapTimeout = 0.5f;

    [SerializeField]
    private int charsPerFrame = 1, speedFactor = 10;
    public Text text_canvas;

    private int cpf, remaining;
    public int Remaining { get { return remaining; } }
    private bool completed = false;
    public bool Completed { get { return completed; } }

    private bool doubleTapCounting = false;
    private float doubleTapElapsed = 0;

    [Header("Debug")]
    public int charsPrinted = 0;

	public void FlushText ()
    {
        if (!text_canvas)
            text_canvas = GetComponent<Text>();

        text_canvas.text = "";
        cpf = charsPerFrame;

        StartCoroutine(Flush());
    }

    // Flush content_text to canvas
	IEnumerator Flush ()
    {
        int previous_cpf = 0;
        remaining = 0;

        if (content_text.Length == 0)
            yield return new WaitForSeconds(0.1f);

        for (int done=0; done < content_text.Length; done += cpf)
        {
            // Rewind pointer if cpf changed at increment
            if (previous_cpf != 0 && previous_cpf != cpf)
                done -= (cpf - previous_cpf);

            remaining = content_text.Length - done;
            if (remaining < 0)
                break;

            string append = content_text.Substring(done, (remaining > cpf) ? cpf: remaining);
            text_canvas.text += append;

            previous_cpf = cpf;

            charsPrinted = done;
            yield return null;
        }
        completed = true;

	}

    void Update()
    {
        InputHandler();
        DoubleTapHandler();
    }

    void InputHandler ()
    {

#if UNITY_ANDROID
        if (Input.touchCount > 0 )
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (doubleTapElapsed == 0)
                    doubleTapCounting = true;
                else
                    ChangeSpeed(true);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                ChangeSpeed(false);
        }
#endif
#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
        {
            if (doubleTapElapsed == 0)
                doubleTapCounting = true;
            else
                ChangeSpeed(true);
        }
        if (Input.GetMouseButtonUp(0))
            ChangeSpeed(false);

#endif
    }

    void DoubleTapHandler()
    {
        if (doubleTapCounting)
            doubleTapElapsed += Time.deltaTime;
        if (doubleTapElapsed >= doubleTapTimeout)
        {
            doubleTapElapsed = 0;
            doubleTapCounting = false;
        }
    }

    void ChangeSpeed(bool speedUp)
    {
        cpf = speedUp ? charsPerFrame * speedFactor : charsPerFrame;
    }
}
