using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private float rotateY = 0;
    // Update is called once per frame
    void Update()
    {
        rotateY += 5;
        transform.rotation = Quaternion.Euler(50, rotateY, 0);

    }
}
