


using MiKu.NET;
using UnityEngine;

public static class NoteRayUtil {
	public static EditorNote NoteUnderMouse(Camera camera, LayerMask layer) {
		RaycastHit hit;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);


		if (Physics.Raycast(ray, out hit, 50f, layer)) {
			if (!hit.transform) return null;


			EditorNote draggableNote = new EditorNote();
			
			//Debug.Log("Hit note name: " + hit.transform.gameObject.name);

			//If it's a rail node segment.
			if (hit.transform.gameObject.name.EndsWith("_Segment")) {
				draggableNote.noteGO = hit.transform.gameObject;

				TempBeatTimeRef tempBeatTimeRef = draggableNote.noteGO.transform.parent.parent.GetComponent<TempBeatTimeRef>();
				
				draggableNote.note = Track.TryGetNoteFromBeatTimeType(tempBeatTimeRef.beatTime, tempBeatTimeRef.type);
				if(draggableNote.note == null) {
					return null;
				}
				
				draggableNote.time = tempBeatTimeRef.beatTime;
				
				//draggableNote.note = Track.TryGetNoteFromName(hit.transform.parent.parent.gameObject.name);
				//Debug.Log("This is a rail node");

				draggableNote.type = EditorNoteType.RailNode;

				draggableNote.startPosition = draggableNote.noteGO.transform.position;

				//We need to add it's fellow notes for future reference.
				draggableNote.GetConnectedNodes();
				
			}

			//Else it's a rail start or normal note
			else {
				draggableNote.noteGO = hit.transform.parent.gameObject;

				//draggableNote.note = Track.TryGetNoteFromName(draggableNote.noteGO.name);
				TempBeatTimeRef tempBeatTimeRef = draggableNote.noteGO.transform.GetComponent<TempBeatTimeRef>();
				draggableNote.note = Track.TryGetNoteFromBeatTimeType(tempBeatTimeRef.beatTime, tempBeatTimeRef.type);
				if(draggableNote.note == null) {
					return null;
				}
				
				draggableNote.time = tempBeatTimeRef.beatTime;

				draggableNote.startPosition = draggableNote.noteGO.transform.position;


				//if it's a normal note, we get the position count of the line renderer to see if it's a rail start.
				if (hit.transform.parent.GetComponentInChildren<LineRenderer>().positionCount > 0) {
					draggableNote.type = EditorNoteType.RailStart;
					
					//Since it's a wave start, we add the children transforms to the connected nodes list.
					draggableNote.GetConnectedNodes();

				}
				
				//Else it's just a boring old note.
				else {
					draggableNote.type = EditorNoteType.Standard;
				}
			}


			if (draggableNote.noteGO) draggableNote.exists = true;

			//Debug.Log(String.Format("Found note of type {0}, name {1}", draggableNote.type, draggableNote.note.name));

			return draggableNote;
		}

		return new EditorNote();
	}
}