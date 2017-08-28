using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class MayaAnimationRecorder : MonoBehaviour {

	Transform[] observeObjs;
	ObjAnimationContainer[] objAnims;

	// the folder path
	public string saveFolderPath;
	public string saveFileName;

	// would copy and paste animation on the another file
	public string originalMaFilePath;

	// control keys
	public KeyCode startKey = KeyCode.Q;
	public KeyCode endKey = KeyCode.W;

	// other settings
	public bool changeTimeScale = false;
	public float startGameWithTimeScale = 0.0f;
	public float startRecordWithTimeScale = 1.0f;

	public bool showLogGUI = false;
	string logMessage = "";

	public bool recordLimitFrames = false;
	public int recordFrames = 1000;

	// prevent feeling like crash
	public int processPerFrame = 20;


	public int frameIndexToStartTransform = 1500;
	bool isTransformStart = false;

	int objNums = 0;

	public int frameIndex = 0;

	bool isStart = false;
	bool isEnd = false;

	void Start () {
		if (changeTimeScale)
			Time.timeScale = startGameWithTimeScale;
		
		SettingRecordItems ();
	}

	// Use this for initialization
	void SettingRecordItems () {

		// get all record objs
		observeObjs = gameObject.GetComponentsInChildren<Transform> ();
		objAnims = new ObjAnimationContainer[ observeObjs.Length ];

		objNums = objAnims.Length;

		for (int i=0; i< observeObjs.Length; i++) {
			string namePath = AnimationRecorderHelper.GetTransformPathName (transform, observeObjs [i]);
			objAnims[i] = new ObjAnimationContainer( observeObjs[i], namePath, saveFolderPath, true, true, true );
		}

		ShowLog ("Setting complete");
	}

	void Update () {

		if (Input.GetKeyDown (startKey)) {
			StartRecording ();
		}

		if (Input.GetKeyDown (endKey)) {
			StopRecording ();
		}
	}

	void OnGUI () {
		if (showLogGUI)
			GUILayout.Label (logMessage);
	}


	public void StartRecording () {
		ShowLog ("Recording start");

		if (changeTimeScale)
			Time.timeScale = startRecordWithTimeScale;

		isStart = true;
	}


	public void StopRecording () {
		ShowLog( "End recording, wait for file process ... " );
		isEnd = true;
		StartCoroutine( "EndRecord" );
	}


	// Update is called once per frame
	void FixedUpdate () {

		if( isStart )
		{
			if( !isEnd ) {

				// split all curveContainer into many parts
				// only record parts of objects per frame
				//				partIndex = (partIndex+1) % splitPartNum;
				//
				//				int from = amountPerPart * partIndex;
				//				int to = amountPerPart * ( partIndex+1 );
				//
				//				if( to > objAnims.Length )
				//					to = objAnims.Length;

				for( int i=0; i< objNums; i++ )
				{
					objAnims[i].recordFrame( frameIndex );
				}

				frameIndex++;

				// if only record limited frames
				if (recordLimitFrames) {
					
					if (frameIndex > recordFrames) {
						ShowLog ("Recording complete, processing ...");
						isEnd = true;
						StartCoroutine ("EndRecord");
					}
				}
			}
		}

	}

	IEnumerator EndRecord () {

		ShowLog ("Terminating Anim Recorders ...");
		// finish recording
		for (int i=0; i< objAnims.Length; i++)
		{
			objAnims [i].EndRecord ();

			if( i % processPerFrame == 0 )
				yield return 0;
		}

		// save into ma file
		StartCoroutine ("exportToMayaAnimation");

	}


	IEnumerator exportToMayaAnimation () {

		// duplicate originalMaFile
		string newFilePath = saveFolderPath + saveFileName;
		StreamWriter writer = new StreamWriter ( newFilePath );

		// read from original ma file
		StreamReader maReader = new StreamReader ( originalMaFilePath );
		writer.Write (maReader.ReadToEnd ());
		maReader.Close ();

		ShowLog ("Combining File into one ...");

		// copy all file content into one single ma file
		for( int i=0; i< objAnims.Length; i++ )
		{
			StreamReader reader = new StreamReader( objAnims[i].getFinalFilePath() );
			writer.Write( reader.ReadToEnd() );
			reader.Close();
			objAnims[i].cleanFile ();

			if( i % processPerFrame == 0 )
				yield return 0;
		}

		// all files loaded
		writer.Close ();

		ShowLog ("Succeed export animation to : " + newFilePath);
	}

	void ShowLog ( string logStr ) {
		if (showLogGUI)
			logMessage = logStr;
		else
			Debug.Log (logStr);
	}
}
