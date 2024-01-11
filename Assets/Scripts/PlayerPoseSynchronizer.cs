using UnityEngine;
using Unity.Netcode;
//using Immersal.AR;
using HoloInteractive.XR.HoloKit;

public class PlayerPoseSynchronizer : NetworkBehaviour
{
    private Transform m_CenterEyePose;
    private HoloKitMarkController m_HoloKitMark;
    [SerializeField] private HoloKitMarkController m_HoloKitMarkPrefab;

    private void Start()
    {
        m_HoloKitMark = Instantiate(m_HoloKitMarkPrefab);
        m_HoloKitMark.PlayerPoseSynchronizer = transform;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (m_HoloKitMark)
            Destroy(m_HoloKitMark.gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            var holokitCameraManager = FindFirstObjectByType<HoloKitCameraManager>();
            if (holokitCameraManager == null)
            {
                Debug.LogWarning("[PlayerPoseSynchronizer_ImageTrackingRelocalization] Failed to find HoloKitCameraManager in the scene");
            }
            m_CenterEyePose = holokitCameraManager.CenterEyePose;
        }
    }

    private void Update()
    {
        if (IsSpawned && IsOwner && m_CenterEyePose != null)
            transform.SetPositionAndRotation(m_CenterEyePose.position, m_CenterEyePose.rotation);
    }
}
