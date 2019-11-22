using System;
using System.Collections.Generic;
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
	//public static bool activated = false;
	public static bool isDragging = false;
	
	public static bool activated = false;

	void OnApplicationFocus(bool hasFocus)
	{
		if(hasFocus) {
			Deactivate();
		} 
	}


	private void Update() {
		if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt)) {
			Activate();
		}

		else if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt)) {
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


		if (selectedNote.exists && Input.GetMouseButtonUp(0)) {
			EndCurrentDrag();
		}
	}


	public void Activate() {
		activated = true;
	}

	public void Deactivate() {
		EndCurrentDrag();
		activated = false;
	}


	public void StartNewDrag() {
		selectedNote = NoteRayUtil.NoteUnderMouse(activatedCamera, notesLayer);
		isDragging = true;
	}


	private void EndCurrentDrag() {
		if (!selectedNote.exists) return;

		//We need to update lines we drag with the parent GO
		if (selectedNote.type == EditorNoteType.RailStart) {
			Game_LineWaveCustom waveCustom = selectedNote.noteGO.GetComponentInChildren<Game_LineWaveCustom>();

			if (waveCustom) {
				var segments = GetLineSegementArrayPoses(selectedNote.connectedNodes);

				//Update the actual values in the note.
				selectedNote.note.Segments = segments;

				waveCustom.targetOptional = segments;
				waveCustom.RenderLine(true, true);

				Vector3 finalPos = selectedNote.noteGO.transform.position;
				selectedNote.note.Position = new float[3] {finalPos.x, finalPos.y, finalPos.z};
			}
		}

		else if (selectedNote.type == EditorNoteType.RailNode) {
			//Get the LineWave thingy from the Wave Start
			Game_LineWaveCustom waveCustom =
				selectedNote.noteGO.transform.parent.GetComponentInChildren<Game_LineWaveCustom>();

			if (waveCustom) {
				var segments = GetLineSegementArrayPoses(selectedNote.connectedNodes);

				//Update the actual values in the note.
				selectedNote.note.Segments = segments;

				waveCustom.targetOptional = segments;
				waveCustom.RenderLine(true, true);
			}
		}

		else if (selectedNote.type == EditorNoteType.Standard) {
			Vector3 finalPos = selectedNote.noteGO.transform.position;
			selectedNote.note.Position = new float[3] {finalPos.x, finalPos.y, finalPos.z};
		}

		selectedNote = new EditorNote();

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


	private Vector3 GetPosOnGridFromMouse() {
		Ray ray = activatedCamera.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 50f, gridLayer)) {
			Vector3 pos = hit.point;

			return pos;
		}

		return new Vector3(0, 0, 0);
	}
}