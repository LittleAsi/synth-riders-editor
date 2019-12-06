using System; 
using System.Collections; 
using System.Collections.Generic; 
using MiKu.NET;
using MiKu.NET.Charting;

namespace LittleAsi.History { 

	/* 
	
	-- How to register changes for undo/redo --
	
	Begin recording a HistoryEvent, which registers all changes associated with a single undo action:
	HistoryEvent historyEvent = new HistoryEvent();
	
	Record a HistoryChange to the HistoryEvent whenever an object is added or removed (two if an object was changed):
	historyEvent.Add(new HistoryChange(
		History.HistoryObjectType, - Type of object changed; see HistoryObjectType enum for types
		bool, - true if registering a post-change state; false if registering a pre-change event 
		Note.NoteType, - Sub-type of the object changed, e.g. note type, slide type; any Note.NoteType value is fine if there are no varieties of the object type
		_noteToDelete.Position, - array of floats indicating the X, Y, and Z position of the object; Z is used to calculate time
		_noteToDelete.Segments - Two dimensional array of floats indicating the positions of any rail nodes; can be an empty array if the object isn't a note with associated segments
	));
	
	Once all changes have been completed, add the HistoryEvent to the History:
	history.Add(historyEvent);
	
	Notes:
	- Only register events for segments when changes are made to them independently of the parent note.
	- The undo and redo behaviors are defined in undo() and redo() in Track.cs.
	- Set History.changingHistory to true to make unrecorded changes to the chart without recording; set it back to false after unrecorded changes are complete.
	
	*/
	
	// The state of one object either before or after a change is made.
	public class HistoryChange {     
		public History.HistoryObjectType Type { get; set; }	
		public Note.NoteType SubType { get; set; }
		public float Beat;
		public float[] Position { get; set; }
		public bool Added { get; set; }
		public float[,] Segments  {get; set; }

		public HistoryChange(History.HistoryObjectType _type, bool _added, Note.NoteType _subType, float _beat, float[] _pos, float[,] _segments) {
			Type = _type; // Note, segment, slider, etc.
			Added = _added; // True if this event is describing a post state object; false if describing original state 
			SubType = _subType; // Note type, slider type, etc.
			Beat = _beat;
			Position = _pos;
			Segments = _segments;
			//Debug.Log("New HistoryChange");
		}
		
		public void Report(){
			//Debug.Log("Type: " + Type + ", Added: " + Added + ", SubType: " + SubType + ", Time: " + Time + ", Position: {" + Position[0] + ", " + Position[1] + ", " + Position[2] + "}" + ", Segments: " + Segments + ". ");
		}
	}
	
	// The before and after states of each object affected by a single action
	public class HistoryEvent{
		public List<HistoryChange> added; // Post-change object states
		public List<HistoryChange> removed; // Pre-change object states
		
		public HistoryEvent(){
			added = new List<HistoryChange>();
			removed = new List<HistoryChange>();
			//Debug.Log("New History Event");
		}
		
		public void Add(HistoryChange _change){
			if (!History.changingHistory){
				if (_change.Added) added.Add(_change);
				else removed.Add(_change);
				//Debug.Log("Adding HistoryChange " + _change + " to HistoryEvent " + this);
			}
		}
		
		public List<HistoryChange> Undo(){
			List<HistoryChange> toUndo = new List<HistoryChange>();
			toUndo.AddRange(added);
			toUndo.AddRange(removed);
			// Ordered so that any removals will occur before any additions
			return toUndo;
		}
		
		public List<HistoryChange> Redo(){
			List<HistoryChange> toRedo = new List<HistoryChange>();
			toRedo.AddRange(removed);
			toRedo.AddRange(added);
			// Ordered so that any removals will occur before any additions
			return toRedo;
		}
		
		public bool isPopulated(){
			if (added.Count>0 || removed.Count>0) return true;
			else return false;
		}
		
		public void Report(){
			//Debug.Log(added.Count + " additions:");
			foreach(HistoryChange addeditem in added){
				addeditem.Report();
			}
			//Debug.Log(added.Count + " removals:");
			foreach(HistoryChange removeditem in removed){
				removeditem.Report();
			}
		}
	}
	
	// The full list of history events
	public class History{
		public LinkedList<HistoryEvent> events; // For undo()
		public LinkedList<HistoryEvent> undoneEvents; // For redo()
		private static int capacity = 50; // Max number of events to maintain in memory
		public static bool changingHistory = false; // Set to true when changes should not be recorded (such as when und0() snd redo() run)
	
		public enum HistoryObjectType {
			HistoryNote, // Standard notes or rail starts
			HistorySegment, // Rail nodes
			HistoryEffect, // Lights actually appear to be Effects in Track.cs
			HistoryJump, // Seems unused
			HistoryCrouch, // Top walls
			HistorySlide, // All other wall types
			HistoryLight // Seems unused
		}
		
		public History(){
			events = new LinkedList<HistoryEvent>();
			undoneEvents = new LinkedList<HistoryEvent>();
		}
		
		public void Add(HistoryEvent _event){
			if (!changingHistory && _event.isPopulated()){
				if (events.Count >= capacity) events.RemoveLast();
				events.AddFirst(_event);
				undoneEvents.Clear();
				//Debug.Log("Adding HistoryEvent " + _event + " to History");
			}
		}
		
		public List<HistoryChange> Undo(){
			List<HistoryChange> toUndo = new List<HistoryChange>(events.First.Value.Undo());
			if (undoneEvents.Count >= capacity) undoneEvents.RemoveLast();
			undoneEvents.AddFirst(events.First.Value);
			events.RemoveFirst();
			return toUndo;
		}
		
		public bool hasHistoryToUndo(){
			if (events.Count>0) return true;
			else return false;
		}
		
		public List<HistoryChange> Redo(){
			List<HistoryChange> toRedo = new List<HistoryChange>(undoneEvents.First.Value.Redo());
			if (events.Count >= capacity) events.RemoveLast();
			events.AddFirst(undoneEvents.First.Value);
			undoneEvents.RemoveFirst();
			return toRedo;
		}
		
		public bool hasHistoryToRedo(){
			if (undoneEvents.Count>0) return true;
			else return false;
		}
	}
}
