using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DialogueController : MonoBehaviour
{
    [SerializeField] private string[] dialogue;

    private int playing = -1;
    private bool canPlay = false;

    private int Playing
    {
        get => playing;
        set
        {
            playing = value;
            if (playing >= 0)
            {
                if (playing < dialogue.Length)
                    PlayDialogue(playing);
            }
            else
            {
                StopPlaying();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            canPlay = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canPlay = false;
            Playing = -1;
        }
    }

    private void PlayDialogue(int index)
    {
        GUIDialogueController.Instance.Text.text = dialogue[index];
    }

    private void StopPlaying()
    {
        GUIDialogueController.Instance.Text.text = "";
    }

    private void Update()
    {
        if (canPlay)
            if ((playing >= 0 && Input.GetKeyDown(KeyCode.Space)) || (playing == -1 && Input.GetKeyDown(KeyCode.E)))
                if (playing < dialogue.Length) 
                    Playing++;
    }
}
