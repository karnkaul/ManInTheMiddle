using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpeedControllerOMD : MonoBehaviour
{
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
}
