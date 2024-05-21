using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class LevelSwitchTrigger : MonoBehaviour
{
    [SerializeField] private string levelName;

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(levelName);
    }
}
