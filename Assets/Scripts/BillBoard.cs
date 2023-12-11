using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.Editor;
public class BillBoard : MonoBehaviour
{
    Transform camTransform;
    private void Awake()
    {
        camTransform = Camera.main.transform;
    }
    private void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdate);
    }
    private void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdate);
    }
    void OnCameraUpdate(CinemachineBrain brain)
    {
        transform.forward = camTransform.forward;
    }
}
