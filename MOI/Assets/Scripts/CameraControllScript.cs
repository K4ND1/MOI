using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllScript : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;

    public Transform _target;

    private Vector3 vel = Vector3.zero;

    private void Start()
    {
        if (_target == null)
            _target = GameObject.FindGameObjectWithTag("Player").transform;
    }


    private void FixedUpdate()
    {
        Vector3 targetPos = _target.position + offset;
        targetPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, damping);

    }
}
