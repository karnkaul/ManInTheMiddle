using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialFloat : MonoBehaviour
{
    [SerializeField]
    private float speed = 10, idleWait=7, animateFor=2;

    public enum AnimState { Idle, Floating };
    [SerializeField][Header("Status")]
    private AnimState state;

    private float elapsed;

    //void OnEnable()
    //{
    //    GameManager.Pause += ChangeState;
    //}

    //void OnDisable()
    //{
    //    GameManager.Pause -= ChangeState;
    //}

    void Update()
    {
        HandleStates();
    }

    void HandleStates()
    {
        if (elapsed >= idleWait && elapsed < (idleWait + animateFor))
        {
            Animate();
            elapsed += Time.unscaledDeltaTime;
        }
        else if (elapsed > (idleWait + animateFor))
        {
            if (Mathf.Abs(transform.localScale.x - 1) + Mathf.Abs(transform.localScale.y - 1) <= 0.06f)
            {
                transform.localScale = Vector3.one;
                elapsed = 0;
            }
            else
                Animate();
        }
        else
            elapsed += Time.unscaledDeltaTime;
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
