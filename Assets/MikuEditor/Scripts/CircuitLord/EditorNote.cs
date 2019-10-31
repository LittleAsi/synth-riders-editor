

using System.Collections.Generic;
using System.Linq;
using MiKu.NET.Charting;
using UnityEngine;

public class EditorNote {
	public GameObject noteGO;

	/// <summary>
	/// The note script related to this note. For rails it's the rail start node.
	/// </summary>
	public Note note;

	public Vector3 startPosition;
	public EditorNoteType type;
	public EditorNoteHand hand;
	public List<Transform> connectedNodes = new List<Transform>();
	public bool exists = false;

	public float time = 0.0f;

	/// <summary>
	/// Gets all the nodes connected to a rail or a rail node. Must be run after setting other note properties.
	/// </summary>
	public void GetConnectedNodes() {

		if (type == EditorNoteType.Standard) return;
		
		connectedNodes.Clear();

		Transform railNodes = noteGO.transform.parent;
		
		switch (type) {
			case EditorNoteType.RailStart:
				railNodes = noteGO.transform.Find("LineArea");
				break;
			case EditorNoteType.RailNode:
				railNodes = noteGO.transform.parent;
				break;
		}
		
		foreach (Transform child in railNodes) {

			if (!child.gameObject.CompareTag("SphereMarker")) continue;
			
			connectedNodes.Add(child);
		}

		connectedNodes = connectedNodes.OrderBy(t => t.position.z).ToList();

	}
	
}

public enum EditorNoteType {
	Standard = 0,
	RailStart = 1,
	RailNode = 2
}

public enum EditorNoteHand {
	Left = 0,
	Right = 1,
	Any = 2
}
