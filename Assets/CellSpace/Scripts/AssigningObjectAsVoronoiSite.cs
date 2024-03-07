// SPDX-FileCopyrightText: Copyright 2023 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Botao Amber Hu <botao.a.hu@gmail.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Linq;
using VoroGen;

namespace CellSpace {

    [RequireComponent(typeof(VoronoiGenerator))]
    public class AssigningObjectAsVoronoiSite : MonoBehaviour 
    {
        public GameObject target;
        // Update is called once per frame
        void Update()
        {
            if (target != null) {
                GetComponent<VoronoiGenerator>().Sites 
                    = target.transform.Cast<Transform>()?.Select(p => p.gameObject).Where(p => p.activeSelf).ToArray();
            }
        }
    }
}
