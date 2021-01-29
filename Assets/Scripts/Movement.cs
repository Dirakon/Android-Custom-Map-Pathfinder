using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Tile
{
    static int idGiver;
    public int id = idGiver++;
    public bool closed = false;
    public Tile from;
    public char type;
    public int g;
    public int f;
    public int h;
    public float xr, yr, zr;
    public static float TileSize = 0.5f;
    public GameObject obj;
    public int pid=-1;
    public int zia, xia, yia;
    public Tile(float xer, float yer, float zer,char typer,GameObject objer,int ziar,int yiar, int xiar)
    {

        xia = xiar;
        zia = ziar;
        yia = yiar;
        xr = xer;

        zr = zer;
        yr = yer;
        obj = objer;
        var p = obj.GetComponent<DebugScript>();
        p.zia = zia;
        p.xia = xia;
        p.yia = yia;
        type = typer;
        if (type == 'u')
        {
            obj.GetComponent<Renderer>().material.color = UnityEngine.Color.green;
        }else if (type == 'd')
        {
            obj.GetComponent<Renderer>().material.color = UnityEngine.Color.red;
        }
    }
}

public class Movement : MonoBehaviour
{
    
    static char[][][] plan;
    public GameObject go;
    void Awake()
    {
    }//https://upload.wikimedia.org/wikipedia/commons/4/47/PNG_transparency_demonstration_1.png
    void Init()
    {
        if (toDelete != null)
        {
            foreach (var a in toDelete)
            {
                Destroy(a);
            }
        }
        var pos = transform.position;
        toDelete = new List<GameObject>();
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 5 * etaz - 5);
        tile.transform.localScale = new Vector3(Tile.TileSize, Tile.TileSize, Tile.TileSize);
        string s = NetLoader.ReadFromFile(@NetLoader.GetFilePath("settings.txt"));
        var ss = s.Split(';');
        lastfloor = int.Parse(ss[2]);
        plan = new char[lastfloor+1][][];
        
        Tiles = new Tile[plan.Length][][];
        for (int i = firstfloor; i <= lastfloor; ++i)
        {
            var d = NetLoader.ReadFromFile(@NetLoader.GetFilePath(i.ToString() + ".txt")); 
            string[] tm = d.Split('\n');
            plan[i] = new char[tm.Length][];
            for (int jpeg = 0; jpeg < tm.Length; jpeg++)
            {
                plan[i][jpeg] = tm[jpeg].ToCharArray();
            }
            Tiles[i] = new Tile[plan[i].Length][];
        }
        for (int z = 0; z < plan.Length; ++z)
        {
            Tiles[z] = new Tile[plan[z].Length][];
            for (int y = 0; y < plan[z].Length; ++y)
            {
                Tiles[z][y] = new Tile[plan[z][y].Length];
                for (int x = 0; x < Tiles[z][y].Length; ++x)
                {
                    if (plan[z][y][x] != '*' && plan[z][y][x] != 'u' && plan[z][y][x] != 'd')
                    {
                        Tiles[z][y][x] = null;
                    }
                    else
                    {
                        GameObject obj = Instantiate(tile);
                        toDelete.Add(obj);
                        float tx = transform.position.x - plan[z][y].Length / 2f * Tile.TileSize + x * Tile.TileSize, ty = transform.position.y + plan[z].Length / 2 * Tile.TileSize - y * Tile.TileSize, tz = 5 * z - 4;
                        obj.transform.position = new Vector3(tx, ty, tz);
                        Tiles[z][y][x] = new Tile(tx, ty, tz, plan[z][y][x], obj, z, y, x);
                        if (plan[z][y][x] == 'u')
                        {
                            plan[z + 1][y][x] = 'd';
                        }
                    }
                }
            }
        }
        string objlist = NetLoader.ReadFromFile(@NetLoader.GetFilePath("objlist.txt"));
        List<Item> items = new List<Item>();
        foreach (var objd in objlist.Split('&'))
        {
            if (objd.Length < 6)
                continue;
            var arr = objd.Split(';');
            var item = new Item();
            item.filename = arr[0];
            item.name = arr[1];
            item.description = arr[2];
            item.type = arr[3];
            item.z = arr[4];
            item.y = arr[5];
            item.x = arr[6];
            item.init(go);
        }
        transform.position = pos;
        canIwork = 2;
    }
    void Start()
    {

        cam = GetComponent<Camera>();
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 5 * etaz - 5);
        if (File.Exists(NetLoader.GetFilePath("settings.txt")))
        {
            Init();
        } 

    }
    public int canIwork = 0;
    public GameObject tile;
    Camera cam;
    public Tile first=null;
    public Tile last=null;
    public static  Tile[][][] Tiles;
    public Text text;
    public int smax = 30;
    public int smin = 1;
    public int del = 60;
    public int minx = -15;
    public int maxx = 15;
    public int miny = -15;
    public int maxy = 15;
    public int firstfloor = 0;
    public int etaz = 1;
    public int lastfloor;
    // Update is called once per frame
    float lasty, lastx, lastlen;
    static public List<GameObject> toDelete;
    void Update()
    {
        if (canIwork==0)
        {
            return;
        }
        else if (canIwork == 1)
        {
            Init();
        }
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
            {
                return;
            }
            if (touch.phase != TouchPhase.Began)
            {
                transform.position = new Vector3(transform.position.x + (lastx - touch.position.x) / 100 * cam.orthographicSize/5f, transform.position.y + (lasty - touch.position.y) / 100 * cam.orthographicSize / 5f, transform.position.z);
                if (maxx < transform.position.x)
                {
                    transform.position = new Vector3(maxx, transform.position.y,transform.position.z);
                } else if (minx > transform.position.x)
                {
                    transform.position = new Vector3(minx, transform.position.y, transform.position.z);
                }
                if (maxy < transform.position.y)
                {
                    transform.position = new Vector3(transform.position.x, maxy, transform.position.z);

                }
                else if (miny > transform.position.y)
                {
                    transform.position = new Vector3(transform.position.x, miny, transform.position.z);
                }
            }
            lasty = touch.position.y;
            lastx = touch.position.x;
        }
        else if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            if (touch1.phase == TouchPhase.Canceled || touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Canceled || touch2.phase == TouchPhase.Ended)
            {
                return;
            }
            float len = Mathf.Sqrt((touch1.position.x- touch2.position.x)*(touch1.position.x- touch2.position.x) +( touch1.position.y- touch2.position.y)*(touch1.position.y- touch2.position.y));
            if (touch1.phase != TouchPhase.Began && touch2.phase != TouchPhase.Began)
            {
                cam.orthographicSize = cam.orthographicSize - (len - lastlen)/del;
                if (cam.orthographicSize > smax || cam.orthographicSize < smin)
                    cam.orthographicSize = cam.orthographicSize + (len - lastlen) / del;
            }
            lastlen = len;
        }
    }
}
