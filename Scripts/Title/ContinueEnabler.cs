using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Definitions;

public class ContinueEnabler : MonoBehaviour
{
    public bool checkPointPresent = false;

	void Awake ()
    {
        Checkpoint checkpoint = Persistor.Load();
        if (checkpoint != null)
            checkPointPresent = true;
        if (checkPointPresent)
            GetComponent<Button>().interactable = true;
    }
}
