using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AircraftCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AircraftController airPlaneController;
    [SerializeField] private CinemachineFreeLook freeLook;
    [Header("Camera values")]
    [SerializeField] private float cameraDefaultFov = 60f;
    [SerializeField] private float cameraTurboFov = 40f;

    private void Update()
    {
        CameraFovUpdate();
    }

    private void CameraFovUpdate()
    {
        //Turbo
        if (!airPlaneController.PlaneIsDead())
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                ChangeCameraFov(cameraTurboFov);
            }
            else
            {
                ChangeCameraFov(cameraDefaultFov);
            }
        }
    }

    public void ChangeCameraFov(float _fov)
    {
        float _deltatime = Time.deltaTime * 100f;
        freeLook.m_Lens.FieldOfView = Mathf.Lerp(freeLook.m_Lens.FieldOfView, _fov, 0.05f * _deltatime);
    }
}