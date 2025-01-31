using UnityEngine;
using HoloInteractive.XR.HoloKit;

public class HoloKitMarkController : MonoBehaviour
{
    public Transform PlayerPoseSynchronizer;

    [SerializeField] private Vector3 m_Offset = new(0f, 0.15f, 0f);

    private Transform m_CenterEyePose;

    private void Start()
    {
        m_CenterEyePose = FindObjectOfType<HoloKitCameraManager>().CenterEyePose;
    }

    private void LateUpdate()
    {
        transform.position = PlayerPoseSynchronizer.position + m_Offset;
        transform.rotation = Quaternion.Euler(0f, m_CenterEyePose.rotation.eulerAngles.y, 0f);
    }
}
