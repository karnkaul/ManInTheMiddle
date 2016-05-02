using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Triggered.");
    }
}
