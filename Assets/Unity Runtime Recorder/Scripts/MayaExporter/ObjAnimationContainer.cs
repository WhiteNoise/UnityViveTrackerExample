using UnityEngine;
using System.Collections;
using System.IO;

public class ObjAnimationContainer {

	public Transform observeObj;
	string objName;
	string filePath;

	public bool recordTranslation = false;
	public bool recordRotation = false;
	public bool recordScale = false;

	MayaCurveConatiner[] tCurves;
	MayaCurveConatiner[] rCurves;
	MayaCurveConatiner[] sCurves;

	// visibility part : different header, simply try scale first
	//MayaCurveContainer[] vCurves

	string objFinalFilePath = "";

	public ObjAnimationContainer ( Transform inputObj, string namePath, string inputPath, bool recordT, bool recordR, bool recordS ) {
		
		objName = namePath;
		observeObj = inputObj;
		filePath = inputPath;

		// translate
		if (recordT) {
			recordTranslation = recordT;
			tCurves = new MayaCurveConatiner[3];
			tCurves [0] = new MayaCurveConatiner (objName, "animCurveTL", "translateX", "tx", filePath);
			tCurves [1] = new MayaCurveConatiner (objName, "animCurveTL", "translateY", "ty", filePath);
			tCurves [2] = new MayaCurveConatiner (objName, "animCurveTL", "translateZ", "tz", filePath);
		}

		// rotation
		if (recordR) {
			recordRotation = recordR;
			rCurves = new MayaCurveConatiner[3];
			rCurves [0] = new MayaCurveConatiner (objName, "animCurveTA", "rotateX", "rx", filePath);
			rCurves [1] = new MayaCurveConatiner (objName, "animCurveTA", "rotateY", "ry", filePath);
			rCurves [2] = new MayaCurveConatiner (objName, "animCurveTA", "rotateZ", "rz", filePath);
		}

		// scale
		if (recordS) {
			recordScale = recordS;
			sCurves = new MayaCurveConatiner[3];
			sCurves [0] = new MayaCurveConatiner (objName, "animCurveTU", "scaleX", "sx", filePath);
			sCurves [1] = new MayaCurveConatiner (objName, "animCurveTU", "scaleY", "sy", filePath);
			sCurves [2] = new MayaCurveConatiner (objName, "animCurveTU", "scaleZ", "sz", filePath);
		}
	}

	public void recordFrame ( int frameIndex ) {

		// translation
		if (recordTranslation) {
			Vector3 mayaPos = Export2MayaTranslation (observeObj.localPosition);
			tCurves [0].AddValue (frameIndex, mayaPos.x);
			tCurves [1].AddValue (frameIndex, mayaPos.y);
			tCurves [2].AddValue (frameIndex, mayaPos.z);
		}

		// rotation
		if (recordRotation) {
			Vector3 mayaRot = Export2MayaRotation (observeObj.localRotation.eulerAngles);
			rCurves [0].AddValue (frameIndex, mayaRot.x);
			rCurves [1].AddValue (frameIndex, mayaRot.y);
			rCurves [2].AddValue (frameIndex, mayaRot.z);
		}

		// scaling
		if (recordScale) {
			sCurves [0].AddValue (frameIndex, observeObj.localScale.x);
			sCurves [1].AddValue (frameIndex, observeObj.localScale.y);
			sCurves [2].AddValue (frameIndex, observeObj.localScale.z);
		}
	}

	public void WriteIntoFile () {
		// translation
		if (recordTranslation) {
			tCurves [0].WriteIntoFile ();
			tCurves [1].WriteIntoFile ();
			tCurves [2].WriteIntoFile ();
		}
		
		// rotation
		if (recordRotation) {
			rCurves [0].WriteIntoFile ();
			rCurves [1].WriteIntoFile ();
			rCurves [2].WriteIntoFile ();
		}
		
		// scaling
		if (recordScale) {
			sCurves [0].WriteIntoFile ();
			sCurves [1].WriteIntoFile ();
			sCurves [2].WriteIntoFile ();
		}
	}

	public void EndRecord () {

		if (recordTranslation) {
			tCurves [0].AnimFinish ();
			tCurves [1].AnimFinish ();
			tCurves [2].AnimFinish ();
		}

		if (recordRotation) {
			rCurves [0].AnimFinish ();
			rCurves [1].AnimFinish ();
			rCurves [2].AnimFinish ();
		}

		if (recordScale) {
			sCurves [0].AnimFinish ();
			sCurves [1].AnimFinish ();
			sCurves [2].AnimFinish ();
		}

		combineCurveFiles ();
	}

	// combine all curves' data file into one objectDataFile
	public void combineCurveFiles () {
		objFinalFilePath = filePath + "/" + objName + "_objectAll";

		StreamWriter finalWriter = new StreamWriter (objFinalFilePath);

		// translate
		if (recordTranslation) {
			for (int i=0; i<3; i++) {
				string loadFilePath = tCurves [i].getFinalFilePath ();
				StreamReader reader = new StreamReader (loadFilePath);

				finalWriter.Write (reader.ReadToEnd ());

				reader.Close ();
			}
		}

		// rotation
		if (recordRotation) {
			for (int i=0; i<3; i++) {
				string loadFilePath = rCurves [i].getFinalFilePath ();
				StreamReader reader = new StreamReader (loadFilePath);
			
				finalWriter.Write (reader.ReadToEnd ());
			
				reader.Close ();
			}
		}

		// scaling
		if (recordScale) {
			for (int i=0; i<3; i++) {
				string loadFilePath = sCurves [i].getFinalFilePath ();
				StreamReader reader = new StreamReader (loadFilePath);
			
				finalWriter.Write (reader.ReadToEnd ());
			
				reader.Close ();
			}
		}

		finalWriter.Close();
	}

	public string getFinalFilePath () {

		if (objFinalFilePath == "")
			return null;
		else
			return objFinalFilePath;
	}

	public bool cleanFile () {
		if (objFinalFilePath == "")
			return false;
		else
		{
			File.Delete( objFinalFilePath );
			objFinalFilePath = "";

			// clean child curve files
			for( int i=0; i<3; i++ )
			{
				if( recordTranslation )
					tCurves[i].cleanFile();

				if( recordRotation )
					rCurves[i].cleanFile();

				if( recordScale )
					sCurves[i].cleanFile();
			}
			return true;
		}
	}

	// convert unity translation to maya translation
	Vector3 Export2MayaTranslation (Vector3 t)
	{
		return new Vector3(-t.x, t.y, t.z);
	}

	// convert unity rotation to maya rotation
	Vector3 Export2MayaRotation (Vector3 r)
	{
		return new Vector3(r.x, -r.y, -r.z);
	}

	// not used
//	Vector3 UnityRot2Maya(Quaternion q)
//	{
//		float x =  180f / Mathf.PI *Mathf.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z*q.z  + q.w*q.w));     // Yaw 
//		float y =  180f / Mathf.PI *Mathf.Asin(2f * ( q.x * q.z - q.w * q.y ) );                             // Pitch 
//		float z =  180f / Mathf.PI *Mathf.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y*q.y + q.z*q.z));      // Roll 
//		return new Vector3( (180f-x), y , -z);
//	}
}
