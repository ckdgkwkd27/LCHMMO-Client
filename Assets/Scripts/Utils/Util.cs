using Google.Protobuf;
using UnityEngine;

public class Util
{
    public enum NetworkState
    {
        NONE,
        CONNECTED,
        LOGIN,
        INGAME,
    }

    public static NetworkState _networkState = NetworkState.NONE;

    public static PacketID MessageToID(string msgName)
    {
        switch(msgName)
        {
            case "RequestLogin":
                return PacketID.PKT_CS_LOGIN;
            case "ReturnLogin":
                return PacketID.PKT_SC_LOGIN;
            case "RequestEnterGame":
                return PacketID.PKT_CS_ENTER_GAME;
            case "ReturnEnterGame":
                return PacketID.PKT_SC_ENTER_GAME;
            case "RequestMove":
                return PacketID.PKT_CS_MOVE;
            case "ReturnMove":
                return PacketID.PKT_SC_MOVE;
            default:
                return PacketID.PKT_ERROR;
        }
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }
}

