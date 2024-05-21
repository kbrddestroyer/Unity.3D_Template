using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIDialogueController : MonoBehaviour
{
    private static GUIDialogueController instance;
    public static GUIDialogueController Instance { get => instance; }

    [SerializeField] private TMP_Text text;

    public TMP_Text Text { get => text; }

    private void Awake()
    {
        instance = this; 
    }
}
