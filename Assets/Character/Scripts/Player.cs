﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
#region variables
#endregion
#region getter
#endregion
#region setter
#endregion
#region UnityFunction
#endregion
#region Function
#endregion
*/

public abstract class Character : MonoBehaviour
{
    protected enum State { NOTSPAWNED, DIE, ALIVE }
    protected Sprite sprite;
    protected Animator animator;
    protected float hp;
    protected State pState;
    public CircleCollider2D interactiveCollider2D;
    public float moveSpeed;     // Character move Speed

    public abstract void Die();
}


// 무기 클래스 활용겸 테스트용 player
public class Player : Character
{
    #region variables

    public enum PlayerState { IDLE, DASH, KNOCKBACK, DEAD }
    //public Joystick joystick;


    [SerializeField]
    private PlayerController controller;    // 플레이어 컨트롤 관련 클래스
    private BuffManager buffManager;
    private Transform objTransform;
    private PlayerState state;
    private float playerScale;      // player 크기
    private Vector3 scaleVector;    // player scale 우측 (1, 1, 1), 좌측 : (-1, 1, 1) 
    private float directionDegree;  // 바라보는 각도(총구 방향)
    private bool isRightDirection;    // player 방향이 우측이냐(true) 아니냐(flase = 좌측)

    public WeaponManager weaponManager;
    #endregion

    #region getter
    public PlayerController PlayerController { get { return controller; } }
    public PlayerState State { get { return state; } }
    public bool GetRightDirection() { return isRightDirection; }
    public float GetDirDegree (){ return directionDegree; }
    public Vector3 GetInputVector () { return controller.GetInputVector(); }
    public Vector3 GetRecenteInputVector() { return controller.GetRecenteInputVector(); }
    public Vector3 GetPosition() { return objTransform.position; }
    public BuffManager GetBuffManager() { return buffManager; }
    public WeaponManager GetWeaponManager() { return weaponManager; }
    #endregion
    #region setter
    #endregion

    #region UnityFunction
    void Awake()
    {
        state = PlayerState.IDLE;
        objTransform = GetComponent<Transform>();
        playerScale = 1f;
        scaleVector = new Vector3(playerScale, playerScale, 1);
        buffManager = new BuffManager();
        isRightDirection = true;
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        directionDegree = controller.GetRecenteInputVector().GetDegFromVector();
        // 각도에 따른 player 우측, 좌측 바라보기
        if (-90 <= directionDegree && directionDegree < 90)
        {
            isRightDirection = true;
            scaleVector.x = playerScale;
            objTransform.localScale = scaleVector;
        }
        else
        {
            isRightDirection = false;
            scaleVector.x = -playerScale;
            objTransform.localScale = scaleVector;
        }   
    }

    void FixedUpdate()
    {
        Move();
    }
    #endregion

    #region function
    public void Init()
    {
        // weaponManager 초기화, 바라보는 방향 각도, 방향 벡터함수 넘기기 위해서 해줘야됨
        weaponManager.Init(this);
        // 공격 버튼에 player class 넘기기
        GameObject.Find("AttackButton").GetComponent<AttackButton>().SetPlayer(this);
        GameObject.Find("WeaponSwitchButton").GetComponent<WeaponSwitchButton>().SetPlayer(this);

        controller = new PlayerController(GameObject.Find("VirtualJoystick").GetComponent<Joystick>());
    }
    public override void Die()
    {
        throw new System.NotImplementedException();
    }
    public bool Interact()
    {
        float bestDistance = interactiveCollider2D.radius * 10;
        Collider2D bestCollider = null;

        Collider2D[] collider2D = Physics2D.OverlapCircleAll(transform.position, interactiveCollider2D.radius);

        for (int i = 0; i < collider2D.Length; i++)
        {
            if (null == collider2D[i].GetComponent<CustomObject>())
                continue;
            if (!collider2D[i].GetComponent<CustomObject>().isAvailable)
                continue;
            float distance = Vector2.Distance(transform.position, collider2D[i].transform.position);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestCollider = collider2D[i];
            }
        }

        if (null == bestCollider)
            return false;
        bestCollider.GetComponent<CustomObject>().Active();
        return true;
    }
    // 캐릭터 이동, WASD Key, 테스트 용  
    private void Move()
    {
        // 조이스틱 방향으로 이동
        objTransform.Translate(controller.GetInputVector() * moveSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }
    }
    // 공격 가능 여부 리턴
    public bool AttackAble()
    {
        if (state == PlayerState.IDLE)
            return true;
        else return false;
    }
    #endregion
}

// Player 조작 관련 Class
[System.Serializable]
public class PlayerController
{
    [SerializeField]
    private Joystick joystick; // 조이스틱 스크립트

    // 조이스틱 방향
    private Vector3 inputVector;

    
    public PlayerController(Joystick joystick)
    {
        this.joystick = joystick;
    }

    // 조이스틱 바라보는 방향 벡터 
    public Vector3 GetInputVector()
    {    
        float h = joystick.GetHorizontalValue();
        float v = joystick.GetVerticalValue();

        // 조이스틱 일정 거리 이상 움직였을 때 부터 조작 적용.
        if (h * h + v * v > 0.01)
        {
            inputVector = new Vector3(h, v, 0).normalized;
        }
        else inputVector = Vector3.zero;

        return inputVector;
    }

    // 조이스틱 터치 유무에 상관없이 가장 최근 Input vector
    public Vector3 GetRecenteInputVector()
    {
        return joystick.GetRecenteInputVector();
    }
    
}