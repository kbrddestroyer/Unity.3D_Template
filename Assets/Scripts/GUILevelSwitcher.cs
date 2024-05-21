using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUILevelSwitcher : MonoBehaviour
{
    [SerializeField] private string levelName;

    public void OnClick()
    {
        SceneManager.LoadScene(levelName);
    }
}
