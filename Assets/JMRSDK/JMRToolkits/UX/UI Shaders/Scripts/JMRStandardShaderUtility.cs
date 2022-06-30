// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;

namespace JMRSDK.Toolkit.UI.Utilities
{
    public static class JMRStandardShaderUtility
    {
        public static readonly string StandardShaderName = "JMRSDK/Standard";

        /// <summary>
        /// Returns an instance of the JMRSDK Standard shader.
        /// </summary>
        public static Shader StandardShader
        {
            get
            {
                if (j_StandardShader == null)
                {
                    j_StandardShader = Shader.Find(StandardShaderName);
                }

                return j_StandardShader;
            }

            private set
            {
                j_StandardShader = value;
            }
        }

        private static Shader j_StandardShader = null;

        /// <summary>
        /// Checks if a material is using the JMRSDK Standard shader.
        /// </summary>
        /// <param name="material">The material to check.</param>
        /// <returns>True if the material is using the JMRSDK Standard shader</returns>
        public static bool IsUsingJMRSDKStandardShader(Material material)
        {
            return IsJMRSDKStandardShader((material != null) ? material.shader : null);
        }

        /// <summary>
        /// Checks if a shader is the JMRSDK Standard shader.
        /// </summary>
        /// <param name="shader">The shader to check.</param>
        /// <returns>True if the shader is the JMRSDK Standard shader.</returns>
        public static bool IsJMRSDKStandardShader(Shader shader)
        {
            return shader == StandardShader;
        }
    }
}
