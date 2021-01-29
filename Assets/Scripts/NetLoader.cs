using GDataDB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Version {
    public static string mapver, objver, lastfloor;
    public string subject { get; set; }
    public string version { get; set; }
    public void init(Movement mov)
    {
            if (subject == "map version")
            {
                mapver = version;
            }
              else if (subject == "objects version")
            {
                objver = version;
            }
              else if (subject == "lastfloor")
            {
            lastfloor = version;
            }
            else if (File.Exists(NetLoader.GetFilePath("settings.txt")))
        {
            //Найти старые версии и, если не совпадают, сделать соотв. действия.
            string s = NetLoader.ReadFromFile(@NetLoader.GetFilePath("settings.txt"));
            var ss= s.Split(';');
            string olmapver = ss[0],olobjver=ss[1];
            bool was = false;
            if (olmapver != mapver)
            {
                CreateFloorFiles();
                was = true;
            }
            if (olobjver != objver)
            {
                CreateObjFiles(mov);
                was = true;
            }
            NetLoader.CreateSWFile(NetLoader.GetFilePath("settings.txt"), (mapver + ';' + objver + ';' + lastfloor));
            if (was)
                mov.canIwork = 1;
        }
            else {
            //Сделать соотв. действия.в
            CreateFloorFiles();
            CreateObjFiles(mov);
            NetLoader.CreateSWFile(NetLoader.GetFilePath("settings.txt"), (mapver + ';' + objver + ';' + lastfloor));
            mov.canIwork = 1;
        }
    }
    static void CreateObjFiles(Movement mov)
    {
        var itemsT = NetLoader.database.GetTable<Item>("objects");
        string textToWrite = "";
        List<Item> items = new List<Item>();
        foreach (var s in itemsT.FindAll())
        {
            items.Add(s.Element);
            items[items.Count - 1].filename = (++counter).ToString() + ".png";
            textToWrite += items[items.Count - 1].filename + ";" + items[items.Count - 1].name + ";" + items[items.Count - 1].description + ";" + items[items.Count - 1].type + ";" + items[items.Count - 1].z + ";" + items[items.Count - 1].y + ";" + items[items.Count - 1].x+'&';
        }
        NetLoader.CreateSWFile(NetLoader.GetFilePath("objlist.txt"), textToWrite);
        counter = 0;
        foreach (var i in items)
        {
            Courutiner.link = i.link;
            Courutiner.download = true;
            while (NetLoader.bytessa == null)
            {

            }
            var tex = NetLoader.texture;
            NetLoader.texture = null;
            NetLoader.CreateFileFile(NetLoader.GetFilePath(i.filename), NetLoader.bytessa);
            NetLoader.bytessa = null;
        }
    }
    static int counter = 0;
    static void CreateFloorFiles()
    {
        var fls = int.Parse(lastfloor);
        for (int i = 0; i <= fls; ++i)
        {
            var table = NetLoader.database.GetTable<OneYLine>(i.ToString());
            string etaz = "";
            foreach (var s in table.FindAll())
            {
                etaz += s.Element.str + '\n';
            }
            NetLoader.CreateSWFile(NetLoader.GetFilePath(i.ToString() + ".txt"), etaz);
        }
    }
}
public class OneYLine
{
    public int y { get; set; }
    public string str { get; set; }
    
    public void init(int z)
    {
        Debug.Log(int.Parse("-1"));
    }

}
public class Item {
    public string filename;
    public string name { get; set; }
    public string description { get; set; }
    public string type { get; set; }
    public string x { get; set; }
    public string y { get; set; }
    public string z { get; set; }
    public string link { get; set; }

    public GameObject sprite;

    public void init(GameObject parent)
    {
        sprite = GameObject.Instantiate(parent);
        Movement.toDelete.Add(sprite);
        byte[] byteArray = File.ReadAllBytes(NetLoader.GetFilePath(filename));
        Texture2D sampleTexture = new Texture2D(2, 2);
        sampleTexture.LoadImage(byteArray);
        sprite.GetComponent<SpriteRenderer>().sprite = SpriteScript.ToSprite(sampleTexture);
        string[] zer = z.Split('.'), yer = y.Split('.'), xer = x.Split('.');
        int zs = int.Parse(zer[0]), ys = int.Parse(yer[0]), xs = int.Parse(xer[0]);
        Tile t;
        if (zs < 0 || zs > Movement.Tiles.Length || ys < 0 || ys > Movement.Tiles[zs].Length || xs < 0 || xs > Movement.Tiles[zs][ys].Length || (t = Movement.Tiles[int.Parse(zer[0])][int.Parse(yer[0])][int.Parse(xer[0])]) == null)
        {
            Debug.Log("nope");
            GameObject.Destroy(sprite);
            return;
        }
        sprite.transform.position = new Vector3( t.xr + int.Parse(xer[1]) / 10*Tile.TileSize,t.yr + int.Parse(yer[1]) / 10 * Tile.TileSize, t.zr - 0.4f);
        sprite.GetComponent<SpriteScript>().item = this;
    }
}



public class NetLoader : MonoBehaviour
{
    public static byte[] bytessa;
    public static Movement mov = null;
    public static bool id = false;
    public static Texture2D texture=null;
    public static IEnumerator GetTexture(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        if (texture == null)
        {

        }
        bytessa = texture.EncodeToPNG();

    }
    public static string Encode(string p)
    {
        string ans = "";
        foreach (char c in p)
        {
            switch (c) {
            }

        }
        return null;
    }
    public static string Decode(string p)
    {
        string ans = "";
        foreach (char c in p)
        {
            switch (c)
            {
            }

        }
        return null;
    }
    public static void CreateSWFile(string path, byte[] bytess)
    {
        CreateSWFile(path, Encoding.ASCII.GetString(bytess));
    }
    public static void CreateSWFile(string path, string bytess)
    {
        using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Unicode))
        {
            foreach (string text in bytess.Split('\n'))
            {
                sw.WriteLine(text);
            }
        }
    }
    public static void CreateFileFile (string path, byte [] bytess)
    {
        File.WriteAllBytes(path, bytess);
    }
    public static void CreateFileFile(string path, string bytess)
    {
        CreateFileFile(path, Encoding.ASCII.GetBytes(bytess));
    }
    public static string GetFilePath(string path)
    {
        if (Application.platform == RuntimePlatform.Android)
            return Application.persistentDataPath + "/" + path;

        return "Assets\\" + path;
    }
    public static string ReadFromFile(string path)
    {
        string ans="";
        using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                ans += line + '\n';
            }
        }
        return ans;
    }
    public static void CopyFileFromStreamingAssetsCreateFile(string fileinput)
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            // Android
            string oriPath = System.IO.Path.Combine(Application.streamingAssetsPath, fileinput);

            // Android only use WWW to read file
            WWW reader = new WWW(oriPath);
            while (!reader.isDone) { }
            var s = GetFilePath(fileinput);
            CreateSWFile(s, reader.bytes);
        }
    }
    static bool isReady = false;
    public static List<Version> vers;
    public static List<Item> items;
    public static DatabaseClient databaseClient;
    public static IDatabase database;
    static void forThread()
    {
        string per = GetFilePath("s_a.p12");
        if (!File.Exists(per))
               CopyFileFromStreamingAssetsCreateFile("s_a.p12");
        string secretAccountGmail = NetLoader.ReadFromFile(@NetLoader.GetFilePath("secretAccountData.txt")).Split(';')[1];
        databaseClient = new DatabaseClient(secretAccountGmail, File.ReadAllBytes(per));
         database = databaseClient.GetDatabase("Proikt");
        var Table = database.GetTable<Version>("data");
        var all = Table.FindAll();
        vers = new List<Version>();
        foreach (var i in all)
        {
            vers.Add(i.Element);
        }
        foreach (var i in vers)
        {
            i.init(mov);
        }
        isReady = true;

    }
    void Start()
    {
        if (mov == null)
        {
            mov = GetComponent<Camera>().GetComponent<Movement>();
        }
        Thread thr = new Thread(forThread);
        output.text = "Updating thread has began...";
        thr.Start();

    }
    public Text output;
    void Update()
    {
        if (!isReady)
        {
            return;
        }
        output.text = "Updating thread has ended:\n";
        isReady = false;
        Courutiner.getDestroyed = true;
        Destroy(this);
    }
}



