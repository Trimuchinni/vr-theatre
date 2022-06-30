// Copyright (c) 2020 JioGlass. All Rights Reserved.
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace JMRSDK.Toolkit.UI
{
	[CreateAssetMenu(fileName = "ThemeDataSCRO", menuName = "CreateThemeData", order = 1)]
	public class SO_ThemeData : ScriptableObject
	{
		[SerializeField]
		public JMRThemeIconSet themeData;
	}
}
#endif