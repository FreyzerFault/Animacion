using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPosition : MonoBehaviour
{
    public GameObject thePlayer;

    // Start is called before the first frame update
    void Start()
    {
        thePlayer.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
