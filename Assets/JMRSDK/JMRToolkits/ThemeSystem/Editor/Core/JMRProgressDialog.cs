// Copyright (c) 2020 JioGlass. All Rights Reserved.

//HOW TO USE THIS SCRIPT:
//CREATE PROGRESS DIALOG:
//ProgressBarData data = new ProgressBarData("Dummy", (currentTimer / timer) * 100 + " %", currentTimer / timer);
//ProgressDialog.ToggleProgressDialog(true,data);

//CLOSE PROGRESS DIALOG:
//ProgressDialog.ToggleProgressDialog(false,data);

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace JMRSDK.Toolkit
{
	internal class ProgressBarData
	{
		public string progressDialogTitle;
		public string progressText;
		public float progress;

		public ProgressBarData(string progressDialogTitle, string progressText, float progress)
		{
			this.progressDialogTitle = progressDialogTitle;
			this.progressText = progressText;
			this.progress = progress;
		}
	}

	internal class JMRProgressDialog : Editor
	{
		/// <summary>
		/// Display the editor progressbar
		/// and hide the progress bar according to state mentioned by developer
		/// </summary>
		/// <param name="activeState"></param>
		/// <param name="progressBarData"></param>
		public static void ToggleProgressDialog(bool activeState,ProgressBarData progressBarData = null)
		{
			if (activeState)
			{
				EditorUtility.DisplayProgressBar(progressBarData.progressDialogTitle, progressBarData.progressText,progressBarData.progress);
			}
			else
			{
				EditorUtility.ClearProgressBar();
			}
		}
	}
}
#endif