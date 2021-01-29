using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Courutiner : MonoBehaviour
{
    static public bool getDestroyed = false;
    void Start()
    {
        
    }
    static public bool download = false;
    static public string link;

    // Update is called once per frame
    void Update()
    {
        if (download)
        {
            StartCoroutine(NetLoader.GetTexture(link));
            download = false;
        }
        else if (getDestroyed)
            Destroy(this);
    }
}
