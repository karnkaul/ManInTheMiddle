using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Definitions;

public class ControlsManager : MonoBehaviour
{
    public Button[] buttons;

    public void Left()
    {
        GameManager.Instance.PlayerChose(Choice.Left);
    }

    public void Right()
    {
        GameManager.Instance.PlayerChose(Choice.Right);
    }

    public void Centre()
    {

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
