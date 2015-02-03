﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LOS.Event {

	public class LOSEventManager : MonoBehaviour {

		// Timing
		public float updateFrequency;
		private float _timeSinceLastUpdate;


		// Processes
		private List<LOSEventSource> _sources;
		private List<LOSEventTrigger> _triggers;


		// Split workload to serveral frames
		private bool _isProcessing;
		private int _maxFrames = 10;
		private int _currentLightIndex = 0;



		// Singleton
		private static LOSEventManager _instance;

		public static LOSEventManager instance {
			get {
				if (_instance == null) {
					_instance = FindObjectOfType<LOSEventManager>();

					if (_instance == null) {
						GameObject go = new GameObject();
						go.name = "LOSEventManager";
						_instance = go.AddComponent<LOSEventManager>();
					}
				}
				return _instance;
			}
		}

		public static LOSEventManager TryGetInstance () {
			return _instance;
		}



		void Awake () {
			_instance = this;

			_sources = new List<LOSEventSource>();
			_triggers = new List<LOSEventTrigger>();
		}
		
		void Update () {
			_timeSinceLastUpdate += Time.deltaTime;

			if (_timeSinceLastUpdate >= 1 / updateFrequency && !_isProcessing) {
				_timeSinceLastUpdate = 0;

				ResetSettings();
				Process();
			}
		}

		private void Process () {
			_isProcessing = true;

			List<LOSEventTrigger> triggersToProcess = new List<LOSEventTrigger>();

			foreach (LOSEventTrigger trigger in _triggers) {
				if (trigger.CheckWithinScreen()) {
					triggersToProcess.Add(trigger);
				}
				else {
					trigger.NotTriggered();
				}
			}

			foreach (LOSEventSource source in _sources) {
				source.Process(triggersToProcess);
			}

			_isProcessing = false;
		}

		private void ResetSettings () {
			_currentLightIndex = 0;
		}

		public void AddEventSource (LOSEventSource source) {
			if (!_sources.Contains(source)) {
				_sources.Add(source);
			}
		}

		public void RemoveEventSource (LOSEventSource source) {
			_sources.Remove(source);
		}

		public void AddEventTrigger (LOSEventTrigger trigger) {
			if (!_triggers.Contains(trigger)) {
				_triggers.Add(trigger);
			}
		}

		public void RemoveEventTrigger (LOSEventTrigger trigger) {
			_triggers.Remove(trigger);
		}
	}

}