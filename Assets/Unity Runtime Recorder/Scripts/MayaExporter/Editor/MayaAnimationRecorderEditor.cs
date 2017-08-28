using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MayaAnimationRecorder))]
public class MayaAnimationRecorderEditor : Editor {

	SerializedProperty saveFolderPath;
	SerializedProperty saveFileName;
	SerializedProperty originalMaFilePath;

	SerializedProperty startKey;
	SerializedProperty endKey;

	SerializedProperty changeTimeScale;
	SerializedProperty startGameWithTimeScale;
	SerializedProperty startRecordWithTimeScale;

	SerializedProperty showDebugGUI;

	SerializedProperty recordLimitFrames;
	SerializedProperty recordFrames;

	void OnEnable () {

		saveFileName = serializedObject.FindProperty ("saveFileName");
		saveFolderPath = serializedObject.FindProperty ("saveFolderPath");
		originalMaFilePath = serializedObject.FindProperty ("originalMaFilePath");

		startKey = serializedObject.FindProperty ("startKey");
		endKey = serializedObject.FindProperty ("endKey");

		changeTimeScale = serializedObject.FindProperty ("changeTimeScale");
		startGameWithTimeScale = serializedObject.FindProperty ("startGameWithTimeScale");
		startRecordWithTimeScale = serializedObject.FindProperty ("startRecordWithTimeScale");

		showDebugGUI = serializedObject.FindProperty ("showLogGUI");

		recordLimitFrames = serializedObject.FindProperty ("recordLimitFrames");
		recordFrames = serializedObject.FindProperty ("recordFrames");

	}

	public override void OnInspectorGUI () {
		serializedObject.Update ();

		EditorGUILayout.LabelField ("== Path Settings ==");

		if (GUILayout.Button ("Select MA File")) {
			string[] filters = { "Maya ASCII File", "ma" };
			string maFilePath = EditorUtility.OpenFilePanelWithFilters("Select your original .ma file", "", filters );
			originalMaFilePath.stringValue = maFilePath;
		}
		EditorGUILayout.PropertyField (originalMaFilePath);

		if (GUILayout.Button ("Save File To")) {
			string inputPath = EditorUtility.SaveFilePanel( "select temp folder", "", "someFile.ma", "" );
			int lastIndex = inputPath.LastIndexOf ("/");

			saveFileName.stringValue = inputPath.Substring( lastIndex+1 );
			saveFolderPath.stringValue = inputPath.Substring (0, lastIndex + 1);
		}
		EditorGUILayout.PropertyField (saveFolderPath);
		EditorGUILayout.PropertyField (saveFileName);

		EditorGUILayout.Space ();

		// keys setting
		EditorGUILayout.LabelField( "== Control Keys ==" );
		EditorGUILayout.PropertyField (startKey);
		EditorGUILayout.PropertyField (endKey);

		EditorGUILayout.Space ();

		// Other Settings
		EditorGUILayout.LabelField( "== Other Settings ==" );
		bool timeScaleOption = EditorGUILayout.Toggle ( "Change Time Scale", changeTimeScale.boolValue);
		changeTimeScale.boolValue = timeScaleOption;

		if (timeScaleOption) {
			startGameWithTimeScale.floatValue = EditorGUILayout.FloatField ("TimeScaleOnStart", startGameWithTimeScale.floatValue);
			startRecordWithTimeScale.floatValue = EditorGUILayout.FloatField ("TimeScaleOnRecord", startRecordWithTimeScale.floatValue);
		}

		// gui log message
		showDebugGUI.boolValue = EditorGUILayout.Toggle ("Show Debug On GUI", showDebugGUI.boolValue);

		// recording frames setting
		recordLimitFrames.boolValue = EditorGUILayout.Toggle( "Record Limited Frames", recordLimitFrames.boolValue );

		if (recordLimitFrames.boolValue)
			EditorGUILayout.PropertyField (recordFrames);

		serializedObject.ApplyModifiedProperties ();

		//DrawDefaultInspector ();
	}

}
