using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerTutorial : MonoBehaviour
{
    private bool firstDialogue = false;
    private bool secondDialogue = false;

    public void FinishFirstDialogue()
    {
        firstDialogue = true;
    }
}
