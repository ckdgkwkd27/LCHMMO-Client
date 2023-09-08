using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<ulong, GameObject> _objects = new Dictionary<ulong, GameObject>();

    public static GameObjectType GetObjectTypeById(ulong id)
    {
        ulong type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public void Add(Protocol.ObjectInfo info, bool myPlayer = false)
    {
        if (info.ObjectType == (uint)GameObjectType.PLAYER)
        {
            if (myPlayer)
            {
                GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer");
                go.name = info.Name;
                _objects.Add(info.ActorId, go);
                Debug.Log($"ActorID={info.ActorId}, name={info.Name}");

                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ActorId;
                MyPlayer.PosInfo = info.PosInfo;
                MyPlayer.Stat = info.StatInfo;
                MyPlayer.SyncPos();
            }
            else
            {
                GameObject go = Managers.Resource.Instantiate("Creature/Player");
                go.name = info.Name;
                _objects.Add(info.ActorId, go);

                PlayerController pc = go.GetComponent<PlayerController>();
                pc.Id = info.ActorId;
                pc.PosInfo = info.PosInfo;
                pc.Stat = info.StatInfo;
                pc.SyncPos();
            }
        }
        else if (info.ObjectType == (uint)GameObjectType.MONSTER)
        {
            //GameObject go = Management.Resource.Instantiate("Creature/Monster");
            //go.name = info.name;
            //_objects.Add(info.objectId, go);

            //MonsterController mc = go.GetComponent<MonsterController>();
            //mc.Id = info.objectId;
            //mc.PosInfo = info.posInfo;
            //mc.Stat = info.statInfo;
            //mc.SyncPos();
        }
        else if (info.ObjectType == (uint)GameObjectType.PROJECTILE)
        {
            //GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
            //go.name = "Arrow";
            //_objects.Add(info.ObjectId, go);

            //ArrowController ac = go.GetComponent<ArrowController>();
            //ac.PosInfo = info.PosInfo;
            //ac.Stat = info.StatInfo;
            //ac.SyncPos();
        }
    }

    public void Remove(ulong id)
    {
        GameObject go = FindById(id);
        if (go == null)
            return;

        _objects.Remove(id);
        Debug.Log($"Remove name={go.name}");
        Managers.Resource.Destroy(go);
    }

    public GameObject FindById(ulong id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public GameObject FindCreature(Vector3Int cellPos)
    {
       foreach(GameObject _go in _objects.Values)
        {
            CreatureController cc = _go.GetComponent<CreatureController>();
            if (cc == null)
                continue;

            if (cc.CellPos == cellPos)
                return _go;
        }

        return null;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach(GameObject _go in _objects.Values)
        {
            if (condition.Invoke(_go))
                return _go;
        }

        return null;
    }

    public void Clear()
    {
        foreach (GameObject _go in _objects.Values)
        {
            _objects.Clear();
            MyPlayer = null;
        }
    }
}