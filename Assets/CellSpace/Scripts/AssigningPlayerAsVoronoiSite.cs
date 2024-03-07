// SPDX-FileCopyrightText: Copyright 2023 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Botao Amber Hu <botao.a.hu@gmail.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Linq;
using VoroGen;
using HoloKit.ColocatedMultiplayerBoilerplate;

namespace CellSpace {

    [RequireComponent(typeof(VoronoiGenerator))]
    public class AssigningPlayerAsVoronoiSite : MonoBehaviour 
    {
        // Update is called once per frame
        void Update()
        {
            GetComponent<VoronoiGenerator>().Sites 
                = FindObjectsOfType<PlayerPoseSynchronizer>()?.Select(p => p.gameObject).ToArray();
        }
    }
}
