using UnityEngine;
using System.Collections;

public class UnityObjectAnimation {

	public UnityCurveContainer[] curves;
	public Transform observeGameObject;
	public string pathName = "";

    protected float LastAddFrameTime = -1;

    private float PositionKeyTolerance = 0.002f;
    private float RotationKeyTolerance = 0.005f;
    private float ScaleKeyTolerance = 0.01f;

    public UnityObjectAnimation( string hierarchyPath, Transform observeObj )
    {
        pathName = hierarchyPath;
		observeGameObject = observeObj;

		curves = new UnityCurveContainer[10];

		curves [0] = new UnityCurveContainer( "localPosition.x" );
		curves [1] = new UnityCurveContainer( "localPosition.y" );
		curves [2] = new UnityCurveContainer( "localPosition.z" );

		curves [3] = new UnityCurveContainer( "localRotation.x" );
		curves [4] = new UnityCurveContainer( "localRotation.y" );
		curves [5] = new UnityCurveContainer( "localRotation.z" );
		curves [6] = new UnityCurveContainer( "localRotation.w" );

		curves [7] = new UnityCurveContainer( "localScale.x" );
		curves [8] = new UnityCurveContainer( "localScale.y" );
		curves [9] = new UnityCurveContainer( "localScale.z" );

    }

    public void AddFrame ( float time )
    {

		curves [0].AddValue (time, LastAddFrameTime, observeGameObject.localPosition.x);
		curves [1].AddValue (time, LastAddFrameTime, observeGameObject.localPosition.y);
		curves [2].AddValue (time, LastAddFrameTime, observeGameObject.localPosition.z);

        curves[3].AddQuaternionValue(ref curves[4], ref curves[5], ref curves[6], time, LastAddFrameTime, observeGameObject.localRotation);

		curves [7].AddValue (time, LastAddFrameTime, observeGameObject.localScale.x);
		curves [8].AddValue (time, LastAddFrameTime, observeGameObject.localScale.y);
		curves [9].AddValue (time, LastAddFrameTime, observeGameObject.localScale.z);

        LastAddFrameTime = time;

    }
}
