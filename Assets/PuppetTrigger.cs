using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetTrigger : MonoBehaviour {
	private SteamVR_TrackedController _controller;
	public TrackerPuppet2 target;
	// Use this for initialization
	void Start () {
		
	}

	void OnEnable() {
		_controller = GetComponent<SteamVR_TrackedController>();
		_controller.TriggerClicked += HandleTriggerClicked;
	}
	
	private void OnDisable()
	{
		_controller.TriggerClicked -= HandleTriggerClicked;
	}

	private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{
		target.BeginPose();
	}
}
