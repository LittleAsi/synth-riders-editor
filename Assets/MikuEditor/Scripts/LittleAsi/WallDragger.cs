using System;
using System.Collections.Generic;
using System.Data;
using MiKu.NET;
using MiKu.NET.Charting;
using ThirdParty.Custom;
using UnityEngine;

public class WallDragger : MonoBehaviour {
	public LayerMask wallsLayer;
	public LayerMask gridLayer;
	public Camera activatedCamera;
	public GridManager gridManager;
	public NotesArea notesArea;
	private EditorWall selectedWall = new EditorWall();
	private float[] originWallPosition = new float[] {0, 0, 0};
	private bool isDragging = false;
	private static Vector3 mouseWallOffset = Vector3.zero;

	private void Update() {
		if (isDragging && selectedWall.exists) {
			Vector3 mousePos = GetPosOnGridFromMouse();
			if (mousePos.z != 0) { // If the mouse moves out of the grid boundary, don't adjust the wall position
				Vector3 adjustedPos = mousePos-mouseWallOffset;
				if (notesArea.SnapToGrip) adjustedPos = gridManager.GetNearestPointOnGrid(adjustedPos);
				selectedWall.wallGO.transform.position = new Vector3(adjustedPos.x, adjustedPos.y, selectedWall.wallGO.transform.position.z);
			}
			if (selectedWall.exists && Input.GetMouseButtonUp(0)) {
				EndCurrentDrag();
			}
		}
	}

	public void StartNewDrag() {
		selectedWall = WallUnderMouse(activatedCamera, wallsLayer);
		originWallPosition = selectedWall.getPosition();
		isDragging = true;
	}

	private void EndCurrentDrag() {
		if (!selectedWall.exists) return;
		Vector3 finalPos = selectedWall.wallGO.transform.position;
		selectedWall.setPosition(new float[3] {finalPos.x, finalPos.y, finalPos.z});
		Track.FinalizeWallDrag(selectedWall.time, selectedWall.getType(), originWallPosition, selectedWall.getPosition());
		Track.HistoryChangeDragWall(selectedWall.getType(), selectedWall.time, originWallPosition, selectedWall.getPosition());
		selectedWall = new EditorWall();
		originWallPosition = new float[] {0, 0, 0};
		mouseWallOffset = Vector3.zero;
		isDragging = false;
	}
	
	public bool isWallUnderMouse(){
		EditorWall checkedWall = WallUnderMouse(activatedCamera, wallsLayer);
		if (checkedWall !=null && checkedWall.exists && checkedWall.wallGO != null) return true;
		else return false;
	}
	
	public Vector3 getWallUnderMousePosition(){
		EditorWall checkedWall = WallUnderMouse(activatedCamera, wallsLayer);
		if (checkedWall !=null && checkedWall.exists && checkedWall.wallGO != null) return checkedWall.wallGO.transform.position;
		else return new Vector3(0, 0, 0);
	}
	
	public EditorWall WallUnderMouse(Camera camera, LayerMask layer) {
		RaycastHit hit;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 50f, layer)) {
			if (!hit.transform) return null;
			EditorWall draggableWall = new EditorWall();
			draggableWall.wallGO = hit.transform.parent.gameObject;
			mouseWallOffset = hit.point-draggableWall.wallGO.transform.position;
			if (draggableWall.wallGO != null) {
				draggableWall.slide = Track.TryGetSlideAtPositionZ(draggableWall.wallGO.transform.position.z);
				if (draggableWall.slide.initialized){ 
					draggableWall.exists = true;
					draggableWall.time = draggableWall.slide.time;
					return draggableWall;
				} else {
					draggableWall.crouch = Track.TryGetCrouchAtPositionZ(draggableWall.wallGO.transform.position.z);
					if (draggableWall.crouch.initialized) {
						draggableWall.exists = true;
						draggableWall.isCrouch = true;
						draggableWall.time = draggableWall.crouch.time;
						return draggableWall;
					}
				}
			}
		}
		return new EditorWall();
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

public class EditorWall {
	public GameObject wallGO;
	public Slide slide;
	public Crouch crouch;
	public bool exists = false;
	public bool isCrouch = false;
	public float time;
	
	public Note.NoteType getType(){
		if (isCrouch) return Note.NoteType.NoHand;
		else if (slide.initialized) return slide.slideType;
		else return Note.NoteType.NoHand;
	}
	
	public void setPosition(float[] _pos){
		if (isCrouch && crouch.initialized){
			crouch.position = _pos;
			wallGO.transform.position = new Vector3 (_pos[0], _pos[1], _pos[2]);
		}
		else if (slide.initialized){
			slide.position = _pos;
			wallGO.transform.position = new Vector3 (_pos[0], _pos[1], _pos[2]);
		}
	}
	
	public float[] getPosition(){
		if (isCrouch && crouch.initialized) return crouch.position;
		else if (slide.initialized)return slide.position;
		else return new float[] {};
	}
}