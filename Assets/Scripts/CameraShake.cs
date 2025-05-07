using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    
    private CinemachineVirtualCameraBase cam;
    private CinemachineBasicMultiChannelPerlin noise;

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        
        cam = GetComponent<CinemachineVirtualCameraBase>();
        noise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake(float intensity, float time)
    {
        Debug.Log("Shake");
        noise.AmplitudeGain = intensity;
        
        startingIntensity = intensity;
        shakeTimer = time;
        shakeTimerTotal = time;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            noise.AmplitudeGain = Mathf.Lerp(0f, startingIntensity, shakeTimer / shakeTimerTotal);
        }
    }
}
