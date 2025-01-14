﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillObject : MonoBehaviour
{
    protected LayerMask enemyLayer, enemyBulletLayer;
    protected CircleCollider2D circleCollider;
    protected SpriteRenderer spriteRenderer;
    protected Transform bodyTransform;
    protected Animator animator;
    protected List<SkillData> preSkillData, postSkillData;

    protected Character caster, other;
    protected CustomObject customObject;
    protected Vector3 scaleVector;

    protected bool isActive;
    protected bool isCollisionable;
    protected bool isAvailable;
    #region skillDataParameter
    protected float radius;
    protected float amount;
    protected string animName;
    protected float destroyTime;
    #endregion

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        bodyTransform = GetComponent<Transform>();
        scaleVector = Vector3.one;
    }

    protected void Init(SkillData skillData)
    {
        isActive = true;
        isAvailable = true;
        radius = skillData.Radius;
        amount = skillData.Amount;
        circleCollider.radius = radius;

        if(radius > 0)
            isCollisionable = true;
    }
    public void Init(string aniName)
    {
        animator.SetTrigger(aniName);
    }
    public void Init(Character other)
    {
        this.other = other;
    }
    public void Init(ref CustomObject customObject, SkillData skillData, float time)
    {
        Init(skillData);
        this.customObject = customObject;
        this.enemyLayer = UtilityClass.GetEnemyLayer(null);
        this.enemyBulletLayer = UtilityClass.GetEnemyBulletLayer(null);
        this.destroyTime = time;
        UtilityClass.Invoke(this, DestroyAndDeactive, time);
    }
    public void Init(ref Character caster, SkillData skillData, float time)
    {
        Init(skillData);
        this.caster = caster;
        this.enemyLayer = UtilityClass.GetEnemyLayer(caster);
        this.enemyBulletLayer = UtilityClass.GetEnemyBulletLayer(caster);
        this.destroyTime = time;
        UtilityClass.Invoke(this, DestroyAndDeactive, time);
    }
    public void SetSkillData(List<SkillData> preSkillData, List<SkillData> postSkillData)
    {
        this.preSkillData = preSkillData;
        this.postSkillData = postSkillData;
    }
    protected virtual void DestroyAndDeactive()
    {
        isAvailable = false;
        Destroy(this);
        this.gameObject.SetActive(false);
    }
}
/// <summary>
/// 충돌 스킬
/// </summary>
public class CollisionSkillObject : SkillObject
{
    protected StatusEffectInfo statusEffectInfo;
    protected CRangeEffect.EffectType effectType;
    public void Set(StatusEffectInfo statusEffectInfo)
    {
        this.statusEffectInfo = statusEffectInfo;
    }
    public void Set(CRangeEffect.EffectType effectType)
    {
        this.effectType = effectType;
    }
    public void SetAnim(string animName)
    {
        animator.SetTrigger(animName);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isCollisionable)
            return;
        if (UtilityClass.CheckLayer(collision.gameObject.layer, enemyLayer))
        {
            Character triggeredCharacter = collision.GetComponent<Character>();
            triggeredCharacter.Attacked(Vector2.zero, bodyTransform.position, amount, 0, 0);
            triggeredCharacter.ApplyStatusEffect(statusEffectInfo);
        }
        if (UtilityClass.CheckLayer(collision.gameObject.layer, enemyBulletLayer))
        {
            Bullet bullet = collision.transform.parent.GetComponent<Bullet>();
            if (!bullet)
                return;
            switch (effectType)
            {
                case CRangeEffect.EffectType.REMOVE:
                    bullet.DestroyBullet();
                    break;
                case CRangeEffect.EffectType.REFLECT:
                    bullet.SetOwnerType(UtilityClass.GetMainOnwerType(caster));
                    bullet.RotateDirection(180);
                    break;
                case CRangeEffect.EffectType.NONE:
                default:
                    break;
            }
        }
    }
}

/// <summary>
/// 날아가는 스킬
/// </summary>
public class ProjectileSkillObject : CollisionSkillObject
{
    protected float directionDegree;
    protected float speed, acceleration;
    protected Vector3 direction;
    protected bool isDestroy = true;
    protected bool isTimeBoom = false;
    protected bool isReachBoom = false;
    protected Vector3 dest;

    public void Set(string animName, float speed, float acceleration, Vector3 direction)
    {
        this.speed = speed;
        this.acceleration = acceleration;
        this.direction = direction.normalized;
        animator.SetTrigger(animName);

        StartCoroutine(CoroutineThrow());
    }
    public void Set(string attachedParticleName)
    {
        ParticleManager.Instance.PlayParticle(attachedParticleName, bodyTransform.position, bodyTransform);
    }
    public void Set(string particleName, float term)
    {
        if (particleName == "")
            return;
        StartCoroutine(CoroutineParticle(particleName, term));
    }
    public void Set(bool isDestroy, bool isTimeBoom)
    {
        this.isDestroy = isDestroy;
        this.isTimeBoom = isTimeBoom;
    }
    public void SetReachBoom(Vector3 dest)
    {
        this.dest = dest;
        isReachBoom = true;
    }

    protected IEnumerator CoroutineParticle(string particleName, float term)
    {
        while (isActive)
        {
            ParticleManager.Instance.PlayParticle(particleName, bodyTransform.position);
            yield return YieldInstructionCache.WaitForSeconds(term);
        }
    }

    protected IEnumerator CoroutineThrow()
    {
        float elapsedDist = 0;
        float elapsedTime = 0;
        float deltaTime = 0;
        while (speed > 0)
        {
            if(isReachBoom)
            {
                if(Vector2.Distance(bodyTransform.localPosition,dest) < .1f)
                {
                    break;
                }
            }

            directionDegree = direction.GetDegFromVector();
            if (-90 <= directionDegree && directionDegree < 90)
            {
                scaleVector.x = Mathf.Abs(scaleVector.x);
                bodyTransform.localScale = scaleVector;
            }
            else
            {
                scaleVector.x = -Mathf.Abs(scaleVector.x);
                bodyTransform.localScale = scaleVector;
            }

            deltaTime = Time.deltaTime;

            bodyTransform.localPosition = bodyTransform.localPosition + direction * speed * deltaTime;
            elapsedDist += speed * deltaTime;
            elapsedTime += deltaTime;
            speed += acceleration * elapsedTime * deltaTime;
            
            if (!isActive)
                break;
            destroyTime -= deltaTime;
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }
        isActive = false;

        while(destroyTime >=0)
        {
            destroyTime -= Time.deltaTime;
            yield return YieldInstructionCache.WaitForSeconds(.1f);
        }

        if (postSkillData != null && postSkillData.Count > 0)
        {
            animator.SetTrigger("default");
            float lapsedTime = 9999;
            foreach (SkillData item in postSkillData)
            {
                if (null == item)
                    continue;
                if (other)
                    item.Run(caster, other, bodyTransform.position, ref lapsedTime);
                else if (caster)
                    item.Run(caster, bodyTransform.position, ref lapsedTime);
                if (customObject)
                    item.Run(customObject, bodyTransform.position, ref lapsedTime);
            }
        }

        DestroyAndDeactive();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isCollisionable)
            return;
        base.OnTriggerEnter2D(collision);

        if (UtilityClass.CheckLayer(collision.gameObject.layer, enemyLayer) ||
            UtilityClass.CheckLayer(collision.gameObject.layer, 14, 1))
        {
            StopCoroutine(CoroutineThrow());
            isAvailable = false;
            animator.SetTrigger("default");
            float lapsedTime = 9999;
            foreach (SkillData item in postSkillData)
            {
                if (null == item)
                    continue;
                if (other)
                    item.Run(caster, other, bodyTransform.position, ref lapsedTime);
                else if (caster)
                    item.Run(caster, bodyTransform.position, ref lapsedTime);
                if (customObject)
                    item.Run(customObject, bodyTransform.position, ref lapsedTime);
            }

            if (isDestroy)
            {
                DestroyAndDeactive();
            }
        }
    }

    protected override void DestroyAndDeactive()
    {
        base.DestroyAndDeactive();
        if (!isTimeBoom)
            return;
        float lapsedTime = 9999;

        foreach (SkillData item in postSkillData)
        {
            if (null == item)
                continue;
            if (other)
                item.Run(caster, other, bodyTransform.position, ref lapsedTime);
            else if (caster)
                item.Run(caster, bodyTransform.position, ref lapsedTime);
            if (customObject)
                item.Run(customObject, bodyTransform.position, ref lapsedTime);
        }
    }
}

/// <summary>
/// 전달하는 스킬
/// ex) 힐 버프 스킬을 피격자에게 전달
/// </summary>
public class PassSkillObject : ProjectileSkillObject
{
    CharacterInfo.OwnerType target;

    public void Set(CharacterInfo.OwnerType target)
    {
        this.target = target;
        enemyLayer = UtilityClass.GetOwnerLayer(target);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isCollisionable)
            return;
        if (UtilityClass.CheckLayer(collision.gameObject.layer, enemyLayer) ||
            UtilityClass.CheckLayer(collision.gameObject.layer, 14, 1))
        {
            Character collisionCaster = collision.GetComponent<Character>();
            CustomObject collisionObject = collision.GetComponent<CustomObject>();

            if (collisionCaster == caster)
                return;

            StopCoroutine(CoroutineThrow());
            isAvailable = false;
            animator.SetTrigger("default");
            float lapsedTime = 9999;
            foreach (SkillData item in postSkillData)
            {
                if (null == item)
                    continue;
                if (other)
                {
                    if (collisionCaster)
                    {
                        if(collisionCaster.GetOwnerType() == target)
                            item.Run(collisionCaster, other, bodyTransform.position, ref lapsedTime);
                    }
                }
                else
                {
                    if (collisionCaster)
                    {
                        if (collisionCaster.GetOwnerType() == target)
                            item.Run(collisionCaster, bodyTransform.position, ref lapsedTime);
                    }
                    if (collisionObject)
                        if (CharacterInfo.OwnerType.OBJECT == target)
                            item.Run(collisionObject, bodyTransform.position, ref lapsedTime);
                }
            }
            if (isDestroy)
                DestroyAndDeactive();
        }
    }
}
