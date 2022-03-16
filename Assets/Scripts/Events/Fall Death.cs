using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckBoundaries : MonoBehaviour
{
    public GameObject thePlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == thePlayer.GetComponent<Collider>())
        {
            SceneManager.LoadScene(0);
            print("You lose");
        }
    }
}
