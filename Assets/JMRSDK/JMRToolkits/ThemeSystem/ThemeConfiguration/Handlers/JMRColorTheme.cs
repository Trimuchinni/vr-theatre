// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JMRSDK.Toolkit.ThemeSystem
{
    public static class JMRColorTheme 
    {
        public static void SetColor(Transform bTransform, string colorHex)
        {
            ColorUtility.TryParseHtmlString(colorHex, out Color colorRGB);
//#if UNITY_EDITOR
//            UnityEditor.EditorUtility.SetDirty(bTransform);
//#endif
            if (TrySetImageViewColor(colorRGB, bTransform))
            {
                return;
            }

            if (TrySetRendererColor(colorRGB, bTransform))
            {
                return;
            }

            if (TrySetTextMeshProColor(colorRGB, bTransform))
            {
                return;
            }
            if (TrySetTextMeshProUGUIColor(colorRGB, bTransform))
            {
                return;
            }
            if (TrySetUGUITextColor(colorRGB, bTransform))
            {
                return;
            }
            if (TrySetSpriteRendererColor(colorRGB, bTransform))
            {
                return;
            }

            //   Debug.Log("Apply on object :  " + bTransform.root.name);
        }


        private static bool TrySetImageViewColor(Color color, Transform host)
        {
            Image tmp = host.GetComponent<Image>();
            if (tmp)
            {
                tmp.color = new Color(color.r,color.g,color.b,tmp.color.a);
                return true;
            }

            return false;
        }

        private static bool TrySetTextMeshProColor(Color color, Transform host)
        {
            TextMeshPro tmp = host.GetComponent<TextMeshPro>();
            if (tmp)
            {
                tmp.color = new Color(color.r, color.g, color.b, tmp.color.a);
                return true;
            }

            return false;
        }

        private static bool TrySetTextMeshProUGUIColor(Color color, Transform host)
        {
            TextMeshProUGUI tmp = host.GetComponent<TextMeshProUGUI>();
            if (tmp)
            {
                tmp.color = new Color(color.r, color.g, color.b, tmp.color.a);
                return true;
            }

            return false;
        }

        private static bool TrySetUGUITextColor(Color color, Transform host)
        {
            Text tmp = host.GetComponent<Text>();
            if (tmp)
            {
                tmp.color = new Color(color.r, color.g, color.b, tmp.color.a);
                return true;
            }

            return false;
        }

        private static bool TrySetSpriteRendererColor(Color color, Transform host)
        {
            SpriteRenderer tmp = host.GetComponent<SpriteRenderer>();
            if (tmp)
            {
                tmp.materials[0].color = new Color(color.r, color.g, color.b, tmp.materials[0].color.a);
                return true;
            }

            return false;
        }


        //TO DO :  Render color should change by shaders only so this needs to be changed.
        // Hard coded index should change as well

        private static bool TrySetRendererColor(Color color, Transform host)
        {
            MeshRenderer tmp = host.GetComponent<MeshRenderer>();
            if (tmp)
            {
                tmp.materials[0].color = new Color(color.r, color.g, color.b, tmp.materials[0].color.a);
                return true;
            }

            return false;
        }
    }

}
