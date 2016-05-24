using UnityEngine;
using System.Collections;

public class SpeedController : MonoBehaviour
{
    private bool doubleTapCounting = false;
    private float doubleTapElapsed = 0;

    public enum Type { Single, Double };
    public Type tapType;

    [Range(0.1f, 2.0f)]
    public float doubleTapTimeout = 0.5f;

    void Update()
    {
        InputHandler();
        DoubleTapHandler();
    }

    void InputHandler()
    {

#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (tapType == Type.Double && doubleTapElapsed == 0)
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
            if (tapType == Type.Double && doubleTapElapsed == 0)
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
        Time.timeScale = speedUp ? GameManager.Instance.TimeScaler : 1;
    }
}
