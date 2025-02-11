﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { NONE, MONSTER, EVENT, BOSS, STORE, REST, HALL }

public class RoomSet : ScriptableObject
{
    public int x;
    public int y;
    public int width;
    public int height;
    public int size;
    public int gage;

    public List<ObjectData> objectDatas;
    public RoomType roomType;
    public bool isLock;

    public void Init(int _width, int _height, int _size, int _gage, RoomType _roomType)
    {
        width = _width;
        height = _height;
        size = _size;
        gage = _gage;
        roomType = _roomType;
        objectDatas = new List<ObjectData>();
    }

    public void Add(ObjectData _obj)
    {
        objectDatas.Add(_obj);
    }
}

[System.Serializable]
public struct ObjectData
{
    public Vector2 position;
    public Vector3 scale;
    public Sprite[] sprites;
    public bool isActive;
    public ObjectType objectType;
    public ObjectAbnormalType objectAbnormalType;
    public string className;

    public ObjectData(Vector2 _position,Vector3 _scale, ObjectType _objectType, ObjectAbnormalType objectAbnormalType, Sprite[] _sprites, string className = "")
    {
        this.position = _position;
        this.scale = _scale;
        this.sprites = _sprites;
        this.objectType = _objectType;
        this.objectAbnormalType = objectAbnormalType;
        this.className = className;
        isActive = false;
    }

    public GameObject LoadObject(GameObject _gameObject)
    {
        switch (objectType)
        {
            case ObjectType.NONE:
                _gameObject.AddComponent<NoneBox>().LoadAwake();
                _gameObject.GetComponent<NoneBox>().sprites = sprites;
                _gameObject.GetComponent<NoneBox>().Init();
                break;
            case ObjectType.UNBREAKABLE:
                _gameObject.AddComponent<UnbreakableBox>().LoadAwake();
                _gameObject.GetComponent<UnbreakableBox>().sprites = sprites;
                _gameObject.GetComponent<UnbreakableBox>().Init();
                break;
            case ObjectType.BREAKABLE:
                _gameObject.AddComponent<BreakalbeBox>().LoadAwake();
                _gameObject.GetComponent<BreakalbeBox>().sprites = sprites;
                _gameObject.GetComponent<BreakalbeBox>().Init();
                break;
            case ObjectType.PUSHBOX:
                _gameObject.AddComponent<PushBox>().LoadAwake();
                _gameObject.GetComponent<PushBox>().sprites = sprites;
                _gameObject.GetComponent<PushBox>().Init();
                break;
            case ObjectType.ITEMBOX:
                _gameObject.AddComponent<ItemBox>().LoadAwake();
                _gameObject.GetComponent<ItemBox>().sprites = sprites;
                _gameObject.GetComponent<ItemBox>().Init();
                break;
            case ObjectType.VENDINMACHINE:
                _gameObject.AddComponent<VendingMachine>().LoadAwake();
                _gameObject.GetComponent<VendingMachine>().sprites = sprites;
                _gameObject.GetComponent<VendingMachine>().Init();
                break;
            case ObjectType.PORTAL:
                _gameObject.AddComponent<Portal>().LoadAwake();
                _gameObject.GetComponent<Portal>().sprites = sprites;
                _gameObject.GetComponent<Portal>().Init();
                break;
            case ObjectType.SNACKBOX:
                _gameObject.AddComponent<SnackBox>().LoadAwake();
                _gameObject.GetComponent<SnackBox>().sprites = sprites;
                _gameObject.GetComponent<SnackBox>().Init();
                break;
            case ObjectType.MEDKITBOX:
                _gameObject.AddComponent<MedkitBox>().LoadAwake();
                _gameObject.GetComponent<MedkitBox>().sprites = sprites;
                _gameObject.GetComponent<MedkitBox>().Init();
                break;
            case ObjectType.SUBSTATION:
                _gameObject.AddComponent<SubStation>().LoadAwake();
                _gameObject.GetComponent<SubStation>().sprites = sprites;
                _gameObject.GetComponent<SubStation>().Init();
                break;
            case ObjectType.STOREITEM:
                _gameObject.AddComponent<StoreItem>().LoadAwake();
                _gameObject.GetComponent<StoreItem>().sprites = null;
                _gameObject.GetComponent<StoreItem>().Init();
                break;
            case ObjectType.NPC:
                if(className == "Astrologer")
                {
                    _gameObject.AddComponent<Astrologer>().LoadAwake();
                    _gameObject.GetComponent<Astrologer>().sprites = sprites;
                    _gameObject.GetComponent<Astrologer>().Init();
                }
                break;
            case ObjectType.STATUE:
                _gameObject.AddComponent<Statue>().LoadAwake();
                _gameObject.GetComponent<Statue>().sprites = sprites;
                _gameObject.GetComponent<Statue>().Init();
                break;
            case ObjectType.TRAPBOX:
                _gameObject.AddComponent<TrapBox>().LoadAwake();
                _gameObject.GetComponent<TrapBox>().sprites = sprites;
                _gameObject.GetComponent<TrapBox>().Init();
                break;
            case ObjectType.SKILLBOX:
                _gameObject.AddComponent<SkillBox>().LoadAwake();
                _gameObject.GetComponent<SkillBox>().sprites = sprites;
                _gameObject.GetComponent<SkillBox>().Init(objectAbnormalType);
                break;
            case ObjectType.PARTITION:
                _gameObject.AddComponent<Partition>().LoadAwake();
                _gameObject.GetComponent<Partition>().sprites = sprites;
                _gameObject.GetComponent<Partition>().Init();
                break;
            case ObjectType.DESTROYTRAPOBJ:
                _gameObject.AddComponent<DestoyTrapObj>().LoadAwake();
                _gameObject.GetComponent<DestoyTrapObj>().sprites = sprites;
                _gameObject.GetComponent<DestoyTrapObj>().Init();
                break;
        }

        return _gameObject;
    }
}