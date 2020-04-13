using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using MiKu.NET;
using MiKu.NET.Charting;

namespace LittleAsi.SelectionOptions {
	public class SelectionOptions : MonoBehaviour {
		
		[HideInInspector] public bool panelActive = false;
		[SerializeField] private Toggle selectAll;
		[SerializeField] private Toggle leftHandNote;
		[SerializeField] private Toggle rightHandNote;
		[SerializeField] private Toggle oneHandSpecialNote;
		[SerializeField] private Toggle twoHandSpecialNote;
		[SerializeField] private Toggle leftWall;
		[SerializeField] private Toggle rightWall;
		[SerializeField] private Toggle leftAngleWall;
		[SerializeField] private Toggle rightAngleWall;
		[SerializeField] private Toggle centerWall;
		[SerializeField] private Toggle crouchWall;
		[SerializeField] private Toggle deleteExistingRange;
		[SerializeField] private Toggle prioritizeExisting;
		[SerializeField] private Toggle prioritizePasted;
		public static SelectionOptions selectionOptions;
		
		void Awake(){
		if(selectionOptions == null){
			//DontDestroyOnLoad(gameObject);
			selectionOptions = this;
		} else if(selectionOptions != this){
			Destroy(gameObject);
		}
	}
		
		void Start(){
			leftHandNote.isOn = Track.s_instance.leftHandNoteSelected;
			rightHandNote.isOn = Track.s_instance.rightHandNoteSelected;
			oneHandSpecialNote.isOn = Track.s_instance.oneHandSpecialNoteSelected;
			twoHandSpecialNote.isOn = Track.s_instance.twoHandSpecialNoteSelected;
			leftWall.isOn = Track.s_instance.leftWallSelected;
			rightWall.isOn = Track.s_instance.rightWallSelected;
			leftAngleWall.isOn = Track.s_instance.leftAngleWallSelected;
			rightAngleWall.isOn = Track.s_instance.rightAngleWallSelected;
			centerWall.isOn = Track.s_instance.centerWallSelected;
			deleteExistingRange.isOn = Track.s_instance.pasteDeletesExistingRange;
			prioritizeExisting.isOn = Track.s_instance.pastePrioritizesExisting;
			prioritizePasted.isOn = Track.s_instance.pastePrioritizesPasted;
			panelActive = false;
		}
		
		public void ToggleAll(bool _enabled){
			leftHandNote.isOn = selectAll.isOn;
			rightHandNote.isOn = selectAll.isOn;
			oneHandSpecialNote.isOn = selectAll.isOn;
			twoHandSpecialNote.isOn = selectAll.isOn;
			leftWall.isOn = selectAll.isOn;
			rightWall.isOn = selectAll.isOn;
			leftAngleWall.isOn = selectAll.isOn;
			rightAngleWall.isOn = selectAll.isOn;
			centerWall.isOn = selectAll.isOn;
			crouchWall.isOn = selectAll.isOn;
		}
		
		public void ToggleLeftHandNote(bool _enabled){
			Track.s_instance.leftHandNoteSelected = leftHandNote.isOn;
			/* Debug.Log("ToggleLeftHandNote: ");
			Debug.Log("Track.s_instance.leftHandNoteSelected: " + Track.s_instance.leftHandNoteSelected);
			Debug.Log("leftHandNote.isOn: " + leftHandNote.isOn);
			Debug.Log(""); */
		}
		
		public void ToggleRightHandNote(bool _enabled){
			Track.s_instance.rightHandNoteSelected = rightHandNote.isOn;
			/* Debug.Log("ToggleRightHandNote: ");
			Debug.Log("Track.s_instance.rightHandNoteSelected: " + Track.s_instance.rightHandNoteSelected);
			Debug.Log("rightHandNote.isOn: " + rightHandNote.isOn);
			Debug.Log(""); */
		}
		
		public void ToggleOneHandSpecialNote(bool _enabled){
			Track.s_instance.oneHandSpecialNoteSelected = oneHandSpecialNote.isOn;
			/* Debug.Log("ToggleOneHandSpecialNote: ");
			Debug.Log("Track.s_instance.oneHandSpecialNoteSelected: " + Track.s_instance.oneHandSpecialNoteSelected);
			Debug.Log("oneHandSpecialNote.isOn: " + oneHandSpecialNote.isOn);
			Debug.Log(""); */
		}
		
		public void ToggleTwoHandSpecialNote(bool _enabled){
			Track.s_instance.twoHandSpecialNoteSelected = twoHandSpecialNote.isOn;
			/* Debug.Log("ToggleTwoHandSpecialNote: ");
			Debug.Log("Track.s_instance.twoHandSpecialNoteSelected: " + Track.s_instance.twoHandSpecialNoteSelected);
			Debug.Log("twoHandSpecialNote.isOn: " + twoHandSpecialNote.isOn);
			Debug.Log(""); */
		}
		
		public void ToggleLeftWall(bool _enabled){
			Track.s_instance.leftWallSelected = leftWall.isOn;
			/* Debug.Log("ToggleLeftWall: ");
			Debug.Log("Track.s_instance.leftWallSelected: " + Track.s_instance.leftWallSelected);
			Debug.Log("leftWall.isOn: " + leftWall.isOn);
			Debug.Log(""); */
		}
		
		public void ToggleRightWall(bool _enabled){
			Track.s_instance.rightWallSelected = rightWall.isOn;
			/* Debug.Log("ToggleRightWall: ");
			Debug.Log("Track.s_instance.rightWallSelected: " + Track.s_instance.rightWallSelected);
			Debug.Log("rightWall.isOn: " + rightWall.isOn);
			Debug.Log(""); */
		}
		
		public void ToggleLeftAngleWall(bool _enabled){
			Track.s_instance.leftAngleWallSelected = leftAngleWall.isOn;
			/* Debug.Log("ToggleLeftAngleWall: ");
			Debug.Log("Track.s_instance.leftAngleWallSelected: " + Track.s_instance.leftAngleWallSelected);
			Debug.Log("leftAngleWall.isOn: " + leftAngleWall.isOn);
			Debug.Log(""); */
		}
		
		public void ToggleRightAngleWall(bool _enabled){
			Track.s_instance.rightAngleWallSelected = rightAngleWall.isOn;
			/* Debug.Log("ToggleRightAngleWall: ");
			Debug.Log("Track.s_instance.rightAngleWallSelected: " + Track.s_instance.rightAngleWallSelected);
			Debug.Log("rightAngleWall.isOn: " + rightAngleWall.isOn);
			Debug.Log(""); */
		}
		
		public void ToggleCenterWall(bool _enabled){
			Track.s_instance.centerWallSelected = centerWall.isOn;
			/* Debug.Log("ToggleCenterWall: ");
			Debug.Log("Track.s_instance.centerWallSelected: " + Track.s_instance.centerWallSelected);
			Debug.Log("centerWall.isOn: " + centerWall.isOn);
			Debug.Log(""); */
		}
		
		public void ToggleCrouchWall(bool _enabled){
			Track.s_instance.crouchWallSelected = crouchWall.isOn;
			/* Debug.Log("ToggleCrouchWall: ");
			Debug.Log("Track.s_instance.crouchWallSelected: " + Track.s_instance.crouchWallSelected);
			Debug.Log("crouchWall.isOn: " + crouchWall.isOn);
			Debug.Log(""); */
		}
		
		public void ToggleDeleteExistingRange(bool _enabled){
			Track.s_instance.pasteDeletesExistingRange = deleteExistingRange.isOn;
			/* Debug.Log("ToggleDeleteExistingRange: ");
			Debug.Log("Track.s_instance.pasteDeletesExistingRange: " + Track.s_instance.pasteDeletesExistingRange);
			Debug.Log("deleteExistingRange.isOn: " + deleteExistingRange.isOn);
			Debug.Log(""); */
		}
		
		public void TogglePrioritizeExisting(bool _enabled){
			Track.s_instance.pastePrioritizesExisting = prioritizeExisting.isOn;
			/* Debug.Log("TogglePrioritizeExisting: ");
			Debug.Log("Track.s_instance.pastePrioritizesExisting: " + Track.s_instance.pastePrioritizesExisting);
			Debug.Log("prioritizeExisting.isOn: " + prioritizeExisting.isOn);
			Debug.Log(""); */
		}
		
		public void TogglePrioritizePasted(bool _enabled){
			Track.s_instance.pastePrioritizesPasted = prioritizePasted.isOn;
			/* Debug.Log("TogglePrioritizePasted: ");
			Debug.Log("Track.s_instance.pastePrioritizesPasted: " + Track.s_instance.pastePrioritizesPasted);
			Debug.Log("prioritizePasted.isOn: " + prioritizePasted.isOn);
			Debug.Log(""); */
		}
			
	}
}