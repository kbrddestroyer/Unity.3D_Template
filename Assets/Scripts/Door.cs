using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float speed;
    [SerializeField, Range(0f, 10f)] private float interactDistance;
    [SerializeField] private Color gizmoColor;

    private bool bIsOpen = false;
    private Vector3 targetRotation = Vector3.zero;
    private static Player player = null;

    private void Awake()
    {
       if (player == null)
           player = FindObjectOfType<Player>();
    }

    public void ToggleDoor()
    {
        bIsOpen = !bIsOpen;

        targetRotation = new Vector3(0, (bIsOpen) ? 90 : 0, 0);
        Debug.Log(targetRotation);
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }
        transform.localRotation = Quaternion.Euler(Vector3.Lerp(transform.localRotation.eulerAngles, targetRotation, speed * Time.deltaTime));
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
#endif
}
