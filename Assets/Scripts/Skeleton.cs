using System.Collections;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Skeleton : MonoBehaviour
{

    private Rigidbody[] _radollRigidbodies;
    private Collider[] _radollColliders;
    private Animator _animator;
    // private Controller _controller;
    private BoxCollider _boxCollider;
    private Rigidbody _rigidbody;

    void Awake()
    {
        _radollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _radollColliders = GetComponentsInChildren<Collider>();
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider>();
        _rigidbody = GetComponent<Rigidbody>();

        DisableRagdoll();
        
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     Debug.Log("Space key pressed. Enabling ragdoll.");
        //     EnableRagdoll();
        // }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // if (collision.gameObject.CompareTag("Weapons"))
        // {
        //     EnableRagdoll();
        // }
    }

    public void DisableRagdoll()
    {
        foreach (var rb in _radollRigidbodies)
        {
            rb.isKinematic = true;
        }

        foreach (var col in _radollColliders)
        {
            col.enabled = false;
        }

        _animator.enabled = true;
        _boxCollider.enabled = true;
        _rigidbody.isKinematic = false;

    }

    public void EnableRagdoll()
    {
        foreach (var rb in _radollRigidbodies)
        {
            rb.isKinematic = false;
        }

        foreach (var col in _radollColliders)
        {
            col.enabled = true;
        }

        _animator.enabled = false;
        _boxCollider.enabled = false;
        _rigidbody.isKinematic = true;
    }


}
