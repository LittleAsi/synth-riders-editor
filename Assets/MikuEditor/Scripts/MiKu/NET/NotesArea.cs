using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiKu.NET {
    public class NotesArea : MonoBehaviour {

        public static NotesArea s_instance;

        [SerializeField]
        private GridManager grid;

        [Space(20)]
        [SerializeField]
        private GameObject m_boundBox;
        private Transform boundBoxTransform;
        [SerializeField]
        private GameObject[] m_historyCircle;
        private Transform[] historyCircleTransform;

        [Space(20)]
        [Header("Confort Boundaries")]
        [SerializeField]
        private float m_confortableBoundarie = 0.35f;

        [SerializeField]
        private float m_moderateBoundarie = 0.36f;

        /* [SerializeField]
        private float m_intense = 0.76f; */

        [Space(20)]
        [Header("Confortable Colors")]
        [SerializeField]
        private Color m_confortableColor = Color.blue;
        [SerializeField]
        private Color m_moderateColor = Color.yellow;
        [SerializeField]
        private Color m_intenseColor = Color.red;

        [Space(20)]
        [Header("Note Colors")]
        [SerializeField]
        private Color m_leftHandColor = Color.blue;
        [SerializeField]
        private Color m_rightHandColor = Color.yellow;
        [SerializeField]
        private Color m_OneHandColor = Color.red;
        [SerializeField]
        private Color m_BothHandColor = Color.red;	

        private GameObject selectedNote;
        private GameObject mirroredNote;

        private bool snapToGrip = true;

        RaycastHit hit;
        Ray ray;
        Vector3 rayPos = Vector3.zero;

        /* bool rayEnabled = false; */

        SpriteRenderer boundBoxSpriteRenderer;
        SpriteRenderer[] historyCircleSpriteRenderer;

        public LayerMask targetMask = 11;
        private bool isCTRLDown;
        private bool isALTDown;
        private bool isSHIFDown;

        Vector3 finalPosition, mirroredPosition;
        GameObject[] multipleNotes;

        public bool SnapToGrip
        {
            get
            {
                return snapToGrip;
            }

            set
            {
                snapToGrip = value;
            }
        }

        void Awake() {
            s_instance = this;

            multipleNotes = new GameObject[2];
        }

        void Start() {
            boundBoxTransform = m_boundBox.transform;
            boundBoxSpriteRenderer = m_boundBox.GetComponent<SpriteRenderer>();
            m_boundBox.SetActive(false);

            historyCircleTransform = new Transform[m_historyCircle.Length];
            historyCircleSpriteRenderer = new SpriteRenderer[m_historyCircle.Length];
            for(int i = 0; i < m_historyCircle.Length; ++i) {
                historyCircleTransform[i] = m_historyCircle[i].transform;
                historyCircleSpriteRenderer[i] = m_historyCircle[i].GetComponent<SpriteRenderer>();
            }
        }		

        void OnDisable() {
            if(selectedNote != null) {
                GameObject.DestroyImmediate(selectedNote);
            }

            if(mirroredNote != null) {
                GameObject.DestroyImmediate(mirroredNote);
            }

            m_boundBox.SetActive(false);
        }

        void EnabledSelectedNote() {
            if(selectedNote == null) {
                selectedNote = Track.GetSelectedNoteMarker();
                SphereCollider coll = selectedNote.GetComponent<SphereCollider>();
                if(coll == null) {
                    coll = selectedNote.GetComponentInChildren<SphereCollider>();
                }

                coll.enabled = false;

                m_boundBox.SetActive(true);	

                if(Track.IsOnMirrorMode) {
                    mirroredNote = Track.GetMirroredNoteMarker();
                }
            }					
        }

        void DisableSelectedNote() {
            if(selectedNote != null) {
                GameObject.DestroyImmediate(selectedNote);
                m_boundBox.SetActive(false);
            }	

            if(mirroredNote != null) {
                GameObject.DestroyImmediate(mirroredNote);
            }		
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if(hasFocus) {
                isCTRLDown = false;
                isALTDown = false;
                isSHIFDown = false;
            } 
        }

        void Update()
        {
            if(Input.GetButtonDown("Input Modifier1"))
            {
                isCTRLDown = true;
            }

            // Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl)
            if(Input.GetButtonUp("Input Modifier1")) {
                isCTRLDown = false;
            }

            // Input.GetKeyDown(KeyCode.LeftAlt)
            if(Input.GetButtonDown("Input Modifier2")) {
                isALTDown = true;
            }

            // Input.GetKeyUp(KeyCode.LeftAlt)
            if(Input.GetButtonUp("Input Modifier2")) {
                isALTDown = false;
            }

            if(Input.GetButtonDown("Input Modifier3")) {
                isSHIFDown = true;				
            }

            // Input.GetKeyUp(KeyCode.LeftAlt)
            if(Input.GetButtonUp("Input Modifier3")) {
                isSHIFDown = false;
            }
            
            if (Input.GetMouseButtonDown(0) && selectedNote != null) {
                if(!isALTDown && !isCTRLDown && !isSHIFDown) {
                    if(Track.IsOnMirrorMode) {
                        System.Array.Clear(multipleNotes, 0, 2);
                        multipleNotes[0] = selectedNote;
                        multipleNotes[1] = mirroredNote;
                        Track.AddNoteToChart(multipleNotes);
                    } else {
                        Track.AddNoteToChart(selectedNote);
                    }	
                } else {
                    if(isCTRLDown && !isALTDown && !isSHIFDown) {
                        Track.TryMirrorSelectedNote(selectedNote.transform.position);
                    } else if(isSHIFDown && !isALTDown && !isCTRLDown) { 
                        Track.TryChangeColorSelectedNote(selectedNote.transform.position);
                    }
                }               			
            }
        }

        void FixedUpdate() {	
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, targetMask.value)) {
                EnabledSelectedNote();

                rayPos = hit.point;
                rayPos.z = (float)Track.CurrentUnityUnit;

                finalPosition = (SnapToGrip) ? grid.GetNearestPointOnGrid(rayPos) : rayPos;

                selectedNote.transform.position = finalPosition;
                boundBoxTransform.position = finalPosition;

                if(Track.IsOnMirrorMode) {
                    mirroredPosition = finalPosition;

                    if(Track.XAxisInverse) {
                        mirroredPosition.x *= -1;
                    }

                    if(Track.YAxisInverse) {
                        mirroredPosition.y *= -1;
                    }
                    
                    mirroredNote.transform.position = mirroredPosition;
                }

                //float toCenter = Mathf.Abs(Vector3.Distance(transform.position, finalPosition));
                SetBoundaireBoxColor(DistanceToCenter(finalPosition));					
            } else {
                DisableSelectedNote();
            }
        }

        public void RefreshSelectedObjec() {
            if(selectedNote != null) {
                GameObject.DestroyImmediate(selectedNote);
                selectedNote = Track.GetSelectedNoteMarker();
                if(Track.IsOnMirrorMode) {
                    GameObject.DestroyImmediate(mirroredNote);
                    mirroredNote = Track.GetMirroredNoteMarker();
                } else {
                    if(mirroredNote != null) {
                        GameObject.DestroyImmediate(mirroredNote);
                    }
                }

                SphereCollider coll = selectedNote.GetComponent<SphereCollider>();
                if(coll == null) {
                    coll = selectedNote.GetComponentInChildren<SphereCollider>();
                }

                coll.enabled = false;
            }			
        }

        private void SetBoundaireBoxColor(float distanceToCenter) {
            boundBoxSpriteRenderer.color = GetColorToDistance(distanceToCenter);
        }

        public void HideHistoryCircle() {
            foreach(GameObject hc in m_historyCircle) {
                hc.SetActive(false);
            }
        }

        public void SetHistoryCircleColor(Vector3[] points, Charting.Note.NoteType[] types) {
            HideHistoryCircle();

            for(int i = 0; i < points.Length; ++i) {
                m_historyCircle[i].SetActive(true);
                historyCircleTransform[i].localPosition = points[i];
                if(types[i] == Charting.Note.NoteType.LeftHanded) {
                    historyCircleSpriteRenderer[i].color = m_leftHandColor;
                } else if(types[i] == Charting.Note.NoteType.RightHanded) {
                    historyCircleSpriteRenderer[i].color = m_rightHandColor;
                } else if(types[i] == Charting.Note.NoteType.OneHandSpecial) {
                    historyCircleSpriteRenderer[i].color = m_OneHandColor;
                } else if(types[i] == Charting.Note.NoteType.BothHandsSpecial) {
                    historyCircleSpriteRenderer[i].color = m_BothHandColor;
                } 
            }
        }

#region Static Methods
        public static float DistanceToCenter(Vector3 targetPoint) {
            return Mathf.Abs(Vector3.Distance(s_instance.transform.position, targetPoint));
        }

        public static Color GetColorToDistance(float distanceToCenter) {
            if(distanceToCenter <= s_instance.m_confortableBoundarie) {
                return s_instance.m_confortableColor;
                ;
            }

            if(distanceToCenter <= s_instance.m_moderateBoundarie) {
                return s_instance.m_moderateColor;
            }

            return s_instance.m_intenseColor;
        }
#endregion
        
    }
}