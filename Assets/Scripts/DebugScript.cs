using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class etaz
{
    public LinkedList<Tile> up;
    public LinkedList<Tile> down;
    public int id;
    etaz()
    {

    }
    public etaz(int ider)
    {
        up = new LinkedList<Tile>();
        down = new LinkedList<Tile>();
        id = ider;
        for (int y = 0; y < Movement.Tiles[id].Length; ++y)
        {
            for (int x = 0; x < Movement.Tiles[id][y].Length; ++x)
            {
                if (Movement.Tiles[id][y][x] == null)
                    continue;
                if (Movement.Tiles[id][y][x].type == 'd')
                {
                    down.AddLast(Movement.Tiles[id] [y] [x]);
                }
                else if (Movement.Tiles[id][y][x].type == 'u')
                {
                    up.AddLast(Movement.Tiles[id][y][x]);
                }
            }
        }
    }

};
public class DebugScript : MonoBehaviour
{
    // Start is called before the first frame update
    static public int mod (int c)
    {
        if (c < 0)
            return -c;
        return c;
    }
    void Start()
    {
        if (cam == null)
        {
            cam = GameObject.FindObjectOfType<Camera>();
        }
        if (mov == null)
        mov = cam.GetComponent<Movement>();
        if (floors == null)
        {
            floors = new etaz[Movement.Tiles.Length];
            for (int i = 0; i < Movement.Tiles.Length; ++i)
            {
                floors[i] = new etaz(i);
            }
        }
    }
    static int h(int z, int y, int x)
    {
        int[] ste;
        if (z < mov.last.zia)
        {
            ste = new int[floors[z].up.Count];
            int i = 0;
            foreach (var it in floors[z].up)
            {
                ste[i] = mod(it.yia - y) + mod(it.xia - x);
                ++i;
            }
        }
        else if (z > mov.last.zia)
        {
            ste = new int[floors[z].down.Count];
            int i = 0;
            foreach (var it in floors[z].down)
            {
                ste[i] = mod(it.yia - y) + mod(it.xia - x);
                ++i;
            }
        }
        else
        {
            return mod(y - mov.last.yia) + mod(x - mov.last.xia);
        }
        int mins = ste[0];
        for (int i = 1; i < ste.Length; ++i)
        {
            if (ste[i] < mins)
            {
                mins = ste[i];
            }
        }
        return mins * mod(mov.last.zia - z);
    }
    void Update()
    {

    }
    static etaz[] floors = null;
    static public Camera cam=null;
    public static Movement mov=null;
    public int zia, xia, yia;
    static LinkedList<Tile> opened;
    static int pidgiver = 0;

    static void nbrcheck(Tile doer, int nz, int ny, int nx,int pid)
    {
        if (nx < 0 || ny < 0 || ny >= Movement.Tiles[nz].Length || nx >= Movement.Tiles[nz][ny].Length || Movement.Tiles[nz][ny][nx] == null || (Movement.Tiles[nz][ny][nx].closed && Movement.Tiles[nz][ny][nx].pid == pid))
        {
            return;
        }
        if (Movement.Tiles[nz][ny][nx].pid != pid)
        {
            opened.AddLast(Movement.Tiles[nz] [ny] [nx]);
            Movement.Tiles[nz][ny][nx].pid = pid;
            Movement.Tiles[nz][ny][nx].g = doer.g + 1;
            Movement.Tiles[nz][ny][nx].from = doer;
            Movement.Tiles[nz][ny][nx].f = Movement.Tiles[nz][ny][nx].g + Movement.Tiles[nz][ny][nx].h;
        }
        else if (doer.g + 1 < Movement.Tiles[nz][ny][nx].g)
        {
            Movement.Tiles[nz][ny][nx].g = doer.g + 1;
            Movement.Tiles[nz][ny][nx].from = doer;
            Movement.Tiles[nz][ny][nx].f = Movement.Tiles[nz][ny][nx].g + Movement.Tiles[nz][ny][nx].h;
        }
    }
    static LinkedList<Tile>.Enumerator minf()
    {
        var it = opened.GetEnumerator();
        it.MoveNext();
        int minfv = it.Current.f;
        var minfp = it;
        for (; ; it.MoveNext())
        {
            if (minfv > it.Current.f)
            {
                minfv = it.Current.f;
                minfp = it;
            }
            if (it.Current.id == opened.Last.Value.id)
            {
                break;
            }
        }
        return minfp;
    }
    public static Tile fist, sicc;
    static void doIt()
    {
        fist = mov.first;
        sicc = mov.last;
        int pid = pidgiver++;
        opened = new LinkedList<Tile>();
        opened.AddFirst(mov.first);
        opened.First.Value.g = 0;
        opened.First.Value.f = 0;
        opened.First.Value.pid = pid;
        for (int z = 0; z < Movement.Tiles.Length; ++z)
        {
            for (int y = 0; y < Movement.Tiles[z].Length; ++y)
            {
                for (int x = 0; x < Movement.Tiles[z][y].Length; ++x)
                {
                    if (Movement.Tiles[z][y][x] != null)
                    {
                        Movement.Tiles[z][y][x].h = h(z, y, x);
                    }
                }
            }
        }
        while (opened.Count > 0)
        {

            var p = minf();
            var ap = p.Current;
            opened.Remove(ap);
            ap.closed = true;
            if (ap.id == mov.last.id)
            {
                break;
            }
            nbrcheck(ap, ap.zia, ap.yia, ap.xia - 1,pid);
            nbrcheck(ap, ap.zia, ap.yia, ap.xia + 1, pid);
            nbrcheck(ap, ap.zia, ap.yia - 1, ap.xia, pid);
            nbrcheck(ap, ap.zia, ap.yia + 1, ap.xia, pid);
            if (ap.type == 'd')
            {
                nbrcheck(ap, ap.zia - 1, ap.yia, ap.xia, pid);
            }
            else if (ap.type == 'u')
            {
                nbrcheck(ap, ap.zia + 1, ap.yia, ap.xia, pid);
            }
        }
    }

    static void changeIntoColor(Color col)
    {
        var s = sicc;
        do
        {
            if (s.type == '*')
             s.obj.GetComponent<Renderer>().material.color = col;
            if (s.id == fist.id)
                break;
            s = s.from;
        } while (true);
    }
   
    
    void OnMouseDown()
    {
        if (mov.first == null)
        {
            mov.first = Movement.Tiles[zia][yia][xia];
        }
        else
        {
            mov.last = Movement.Tiles[zia][yia][xia];
            if (fist != null && sicc != null && fist.obj != null && sicc.obj != null)
              changeIntoColor(Color.white);
            doIt();
            changeIntoColor(Color.yellow);
            mov.first = null;
            mov.last = null;
        }
    }
}
