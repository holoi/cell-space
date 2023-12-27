// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Linq;
using VoroGen;

namespace CellSpace {

    [RequireComponent(typeof(VoronoiGenerator))]
    public class AssigningPlayerAndObjectAsVoronoiSite : MonoBehaviour 
    {
        public GameObject target;
        // Update is called once per frame
        void Update()
        {
            if (target != null) {
                GetComponent<VoronoiGenerator>().Sites 
                    = FindObjectsOfType<PlayerPoseSynchronizer>()?.Select(p => p.gameObject)
                        .Concat(target.transform.Cast<Transform>()?.Select(p => p.gameObject)).ToArray();
            } else {
                GetComponent<VoronoiGenerator>().Sites 
                    = FindObjectsOfType<PlayerPoseSynchronizer>()?.Select(p => p.gameObject).ToArray();
            }        
        }
    }
}
