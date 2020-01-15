using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic; 
using MiKu.NET;
 
public class Controller : MonoBehaviour {
	public static Controller controller;
	public Dictionary<string, KeyBinding> keyBindings = new Dictionary<string, KeyBinding>();
	public Dictionary<string, KeyBinding> defaultKeyBindings = new Dictionary<string, KeyBinding>();
	public KeyCode[,] inputModifiers = new KeyCode[3, 2];
	public KeyCode[,] defaultInputModifiers = new KeyCode[3, 2];
	public bool isMod1Down = false;
	public bool isMod2Down = false;
	public bool isMod3Down = false;
	[SerializeField]
    private GameObject m_KeybindingCheatsheet;

    void Awake(){
		if(controller == null){
			DontDestroyOnLoad(this.gameObject);
			controller = this;
		} else if(controller != this){
			Destroy(this.gameObject);
		}
    }
 
    void Start (){
		// Create headers, input modifiers, and keybindings in the order they should appear in the keybindings UI
		
		CreateKeyBindSection("Input Modifiers", true);
		
		/* Define each input modifier here:
		Everything is set up to expect exactly three input modifiers
		CreateInputModifier(int _modifierNumber, string _defaultPrimary, string _defaultSecondary);
			_modifierNumber - Valid values are 1, 2, and 3
			_defaultPrimary - The default KeyCode for the modifier (overridden by PlayerPrefs)
			_defaultSecondary - The default alternate KeyCode for the modifier (overridden by PlayerPrefs)
		A full list of valid KeyCodes is included at the bottom of this file. */
		
		CreateInputModifier(1, "LeftControl", "JoystickButton0");
		CreateInputModifier(2, "LeftAlt", "JoystickButton1");
		CreateInputModifier(3, "LeftShift", "JoystickButton2");
		
		/* Define each keybinding here:
		CreateKeyBinding(string _actionMethodName, string _description, string _defaultPrimary, bool _defaultMod1Primary, bool _defaultMod2Primary, bool _defaultMod3Primary, string _defaultSecondary, bool _defaultMod1Secondary, bool _defaultMod2Secondary, bool _defaultMod3Secondary);
			_actionMethodName - The method the key calls in Track.cs 
			_description - Description of the action that appears in the keybindings UI 
			_defaultPrimary - The default KeyCode for the action (overridden by PlayerPrefs)
			_defaultMod1Primary - True if this action requires input modifier 1 to be down for the primary binding (overridden by PlayerPrefs)
			_defaultMod2Primary - True if this action requires input modifier 2 to be down for the primary binding (overridden by PlayerPrefs)
			_defaultMod3Primary - True if this action requires input modifier 3 to be down for the primary binding (overridden by PlayerPrefs)
			_defaultSecondary - The default alternate KeyCode for the action (overridden by PlayerPrefs)
			_defaultMod1Secondary - True if this action requires input modifier 1 to be down for the secondary binding (overridden by PlayerPrefs)
			_defaultMod2Secondary - True if this action requires input modifier 2 to be down for the secondary binding (overridden by PlayerPrefs)
			_defaultMod3Secondary - True if this action requires input modifier 3 to be down for the secondary binding (overridden by PlayerPrefs)
			_includeInCheatsheet - Optional (false by default) - If true, the keybindings cheatsheet UI will display this action
		A full list of valid KeyCodes is included at the bottom of this file. */
		
		CreateKeyBindSection("Map Editing Controls");
		CreateKeyBinding("AddNoteAction", "Place a Note/Wall on the Grid", "Mouse0", false, false, false, "None", false, false, false);
		CreateKeyBinding("DragSnapObjectAction", "Drag Note/Wall or Snap Note/Wall to Grid", "Mouse0", false, true, false, "None", false, false, false, true);
		CreateKeyBinding("SnapGridToObjectAction", "Snap Grid to Note/Wall", "Mouse1", false, true, false, "None", false, false, false, true);
		CreateKeyBinding("RemoteDeleteAction", "Remote Delete Note/Wall", "Mouse1", true, false, false, "None", false, false, false, true);
		CreateKeyBinding("TogglePlayAction", "Toggle Play", "Space", false, false, false, "JoystickButton3", false, false, false);
		CreateKeyBinding("TogglePlayReturnAction", "Toggle Play (Return)", "R", false, false, false, "Space", true, false, false);
		CreateKeyBinding("SelectLeftHandNoteAction", "Select Left Hand Note", "Alpha1", false, false, false, "Keypad1", false, false, false);
		CreateKeyBinding("SelectRightHandNoteAction", "Select Right Hand Note", "Alpha2", false, false, false, "Keypad2", false, false, false);
		CreateKeyBinding("SelectOneHandSpecialNoteAction", "Select One Hand Special Note", "Alpha3", false, false, false, "Keypad3", false, false, false);
		CreateKeyBinding("SelectTwoHandSpecialNoteAction", "Select Two Hand Special Note", "Alpha4", false, false, false, "Keypad4", false, false, false);
		CreateKeyBinding("AddNodeToExistingRailAction", "Add Node to Nearest Existing Rail", "Mouse0", true, false, false, "None", false, false, false, true);
		CreateKeyBinding("ToggleLongNoteAction", "Toggle Long Note Mode", "L", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleBookmarkAction", "Toggle Bookmark at Current Time", "Alpha9", false, false, false, "Keypad9", false, false, false);
		CreateKeyBinding("ToggleFlashAction", "Toggle Flash Effect at Current Time", "Alpha9", true, false, false, "Keypad9", true, false, false);
		CreateKeyBinding("ToggleLeftSideWallAction", "Toggle Left Side Wall at Current Time", "Alpha1", true, false, false, "Keypad1", true, false, false);
		CreateKeyBinding("ToggleRightSideWallAction", "Toggle Right Side Wall at Current Time", "Alpha2", true, false, false, "Keypad2", true, false, false);
		CreateKeyBinding("ToggleCenterWallAction", "Toggle Center Wall at Current Time", "Alpha3", true, false, false, "Keypad3", true, false, false);
		CreateKeyBinding("ToggleLeftDiagonalWallAction", "Toggle Left Diagonal Wall at Current Time", "Alpha4", true, false, false, "Keypad4", true, false, false);
		CreateKeyBinding("ToggleRightDiagonalWallAction", "Toggle Right Diagonal Wall at Current Time", "Alpha5", true, false, false, "Keypad5", true, false, false);
		CreateKeyBinding("ToggleCrouchWallAction", "Toggle Crouch Wall Wall at Current Time", "Alpha6", true, false, false, "Keypad6", true, false, false);
		CreateKeyBinding("ToggleSelectionAreaAction", "Area Selection", "Z", false, false, false, "None", false, false, false);
		CreateKeyBinding("SelectAllAction", "Select All", "A", true, false, false, "None", false, false, false);
		CreateKeyBinding("DeleteAction", "Delete Current/Selected Objects", "Delete", false, false, false, "Backspace", false, false, false);
		CreateKeyBinding("DeleteAllObjectsAction", "Delete All Objects", "Delete", true, false, false, "None", false, false, false);
		CreateKeyBinding("ChangeNoteColorAction", "Change Existing Note Color", "Mouse0", false, false, true, "None", false, false, false, true);
		CreateKeyBinding("AddMirrorNoteAction", "Add Mirror Note", "Mouse0", true, true, false, "None", false, false, false, true);
		CreateKeyBinding("FlipAction", "Flip Object Handedness and Position Over X Axis", "F", false, false, false, "None", false, false, false, true);
		CreateKeyBinding("CopyKeyAction", "Copy the Current/Selected Objects", "C", true, false, false, "None", false, false, false, true);
		CreateKeyBinding("PasteKeyAction", "Paste the Copied Objects", "V", true, false, false, "None", false, false, false, true);
		CreateKeyBinding("PasteMirrorKeyAction", "Mirror Paste the Copied Objects", "V", false, true, false, "None", false, false, false, true);
		CreateKeyBinding("UndoAction", "Undo Previous Action", "Z", true, false, false, "None", false, false, false, true);
		CreateKeyBinding("RedoAction", "Redo Undone Action", "Y", true, false, false, "None", false, false, false, true);
		CreateKeyBinding("AddNoteWhilePlayingAction", "Add Note While Song is Playing", "N", false, false, false, "None", false, false, false);
		CreateKeyBinding("CycleNoteTypeAction", "Cycle Note Type", "Mouse2", false, false, false, "None", false, false, false);
		CreateKeyBinding("CycleMiddleMouseButtonAction", "Cycle Note Type Cycle Mode", "F5", false, false, false, "None", false, false, false);
		CreateKeyBinding("ClearAllBookmarksAction", "Clear All Bookmarks", "F8", false, false, false, "None", false, false, false);
		CreateKeyBinding("CycleStepSelectorTypeAction", "Cycle Step Type", "Tab", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleMirrorModeAction", "Toggle Mirror Mode", "F3", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleInverseYAction", "Invert Y Axis in Mirror Mode", "Y", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleSnapToGridAction", "Toggle Snap to Grid", "F3", true, false, false, "None", false, false, false, true);
		CreateKeyBinding("SaveKeyAction", "Save the Map", "S", true, false, false, "None", false, false, false);
		
		CreateKeyBindSection("Timeline Controls");
		CreateKeyBinding("MoveBackwardOnTimelineAction", "Move Backward on Timeline", "DownArrow", false, false, false, "None", false, false, false);
		CreateKeyBinding("MoveForwardOnTimelineAction", "Move Forward on Timeline", "UpArrow", false, false, false, "None", false, false, false);
		CreateKeyBinding("JumpToNextBookmarkAction", "Jump to Next Bookmark", "RightBracket", false, false, false, "PageUp", false, false, false);
		CreateKeyBinding("JumpToPreviousBookmarkAction", "Jump to Previous Bookmark", "LeftBracket", false, false, false, "PageDown", false, false, false);
		CreateKeyBinding("TimelineStartAction", "Navigate to Timeline Start", "Home", false, false, false, "None", false, false, false);
		CreateKeyBinding("TimelineEndAction", "Navigate to Timeline End", "End", false, false, false, "None", false, false, false);
		
		CreateKeyBindSection("Interface Controls");
		CreateKeyBinding("AdjustBeatStepMeasureDownAction", "Adjust Beat Step Measure Down", "LeftArrow", false, false, false, "None", false, false, false);
		CreateKeyBinding("AdjustBeatStepMeasureUpAction", "Adjust Beat Step Measure Up", "RightArrow", false, false, false, "None", false, false, false);
		CreateKeyBinding("AdjustPlaybackSpeedMeasureDownAction", "Adjust Playback Speed Down During Play", "LeftArrow", false, false, false, "None", false, false, false);
		CreateKeyBinding("AdjustPlaybackSpeedMeasureUpAction", "Adjust Playback Speed Up During Play", "RightArrow", false, false, false, "None", false, false, false);
		CreateKeyBinding("AdjustMusicVolumeDownAction", "Decrease Music Volume", "Minus", false, false, false, "KeypadMinus", false, false, false);
		CreateKeyBinding("AdjustMusicVolumeUpAction", "Increase Music Volume", "Equals", false, false, false, "KeypadPlus", false, false, false);
		CreateKeyBinding("AdjustSFXVolumeDownAction", "Decrease SFX Volume", "Minus", true, false, false, "KeypadMinus", true, false, false);
		CreateKeyBinding("AdjustSFXVolumeUpAction", "Increase SFX Volume", "Equals", true, false, false, "KeypadPlus", true, false, false);
		CreateKeyBinding("AdjustGridSizeDownAction", "Adjust Grid Size Down", "Minus", false, true, false, "KeypadMinus", false, true, false);
		CreateKeyBinding("AdjustGridSizeUpAction", "Adjust Grid Size Up", "Equals", false, true, false, "KeypadPlus", false, true, false);
		CreateKeyBinding("ToggleGridGuideAction", "Toggle the Grid Guide", "G", false, false, false, "None", false, false, false);
		CreateKeyBinding("SwitchGridGuideAction", "Cycle the Grid Guide Type", "G", true, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleStepSaverAction", "Toggle Between Beat Step Configurations", "X", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleTimelineScrollSoundAction", "Toggle Timeline Scroll Sound", "F9", false, false, false, "None", false, false, false);
		//CreateKeyBinding("ToggleNoteHighlightAction", "Toggle Note Highlight", "P", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleSidebarsAction", "Toggle Sidebars", "Tab", true, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleLastNoteShadowAction", "Toggle Last Note Shadow", "Q", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleMetronomeAction", "Toggle the Metronome", "M", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleKeyBindingsPanelAction", "Toggle the Keybindings Window", "F1", false, false, false, "None", false, false, false);
		CreateKeyBinding("AcceptPromptAction", "Accept Prompt", "Return", false, false, false, "JoystickButton0", false, false, false);
		CreateKeyBinding("DeclinePromptAction", "Decline Prompt/Return to Main Menu", "None", false, false, false, "JoystickButton1", false, false, false); // Escape is hard-coded
		CreateKeyBinding("ToggleStatsAction", "Toggle the Basic Stats Panel", "Alpha0", false, false, false, "Keypad0", false, false, false);
		CreateKeyBinding("ToggleFullStatsAction", "Toggle the Advanced Stats Panel", "F12", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleBookmarkJumpAction", "Toggle the Bookmark Jump Prompt", "B", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleMouseSensitivityPanelAction", "Toggle the Mouse Sensitivity Panel", "F6", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleAudioSpectrumAction", "Toggle the Audio Spectrum", "F7", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleTagEditWindowAction", "Toggle Tag Edit Window", "F10", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleLatencyPanelAction", "Toggle Latency Panel", "F10", true, false, false, "None", false, false, false);
		CreateKeyBinding("EditCustomDifficultyAction", "Edit Custom Difficulty Settings", "F11", false, false, false, "None", false, false, false);
		
		CreateKeyBindSection("Camera Controls");
		CreateKeyBinding("ToggleCenterCameraAction", "Enable the Center View Camera", "Alpha5", false, false, false, "Keypad5", false, false, false);
		CreateKeyBinding("ToggleLeftViewCameraAction", "Enable the Left View Camera", "Alpha6", false, false, false, "Keypad6", false, false, false);
		CreateKeyBinding("ToggleRightViewCameraAction", "Enable the Right View Camera", "Alpha7", false, false, false, "Keypad7", false, false, false);
		CreateKeyBinding("ToggleFreeViewCameraAction", "Enable the Free View Camera", "Alpha8", false, false, false, "Keypad8", false, false, false);
		CreateKeyBinding("RotateFreeViewCameraAction", "Rotate the Free View Camera", "Mouse1", false, false, false, "None", false, false, false);
		CreateKeyBinding("ResetFreeViewCameraAction", "Reset the Free View Camera", "Alpha8", true, false, false, "Keypad8", true, false, false);
		CreateKeyBinding("AdjustFreeCameraPanningLeftAction", "Adjust Free Camera Panning Left", "A", false, false, false, "None", false, false, false);
		CreateKeyBinding("AdjustFreeCameraPanningRightAction", "Adjust Free Camera Panning Right", "D", false, false, false, "None", false, false, false);
		CreateKeyBinding("AdjustFreeCameraPanningUpAction", "Adjust Free Camera Panning Up", "W", false, false, false, "None", false, false, false);
		CreateKeyBinding("AdjustFreeCameraPanningDownAction", "Adjust Free Camera Panning Down", "S", false, false, false, "None", false, false, false);
		
		CreateKeyBindSection("Other Controls");
		CreateKeyBinding("ExportJSONAction", "Export Map JSON", "F2", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleAutosaveAction", "Toggle Autosave", "F4", false, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleVsyncAction", "Toggle VSync", "F9", true, false, false, "None", false, false, false);
		CreateKeyBinding("ToggleProductionModeAction", "Toggle Draft Mode", "F9", true, true, false, "None", false, false, false);
		
		// Each _actionMethodName described above should be associated with a function in Track.cs
		// Pressing the bound keys above will trigger the associated _actionMethodName
		// Use Controller.controller.IsKeyBindingPressed("_actionMethodName") in Track.cs to check for keybindings being held down 
		// Use Controller.controller.GetKeyUp("_actionMethodName") in Track.cs to check for keys being released
		//
		// Escape is hard-coded, but alternates are bindable
		// Joystick axes are hard-coded, but alternates are bindable
		
		UpdateCheatsheet();
    }
 
    void Update (){
		if(Input.GetKeyDown(inputModifiers[0, 0]) || Input.GetKeyDown(inputModifiers[0, 1])) {
			isMod1Down = true;
			UpdateCheatsheet();
		}
		else if(Input.GetKeyUp(inputModifiers[0, 0]) || Input.GetKeyUp(inputModifiers[0, 1])) {
			isMod1Down = false;
			UpdateCheatsheet();
		}
		if(Input.GetKeyDown(inputModifiers[1, 0]) || Input.GetKeyDown(inputModifiers[1, 1])) {
			isMod2Down = true;
			UpdateCheatsheet();
		}
		else if(Input.GetKeyUp(inputModifiers[1, 0]) || Input.GetKeyUp(inputModifiers[1, 1])) {
			isMod2Down = false;
			UpdateCheatsheet();
		}
		if(Input.GetKeyDown(inputModifiers[2, 0]) || Input.GetKeyDown(inputModifiers[2, 1])) {
			isMod3Down = true;
			UpdateCheatsheet();
		}
		else if(Input.GetKeyUp(inputModifiers[2, 0]) || Input.GetKeyUp(inputModifiers[2, 1])) {
			isMod3Down = false;
			UpdateCheatsheet();
		}
		Type track = Track.s_instance.GetType();
		// If the KeyBindings panel is open, only check input for the panel toggle key
		if (KeyBinder.keyBinder.panelActive){
			KeyCode[] toggleKeyCodes = keyBindings["ToggleKeyBindingsPanelAction"].keyCodes;
			MethodInfo toggleMethod = track.GetMethod("ToggleKeyBindingsPanelAction");
			if((Input.GetKeyDown(toggleKeyCodes[0]) || Input.GetKeyDown(toggleKeyCodes[1]))) toggleMethod.Invoke(Track.s_instance, null);
			return;
		}
		// Calls the action function in Track.cs for each bound input
		foreach (KeyValuePair<string, KeyBinding> kvp in keyBindings){
			string actionMethodName = kvp.Key;
			KeyBinding keyBinding = kvp.Value;
			MethodInfo method = track.GetMethod(actionMethodName);
			if((Input.GetKeyDown(keyBinding.keyCodes[0]) && (!isMod1Down^keyBinding.mods1[0]) && (!isMod2Down^keyBinding.mods2[0]) && (!isMod3Down^keyBinding.mods3[0])) || (Input.GetKeyDown(keyBinding.keyCodes[1]) && (!isMod1Down^keyBinding.mods1[1]) && (!isMod2Down^keyBinding.mods2[1]) && (!isMod3Down^keyBinding.mods3[1]))) method.Invoke(Track.s_instance, null);
		 }
		 if(Input.GetKeyDown(KeyCode.Escape)) {
			 Track.s_instance.DeclinePromptAction(); // Escape is hard-coded
		 }
    }
	
	void OnApplicationFocus(bool hasFocus){
		if(hasFocus) {
			isMod1Down = false;
			isMod2Down = false;
			isMod3Down = false;
		}
	}
	
	private void CreateKeyBindSection(string _description, bool _excludeCheatsheetText = false){
		KeyBinder.keyBinder.CreateKeyBindSectionComposite(_description);
	}
	
	private void CreateInputModifier(int _modifierNumber, string _defaultPrimary, string _defaultSecondary){
		inputModifiers[_modifierNumber-1, 0] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("InputModifier" + _modifierNumber + "PrimaryKey", _defaultPrimary));
		inputModifiers[_modifierNumber-1, 1] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("InputModifier" + _modifierNumber + "SecondaryKey", _defaultSecondary));
		defaultInputModifiers[_modifierNumber-1, 0] = (KeyCode) System.Enum.Parse(typeof(KeyCode), _defaultPrimary);
		defaultInputModifiers[_modifierNumber-1, 1] = (KeyCode) System.Enum.Parse(typeof(KeyCode), _defaultSecondary);
		KeyBinder.keyBinder.CreateInputModifierComposite(_modifierNumber, "Input Modifier " + _modifierNumber, inputModifiers[_modifierNumber-1, 0].ToString(), inputModifiers[_modifierNumber-1, 1].ToString());
	}
	
	private void CreateKeyBinding(string _actionMethodName, string _description, string _defaultPrimary, bool _defaultMod1Primary, bool _defaultMod2Primary, bool _defaultMod3Primary, string _defaultSecondary, bool _defaultMod1Secondary, bool _defaultMod2Secondary, bool _defaultMod3Secondary, bool _defaultIncludeInCheatsheet = false){
		KeyCode[] keyCodes = new KeyCode[2];
		keyCodes[0] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(_actionMethodName + "PrimaryKey", _defaultPrimary));
		keyCodes[1] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(_actionMethodName + "SecondaryKey", _defaultSecondary));
		bool[] modifierListPrimary = new bool[3];
		bool[] modifierListSecondary = new bool[3];
		int parsedModInt = 0;
		if(int.TryParse(PlayerPrefs.GetString(_actionMethodName + "ModifierListCodePrimary", KeyBinding.ParseModiferListAsInt(_defaultMod1Primary, _defaultMod2Primary, _defaultMod3Primary).ToString()), out parsedModInt));
		else Debug.Log("Error parsing primary modifier list code! - _actionMethodName: " + _actionMethodName);
		modifierListPrimary = KeyBinding.ParseIntAsModiferList(parsedModInt);
		if(int.TryParse(PlayerPrefs.GetString(_actionMethodName + "ModifierListCodeSecondary", KeyBinding.ParseModiferListAsInt(_defaultMod1Secondary, _defaultMod2Secondary, _defaultMod3Secondary).ToString()), out parsedModInt));
		else Debug.Log("Error parsing secondary modifier list code! - _actionMethodName: " + _actionMethodName);
		modifierListSecondary = KeyBinding.ParseIntAsModiferList(parsedModInt);
		bool _includeInCheatsheet = false;
		Boolean.TryParse(PlayerPrefs.GetString(_actionMethodName + "IncludeInCheatsheet", _defaultIncludeInCheatsheet.ToString()), out _includeInCheatsheet); 
		KeyBinding workingKeyBinding = new KeyBinding(_actionMethodName, _description, keyCodes[0], modifierListPrimary[0], modifierListPrimary[1], modifierListPrimary[2], keyCodes[1], modifierListSecondary[0], modifierListSecondary[1], modifierListSecondary[2], _includeInCheatsheet);
		keyBindings.Add(_actionMethodName, workingKeyBinding);
		defaultKeyBindings.Add(_actionMethodName, new KeyBinding(_actionMethodName, _description, (KeyCode) System.Enum.Parse(typeof(KeyCode), _defaultPrimary), _defaultMod1Primary, _defaultMod2Primary, _defaultMod3Primary, (KeyCode) System.Enum.Parse(typeof(KeyCode), _defaultSecondary), _defaultMod1Secondary, _defaultMod2Secondary, _defaultMod3Secondary, _defaultIncludeInCheatsheet));
		workingKeyBinding.AssignKeybindComposite(KeyBinder.keyBinder.CreateKeyBindingComposite(_actionMethodName, _description, keyCodes[0].ToString(), modifierListPrimary[0], modifierListPrimary[1], modifierListPrimary[2], keyCodes[1].ToString(), modifierListSecondary[0], modifierListSecondary[1], modifierListSecondary[2], _includeInCheatsheet));
	}
	
	public void ResetToDefaultKeyBindings(){
		KeyCode newInputModifier = new KeyCode();
		newInputModifier = defaultInputModifiers[0, 0];
		UpdateInputModifier(1, 0, newInputModifier);
		newInputModifier = new KeyCode();
		newInputModifier = defaultInputModifiers[0, 1];
		UpdateInputModifier(1, 1, newInputModifier);
		newInputModifier = new KeyCode();
		newInputModifier = defaultInputModifiers[1, 0];
		UpdateInputModifier(2, 0, newInputModifier);
		newInputModifier = new KeyCode();
		newInputModifier = defaultInputModifiers[1, 1];
		UpdateInputModifier(2, 1, newInputModifier);
		newInputModifier = new KeyCode();
		newInputModifier = defaultInputModifiers[2, 0];
		UpdateInputModifier(3, 0, newInputModifier);
		newInputModifier = new KeyCode();
		newInputModifier = defaultInputModifiers[2, 1];
		UpdateInputModifier(3, 1, newInputModifier);
		string actionMethodName = "";
		string description = "";
		bool[] mods1 = new bool[2];
		bool[] mods2 = new bool[2];
		bool[] mods3 = new bool[2];
		bool includeInCheatsheet = false;
		KeyCode[] keyCodes = new KeyCode[2];
		foreach(KeyValuePair<string, KeyBinding> defaultKeyBinding in defaultKeyBindings){
			actionMethodName = "";
			description = "";
			mods1 = new bool[2];
			mods2 = new bool[2];
			mods3 = new bool[2];
			includeInCheatsheet = false;
			actionMethodName = defaultKeyBinding.Value.actionMethodName;
			description = defaultKeyBinding.Value.description;
			mods1[0] = defaultKeyBinding.Value.mods1[0];
			mods2[0] = defaultKeyBinding.Value.mods2[0];
			mods3[0] = defaultKeyBinding.Value.mods3[0];
			mods1[1] = defaultKeyBinding.Value.mods1[1];
			mods2[1] = defaultKeyBinding.Value.mods2[1];
			mods3[1] = defaultKeyBinding.Value.mods3[1];
			includeInCheatsheet = defaultKeyBinding.Value.includeInCheatsheet;
			keyCodes[0] = defaultKeyBinding.Value.keyCodes[0];
			keyCodes[1] = defaultKeyBinding.Value.keyCodes[1];
			UpdateKeyBinding(0, defaultKeyBinding.Key, keyCodes[0], mods1[0], mods2[0], mods3[0]);
			UpdateKeyBinding(1, defaultKeyBinding.Key, keyCodes[1], mods1[1], mods2[1], mods3[1]);
			if(keyBindings[defaultKeyBinding.Key].keybindComposite!=null) KeyBinder.keyBinder.RefreshKeybindComposite(keyBindings[defaultKeyBinding.Key].keybindComposite, keyCodes[0].ToString(), mods1[0], mods2[0], mods3[0], keyCodes[1].ToString(), mods1[1], mods2[1], mods3[1], includeInCheatsheet);
			else Debug.Log("KeybindComposite not found!");
		}
		KeyBinder.keyBinder.RefreshModComposites();
	}
	
	public void RefreshKeybindComposites(){
		foreach(KeyValuePair<string, KeyBinding> keyBinding in keyBindings){
			if(keyBindings[keyBinding.Key].keybindComposite!=null) KeyBinder.keyBinder.RefreshKeybindComposite(keyBindings[keyBinding.Key].keybindComposite, keyBindings[keyBinding.Key].keyCodes[0].ToString(), keyBindings[keyBinding.Key].mods1[0], keyBindings[keyBinding.Key].mods2[0], keyBindings[keyBinding.Key].mods3[0], keyBindings[keyBinding.Key].keyCodes[1].ToString(), keyBindings[keyBinding.Key].mods1[1], keyBindings[keyBinding.Key].mods2[1], keyBindings[keyBinding.Key].mods3[1], keyBindings[keyBinding.Key].includeInCheatsheet);
			else Debug.Log("KeybindComposite not found!");
		}
		KeyBinder.keyBinder.RefreshModComposites();
	}
	
	public void UpdateInputModifier(int _modifierNumber, int _index, KeyCode _newKey){
		inputModifiers[_modifierNumber-1, _index] = _newKey;
		string _playerPrefsSetting;
		if (_index==0) _playerPrefsSetting = ("InputModifier" + _modifierNumber + "PrimaryKey");
		else _playerPrefsSetting = ("InputModifier" + _modifierNumber + "SecondaryKey");
		PlayerPrefs.SetString(_playerPrefsSetting, _newKey.ToString());
	}
	
	public void UpdateKeyBinding(int _index, string _actionMethodName, KeyCode _newKey, bool _mod1, bool _mod2, bool _mod3){
		keyBindings[_actionMethodName].UpdateKeyBinding(_index, _newKey, _mod1, _mod2, _mod3);
		string _playerPrefsSetting;
		if (_index==0) _playerPrefsSetting = (_actionMethodName + "PrimaryKey");
		else _playerPrefsSetting = (_actionMethodName + "SecondaryKey");
		PlayerPrefs.SetString(_playerPrefsSetting, _newKey.ToString());
		if (_index==0) _playerPrefsSetting = (_actionMethodName + "ModifierListCodePrimary");
		else _playerPrefsSetting = (_actionMethodName + "ModifierListCodeSecondary");
		PlayerPrefs.SetString(_playerPrefsSetting, KeyBinding.ParseModiferListAsInt(_mod1, _mod2, _mod3).ToString());
	}
	
	public void UpdateKeyCodeOnly(int _index, string _actionMethodName, KeyCode _newKey){
		keyBindings[_actionMethodName].keyCodes[_index] = _newKey;
		string _playerPrefsSetting;
		if (_index==0) _playerPrefsSetting = (_actionMethodName + "PrimaryKey");
		else _playerPrefsSetting = (_actionMethodName + "SecondaryKey");
		PlayerPrefs.SetString(_playerPrefsSetting, _newKey.ToString());
	}
	
	public void ToggleCheatsheetInclusion(string _actionMethodName, bool _include){
		keyBindings[_actionMethodName].includeInCheatsheet = _include;
		PlayerPrefs.SetString((_actionMethodName + "IncludeInCheatsheet"), _include.ToString());
	}
	
	public bool IsKeyInputModifier(KeyCode _keyCode){
		if(inputModifiers[0, 0]==_keyCode || inputModifiers[0, 1]==_keyCode || inputModifiers[1, 0]==_keyCode || inputModifiers[1, 1]==_keyCode || inputModifiers[2, 0]==_keyCode || inputModifiers[2, 1]==_keyCode) return true;
		else return false;
	}
	
	public bool IsKeyAlreadyBound(KeyCode _keyCode, bool _mod1, bool _mod2, bool _mod3){
		bool keyIsBound = false;
		foreach(KeyValuePair<string, KeyBinding> keyBinding in keyBindings){
			if(keyBinding.Value.CompareKeybindings(_keyCode, _mod1, _mod2, _mod3)) keyIsBound = true;
		}
		return keyIsBound;
	}
	
	public void ReplaceBoundKey(KeyCode _newKey, bool _mod1, bool _mod2, bool _mod3){
		foreach(KeyValuePair<string, KeyBinding> keyBinding in keyBindings){
			string actionMethodName = keyBinding.Key;
			if(keyBinding.Value.CompareKeybinding(0, _newKey, _mod1, _mod2, _mod3)) UpdateKeyBinding(0, actionMethodName, KeyCode.None, false, false, false);
			if(keyBinding.Value.CompareKeybinding(1, _newKey, _mod1, _mod2, _mod3)) UpdateKeyBinding(1, actionMethodName, KeyCode.None, false, false, false);
		}
		RefreshKeybindComposites();
	}
	
	public bool IsKeyCodeOnlyAlreadyBound(KeyCode _keyCode){
		bool keyIsBound = false;
		foreach(KeyValuePair<string, KeyBinding> keyBinding in keyBindings){
			if(keyBinding.Value.CompareKeyCodesOnly(_keyCode)) keyIsBound = true;
		}
		return keyIsBound;
	}
	
	public void ReplaceBoundKeyCodeOnly(KeyCode _newKey){
		foreach(KeyValuePair<string, KeyBinding> keyBinding in keyBindings){
			string actionMethodName = keyBinding.Key;
			if(keyBinding.Value.CompareKeyCodeOnly(0, _newKey)) UpdateKeyCodeOnly(0, actionMethodName, KeyCode.None);
			if(keyBinding.Value.CompareKeyCodeOnly(1, _newKey)) UpdateKeyCodeOnly(1, actionMethodName, KeyCode.None);
		}
		RefreshKeybindComposites();
	}
	
	public bool IsKeyBindingPressed(string _actionMethodName){
		KeyBinding keyBinding = keyBindings[_actionMethodName];
		if((Input.GetKey(keyBinding.keyCodes[0]) && (!isMod1Down^keyBinding.mods1[0]) && (!isMod2Down^keyBinding.mods2[0]) && (!isMod3Down^keyBinding.mods3[0])) || (Input.GetKey(keyBinding.keyCodes[1]) && (!isMod1Down^keyBinding.mods1[1]) && (!isMod2Down^keyBinding.mods2[1]) && (!isMod3Down^keyBinding.mods3[1]))) return true;
		else return false;
	}
	
	public bool GetKeyUp(string _actionMethodName){
		// Only checks for main keys, not modifiers
		KeyBinding keyBinding = keyBindings[_actionMethodName];
		if(Input.GetKeyUp(keyBinding.keyCodes[0]) || Input.GetKeyUp(keyBinding.keyCodes[1])) return true;
		else return false;
	}
	
	public bool GetKeyDown(string _actionMethodName){
		// Only checks for main keys, not modifiers
		KeyBinding keyBinding = keyBindings[_actionMethodName];
		if((Input.GetKeyDown(keyBinding.keyCodes[0]) && (!isMod1Down^keyBinding.mods1[0]) && (!isMod2Down^keyBinding.mods2[0]) && (!isMod3Down^keyBinding.mods3[0])) || (Input.GetKeyDown(keyBinding.keyCodes[1]) && (!isMod1Down^keyBinding.mods1[1]) && (!isMod2Down^keyBinding.mods2[1]) && (!isMod3Down^keyBinding.mods3[1]))) return true;
		else return false;
	}
	
	private void UpdateCheatsheet(){
		bool displayCheatsheet = false;
		string cheatsheetText = "";
		if(isMod1Down || isMod2Down || isMod3Down){
			foreach(KeyBinding keyBinding in keyBindings.Values){
				if(keyBinding.includeInCheatsheet){
					string workingCheatsheetText = "";
						   if(((isMod1Down && keyBinding.mods1[0]) || !isMod1Down) && 
							  ((isMod2Down && keyBinding.mods2[0]) || !isMod2Down) && 
							  ((isMod3Down && keyBinding.mods3[0]) || !isMod3Down)){
									workingCheatsheetText += (("<b>") + 
															 (keyBinding.mods1[0] ? (inputModifiers[0, 0].ToString() + "+") : "") + 
															 (keyBinding.mods2[0] ? (inputModifiers[1, 0].ToString() + "+") : "") + 
															 (keyBinding.mods3[0] ? (inputModifiers[2, 0].ToString() + "+") : "") + 
															 (keyBinding.keyCodes[0].ToString() + ":\n") + 
															 ("</b>") + 
															 (keyBinding.description + "\n\n"));
									displayCheatsheet = true;
					} else if(((isMod1Down && keyBinding.mods1[1]) || !isMod1Down) && 
							  ((isMod2Down && keyBinding.mods2[1]) || !isMod2Down) && 
							  ((isMod3Down && keyBinding.mods3[1]) || !isMod3Down)){
									workingCheatsheetText += (("<b>") + 
															 (keyBinding.mods1[1] ? (inputModifiers[0, 0].ToString() + "+") : "") + 
															 (keyBinding.mods2[1] ? (inputModifiers[1, 0].ToString() + "+") : "") + 
															 (keyBinding.mods3[1] ? (inputModifiers[2, 0].ToString() + "+") : "") + 
															 (keyBinding.keyCodes[1].ToString() + ":\n") + 
															 ("</b>") + 
															 (keyBinding.description + "\n\n"));
									displayCheatsheet = true;
					}
					cheatsheetText += workingCheatsheetText;
				}
			}
		}
		if(displayCheatsheet) m_KeybindingCheatsheet.transform.localScale = new Vector3(1, 1, 1);
		else m_KeybindingCheatsheet.transform.localScale = new Vector3(0, 0, 0);
		m_KeybindingCheatsheet.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = cheatsheetText;
	}
}

public class KeyBinding {
	public string actionMethodName;
	public string description;
	public bool[] mods1;
	public bool[] mods2;
	public bool[] mods3;
	public KeyCode[] keyCodes;
	public bool includeInCheatsheet;
	public GameObject keybindComposite;
	
	public KeyBinding(string _actionMethodName, string _description, KeyCode _primary, bool _mod1Primary, bool _mod2Primary, bool _mod3Primary, KeyCode _secondary, bool _mod1Secondary, bool _mod2Secondary, bool _mod3Secondary)
		: this(_actionMethodName, _description, _primary, _mod1Primary, _mod2Primary, _mod3Primary, _secondary, _mod1Secondary, _mod2Secondary, _mod3Secondary, false) {
	}
	
	public KeyBinding(string _actionMethodName, string _description, KeyCode _primary, bool _mod1Primary, bool _mod2Primary, bool _mod3Primary, KeyCode _secondary, bool _mod1Secondary, bool _mod2Secondary, bool _mod3Secondary, bool _includeInCheatsheet){
		actionMethodName = _actionMethodName;
		description = _description;
		mods1 = new bool[2];
		mods2 = new bool[2];
		mods3 = new bool[2];
		mods1[0] = _mod1Primary;
		mods2[0] = _mod2Primary;
		mods3[0] = _mod3Primary;
		mods1[1] = _mod1Secondary;
		mods2[1] = _mod2Secondary;
		mods3[1] = _mod3Secondary;
		keyCodes = new KeyCode[2];
		keyCodes[0] = _primary;
		keyCodes[1] = _secondary;
		includeInCheatsheet = _includeInCheatsheet;
	}
	
	public void UpdateKeyBinding(int _index, KeyCode _keyCode, bool _mod1, bool _mod2, bool _mod3){
		mods1[_index] = _mod1;
		mods2[_index] = _mod2;
		mods3[_index] = _mod3;
		keyCodes[_index] = _keyCode;
	}
	
	public bool CompareKeybinding(int _index, KeyCode _keyCode, bool _mod1, bool _mod2, bool _mod3){
		if(_keyCode==keyCodes[_index] && _mod1==mods1[_index] && _mod2==mods2[_index] && _mod3==mods3[_index]) return true;
		else return false;
	}
	
	public bool CompareKeybindings(KeyCode _keyCode, bool _mod1, bool _mod2, bool _mod3){
		if(CompareKeybinding(0, _keyCode, _mod1, _mod2, _mod3) || CompareKeybinding(1, _keyCode, _mod1, _mod2, _mod3)) return true;
		else return false;
	}
	
	public bool CompareKeyCodesOnly(KeyCode _keyCode){
		if(_keyCode==keyCodes[0] || _keyCode==keyCodes[1]) return true;
		else return false;
	}
	
	public bool CompareKeyCodeOnly(int _index, KeyCode _keyCode){
		if(_keyCode==keyCodes[_index]) return true;
		else return false;
	}
	
	public int GetModifierListCode(bool _primary){
		if(_primary) return ParseModiferListAsInt(mods1[0], mods2[0], mods3[0]);
		else return ParseModiferListAsInt(mods1[1], mods2[1], mods3[1]);
	}
	
	public static bool[] ParseIntAsModiferList(int _modifierListCode){
		bool[] modiferList;
		if(_modifierListCode==0) modiferList = new bool[3]{false, false, false};
		else if(_modifierListCode==1) modiferList = new bool[3]{true, false, false};
		else if(_modifierListCode==2) modiferList = new bool[3]{true, true, false};
		else if(_modifierListCode==3) modiferList = new bool[3]{true, true, true};
		else if(_modifierListCode==4) modiferList = new bool[3]{true, false, true};
		else if(_modifierListCode==5) modiferList = new bool[3]{false, true, false};
		else if(_modifierListCode==6) modiferList = new bool[3]{false, true, true};
		else if(_modifierListCode==7) modiferList = new bool[3]{false, false, true};
		else modiferList = new bool[3]{false, false, false};
		return modiferList;
	}
	
	public static int ParseModiferListAsInt(bool _mod1, bool _mod2, bool _mod3){
		int modifierListCode;
		if (!_mod1 && !_mod2 && !_mod3) modifierListCode = 0;
		else if (_mod1 && !_mod2 && !_mod3) modifierListCode = 1;
		else if (_mod1 && _mod2 && !_mod3) modifierListCode = 2;
		else if (_mod1 && _mod2 && _mod3) modifierListCode = 3;
		else if (_mod1 && !_mod2 && _mod3) modifierListCode = 4;
		else if (!_mod1 && _mod2 && !_mod3) modifierListCode = 5;
		else if (!_mod1 && _mod2 && _mod3) modifierListCode = 6;
		else if (!_mod1 && !_mod2 && _mod3) modifierListCode = 7;	
		else modifierListCode = 0;
		return modifierListCode;
	}
	
	public void AssignKeybindComposite(GameObject _keybindComposite){
		keybindComposite = _keybindComposite;
	}
}

/*
List of strings to use for KeyCodes:

None = Not assigned (never returned as the result of a keystroke).
Backspace = The backspace key.
Delete = The forward delete key.
Tab = The tab key.
Clear = The Clear key.
Return = Return key.
Pause = Pause on PC machines.
Escape = Escape key.
Space = Space key.
Keypad0 = Numeric keypad 0.
Keypad1 = Numeric keypad 1.
Keypad2 = Numeric keypad 2.
Keypad3 = Numeric keypad 3.
Keypad4 = Numeric keypad 4.
Keypad5 = Numeric keypad 5.
Keypad6 = Numeric keypad 6.
Keypad7 = Numeric keypad 7.
Keypad8 = Numeric keypad 8.
Keypad9 = Numeric keypad 9.
KeypadPeriod = Numeric keypad '.'.
KeypadDivide = Numeric keypad '/'.
KeypadMultiply = Numeric keypad '*'.
KeypadMinus = Numeric keypad '='.
KeypadPlus = Numeric keypad '+'.
KeypadEnter = Numeric keypad enter.
KeypadEquals = Numeric keypad '='.
UpArrow = Up arrow key.
DownArrow = Down arrow key.
RightArrow = Right arrow key.
LeftArrow = Left arrow key.
Insert = Insert key key.
Home = Home key.
End = End key.
PageUp = Page up.
PageDown = Page down.
F1 = F1 function key.
F2 = F2 function key.
F3 = F3 function key.
F4 = F4 function key.
F5 = F5 function key.
F6 = F6 function key.
F7 = F7 function key.
F8 = F8 function key.
F9 = F9 function key.
F10 = F10 function key.
F11 = F11 function key.
F12 = F12 function key.
F13 = F13 function key.
F14 = F14 function key.
F15 = F15 function key.
Alpha0 = The '0' key on the top of the alphanumeric keyboard.
Alpha1 = The '1' key on the top of the alphanumeric keyboard.
Alpha2 = The '2' key on the top of the alphanumeric keyboard.
Alpha3 = The '3' key on the top of the alphanumeric keyboard.
Alpha4 = The '4' key on the top of the alphanumeric keyboard.
Alpha5 = The '5' key on the top of the alphanumeric keyboard.
Alpha6 = The '6' key on the top of the alphanumeric keyboard.
Alpha7 = The '7' key on the top of the alphanumeric keyboard.
Alpha8 = The '8' key on the top of the alphanumeric keyboard.
Alpha9 = The '9' key on the top of the alphanumeric keyboard.
Exclaim = Exclamation mark key '!'.
DoubleQuote = Double quote key '"'.
Hash = Hash key '#'.
Dollar = Dollar sign key '$'.
Ampersand = Ampersand key '&'.
Quote = Quote key '.
LeftParen = Left Parenthesis key '('.
RightParen = Right Parenthesis key ')'.
Asterisk = Asterisk key '*'.
Plus = Plus key '+'.
Comma = Comma ',' key.
Minus = Minus '-' key.
Period = Period '.' key.
Slash = Slash '/' key.
Colon = Colon ':' key.
Semicolon = Semicolon ';' key.
Less = Less than '<' key.
Equals = Equals '=' key.
Greater = Greater than '>' key.
Question = Question mark '?' key.
At = At key '@'.
LeftBracket = Left square bracket key '['.
Backslash = Backslash key '\'.
RightBracket = Right square bracket key ']'.
Caret = Caret key '^'.
Underscore = Underscore '_' key.
BackQuote = Back quote key '`'.
A = 'a' key.
B = 'b' key.
C = 'c' key.
D = 'd' key.
E = 'e' key.
F = 'f' key.
G = 'g' key.
H = 'h' key.
I = 'i' key.
J = 'j' key.
K = 'k' key.
L = 'l' key.
M = 'm' key.
N = 'n' key.
O = 'o' key.
P = 'p' key.
Q = 'q' key.
R = 'r' key.
S = 's' key.
T = 't' key.
U = 'u' key.
V = 'v' key.
W = 'w' key.
X = 'x' key.
Y = 'y' key.
Z = 'z' key.
Numlock = Numlock key.
CapsLock = Capslock key.
ScrollLock = Scroll lock key.
RightShift = Right shift key.
LeftShift = Left shift key.
RightControl = Right Control key.
LeftControl = Left Control key.
RightAlt = Right Alt key.
LeftAlt = Left Alt key.
LeftCommand = Left Command key.
LeftApple = Left Command key.
LeftWindows = Left Windows key.
RightCommand = Right Command key.
RightApple = Right Command key.
RightWindows = Right Windows key.
AltGr = Alt Gr key.
Help = Help key.
Print = Print key.
SysReq = Sys Req key.
Break = Break key.
Menu = Menu key.
Mouse0 = First (primary) mouse button.
Mouse1 = Second (secondary) mouse button.
Mouse2 = Third mouse button.
Mouse3 = Fourth mouse button.
Mouse4 = Fifth mouse button.
Mouse5 = Sixth mouse button.
Mouse6 = Seventh mouse button.
*/