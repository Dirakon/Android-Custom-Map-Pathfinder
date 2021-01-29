using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdderToEtaz : MonoBehaviour
{
    // Start is called before the first frame update
    public Text etaztxt;
    public Camera cam;
    public int addToEtaz;
    static Movement mov=null;
    
    void Start()
    {
        if (mov == null)
            mov = cam.GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnMouseDown()
    {
        mov.etaz += addToEtaz;
        if (mov.etaz > mov.lastfloor || mov.etaz < mov.firstfloor)
        {
            mov.etaz -= addToEtaz;
        }
        else
        {
            etaztxt.text = "Floor: " + mov.etaz.ToString();
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 5 * mov.etaz - 5);
        }
    }
}
