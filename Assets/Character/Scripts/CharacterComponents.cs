﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterComponents : MonoBehaviour
{
    #region components
    [SerializeField]
    private WeaponManager weaponManager;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Transform spriteTransform;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AnimationHandler animationHandler;
    [SerializeField]
    private CircleCollider2D interactiveCollider2D;
    [SerializeField]
    private CircleCollider2D circleCollider2D;
    [SerializeField]
    private BoxCollider2D hitBox;
    [SerializeField]
    private Transform shadowTransform;

    #endregion
    #region parameter
    public WeaponManager WeaponManager
    {
        get
        {
            return weaponManager;
        }
    }
    public SpriteRenderer SpriteRenderer
    {
        get
        {
            return spriteRenderer;
        }
    }
    public Transform SpriteTransform
    {
        get
        {
            return spriteTransform;
        }
    }
    public Animator Animator
    {
        get
        {
            return animator;
        }
    }
    public AnimationHandler AnimationHandler
    {
        get
        {
            return animationHandler;
        }
    }
    public CircleCollider2D InteractiveCollider2D
    {
        get
        {
            return interactiveCollider2D;
        }
    }
    public CircleCollider2D CircleCollider2D
    {
        get
        {
            return circleCollider2D;
        }
    }
    public BoxCollider2D HitBox
    {
        get
        {
            return hitBox;
        }
    }
    public Transform ShadowTransform
    {
        get
        {
            return shadowTransform;
        }
    }

    public BuffManager BuffManager { get; private set; }
    public Rigidbody2D Rigidbody2D { get; private set; }
    public AIController AIController { get; private set; }
    #endregion
    #region Func
    public void Init()
    {
        BuffManager = GetComponent<BuffManager>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        AIController = GetComponent<AIController>();
    }
    #endregion

}
