using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public void Caller()
    {
        Debug.Log($"Enemy Hit!!!. Enemy id:{gameObject.GetInstanceID()}");
    }
}
