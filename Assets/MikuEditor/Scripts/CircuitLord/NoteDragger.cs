using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using MiKu.NET;
using MiKu.NET.Charting;
using ThirdParty.Custom;
using UnityEngine;




public class NoteDragger : MonoBehaviour {
	public LayerMask notesLayer;
	public LayerMask gridLayer;

	public Camera activatedCamera;


	public GridManager gridManager;

	public NotesArea notesArea;


	private EditorNote selectedNote = new EditorNote();
	private EditorNote originNote = new EditorNote();
	public static bool isDragging = false;
	
	public static bool activated = false;

	void OnApplicationFocus(bool hasFocus)
	{
		if(hasFocus) {
			Deactivate();
		} 
	}


	private void Update() {
		//if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt)) {
		//if (Controller.controller.GetKeyDown("DragSnapObjectAction")) {
		//	Activate();
		//}

		//else if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt)) {
		if (Controller.controller.GetKeyUp("DragSnapObjectAction")) {
			if (isDragging && selectedNote.exists) {
				Vector3 snappedPos = gridManager.GetNearestPointOnGrid(GetPosOnGridFromMouse());
				selectedNote.noteGO.transform.position = new Vector3(snappedPos.x, snappedPos.y, selectedNote.noteGO.transform.position.z);
			}
			if(selectedNote.exists) EndCurrentDrag();
			Deactivate();
		}

		if (!activated) return;

		// Moved to Track.cs
/* 		if (Input.GetMouseButtonDown(0)) { 
			StartNewDrag();
		} */

		if (isDragging && selectedNote.exists) {
			Vector3 snappedPos = gridManager.GetNearestPointOnGrid(GetPosOnGridFromMouse());

			selectedNote.noteGO.transform.position =
				new Vector3(snappedPos.x, snappedPos.y, selectedNote.noteGO.transform.position.z);
		}


		/* if (selectedNote.exists && Input.GetMouseButtonUp(0)) {
			EndCurrentDrag();
		} */
	}


	public void Activate() {
		activated = true;
		NotesArea.s_instance.DisableSelectedNote();
	}

	public void Deactivate() {
		EndCurrentDrag();
		activated = false;
	}


	public void StartNewDrag() {
		Activate();
		selectedNote = NoteRayUtil.NoteUnderMouse(activatedCamera, notesLayer);
		originNote = new EditorNote();
		originNote.note = new Note(new Vector3(0, 0, 0));
		originNote.note.Type = selectedNote.note.Type;
		if (selectedNote.type==EditorNoteType.RailNode && selectedNote.noteGO!=null) originNote.note.Position = new float[] {selectedNote.noteGO.transform.position.x, selectedNote.noteGO.transform.position.y, selectedNote.noteGO.transform.position.z};
		else originNote.note.Position = selectedNote.note.Position;
		originNote.note.Segments = selectedNote.note.Segments;
		isDragging = true;
	}


	private void EndCurrentDrag() {
		if (!selectedNote.exists) return;
		float[] historyTargetPos = new float[] {};
		float[] historyOriginPos = {originNote.note.Position[0], originNote.note.Position[1], originNote.note.Position[2]};
		//We need to update lines we drag with the parent GO
		if (selectedNote.type == EditorNoteType.RailStart) {
			Game_LineWaveCustom waveCustom = selectedNote.noteGO.GetComponentInChildren<Game_LineWaveCustom>();
			if (waveCustom) {
				var segments = GetLineSegementArrayPoses(selectedNote.connectedNodes);
				//Update the actual values in the note.
				selectedNote.note.Segments = segments;
				waveCustom.targetOptional = segments;
				waveCustom.RenderLine(true, true);
			}
			Vector3 finalPos = selectedNote.noteGO.transform.position;
			selectedNote.note.Position = new float[3] {finalPos.x, finalPos.y, finalPos.z};
			historyTargetPos = new float[] {selectedNote.note.Position[0], selectedNote.note.Position[1], selectedNote.note.Position[2]};
			Track.HistoryChangeDragNote(selectedNote.type, selectedNote.note.Type, Track.CurrentSelectedMeasure, historyOriginPos, historyTargetPos, originNote.note.Segments, selectedNote.note.Segments);
		}
		else if (selectedNote.type == EditorNoteType.RailNode) {
			//Get the LineWave thingy from the Wave Start
			Game_LineWaveCustom waveCustom = selectedNote.noteGO.transform.parent.GetComponentInChildren<Game_LineWaveCustom>();
			if (waveCustom) {
				var segments = GetLineSegementArrayPoses(selectedNote.connectedNodes);
				//Update the actual values in the note.
				selectedNote.note.Segments = segments;
				waveCustom.targetOptional = segments;
				waveCustom.RenderLine(true, true);
			}			
			List<Segment> segmentsList = Track.s_instance.SegmentsList.FindAll(x => (Mathf.Abs(x.measure-Track.CurrentSelectedMeasure)<Track.MeasureCheckTolerance) && x.note.Type == selectedNote.note.Type);
			if(segmentsList == null || segmentsList.Count <= 0) { 
				Debug.Log("Error finding segment!");
			} else {
				Note parentNote = segmentsList.First().note;
				if (parentNote==null) Debug.Log("Error finding parent note!");
				else {
					float noteBeat = Track.s_instance.FindClosestNoteBeat(Track.s_instance.GetBeatMeasureByUnit(parentNote.Position[2]));
					// Instead of recording a change in position to an individual node, we have to record the before and after state of the parent note; otherwise there will be Undo errors when the moved node is the only node on the rail.
					Track.HistoryChangeRailNodeParent(selectedNote.note.Type, noteBeat, new float[] {parentNote.Position[0], parentNote.Position[1], parentNote.Position[2]}, originNote.note.Segments, parentNote.Segments);
				}
			}
		}
		else if (selectedNote.type == EditorNoteType.Standard) {
			Vector3 finalPos = selectedNote.noteGO.transform.position;
			selectedNote.note.Position = new float[3] {finalPos.x, finalPos.y, finalPos.z};
			historyTargetPos = new float[] {selectedNote.note.Position[0], selectedNote.note.Position[1], selectedNote.note.Position[2]};
			Track.HistoryChangeDragNote(selectedNote.type, selectedNote.note.Type, Track.CurrentSelectedMeasure, historyOriginPos, historyTargetPos, originNote.note.Segments, selectedNote.note.Segments);
		}
		selectedNote = new EditorNote();
		originNote = new EditorNote();
		isDragging = false;
	}

	private float[,] GetLineSegementArrayPoses(List<Transform> connectNodes) {
		float[,] segments = new float[connectNodes.Count, 3];

		int i = 0;
		foreach (Transform t in connectNodes) {
			segments[i, 0] = t.position.x;
			segments[i, 1] = t.position.y;
			segments[i, 2] = t.position.z;

			i++;
		}

		return segments;
	}


	public Vector3 GetPosOnGridFromMouse() {
		Ray ray = activatedCamera.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 50f, gridLayer)) {
			Vector3 pos = hit.point;

			return pos;
		}

		return new Vector3(0, 0, 0);
	}
}