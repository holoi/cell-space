﻿/*===============================================================================
Copyright (C) 2019 Immersal Ltd. All Rights Reserved.

This file is part of the Immersal AR Cloud SDK Early Access project.

The Immersal AR Cloud SDK cannot be copied, distributed, or made available to
third-parties for commercial purposes without written permission of Immersal Ltd.

Contact sdk@immersal.com for licensing requests.
===============================================================================*/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Immersal.Samples.Navigation
{
    public class NavigationTargetListButton : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector]
        public GameObject targetObject = null;

        [SerializeField]
        private TextMeshProUGUI m_TextMeshProUGUI = null;
        [SerializeField]
        private Image m_Image = null;

        private string targetName = null;

        public void SetText(string text)
        {
            targetName = text;
            if (m_TextMeshProUGUI != null)
            {
                m_TextMeshProUGUI.text = targetName;
            }
        }

        public void SetIcon(Sprite icon)
        {
            if (m_Image != null)
            {
                m_Image.sprite = icon;
            }
        }

        public void SetTarget(GameObject go)
        {
            targetObject = go;
        }

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            NavigationManager.Instance.TryToFindPath(this);
        }
    }
}