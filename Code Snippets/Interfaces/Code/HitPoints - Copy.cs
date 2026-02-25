using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDie))]
public class HitPoints : MonoBehaviour, IDamagable
{
    [SerializeField]
    public float Health;
    /*{
        get { return Health; }
        internal set { Health = value; }
    }*/


    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health < 0)
            GetComponent<IDie>().Die();
    }
}
