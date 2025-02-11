﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 연출에 따라서 지속적으로 하면 어지러운 것들이 좀 있어서
// 무기 사용시 넉백같은 느낌으로 카메라 흔든것도 있는데 가만히 있을 때 쓰면 느껴지는데
// 움직이면서 카메라 따라오면서 쉐이크하면 느낌이 안나고 어지러움증 유발함
// 
// 봐가지고 스킬이나 기타 큼직큼직한 연출에서 써야할 듯.

public class CameraController : MonoBehaviourSingleton<CameraController> {
    public enum IncreaseType { NORMAL, INCREASE, DECREASE }
    Transform cameraTransform;
    Vector2 m_velocity = Vector2.zero;
    [SerializeField]
    float m_cameraDepth = -1;
    Vector3 zeroPos;
    private bool isShaking;
    new Camera camera;
    private void Awake()
    {
        cameraTransform = this.transform;
        zeroPos = new Vector3(0, 0, m_cameraDepth);
        camera = GetComponent<Camera>();

    }
    IEnumerator CoroutineShaking(float duration, float magnitude, IncreaseType increaseType)
    {
        float elapsed = 0.0f;
        float value = 1;

        while (elapsed < duration)
        {
            if(increaseType == IncreaseType.INCREASE)
            {
                value = elapsed / duration;
            }
            else if(increaseType == IncreaseType.DECREASE)
            {
                value = (duration - elapsed) / duration;
            }
            float x = Random.Range(-1.0f, 1.0f) * magnitude * value;
            float y = Random.Range(-1.0f, 1.0f) * magnitude * value;

            cameraTransform.localPosition = new Vector3(x, y, m_cameraDepth);

            elapsed += Time.deltaTime;

            yield return null;
        }

        cameraTransform.localPosition = zeroPos;
        isShaking = false;
    }
    #region func
    public Vector3 worldToScreen(Vector3 worldPos)
    {
        return camera.WorldToScreenPoint(worldPos);
    }
    public void ComeBackPosition()
    {
        cameraTransform.localPosition = zeroPos;
    }
    public void AttachObject(Transform targetTransform)
    {
        cameraTransform.parent = targetTransform;
        cameraTransform.localPosition = zeroPos;
    }
    public void FindOther(Vector2 dest)
    {
        cameraTransform.position = new Vector3(dest.x,dest.y,m_cameraDepth);
    }
    public void Shake(float amount, float time, IncreaseType increaseType = IncreaseType.NORMAL)
    {
        if (isShaking)
            return;
        isShaking = true;
        StartCoroutine(CoroutineShaking(time, amount, increaseType));
    }
    void SlideOther(Vector2 _targetPos)
    {
        Vector2 temp = Vector2.SmoothDamp(cameraTransform.position, _targetPos, ref m_velocity, 5, 0.5f, .45f);
        cameraTransform.position = new Vector3(temp.x, temp.y, m_cameraDepth);
    }
    #endregion
}
