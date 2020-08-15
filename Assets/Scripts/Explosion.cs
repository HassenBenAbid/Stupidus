using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionTimer = 0.6f;

    private void OnEnable()
    {
        Destroy(this.gameObject, explosionTimer);
    }
}
