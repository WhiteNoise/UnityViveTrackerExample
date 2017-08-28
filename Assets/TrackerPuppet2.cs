using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerPuppet2 : MonoBehaviour {
    [Header("Player Height")]
    // Your head height
    public float myHeight = 1.53f;

    // Just an offset that gets applied when you start animating
    // You may want to zero this out, since it will get recorded
    public Vector3 offset = new Vector3(-1.0f, 0.0f, 0.0f);

	[Header("Trackers")]
    public Transform controllerLeft;
    public Transform controllerRight;
    public Transform trackerLeft;
    public Transform trackerRight;
    public Transform playerHead;

    private Quaternion controllerLeftStart;
    private Quaternion controllerRightStart;

    [Header("Bones")]
    public Transform main;
    public Transform root;
    public Transform leftUpperArm;
    public Transform leftForeArm;
    public Transform rightUpperArm;
    public Transform rightForeArm;
    public Transform head;
    public Transform neck;
    public Transform ribs;
    public Transform hips;

    private Quaternion leftForeArmStart;
    private Quaternion rightForeArmStart;

    private Quaternion startingRibsRotation;
    private Quaternion startingHipsRotation;

    private Vector3 leftForeArmForwardLocal;
    private Vector3 leftForeArmUpLocal;

    private Vector3 rightForeArmForwardLocal;
    private Vector3 rightForeArmUpLocal;

    private Vector3 leftUpperArmForwardLocal;
    private Vector3 leftUpperArmUpLocal;

    private Vector3 rightUpperArmForwardLocal;
    private Vector3 rightUpperArmUpLocal;

    private Vector3 headForwardLocal;
    private Vector3 headUpLocal;

    // this will be true once we start actually puppeting the character
    private bool isPosing = false;

	
	void Start () {
    	controllerLeft = transform.parent.Find("TrackedObjects/ControllerL");
    	controllerRight = transform.parent.Find("TrackedObjects/ControllerR");
    	trackerLeft = transform.parent.Find("TrackedObjects/TrackerL");
    	trackerRight = transform.parent.Find("TrackedObjects/TrackerR");
    	playerHead = transform.parent.Find("TrackedObjects/Head");

		main = transform;
		root = main.Find("Root");
		leftUpperArm = main.Find("Root/Ribs/Left_Shoulder_Joint_01/Left_Upper_Arm_Joint_01");
		leftForeArm = main.Find("Root/Ribs/Left_Shoulder_Joint_01/Left_Upper_Arm_Joint_01/Left_Forearm_Joint_01");
		rightUpperArm = main.Find("Root/Ribs/Right_Shoulder_Joint_01/Right_Upper_Arm_Joint_01");
		rightForeArm = main.Find("Root/Ribs/Right_Shoulder_Joint_01/Right_Upper_Arm_Joint_01/Right_Forearm_Joint_01");
		neck = main.Find("Root/Ribs/Neck");
		head = main.Find("Root/Ribs/Neck/Head");
		ribs = main.Find("Root/Ribs");
		hips = main.Find("Root/Hip");
	}

	public void DelayedBegin() {
		isPosing = true;

		leftForeArmStart = leftForeArm.localRotation;
		rightForeArmStart = rightForeArm.localRotation;
		startingRibsRotation = ribs.rotation;
		startingHipsRotation = hips.rotation;

		controllerLeftStart = controllerLeft.localRotation;

		// This is dependent on the two models facing the same direction to start with. 
		leftForeArmForwardLocal = controllerLeft.InverseTransformDirection( leftForeArm.forward );
		leftForeArmUpLocal = controllerLeft.InverseTransformDirection( leftForeArm.up );

		rightForeArmForwardLocal = controllerRight.InverseTransformDirection( rightForeArm.forward );
		rightForeArmUpLocal = controllerRight.InverseTransformDirection( rightForeArm.up );

		leftUpperArmForwardLocal = trackerLeft.InverseTransformDirection( leftUpperArm.forward );
		leftUpperArmUpLocal = trackerLeft.InverseTransformDirection( leftUpperArm.up );

		rightUpperArmForwardLocal = trackerRight.InverseTransformDirection( rightUpperArm.forward );
		rightUpperArmUpLocal = trackerRight.InverseTransformDirection( rightUpperArm.up );

		headForwardLocal = playerHead.InverseTransformDirection( head.forward );
		headUpLocal = playerHead.InverseTransformDirection( head.up );

	}

	public void BeginPose() {
		trackerLeft.GetComponentInParent<SteamVR_TrackedObject>().SetDeviceIndex(
			SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.FarthestLeft, Valve.VR.ETrackedDeviceClass.GenericTracker) );
		trackerRight.GetComponentInParent<SteamVR_TrackedObject>().SetDeviceIndex(
			SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.FarthestRight, Valve.VR.ETrackedDeviceClass.GenericTracker) );

		controllerLeft.GetComponentInParent<SteamVR_TrackedObject>().SetDeviceIndex(
			SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost, Valve.VR.ETrackedDeviceClass.Controller) );
		controllerRight.GetComponentInParent<SteamVR_TrackedObject>().SetDeviceIndex(
			SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost, Valve.VR.ETrackedDeviceClass.Controller) );


		Invoke("DelayedBegin", 0.5f);
	}
	
	
	void LateUpdate () {

		if(isPosing) {
			leftForeArm.rotation = Quaternion.LookRotation(controllerLeft.TransformDirection(leftForeArmForwardLocal), controllerLeft.TransformDirection(leftForeArmUpLocal));
			rightForeArm.rotation = Quaternion.LookRotation(controllerRight.TransformDirection(rightForeArmForwardLocal), controllerRight.TransformDirection(rightForeArmUpLocal));

			leftUpperArm.rotation = Quaternion.LookRotation(trackerLeft.TransformDirection(leftUpperArmForwardLocal), trackerLeft.TransformDirection(leftUpperArmUpLocal));
			rightUpperArm.rotation = Quaternion.LookRotation(trackerRight.TransformDirection(rightUpperArmForwardLocal), trackerRight.TransformDirection(rightUpperArmUpLocal));

			head.rotation = Quaternion.LookRotation(playerHead.TransformDirection(headForwardLocal), playerHead.TransformDirection(headUpLocal));
		
			// old

			Vector3 shoulderOffset = trackerRight.localPosition - trackerLeft.localPosition;
			Vector3 shoulderLine2d = shoulderOffset;
			Vector3 facing = Vector3.Cross( shoulderLine2d.normalized, Vector3.up);
					
			Quaternion hipsRot = Quaternion.FromToRotation(Vector3.forward, facing);
			shoulderLine2d.y = 0.0f;

			Vector3 shoulderCenter = (trackerRight.localPosition + trackerLeft.localPosition)/2.0f;

			float leftRightTiltAng = Vector3.Angle(shoulderOffset, shoulderLine2d);

			if(shoulderOffset.y > 0.0f)
					leftRightTiltAng *= -1.0f;

			Vector3 headPos = playerHead.localPosition;
			Vector3 headOffset = headPos - shoulderCenter;
			float leanRotation = 90.0f * Vector3.Dot(facing, headOffset.normalized);
		
			float facingAng = Vector3.Angle(Vector3.forward, facing);
			if(Vector3.Dot(facing, Vector3.right) < 0.0f) {
				facingAng = -facingAng;
			}

			hips.rotation = startingHipsRotation * Quaternion.Euler(facingAng * 0.5f, 0.0f, 0.0f);
			ribs.rotation = startingRibsRotation * Quaternion.Euler(-facingAng, -leanRotation * 0.5f, leftRightTiltAng);

			main.localPosition = new Vector3(headPos.x, Mathf.Max( headPos.y - myHeight, 0.0f), headPos.z) + offset;


		}

	}
}
