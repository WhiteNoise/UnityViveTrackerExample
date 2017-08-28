using UnityEngine;
using System.Collections;
using System;

public class UnityCurveContainer {

    public string propertyName = "";
    public string debugPathName = "";
    public AnimationCurve animCurve;

    public float KeyTolerance = 0.0005f;

	public UnityCurveContainer( string _propertyName ) {
		animCurve = new AnimationCurve ();
		propertyName = _propertyName;
	}

	public void AddValue( float animTime, float previousFrameTime, float animValue )
	{
		Keyframe key = new Keyframe (animTime, animValue, 0.0f, 0.0f);
        if ( animCurve.keys.Length > 1 && previousFrameTime >= 0)
        {
            if ( Mathf.Abs(animCurve.keys[animCurve.keys.Length-1].value - key.value) > KeyTolerance )
            {
                if ( animCurve.keys[animCurve.keys.Length - 1].time != previousFrameTime)
                {
                    //Linear Model
                    //Keyframe repeatOfLastKey = new Keyframe(previousFrameTime, animCurve.keys[animCurve.keys.Length - 1].value, 0.0f, 0.0f);
                    //animCurve.AddKey(repeatOfLastKey);

                    //Insert keyframe at midpoint with tangent
                    Keyframe repeatOfLastKey = new Keyframe(0.5f * (previousFrameTime + animCurve.keys[animCurve.keys.Length - 1].time), animCurve.keys[animCurve.keys.Length - 1].value, 0.0f, 0.0f);

                    animCurve.RemoveKey(animCurve.keys.Length - 1);
                    animCurve.AddKey(repeatOfLastKey);
                    animCurve.SmoothTangents(animCurve.keys.Length - 1, 0);
                    Debug.Log("Adding a duplicate of existing keyframe (@"+previousFrameTime+") before inserting new keyframe @" + key.time);
                }
                animCurve.AddKey(key);
            }
            else
            {
//                Debug.Log(debugPathName+":"+propertyName + " - Key @" + animCurve.keys[animCurve.keys.Length - 1].time + ":" + animCurve.keys[animCurve.keys.Length - 1].value + " deemed same as " + key.time + ":" + key.value + " ... so not added");
            }
        }
        else
        {
            animCurve.AddKey(key);
        }
    }

    public void AddQuaternionValue(ref UnityCurveContainer animCurveY, ref UnityCurveContainer animCurveZ, ref UnityCurveContainer animCurveW, float animTime, float previousFrameTime, Quaternion animValue)
    {
        Keyframe keyX = new Keyframe(animTime, animValue.x, 0.0f, 0.0f);
        Keyframe keyY = new Keyframe(animTime, animValue.y, 0.0f, 0.0f);
        Keyframe keyW = new Keyframe(animTime, animValue.z, 0.0f, 0.0f);
        Keyframe keyZ = new Keyframe(animTime, animValue.w, 0.0f, 0.0f);

        if (animCurve.keys.Length > 1 && previousFrameTime >= 0)
        {
            Keyframe lastKeyX = animCurve.keys[animCurve.keys.Length - 1];
            Keyframe lastKeyY = animCurveY.animCurve.keys[animCurveY.animCurve.keys.Length - 1];
            Keyframe lastKeyZ = animCurveZ.animCurve.keys[animCurveZ.animCurve.keys.Length - 1];
            Keyframe lastKeyW = animCurveW.animCurve.keys[animCurveW.animCurve.keys.Length - 1];

            Quaternion lastQuaternion = new Quaternion(lastKeyX.value, lastKeyY.value, lastKeyZ.value, lastKeyW.value);
            float lastKeyFrameTime = animCurve.keys[animCurve.keys.Length - 1].time;

            float FPS = 45f;

            if ( Mathf.Abs(Quaternion.Angle(lastQuaternion, animValue)) > KeyTolerance )
            {
                Quaternion latestQuaternion = animValue;

                if (lastKeyFrameTime != previousFrameTime)
                {
                    float timeSpan = (animTime - lastKeyFrameTime);
                    float numKeys = (animTime - lastKeyFrameTime) * FPS;
                    float CurrentTime = 0;
                    for( int i = 0; i < numKeys; ++i )
                    {
                        CurrentTime = (i * timeSpan / FPS);
                        Quaternion interpolatedQuaternion = Quaternion.Lerp(lastQuaternion, animValue, CurrentTime/timeSpan);
                        CurrentTime += lastKeyFrameTime;

                        animCurve.AddKey(new Keyframe(CurrentTime, interpolatedQuaternion.x, 0.0f, 0.0f));
                        animCurveY.animCurve.AddKey(new Keyframe(CurrentTime, interpolatedQuaternion.y, 0.0f, 0.0f));
                        animCurveZ.animCurve.AddKey(new Keyframe(CurrentTime, interpolatedQuaternion.z, 0.0f, 0.0f));
                        animCurveW.animCurve.AddKey(new Keyframe(CurrentTime, interpolatedQuaternion.w, 0.0f, 0.0f));
                    }
                }
                else
                {
                    animCurve.AddKey(new Keyframe(animTime, animValue.x, 0.0f, 0.0f));
                    animCurveY.animCurve.AddKey(new Keyframe(animTime, animValue.y, 0.0f, 0.0f));
                    animCurveZ.animCurve.AddKey(new Keyframe(animTime, animValue.z, 0.0f, 0.0f));
                    animCurveW.animCurve.AddKey(new Keyframe(animTime, animValue.w, 0.0f, 0.0f));
                    //animCurve.AddKey(key);
                }
            }
  
        }
        else
        {
            animCurve.AddKey(new Keyframe(animTime, animValue.x, 0.0f, 0.0f));
            animCurveY.animCurve.AddKey(new Keyframe(animTime, animValue.y, 0.0f, 0.0f));
            animCurveZ.animCurve.AddKey(new Keyframe(animTime, animValue.z, 0.0f, 0.0f));
            animCurveW.animCurve.AddKey(new Keyframe(animTime, animValue.w, 0.0f, 0.0f));
        }
    }
}
