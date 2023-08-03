using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }

    //MapManager _map = new MapManager();
    NetworkManager _network = new NetworkManager();
    UIManager _ui = new UIManager();

    //public static MapManager Map { get { return Instance._map; } }
    public static NetworkManager Network { get { return Instance._network; } }
    public static UIManager UI { get { return Instance._ui; } }

    void Start()
    {
        Init();
    }

    private void Update()
    {
        _network.Update();
    }

    static void Init()
    {
        if(s_instance == null)
        {
            Debug.Log("Manager Initialize!");

            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._network.Init();
        }
    }

    public static void Clear()
    {

    }
}