// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Linq;

namespace CellSpace {

    [RequireComponent(typeof(CellSpaceVoronoiGenerator))]
    public class AssigningObjectAsVoronoiSite : MonoBehaviour 
    {
        public GameObject target;
        // Update is called once per frame
        void Update()
        {
            if (target != null) {
                GetComponent<CellSpaceVoronoiGenerator>().sites 
                    = target.transform.Cast<Transform>().Select(p => p.gameObject).ToArray();
            }
        }
    }
}
