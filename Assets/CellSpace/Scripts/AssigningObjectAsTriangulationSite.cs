
// SPDX-FileCopyrightText: Copyright 2023 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Botao Amber Hu <botao.a.hu@gmail.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Linq;
using VoroGen;

namespace CellSpace {

    [RequireComponent(typeof(TriangulationGenerator))]
    public class AssigningObjectAsTriangulationSite : MonoBehaviour 
    {
        public GameObject target;
        // Update is called once per frame
        void Update()
        {
            if (target != null) {
                GetComponent<TriangulationGenerator>().sites 
                    = target.transform.Cast<Transform>()?.Select(p => p.gameObject).Where(p => p.activeSelf).ToArray();
            }
        }
    }
}
