using System.Collections;
using System.Collections.Generic;
using OscJack;
using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
public class PropertySetter : NetworkBehaviour
{
    [SerializeField] private OscPropertySender _positionSender;
    [SerializeField] private OscPropertySender _rotationSender;

    public override void OnNetworkSpawn()
    {
    //    //Debug.Log()
    //    if (_positionSender)
    //        _positionSender._oscAddress = "/a" + OwnerClientId + "/position";
    //    if (_rotationSender)
    //        _rotationSender._oscAddress = "/a" + OwnerClientId + "/rotation";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
