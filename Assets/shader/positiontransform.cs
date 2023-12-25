using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class test : MonoBehaviour
{
     public string Name01 = "PPP";
     public string Name02 = "SSS";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        Vector3 scl = transform.localScale;
        //float distance = Vector3.Distance(sphere1Position, sphere2Position);
        Shader.SetGlobalVector(Name01, pos);
        Shader.SetGlobalVector(Name02, scl);
        
    }
}
