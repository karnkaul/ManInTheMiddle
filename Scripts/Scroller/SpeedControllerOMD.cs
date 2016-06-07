using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class SpeedControllerOMD : MonoBehaviour
{
#if UNITY_STANDALONE || UNITY_EDITOR
    public void OnMouseDown()
    {
        if (!GameManager.Instance.IsPaused)
            Time.timeScale = GameManager.Instance.TimeScaler;
    }

    public void OnMouseUp()
    {
        if (!GameManager.Instance.IsPaused)
            Time.timeScale = 1;
    }
#endif

#if UNITY_ANDROID

    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            int pointerID = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(pointerID))
            {
                if (!GameManager.Instance.IsPaused)
                    Time.timeScale = GameManager.Instance.TimeScaler;
                // at least on touch is over a canvas UI
                return;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if (!GameManager.Instance.IsPaused)
                    Time.timeScale = 1;
                // here we don't know if the touch was over an canvas UI
                return;
            }
        }
    }
#endif
}
