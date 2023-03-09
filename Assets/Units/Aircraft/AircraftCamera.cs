using UnityEngine; // Required to access Unity's GameObject, MonoBehaviour and other important classes.
using Cinemachine; // Required to access Cinemachine camera classes for Unity.

public class AircraftCamera : MonoBehaviour
{
    #region Variables that are assigned
    [Header("References")]
    [SerializeField] private AircraftController airPlaneController; // Reference to the AircraftController script
    [SerializeField] private CinemachineFreeLook freeLook; // Reference to the CinemachineFreeLook component
    [Header("Camera values")]
    [SerializeField] private float cameraDefaultFov = 60f; // The default field of view for the camera
    [SerializeField] private float cameraTurboFov = 40f; // The field of view for the camera when the aircraft is in turbo mode
    #endregion

    #region Basic
    private void Update()
    {
        CameraFovUpdate();
    }

    // Updates the field of view of the camera based on the input from the player
    private void CameraFovUpdate()
    {
        // Turbo
        if (!airPlaneController.PlaneIsDead()) // Check if the aircraft is not dead
        {
            if (Input.GetKey(KeyCode.LeftShift)) // Check if the player is holding down the left shift key
            {
                ChangeCameraFov(cameraTurboFov); // Change the camera's field of view to the turbo mode value
            }
            else // If the player is not holding down the left shift key
            {
                ChangeCameraFov(cameraDefaultFov); // Change the camera's field of view to the default value
            }
        }
    }

    // Changes the field of view of the camera
    public void ChangeCameraFov(float _fov)
    {
        float _deltatime = Time.deltaTime * 100f; // Get the delta time multiplied by 100
        freeLook.m_Lens.FieldOfView = Mathf.Lerp(freeLook.m_Lens.FieldOfView, _fov, 0.05f * _deltatime); // Interpolates the field of view of the camera to the new value over time
    }
    #endregion
}
