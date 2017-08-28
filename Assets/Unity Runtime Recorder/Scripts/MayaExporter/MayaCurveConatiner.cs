using System.Collections;
using System.IO;
using System.Collections.Generic;

public class MayaCurveConatiner {

	public string objName = "";
	public string curveName = "";
	public string propertyName = "";
	public string footPropertyName = "";
	string inputPath = "";
	string tempFilePath = "";
	string finalFilePath = "";

	int animCount = 0;

	string storedData = "";

	public MayaCurveConatiner ( string inputObjName, string inputCurveName, string inputPropertyName, string inputFootName, string filePath ) {

		// init the vars
		inputPath = filePath;
		objName = inputObjName;
		curveName = inputCurveName;
		propertyName = inputPropertyName;
		footPropertyName = inputFootName;

		tempFilePath = inputPath + compressFileName() + "_dataTemp";
	}

	string compressFileName () {
		return objName + "_" + propertyName;
	}

	public void AddValue ( int frameIndex, float inputValue ) {

		string addContent = " " + frameIndex + " " + inputValue;
		storedData += addContent;

		animCount++;
	}

	public void WriteIntoFile () {
		if (storedData != "") {
			File.AppendAllText (tempFilePath, storedData);
			storedData = "";
		}
	}

	string getHeadContent () {

		string fileContent = "";
		fileContent += "createNode " + curveName + " -n \"" + objName + "_" + propertyName + "\";\n";
		fileContent += "\tsetAttr \".tan\" 18;\n";
		fileContent += "\tsetAttr \".wgt\" no;\n";
		fileContent += "\tsetAttr -s "+animCount.ToString()+" \".ktv[0:"+(animCount-1).ToString()+"]\" ";

		return fileContent;
	}

	string getFootContent () {
		string fileContent = "";
		fileContent += "connectAttr \"" + objName + "_" + propertyName + ".o\" \"" + objName + "." + footPropertyName + "\";\n";

		return fileContent;
	}

	public void AnimFinish () {

		//File.AppendAllText (tempFilePath, ";\n");
		storedData += ";\n";
		WriteIntoFile ();

		processFinalFile ();
	}

	void processFinalFile ()
	{
		// write header part
		finalFilePath = inputPath + compressFileName () + "_final";
		StreamWriter finalWriter = new StreamWriter ( finalFilePath );
		finalWriter.Write( getHeadContent () );
		
		// get and write data content
		StreamReader reader = new StreamReader ( tempFilePath );
		finalWriter.Write (reader.ReadToEnd ());
		reader.Close ();
		File.Delete (tempFilePath);

		// write connectAttr part
		finalWriter.Write (getFootContent ());
		finalWriter.Close ();
	}

	public string getFinalFilePath () {

		if (finalFilePath == "")
			return null;
		else
			return finalFilePath;
	}

	public bool cleanFile () {

		if (finalFilePath == "")
			return false;
		else
		{
			File.Delete (finalFilePath);
			finalFilePath = "";
			return true;
		}

	}
}
