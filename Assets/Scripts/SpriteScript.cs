using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Item item;
     Text output=null;
    void Start()
    {
        if (output == null)
        {
            var outputs = GameObject.FindObjectsOfType<Text>();
            foreach (var s in outputs)
            {
                if (s.name == "text")
                {
                    output = s;
                    break;
                }
            }
        }
    }
    static public Sprite ToSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 200.0f);
    } 
    // Update is called once per frame
    void Update()
    {
        
    }
    void OnMouseDown()
    {
        output.text = item.name + ": " + item.description;
    }
}
