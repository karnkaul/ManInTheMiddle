using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuFloat : MonoBehaviour
{
    [SerializeField]
    private float speed = 10;

    public enum AnimState { Idle, Floating };
    [SerializeField][Header("Status")]
    private AnimState state;

    void OnEnable()
    {
        GameManager.Pause += ChangeState;
    }

    void OnDisable()
    {
        GameManager.Pause -= ChangeState;
    }

    void Update()
    {
        HandleStates();
    }

    void HandleStates()
    {
        switch(state)
        {
            case AnimState.Floating:
                Animate();
                break;
            default:
                break;
        }
    }

    void Animate()
    {
        float x = 1 + 0.05f * Mathf.Sin(Time.unscaledTime * speed);
        float y = 1 + 0.05f * Mathf.Cos(Time.unscaledTime * speed);
        transform.localScale = new Vector3(x, y, 1);
    }

    void Freeze()
    {
        transform.localScale = Vector3.one;
    }

    public void ChangeState(bool setFloating)
    {
        state = setFloating ? AnimState.Floating : AnimState.Idle;
    }

    public void SetIdle() { state = AnimState.Idle; }
    public void SetFloating() { state = AnimState.Floating; }

}
