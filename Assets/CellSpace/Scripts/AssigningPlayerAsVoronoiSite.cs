// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Linq;
using VoroGen;

namespace CellSpace {

    [RequireComponent(typeof(VoronoiGenerator))]
    public class AssigningPlayerAsVoronoiSite : MonoBehaviour 
    {
        // Update is called once per frame
        void Update()
        {
            GetComponent<VoronoiGenerator>().sites 
                = FindObjectsOfType<PlayerPoseSynchronizer>()?.Select(p => p.gameObject).ToArray();
        }
    }
}
