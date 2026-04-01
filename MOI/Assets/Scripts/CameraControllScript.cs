using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllScript : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;
    private Vector3 active_offset = new Vector3(0,-0.2f,0);

    public Transform _target;

    private Vector3 vel = Vector3.zero;

    private void Start()
    {
        if (_target == null)
            _target = GameObject.FindGameObjectWithTag("Player").transform;
    }


    private void FixedUpdate()
    {
        Vector3 targetPos = _target.position + offset + active_offset;
        targetPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, damping);

    }

    private void Update()
    {
        active_offset.y = Input.GetAxis("Vertical") * 3.7f;
    }
}
