using System;
using System.Collections.Generic;
using System.Linq;
using MiKu.NET;
using MiKu.NET.Charting;
using LittleAsi.History;
using ThirdParty.Custom;
using UnityEngine;


public class RailEditor : MonoBehaviour {
	public float distanceBackThreshold = 20.0f;
	
	[SerializeField] private Transform currentOriginPos;

	public Camera activatedCamera;
	public LayerMask notesLayer;

	private EditorNote activeRail = new EditorNote();
	private Note.NoteType selectedNoteType;

	public static bool activated = false;


	private void Update() {
		if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) {
			Activate();
		}

		else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl)) {
			Deactivate();
		}

		//if (!activated) return;
		
	}

	private void OnApplicationFocus(bool hasFocus) {
		Deactivate();
	}

	public void RemoveNodeFromActiveRail() {
		EditorNote note = NoteRayUtil.NoteUnderMouse(activatedCamera, notesLayer);
		RemoveNodeFromActiveRail(note);
	}
	
	public void RemoveNodeFromActiveRail(EditorNote _note) {
		EditorNote note = _note;
		if (note == null || note.noteGO == null || (note.exists && note.type != EditorNoteType.RailNode)) return;
		Game_LineWaveCustom waveCustom = note.noteGO.transform.parent.GetComponentInChildren<Game_LineWaveCustom>();
		if(note.connectedNodes.Count==1){
			// If this is the only node on the rail, record the before and after states of the parent note instead of the node to avoid errors during Undo; also delete and replace parent note to reset rail line.
			Note.NoteType selectedNoteTypeBackup = Track.s_instance.selectedNoteType;
			Track.s_instance.selectedNoteType = note.note.Type;
			activeRail = FindNearestRailBack();
			if (!activeRail.exists) {
				Debug.Log("No active rail found!");
				return;
			}
			Track.s_instance.selectedNoteType = selectedNoteTypeBackup;
			Track.HistoryChangeRailNodeParent(activeRail.note.Type, activeRail.time, new float[] {activeRail.note.Position[0], activeRail.note.Position[1], activeRail.note.Position[2]}, activeRail.note.Segments, new float[,] {});
			note.connectedNodes.Remove(note.noteGO.transform);
			DestroyImmediate(note.noteGO);
			//DestroyImmediate(activeRail.noteGO.transform.Find("LineArea"));
			//DestroyImmediate(waveCustom);
			note.note.Segments = new float [,] {};
			bool changingHistoryBackup = History.changingHistory;
			History.changingHistory = true;
			float savedBeat = activeRail.time;
			float[] savedPosition = new float[] {activeRail.note.Position[0], activeRail.note.Position[1], activeRail.note.Position[2]};
			Note.NoteType savedType = activeRail.note.Type;
			Track.DeleteIndividualNote(activeRail);
			Track.AddIndividualNote(savedBeat, savedPosition, savedType, new float [,] {});
			History.changingHistory = changingHistoryBackup;
		}
		else {
			Track.HistoryChangeRailNode(note.note.Type, false, Mathf.RoundToInt(Track.s_instance.GetBeatMeasureByUnit(note.noteGO.transform.position.z)), new float[] {note.noteGO.transform.position.x, note.noteGO.transform.position.y, note.noteGO.transform.position.z});
			note.connectedNodes.Remove(note.noteGO.transform);
			DestroyImmediate(note.noteGO);
			if (waveCustom) {
				var segments = GetLineSegementArrayPoses(note.connectedNodes);
				//Update the actual values in the note.
				note.note.Segments = segments;
				waveCustom.targetOptional = segments;
				waveCustom.RenderLine(true, true);
			}
		}
		Track.s_instance.UpdateSegmentsList();
		if(Track.s_instance.FullStatsContainer.activeInHierarchy) Track.s_instance.GetCurrentStats();
	}

	public void AddNodeToActiveRail(GameObject go) {
		AddNodeToActiveRail(go.transform.position.x, go.transform.position.y, go.transform.position.z);
	}
	
	public void AddNodeToActiveRail(float _x, float _y, float _z) {
		if (Track.s_instance.isOnLongNoteMode) {
			Miku_DialogManager.ShowDialog(Miku_DialogManager.DialogType.Info, "Can't add to rail in Long Mode.'");
			return;
		}
		activeRail = FindNearestRailBack();
		if (!activeRail.exists) return;
		List<Segment> segments = Track.s_instance.SegmentsList.FindAll(x => x.measure == Track.CurrentSelectedMeasure && x.note.Type == selectedNoteType);
		if(segments != null && segments.Count > 0) { 
			Debug.Log("Existing node found!");
			return;
		}
		GameObject noteGO = GameObject.Instantiate(Track.s_instance.GetNoteMarkerByType(selectedNoteType, true));
		noteGO.transform.localPosition = new Vector3(_x, _y, _z);
		noteGO.transform.rotation =	Quaternion.identity;
		noteGO.transform.localScale *= Track.s_instance.m_NoteSegmentMarkerRedution;
		noteGO.transform.parent = activeRail.noteGO.transform.Find("LineArea");
		noteGO.name = activeRail.note.Id+"_Segment";
		activeRail.GetConnectedNodes();
		Track.HistoryChangeRailNode(selectedNoteType, true, Track.CurrentSelectedMeasure, new float[] {_x, _y, _z});		
		Dictionary<float, List<Note>> workingTrack = Track.s_instance.GetCurrentTrackDifficulty();
		if(!workingTrack.ContainsKey(activeRail.time)) {
			Debug.Log("Error while finding rail start.");
		}
		float[,] poses = GetLineSegementArrayPoses(activeRail.connectedNodes);
		
		if (workingTrack[activeRail.time][0].Type == selectedNoteType && workingTrack[activeRail.time][0].Segments != null) {
			workingTrack[activeRail.time][0].Segments = poses;
		}
		else {
			workingTrack[activeRail.time][1].Segments = poses;
		}
		var waveCustom = activeRail.noteGO.transform.Find("LineArea").GetComponentInChildren<Game_LineWaveCustom>();
		if (waveCustom) {
			waveCustom.targetOptional = poses;
			waveCustom.RenderLine(true, true);

			if(Track.s_instance.FullStatsContainer.activeInHierarchy) {
                Track.s_instance.GetCurrentStats();
            }
		}
		Track.s_instance.UpdateSegmentsList();
	}
	
	

	private LongNote GetLongNoteFromRailStart(EditorNote rNote) {
		LongNote lNote = new LongNote();
		lNote.startTime = rNote.time;
		lNote.note = rNote.note;
		lNote.gameObject = rNote.noteGO;
		lNote.segmentAxis = new List<int>();
		lNote.segments = new List<GameObject>();
		lNote.lastSegment = rNote.connectedNodes.Count - 1;

		foreach (Transform t in rNote.connectedNodes) {
			lNote.segments.Add(t.gameObject);
			lNote.segmentAxis.Add(Track.YAxisInverse ? -1 : 1);
		}

		return lNote;
	}
	
	public float[,] GetLineSegementArrayPoses(List<Transform> connectNodes) {
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

	public float[,] GetLineSegementArrayPoses(List<GameObject> GOs) {
		List<Transform> transforms = new List<Transform>();

		foreach (GameObject go in GOs) {
			transforms.Add(go.transform);
		}

		return GetLineSegementArrayPoses(transforms);
	}

	public void Activate() {
		activated = true;
	}

	public void Deactivate() {
		activated = false;
	}


	public EditorNote FindNearestRailBack() {
		selectedNoteType = Track.s_instance.selectedNoteType;
		
		EditorNote railStart = new EditorNote();
		
		Dictionary<float, List<Note>> workingTrack = Track.s_instance.GetCurrentTrackDifficulty();

		List<float> keys_tofilter = workingTrack.Keys.ToList();


		List<float> keysOrdered_ToFilter = keys_tofilter.OrderBy(f => f).ToList();


		int totalFilteredTime = keysOrdered_ToFilter.Count - 1;
		for (int filterList = totalFilteredTime; filterList >= 0; filterList--) {
			// If the time key exist, check how many notes are added
			//float targetTime = Track.GetTimeByMeasure(keysOrdered_ToFilter[filterList]);
			float targetBeat = keysOrdered_ToFilter[filterList];

			if (targetBeat > Track.CurrentSelectedMeasure) continue;

			List<Note> notes = workingTrack[targetBeat];
			int totalNotes = notes.Count;

			foreach (Note n in notes) {
				//If it's a rail start and it's the same note type as the user has selected.
				if (n.Segments != null && n.Type == selectedNoteType) {
					Debug.Log("Rail found! Note ID " + n.Id);
					
					railStart.note = n;
					railStart.type = EditorNoteType.RailStart;
					railStart.noteGO = GameObject.Find(n.Id);
					railStart.time = targetBeat;
					railStart.exists = true;
					
					railStart.GetConnectedNodes();
					
					if ((currentOriginPos.position.z - railStart.connectedNodes.Last().position.z) > distanceBackThreshold ) {
						Miku_DialogManager.ShowDialog(Miku_DialogManager.DialogType.Info, "Nearest rail is too far back to edit.");
						return new EditorNote();
					}
					

					return railStart;
				}
			}
		}


		Miku_DialogManager.ShowDialog(Miku_DialogManager.DialogType.Info, "No rails found to edit.");

		return new EditorNote();
	}
}