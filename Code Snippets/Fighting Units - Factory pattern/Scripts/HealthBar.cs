using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Transform bar;

    // Start is called before the first frame update
    void Start()
    {
        bar = transform.Find("Bar");
    }

    public void SetHealth(float size)
    {
        if (size >= 0F && size <= 1F)
            bar.localScale = new Vector3(size, 1F);
        else if (size > 1F)
        {
            bar.localScale = new Vector3(1F, 1F);
        }
        else
        {
            bar.localScale = new Vector3(0F, 1F);
        }
    }
}
