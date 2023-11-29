using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Networking.Transport;


public class CentralController : NetworkBehaviour
{

    private int Index;
    public NetworkVariable<bool> dancerState;
    public NetworkVariable<bool> cellState;
    public NetworkVariable<float> scale;
   

    // Update is called once per frame
    void Update()
    { 
        
    }

    public void OnDancerStateChanged(bool previous, bool current)
    {
       if(dancerState.Value)//Is dancer
        {
            Debug.Log("Player "+Index+"changed to a dancer");
        }
        else
        {
            Debug.Log("Player " + Index + "is not a dancer now");
        }
    }
    public void OnCellStateChanged(bool previous, bool current)
    {
        if (dancerState.Value)//Is dancer
        {
            Debug.Log("Player " + Index + "is a cell");
        }
        else
        {
            Debug.Log("Player " + Index + "is not a cell");
        }
    }

    public override void OnNetworkSpawn()
    {
        dancerState.OnValueChanged += OnDancerStateChanged;
        cellState.OnValueChanged += OnCellStateChanged;
        Index = NetworkManager.Singleton.ConnectedClients.Count;
        if (IsOwner)
        {
            InitializeServerRpc();
        }

        
    }

    

    public bool getDancerState()
    {
        return dancerState.Value;
    }

    public bool getCellState()
    {
        return cellState.Value;
    }
    
  

    [ServerRpc]
    public void InitializeServerRpc()
    {
        dancerState.Value = false;
        cellState.Value = true;
        scale.Value = 1.0f;
    }
   
    
}

