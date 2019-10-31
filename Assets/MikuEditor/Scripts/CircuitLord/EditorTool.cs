


using UnityEngine;

public abstract class EditorTool : MonoBehaviour {
	
	public static bool activated = false;
	public abstract void Activate();
	public abstract void Deactivate();
}