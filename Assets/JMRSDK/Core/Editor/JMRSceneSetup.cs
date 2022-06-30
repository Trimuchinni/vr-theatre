// Copyright (c) 2020 JioGlass. All Rights Reserved.
// Root class for design system
// Responsible for managing all the extended features in future
//#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JMRSDK.Toolkit
{
    [InitializeOnLoad]
    internal class JMRSceneSetup
    {
        // Assets GUID's Don't change
        private const string JioMRPrefabGUID = "633a1e3825e2f6e4c81ef8bf448d11cd";

        #region Static variables

        private static string ToolkitManagerPrefabPath => AssetDatabase.GUIDToAssetPath(JioMRPrefabGUID);
        #endregion

        
        [MenuItem("JioMixedReality/Configure scene for JioMixedReality")]
        static void ConfigureToolkitManager()
        {
            if (Application.isEditor)
            {
                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(ToolkitManagerPrefabPath, typeof(GameObject));

                //Step 1 Remove Main Camera

                string[] Tags = new string[] { "MainCamera" };


                List<GameObject> gameObjects = new List<GameObject>();
                foreach (string Tag in Tags)
                {
                    gameObjects.AddRange(GameObject.FindGameObjectsWithTag(Tag));
                }
                foreach (var go in gameObjects)
                {
                    go.tag = "Finish";
                }


                //Step2 Add Prefab If Not Exsist 
                if (prefab != null)
                {
                    if (IsJioMRManagerExist()) { }
                    else
                    {
                        Selection.activeObject = PrefabUtility.InstantiatePrefab(prefab as GameObject);
                        GameObject clone = Selection.activeGameObject;
                        clone.transform.position = Vector3.zero;

                        //EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        Undo.RegisterCreatedObjectUndo(Selection.activeObject, $"Create {prefab.name} Object");
                    }
                }
            }
        }

        private static bool IsJioMRManagerExist()
        {
            var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            List<JMRManager> componentResult = new List<JMRManager>();

            foreach (var rootObject in rootObjects)
            {
                componentResult.AddRange(rootObject.GetComponentsInChildren<JMRManager>());
            }

            if (componentResult.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
//#endif