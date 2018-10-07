﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;
using UnityEngine.Serialization;
/*
 * Weapon, Bullet, Effect 고유 정보 저장소
 * 
 * 나중에 에디터랑 정보 저장(xml, json 등등) 이런거 고려해서
 * 일단 임시로 따로 빼놨음 언제든지 구조랑 내용 데이터 다루는
 * 방식 바뀔 예정.
 * 
 * 
 */
namespace WeaponAsset
{
    public delegate float DelGetDirDegree();    // 총구 방향 각도
    public delegate Vector3 DelGetPosition();   // owner position이지만 일단 player position 용도로만 사용.

    public enum WeaponState { Idle, Attack, Reload, Charge, Switch, PickAndDrop }
    /// <summary>
    /// 원거리 : 권총, 산탄총, 기관총, 저격소총, 레이저, 활
    /// 근거리 : 창, 몽둥이, 스포츠용품, 검, 청소도구, 주먹장착무기
    /// 함정 : 폭탄, 가스탄, 접근발동무기
    /// 특수 : 지팡이, 쓰레기
    /// </summary>
    // END 는 WeaponType 총 갯수를 알리기 위해서 enum 맨 끝에 기입 했음.
    public enum WeaponType
    {
        NULL, PISTOL, SHOTGUN, MACHINEGUN, SNIPER_RIFLE, LASER, BOW,
        SPEAR, CLUB, SPORTING_GOODS, SWORD, CLEANING_TOOL, KNUCKLE,
        BOMB, GAS_SHELL, TRAP,
        WAND, TRASH, OTHER, END
    }

    // PISTOL, SHOTGUN, MACHINEGUN, SNIPLER_RIFLE, LASER, BOW
    public enum AttackAniType { None, Blow, Strike, Swing, Punch, Shot }
    public enum AttackType { MELEE, RANGED }
    public enum TouchMode { Normal, Charge }
    public enum BulletType { PROJECTILE, LASER, MELEE, NULL, MINE, EXPLOSION}
    /*---*/

    public enum BulletPropertyType { Collision, Update, Delete }
    public enum CollisionPropertyType { BaseNormal, Laser, Undeleted }
    public enum UpdatePropertyType { StraightMove, AccelerationMotion, Laser, Summon, Homing, MineBomb, FixedOwner, Spiral, Rotation, Child }
    public enum DeletePropertyType { BaseDelete, Laser, SummonBullet, SummonPattern }
    public enum BehaviorPropertyType { SpeedControl, Rotate }

    /*---*/

    public enum ColliderType { Beam, Box, Circle, None }

    public enum BulletAnimationType
    {
        NotPlaySpriteAnimation,
        BashAfterImage,
        PowerBullet,
        Wind,
        BashAfterImage2,
        Explosion0,
        BashSkyBlue,
        BashBlue,
        BashRed,
        BashOrange,
    }

    /*---*/


    // 총알 삭제 함수 델리게이트
    public delegate void DelDestroyBullet();
    // 총알 충돌 함수 델리게이트
    public delegate void DelCollisionBullet(Collider2D coll);
}


// 각종 데이터 실제로 저장해서 모아놓은 클래스.
public class DataStore : MonoBehaviourSingleton<DataStore>
{
    #region variables
    [SerializeField]
    private WeaponInfo[] weaponInfos;
    [SerializeField]
    //[FormerlySerializedAs("tests")] 이거 선언, 이전 이름, 새로운 변수 명 한 번에 해야됨.
    private WeaponInfo[] tempWeaponInfos;
    [SerializeField]
    private WeaponInfo[] temp2WeaponInfos;
    [SerializeField]
    //[FormerlySerializedAs("temp2WeaponInfos")]
    private WeaponInfo[] temp3WeaponInfos;

    [SerializeField]
    private WeaponInfo[] test2WeaponInfos;

    /// <summary> 기획자 무기 테스트용 </summary>
    [SerializeField]
    private WeaponInfo[] A1WeaponInfos;
    [SerializeField]
    private WeaponInfo[] testBossWeaponInfos;


    [SerializeField]
    private WeaponInfo[] enemyWeaponInfos;
    [SerializeField]
    private UsableItemInfo[] clothingItemInfos;
    [SerializeField]
    private UsableItemInfo[] etcItemInfos;
    [SerializeField]
    private UsableItemInfo[] foodItemInfos;
    [SerializeField]
    private UsableItemInfo[] medicalItemInfos;
    [SerializeField]
    private UsableItemInfo[] miscItemInfos;
    [SerializeField]
    private UsableItemInfo[] petItemInfos;

    [SerializeField]
    private EffectInfo[] effectInfos;

    [Header("true하고 실행 시 엑셀 내용으로 무기 초기화")]
    [SerializeField]
    private bool canInputWeaponDatas;
    public List<Dictionary<string, object>> weaponDatas;


    // private List<BulletInfo> initializedBulletInfosAtRuntime;
    // private int initializedBulletInfosLength;

    #endregion

    #region getter

    
    public int GetWeaponInfosLength()
    {
        switch(DebugSetting.Instance.weaponModeForDebug)
        {
            case WeaponModeForDebug.TEST:
                return weaponInfos.Length;
            case WeaponModeForDebug.TEMP:
                return tempWeaponInfos.Length;
            case WeaponModeForDebug.TEMP2:
                return temp2WeaponInfos.Length;
            case WeaponModeForDebug.TEST2:
                return test2WeaponInfos.Length;
            case WeaponModeForDebug.A1:
                return A1WeaponInfos.Length;
            case WeaponModeForDebug.TESTBOSS:
                return testBossWeaponInfos.Length;
            default:
                break;
        }
        return 0;
    }

    public int GetClothingItemInfosLength()
    {
        return clothingItemInfos.Length;
    }

    public int GetEtcItemInfosLength()
    {
        return etcItemInfos.Length;
    }

    public int GetFoodItemInfosLength()
    {
        return foodItemInfos.Length;
    }

    public int GetMedicalItemInfosLength()
    {
        return medicalItemInfos.Length;
    }

    public int GetMiscItemInfosLength()
    {
        return miscItemInfos.Length;
    }

    public int GetPetItemInfosLength()
    {
        return petItemInfos.Length;
    }

    public int GetEnemyWeaponInfosLength()
    {
        return enemyWeaponInfos.Length;
    }

    /// <summary>
    /// Owner에 따른 Weapon Data 반환, ownerType 기본 값 Player
    /// </summary>
    /// <param name="id"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    public WeaponInfo GetWeaponInfo(int id, CharacterInfo.OwnerType ownerType)
    {
        // player용 switch 안에 switch 못해서 따로 떼어놓음.
        if (CharacterInfo.OwnerType.Player == ownerType)
        {
            switch (DebugSetting.Instance.weaponModeForDebug)
            {
                case WeaponModeForDebug.TEST:
                    return weaponInfos[id];
                case WeaponModeForDebug.TEMP:
                    return tempWeaponInfos[id];
                case WeaponModeForDebug.TEMP2:
                    return temp2WeaponInfos[id];
                case WeaponModeForDebug.TEST2:
                    return test2WeaponInfos[id];
                case WeaponModeForDebug.A1:
                    return A1WeaponInfos[id];
                case WeaponModeForDebug.TESTBOSS:
                    return testBossWeaponInfos[id];
                default:
                    break;
            }
        }
        switch(ownerType)
        {
            case CharacterInfo.OwnerType.Enemy:
                return enemyWeaponInfos[id];
            case CharacterInfo.OwnerType.Object:
            default:
                break;
        }
        return null;
    }

    public EffectInfo GetEffectInfo(int id) { return effectInfos[id]; }

    public UsableItemInfo GetClothingItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, clothingItemInfos.Length);
        return clothingItemInfos[id];
    }
    public UsableItemInfo GetEtcItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, etcItemInfos.Length);
        return etcItemInfos[id];
    }
    public UsableItemInfo GetFoodItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, foodItemInfos.Length);
        return foodItemInfos[id];
    }
    public UsableItemInfo GetMedicalItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, medicalItemInfos.Length);
        return medicalItemInfos[id];
    }
    public UsableItemInfo GetMiscItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, miscItemInfos.Length);
        return miscItemInfos[id];
    }

    public UsableItemInfo GetPetItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, petItemInfos.Length);
        return petItemInfos[id];
    }
    #endregion


    #region setter
    #endregion


    #region UnityFunction
    void Awake()
    {
        InitWepaonInfo();
        InitMiscItems();
        // initializedBulletInfosAtRuntime = new List<BulletInfo>();
        // initializedBulletInfosLength = 0;
    }

    #endregion
    #region Function

    private void InitMiscItems()
    {
        for (int i = 0; i < miscItemInfos.Length; i++)
            miscItemInfos[i].SetID(i);
    }

    /// <summary> 무기 정보 관련 초기화 </summary>
    public void InitWepaonInfo()
    {
        switch(DebugSetting.Instance.weaponModeForDebug)
        {
            case WeaponModeForDebug.TEST:
                for (int i = 0; i < weaponInfos.Length; i++)
                    weaponInfos[i].Init();
                break;
            case WeaponModeForDebug.TEMP:
                for (int i = 0; i < tempWeaponInfos.Length; i++)
                    tempWeaponInfos[i].Init();
                break;
            case WeaponModeForDebug.TEMP2:
                for (int i = 0; i < temp2WeaponInfos.Length; i++)
                    temp2WeaponInfos[i].Init();
                break;
            case WeaponModeForDebug.TEST2:
                for (int i = 0; i < test2WeaponInfos.Length; i++)
                    test2WeaponInfos[i].Init();
                break;
            case WeaponModeForDebug.A1:
                for (int i = 0; i < A1WeaponInfos.Length; i++)
                    A1WeaponInfos[i].Init();
                break;
            case WeaponModeForDebug.TESTBOSS:
                for (int i = 0; i < testBossWeaponInfos.Length; i++)
                    testBossWeaponInfos[i].Init();
                break;
            default:
                break;
        }

        for (int i = 0; i < enemyWeaponInfos.Length; i++)
        {
            enemyWeaponInfos[i].Init();
        }
        //passiveItems   
        InputWeaponDatas();
    }

    /*
    public void AddInitialedbulletInfo(BulletInfo info)
    {
        initializedBulletInfosAtRuntime.Add(info);
        initializedBulletInfosLength += 1;
    }
    */

    public void InputWeaponDatas()
    {
        if (WeaponModeForDebug.TEST == DebugSetting.Instance.weaponModeForDebug
            || WeaponModeForDebug.TEST2 == DebugSetting.Instance.weaponModeForDebug
            || WeaponModeForDebug.A1 == DebugSetting.Instance.weaponModeForDebug)
            return;

        if (false == canInputWeaponDatas)
            return;
        weaponDatas = WeaponDataCSVParser.Read("weaponDatas");
        Debug.Log("CSV 데이터 파싱 후 weapon data 입력");

        WeaponType weaponType;
        AttackAniType attackAniType;
        Rating rating;
        float chargeTimeMax;
        float criticalChance;
        float damage;
        int staminaConsumption;
        float cooldown;
        int ammoCapacity;
        float range;
        float bulletSpeed;
        int size = weaponDatas.Count;
        for (int i = 0; i < size; i++)
        {
            tempWeaponInfos[i].weaponName = (string)weaponDatas[i]["name"];
            //Debug.Log(i + ", name : " + (string)weaponDatas[i]["name"]);

            tempWeaponInfos[i].scaleX = 1.0f;
            tempWeaponInfos[i].scaleY = 1.0f;

            weaponType = (WeaponType)System.Enum.Parse(typeof(WeaponType), (string)weaponDatas[i]["weaponType"]);
            tempWeaponInfos[i].weaponType = weaponType;

            attackAniType = (AttackAniType)System.Enum.Parse(typeof(AttackAniType), (string)weaponDatas[i]["attackAniType"]);
            tempWeaponInfos[i].attackAniType = attackAniType;
            //Debug.Log(attackAniType);

            rating = (Rating)System.Enum.Parse(typeof(Rating), (string)weaponDatas[i]["rating"]);
            tempWeaponInfos[i].rating = rating;
            //Debug.Log(rating);


            float.TryParse(weaponDatas[i]["chargeTimeMax"].ToString(), out chargeTimeMax);
            tempWeaponInfos[i].chargeTimeMax = chargeTimeMax;
            //Debug.Log(chargeTimeMax);
            if (0 == chargeTimeMax)
                tempWeaponInfos[i].touchMode = TouchMode.Normal;
            else
                tempWeaponInfos[i].touchMode = TouchMode.Charge;

            float.TryParse(weaponDatas[i]["criticalChance"].ToString(), out criticalChance);
            tempWeaponInfos[i].criticalChance = criticalChance;
            //Debug.Log(criticalChance);

            float.TryParse(weaponDatas[i]["damage"].ToString(), out damage);
            tempWeaponInfos[i].damage = damage;
            //Debug.Log(damage);

            int.TryParse(weaponDatas[i]["staminaConsumption"].ToString(), out staminaConsumption);
            tempWeaponInfos[i].staminaConsumption = staminaConsumption;
            //Debug.Log(staminaConsumption);

            float.TryParse(weaponDatas[i]["cooldown"].ToString(), out cooldown);
            tempWeaponInfos[i].cooldown = cooldown;
            //Debug.Log(cooldown);

            int.TryParse(weaponDatas[i]["ammoCapacity"].ToString(), out ammoCapacity);
            tempWeaponInfos[i].ammoCapacity = ammoCapacity;
            tempWeaponInfos[i].ammo = ammoCapacity;
            //Debug.Log(ammoCapacity);

            float.TryParse(weaponDatas[i]["range"].ToString(), out range);
            tempWeaponInfos[i].range = range;
            //Debug.Log(range);

            float.TryParse(weaponDatas[i]["bulletSpeed"].ToString(), out bulletSpeed);
            tempWeaponInfos[i].bulletMoveSpeed = bulletSpeed;
            //Debug.Log(bulletSpeed);

            switch (weaponType)
            {
                case WeaponType.PISTOL:
                case WeaponType.SHOTGUN:
                case WeaponType.MACHINEGUN:
                case WeaponType.SNIPER_RIFLE:
                    tempWeaponInfos[i].showsMuzzleFlash = true;
                    break;
                case WeaponType.LASER:
                    tempWeaponInfos[i].cooldown = 0f;
                    tempWeaponInfos[i].cameraShakeAmount = 0f;
                    tempWeaponInfos[i].cameraShakeTime = 0f;
                    break;
                default:
                    tempWeaponInfos[i].showsMuzzleFlash = false;
                    break;
            }

        //    NULL, PISTOL, SHOTGUN, MACHINEGUN, SNIPER_RIFLE, LASER, BOW,
        //SPEAR, CLUB, SPORTING_GOODS, SWORD, CLEANING_TOOL, KNUCKLE,
        //BOMB, GAS_SHELL, TRAP,
        //WAND, TRASH, OTHER, END
            switch (weaponType)
            {
                case WeaponType.PISTOL:
                case WeaponType.SHOTGUN:
                case WeaponType.MACHINEGUN:
                case WeaponType.SNIPER_RIFLE:
                case WeaponType.BOW:
                case WeaponType.WAND:
                case WeaponType.TRASH:
                case WeaponType.OTHER:
                    tempWeaponInfos[i].cameraShakeAmount = 0.1f;
                    tempWeaponInfos[i].cameraShakeTime = 0.1f;
                    break;
                case WeaponType.SPEAR:
                case WeaponType.CLUB:
                case WeaponType.SPORTING_GOODS:
                case WeaponType.SWORD:
                case WeaponType.CLEANING_TOOL:
                case WeaponType.KNUCKLE:
                    tempWeaponInfos[i].cameraShakeAmount = 0.1f;
                    tempWeaponInfos[i].cameraShakeTime = 0.04f;
                    break;
                case WeaponType.BOMB:
                case WeaponType.TRAP:
                    tempWeaponInfos[i].cameraShakeAmount = 0f;
                    tempWeaponInfos[i].cameraShakeTime = 0f;
                    break;
                default:
                    break;
            }

            switch (weaponType)
            {
                case WeaponType.SPEAR:
                case WeaponType.CLUB:
                case WeaponType.SPORTING_GOODS:
                case WeaponType.SWORD:
                case WeaponType.CLEANING_TOOL:
                    tempWeaponInfos[i].addDirVecMagnitude = 1.2f;
                    break;
                case WeaponType.KNUCKLE:
                case WeaponType.SHOTGUN:
                case WeaponType.BOW:
                case WeaponType.WAND:
                case WeaponType.SNIPER_RIFLE:
                    tempWeaponInfos[i].addDirVecMagnitude = 0.5f;
                    break;
                case WeaponType.LASER:
                case WeaponType.MACHINEGUN:
                case WeaponType.TRASH:
                case WeaponType.OTHER:
                    tempWeaponInfos[i].addDirVecMagnitude = 0.3f;
                    break;
                case WeaponType.PISTOL:
                //    tempWeaponInfos[i].addDirVecMagnitude = 0.2f;
                   break;
                default:
                    tempWeaponInfos[i].addDirVecMagnitude = 0f;
                    break;
            }

            //시전 시간
            switch (tempWeaponInfos[i].attackAniType)
            {
                case AttackAniType.Strike:
                    tempWeaponInfos[i].soundId = 0;
                    tempWeaponInfos[i].castingTime = 0.25f;
                    break;
                case AttackAniType.Blow:
                    tempWeaponInfos[i].castingTime = 0.1f;
                    tempWeaponInfos[i].soundId = 3;
                    break;
                case AttackAniType.Swing:
                    tempWeaponInfos[i].castingTime = 0.3f;
                    tempWeaponInfos[i].soundId = 3;
                    break;
                case AttackAniType.Shot:
                    tempWeaponInfos[i].soundId = 0;
                    break;
                default:
                    break;
            }

            //sound
            switch (weaponType)
            {
                case WeaponType.BOMB:
                case WeaponType.TRAP:
                    tempWeaponInfos[i].soundId = 3;
                    break;
                case WeaponType.SHOTGUN:
                    tempWeaponInfos[i].soundId = 2;
                    break;
                case WeaponType.LASER:
                    tempWeaponInfos[i].soundId = -1;
                    break;
                default:
                    break;
            }
        }
        
    }
    #endregion
}