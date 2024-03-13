using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollActivator : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;

    public void Awake()
    {
        rb.isKinematic = true;
        col.enabled = false;
    }

    public void Activate()
    {
        rb.isKinematic = false;
        col.enabled = true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
#endif
}
