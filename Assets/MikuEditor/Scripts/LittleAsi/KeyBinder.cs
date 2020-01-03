using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using MiKu.NET;

public class KeyBinder : MonoBehaviour {
	public static KeyBinder keyBinder;
    Event keyEvent;
	bool eventKeyIsMod = false;
    KeyCode newKey;
	bool newMod1 = false;
	bool newMod2 = false;
	bool newMod3 = false;
	public bool waitingForKey = false;
	public bool waitingForInputModifier = false;
	bool continueKeyBinding = false;
	public bool panelActive = false;
	[SerializeField]
    private GameObject m_KeybindPanelContent;
	[SerializeField]
    private GameObject m_KeybindComposite;
	[SerializeField]
    private GameObject m_KeybindHeaderComposite;
	[SerializeField]
	private Animator m_KeyBindingWaitingForKeyWindowAnimator;
	[SerializeField]
	private Animator m_KeyBindingConfirmRebindWindowAnimator;
	[SerializeField]
	private Animator m_KeyBindingConfirmResetWindowAnimator;
	[SerializeField]
	private Button m_KeyBindingConfirmRebindReplace;
	[SerializeField]
	private Button m_KeyBindingConfirmRebindDuplicate;
	[SerializeField]
	private Button m_KeyBindingConfirmRebindCancel;
	[SerializeField]
	private GameObject m_KeyBindingConfirmRebindContent;
	private string mod1String;
	private string mod2String;
	private string mod3String;
	private List<Text> mod1Texts = new List<Text>();
	private List<Text> mod2Texts = new List<Text>();
	private List<Text> mod3Texts = new List<Text>();
	private List<Text> keyTexts = new List<Text>();
	private GameObject[] modComposites = new GameObject[3];

	void Awake(){
		if(keyBinder == null){
			DontDestroyOnLoad(gameObject);
			keyBinder = this;
		} else if(keyBinder != this){
			Destroy(gameObject);
		}
	}
	
	void OnGUI(){
		if(waitingForInputModifier || waitingForKey){
			keyEvent = Event.current;
			bool isValidEventType = false;
			KeyCode eventKeyCode;
			if (keyEvent.isMouse){
				isValidEventType = true;
				eventKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Mouse" + keyEvent.button, true);
			}
			else if(keyEvent.isKey){
				isValidEventType = true;
				eventKeyCode = keyEvent.keyCode;
			} else if(GetJoystickInput()!=KeyCode.None){
				isValidEventType = true;
				eventKeyCode = GetJoystickInput();
			} else eventKeyCode = KeyCode.None;
			if(isValidEventType && eventKeyCode!=KeyCode.None){
				if(waitingForInputModifier){
					if(!Controller.controller.IsKeyInputModifier(eventKeyCode)){
						newKey = eventKeyCode;
						newMod1 = false;
						newMod2 = false;
						newMod3 = false;
						waitingForInputModifier = false;
						continueKeyBinding = true;
					}
				}
				else if(waitingForKey){
					if(!Controller.controller.IsKeyInputModifier(eventKeyCode)){
						if(Controller.controller.isMod1Down) newMod1 = true;
						else newMod1 = false;
						if(Controller.controller.isMod2Down) newMod2 = true;
						else newMod2 = false;
						if(Controller.controller.isMod3Down) newMod3 = true;
						else newMod3 = false;
						newKey = eventKeyCode;
						waitingForKey = false;
						continueKeyBinding = true;
					}
				}
			}
		}
    }
	
	private KeyCode GetJoystickInput(){
		if(Input.GetKey(KeyCode.JoystickButton0)) return KeyCode.JoystickButton0;
		else if(Input.GetKey(KeyCode.JoystickButton1)) return KeyCode.JoystickButton1;
		else if(Input.GetKey(KeyCode.JoystickButton2)) return KeyCode.JoystickButton2;
		else if(Input.GetKey(KeyCode.JoystickButton3)) return KeyCode.JoystickButton3;
		else if(Input.GetKey(KeyCode.JoystickButton4)) return KeyCode.JoystickButton4;
		else if(Input.GetKey(KeyCode.JoystickButton5)) return KeyCode.JoystickButton5;
		else if(Input.GetKey(KeyCode.JoystickButton6)) return KeyCode.JoystickButton6;
		else if(Input.GetKey(KeyCode.JoystickButton7)) return KeyCode.JoystickButton7;
		else if(Input.GetKey(KeyCode.JoystickButton8)) return KeyCode.JoystickButton8;
		else if(Input.GetKey(KeyCode.JoystickButton9)) return KeyCode.JoystickButton9;
		else if(Input.GetKey(KeyCode.JoystickButton10)) return KeyCode.JoystickButton10;
		else if(Input.GetKey(KeyCode.JoystickButton11)) return KeyCode.JoystickButton11;
		else if(Input.GetKey(KeyCode.JoystickButton12)) return KeyCode.JoystickButton12;
		else if(Input.GetKey(KeyCode.JoystickButton13)) return KeyCode.JoystickButton13;
		else if(Input.GetKey(KeyCode.JoystickButton14)) return KeyCode.JoystickButton14;
		else if(Input.GetKey(KeyCode.JoystickButton15)) return KeyCode.JoystickButton15;
		else if(Input.GetKey(KeyCode.JoystickButton16)) return KeyCode.JoystickButton16;
		else if(Input.GetKey(KeyCode.JoystickButton17)) return KeyCode.JoystickButton17;
		else if(Input.GetKey(KeyCode.JoystickButton18)) return KeyCode.JoystickButton18;
		else if(Input.GetKey(KeyCode.JoystickButton19)) return KeyCode.JoystickButton19;
        else return KeyCode.None;
	}
	
	public void BeginSetInputModifier(int _modifierNumber, int _index, Transform _button){
        if(!waitingForKey) StartCoroutine(SetInputModifier(_modifierNumber, _index, _button));
    }
	
	public IEnumerator SetInputModifier(int _modifierNumber, int _index, Transform _button){
		m_KeyBindingWaitingForKeyWindowAnimator.Play("Panel In");
        waitingForInputModifier = true;
		continueKeyBinding = false;
        yield return WaitForKey();
		continueKeyBinding = false;
		m_KeyBindingWaitingForKeyWindowAnimator.Play("Panel Out");
		if(newKey != KeyCode.Escape) {
			if(IsKeyCodeOnlyAlreadyBound(newKey)){
				m_KeyBindingConfirmRebindContent.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = ("The input \"" + newKey.ToString() + "\" is already bound to another action. Replace key binding?");
				m_KeyBindingConfirmRebindReplace.onClick.RemoveAllListeners();
				m_KeyBindingConfirmRebindDuplicate.onClick.RemoveAllListeners();
				m_KeyBindingConfirmRebindCancel.onClick.RemoveAllListeners();
				m_KeyBindingConfirmRebindDuplicate.interactable = false;
				m_KeyBindingConfirmRebindDuplicate.gameObject.SetActive(false);
				m_KeyBindingConfirmRebindReplace.onClick.AddListener(delegate {confirmModRebind(true, _modifierNumber, _index, _button); });
				m_KeyBindingConfirmRebindCancel.onClick.AddListener(delegate {cancelRebind(); });
				m_KeyBindingConfirmRebindWindowAnimator.Play("Panel In");
			} else {
				FinalizeModBinding(_modifierNumber, _index, _button);
			}
		}
        yield return null;
    }
	
	public void BeginSetKeyBinding(int _index, string _actionMethodName, Transform _button){
        if(!waitingForKey) StartCoroutine(SetKeyBinding(_index, _actionMethodName, _button));
    }
	
	public IEnumerator SetKeyBinding(int _index, string _actionMethodName, Transform _button){
		m_KeyBindingWaitingForKeyWindowAnimator.Play("Panel In");
        waitingForKey = true;
		continueKeyBinding = false;
        yield return WaitForKey();
		continueKeyBinding = false;
		m_KeyBindingWaitingForKeyWindowAnimator.Play("Panel Out");
		if(newKey != KeyCode.Escape) {
			if(IsKeyAlreadyBound(newKey, newMod1, newMod2, newMod3)){
				string keyBindingString = "";
				if(newMod1) keyBindingString += (Controller.controller.inputModifiers[0, 0].ToString() + " + ");
				if(newMod2) keyBindingString += (Controller.controller.inputModifiers[1, 0].ToString() + " + ");
				if(newMod3) keyBindingString += (Controller.controller.inputModifiers[2, 0].ToString() + " + ");
				keyBindingString += newKey.ToString();
				m_KeyBindingConfirmRebindContent.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = ("The input \"" + keyBindingString + "\" is already bound to another action. Duplicating keybindings may result in unexpected behavior. Replace key binding?");
				m_KeyBindingConfirmRebindDuplicate.interactable = true;
				m_KeyBindingConfirmRebindDuplicate.gameObject.SetActive(true);
				m_KeyBindingConfirmRebindReplace.onClick.RemoveAllListeners();
				m_KeyBindingConfirmRebindDuplicate.onClick.RemoveAllListeners();
				m_KeyBindingConfirmRebindCancel.onClick.RemoveAllListeners();
				m_KeyBindingConfirmRebindReplace.onClick.AddListener(delegate {confirmRebind(true, _index, _actionMethodName, _button); });
				m_KeyBindingConfirmRebindDuplicate.onClick.AddListener(delegate {confirmRebind(false, _index, _actionMethodName, _button); });
				m_KeyBindingConfirmRebindCancel.onClick.AddListener(delegate {cancelRebind(); });
				m_KeyBindingConfirmRebindWindowAnimator.Play("Panel In");
			} else {
				FinalizeKeyBinding(_index, _actionMethodName, _button);
			}
		}
        yield return null;
    }
	
	void FinalizeModBinding(int _modiferNumber, int _index, Transform _button){
		Controller.controller.UpdateInputModifier(_modiferNumber, _index, newKey);
		_button.transform.Find("Normal").GetComponentInChildren<Text>().text = "\n" + newKey.ToString();
		_button.transform.Find("Highlighted").GetComponentInChildren<Text>().text = "\n" + newKey.ToString();
		if(_index==0){
			if(_modiferNumber==1) mod1String = newKey.ToString() + "\n\n";
			else if(_modiferNumber==2) mod2String = newKey.ToString() + "\n\n";
			else if(_modiferNumber==3) mod3String = newKey.ToString() + "\n\n";
			UpdateModTexts(_modiferNumber);
		}
	}
	
	void UpdateModTexts(int _modiferNumber){
		List<Text> modTexts;
		string modString;
		if(_modiferNumber==1) {
			modTexts = mod1Texts;
			modString = mod1String;
		}
		else if(_modiferNumber==2) {
			modTexts = mod2Texts;
			modString = mod2String;
		}
		else if(_modiferNumber==3) {
			modTexts = mod3Texts;
			modString = mod3String;
		} else {
			Debug.Log("Invalid Input Modifier Number!");
			modTexts = mod1Texts;
			modString = mod1String;
		}
		foreach(Text modText in modTexts){
			modText.text = modString;
		}
	}
	
	void FinalizeKeyBinding(int _index, string _actionMethodName, Transform _button){
		Controller.controller.UpdateKeyBinding(_index, _actionMethodName, newKey, newMod1, newMod2, newMod3);
		_button.transform.Find("Normal").GetComponentInChildren<Text>().text = "\n" + newKey.ToString();
		_button.transform.Find("Highlighted").GetComponentInChildren<Text>().text = "\n" + newKey.ToString();
		_button.transform.Find("Normal").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = newMod1;
		_button.transform.Find("Normal").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = newMod2;
		_button.transform.Find("Normal").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = newMod3;
		_button.transform.Find("Highlighted").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = newMod1;
		_button.transform.Find("Highlighted").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = newMod2;
		_button.transform.Find("Highlighted").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = newMod3;
	}

    IEnumerator WaitForKey(){
        while(!continueKeyBinding) yield return null;
    }
	
	public bool IsKeyAlreadyBound(KeyCode _keyCode, bool _mod1, bool _mod2, bool _mod3){
		return Controller.controller.IsKeyAlreadyBound(_keyCode, _mod1, _mod2, _mod3);
	}
	
	public bool IsKeyCodeOnlyAlreadyBound(KeyCode _keyCode){
		return Controller.controller.IsKeyCodeOnlyAlreadyBound(_keyCode);
	}
	
	void confirmModRebind(bool _replace, int _modifierNumber, int _index, Transform _button){
		m_KeyBindingConfirmRebindWindowAnimator.Play("Panel Out");
		if(_replace){
			Controller.controller.ReplaceBoundKeyCodeOnly(newKey);
			for(int i = 0; i < m_KeybindPanelContent.transform.childCount; i++){
				if(m_KeybindPanelContent.transform.GetChild(i).Find("PrimaryButton")!=null){
					if(m_KeybindPanelContent.transform.GetChild(i).Find("PrimaryButton").GetComponentInChildren<Text>().text == "\n" + newKey.ToString()) {
						m_KeybindPanelContent.transform.GetChild(i).Find("PrimaryButton").Find("Normal").GetComponentInChildren<Text>().text = "\n" + KeyCode.None.ToString();
						m_KeybindPanelContent.transform.GetChild(i).Find("PrimaryButton").Find("Highlighted").GetComponentInChildren<Text>().text = "\n" + KeyCode.None.ToString();
					} else if(m_KeybindPanelContent.transform.GetChild(i).Find("SecondaryButton").GetComponentInChildren<Text>().text == "\n" + newKey.ToString()) {
						m_KeybindPanelContent.transform.GetChild(i).Find("SecondaryButton").Find("Normal").GetComponentInChildren<Text>().text = "\n" + KeyCode.None.ToString();
						m_KeybindPanelContent.transform.GetChild(i).Find("SecondaryButton").Find("Highlighted").GetComponentInChildren<Text>().text = "\n" + KeyCode.None.ToString();
					}
				}
			}
		}
		FinalizeModBinding(_modifierNumber, _index, _button);
	}
	
	void confirmRebind(bool _replace, int _index, string _actionMethodName, Transform _button){
		m_KeyBindingConfirmRebindWindowAnimator.Play("Panel Out");
		if(_replace){
			Controller.controller.ReplaceBoundKey(newKey, newMod1, newMod2, newMod3);
			for(int i = 0; i < m_KeybindPanelContent.transform.childCount; i++){
				if(m_KeybindPanelContent.transform.GetChild(i).Find("PrimaryButton")!=null){
					if(m_KeybindPanelContent.transform.GetChild(i).Find("PrimaryButton").GetComponentInChildren<Text>().text == "\n" + newKey.ToString()) {
						Transform button = m_KeybindPanelContent.transform.GetChild(i).Find("PrimaryButton");
						button.Find("Normal").GetComponentInChildren<Text>().text = "\n" + KeyCode.None.ToString();
						button.Find("Highlighted").GetComponentInChildren<Text>().text = "\n" + KeyCode.None.ToString();
						button.Find("Normal").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = newMod1;
						button.Find("Highlighted").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = newMod1;
						button.Find("Normal").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = newMod2;
						button.Find("Highlighted").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = newMod2;
						button.Find("Normal").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = newMod3;
						button.Find("Highlighted").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = newMod3;
					} else if(m_KeybindPanelContent.transform.GetChild(i).Find("SecondaryButton").GetComponentInChildren<Text>().text == "\n" + newKey.ToString()) {
						Transform button = m_KeybindPanelContent.transform.GetChild(i).Find("SecondaryButton");
						button.Find("Normal").GetComponentInChildren<Text>().text = "\n" + KeyCode.None.ToString();
						button.Find("Highlighted").GetComponentInChildren<Text>().text = "\n" + KeyCode.None.ToString();
						button.Find("Normal").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = newMod1;
						button.Find("Highlighted").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = newMod1;
						button.Find("Normal").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = newMod2;
						button.Find("Highlighted").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = newMod2;
						button.Find("Normal").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = newMod3;
						button.Find("Highlighted").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = newMod3;
					}
				}
			}
		}
		FinalizeKeyBinding(_index, _actionMethodName, _button);
	}
	
	void cancelRebind(){
		m_KeyBindingConfirmRebindWindowAnimator.Play("Panel Out");
	}
	
	public void CancelResetToDefault(){
		m_KeyBindingConfirmResetWindowAnimator.Play("Panel Out");
	}
	
	public void ConfirmResetToDefault(){
		m_KeyBindingConfirmResetWindowAnimator.Play("Panel Out");
		Controller.controller.ResetToDefaultKeyBindings();
	}
	
	public void BeginResetToDefault(){
		m_KeyBindingConfirmResetWindowAnimator.Play("Panel In");
	}
	
	public void CreateKeyBindHeaderComposite(string _description){
		GameObject keybindComposite = GameObject.Instantiate(m_KeybindHeaderComposite);
		keybindComposite.transform.parent = m_KeybindPanelContent.transform;
		keybindComposite.transform.Find("Description").GetComponentInChildren<Text>().text = _description;
	}
	
	public void CreateInputModifierComposite(int _modifierNumber, string _description, string _primary, string _secondary){
		if(_modifierNumber==1) mod1String = _primary + "\n\n";
		else if(_modifierNumber==2) mod2String = _primary + "\n\n";
		else if(_modifierNumber==3) mod3String = _primary + "\n\n";
		GameObject keybindComposite = GameObject.Instantiate(m_KeybindComposite);
		keybindComposite.transform.parent = m_KeybindPanelContent.transform;
		Transform primaryButton = keybindComposite.transform.Find("PrimaryButton");
		Transform secondaryButton = keybindComposite.transform.Find("SecondaryButton");
		keybindComposite.transform.Find("Description").GetComponentInChildren<Text>().text = _description;
		primaryButton.Find("Normal").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + _primary;
		primaryButton.Find("Highlighted").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + _primary;
		secondaryButton.Find("Normal").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + _secondary;
		secondaryButton.Find("Highlighted").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + _secondary;
		primaryButton.Find("Normal").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = false;
		primaryButton.Find("Normal").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = false;
		primaryButton.Find("Normal").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = false;
		primaryButton.Find("Highlighted").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = false;
		primaryButton.Find("Highlighted").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = false;
		primaryButton.Find("Highlighted").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = false;
		secondaryButton.Find("Normal").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = false;
		secondaryButton.Find("Normal").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = false;
		secondaryButton.Find("Normal").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = false;
		secondaryButton.Find("Highlighted").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = false;
		secondaryButton.Find("Highlighted").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = false;
		secondaryButton.Find("Highlighted").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = false;
		primaryButton.GetComponentInChildren<Button>().onClick.AddListener(delegate {BeginSetInputModifier(_modifierNumber, 0, primaryButton); });
		secondaryButton.GetComponentInChildren<Button>().onClick.AddListener(delegate {BeginSetInputModifier(_modifierNumber, 1, secondaryButton); }); 
		modComposites[_modifierNumber-1] = keybindComposite;
	}
	
	public GameObject CreateKeyBindingComposite(string _actionMethodName, string _description, string _primary, bool _mod1Primary, bool _mod2Primary, bool _mod3Primary, string _secondary, bool _mod1Secondary, bool _mod2Secondary, bool _mod3Secondary){
		GameObject keybindComposite = GameObject.Instantiate(m_KeybindComposite);
		keybindComposite.transform.parent = m_KeybindPanelContent.transform;
		Transform primaryButton = keybindComposite.transform.Find("PrimaryButton");
		Transform secondaryButton = keybindComposite.transform.Find("SecondaryButton");
		keybindComposite.transform.Find("Description").GetComponentInChildren<Text>().text = _description;
		keyTexts.Add(primaryButton.Find("Normal").Find("TextScaler").GetComponentInChildren<Text>());
		keyTexts.Add(primaryButton.Find("Highlighted").Find("TextScaler").GetComponentInChildren<Text>());
		keyTexts.Add(secondaryButton.Find("Normal").Find("TextScaler").GetComponentInChildren<Text>());
		keyTexts.Add(secondaryButton.Find("Highlighted").Find("TextScaler").GetComponentInChildren<Text>());
		primaryButton.Find("Normal").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + _primary;
		primaryButton.Find("Highlighted").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + _primary;
		Text modText = primaryButton.Find("Normal").Find("TextScalerMod1").GetComponentInChildren<Text>();
		modText.text = mod1String;
		mod1Texts.Add(modText);
		modText = primaryButton.Find("Normal").Find("TextScalerMod2").GetComponentInChildren<Text>();
		modText.text = mod2String;
		mod2Texts.Add(modText);
		modText = primaryButton.Find("Normal").Find("TextScalerMod3").GetComponentInChildren<Text>();
		modText.text = mod3String;
		mod3Texts.Add(modText);
		modText = primaryButton.Find("Highlighted").Find("TextScalerMod1").GetComponentInChildren<Text>();
		modText.text = mod1String;
		mod1Texts.Add(modText);
		modText = primaryButton.Find("Highlighted").Find("TextScalerMod2").GetComponentInChildren<Text>();
		modText.text = mod2String;
		mod2Texts.Add(modText);
		modText = primaryButton.Find("Highlighted").Find("TextScalerMod3").GetComponentInChildren<Text>();
		modText.text = mod3String;
		mod3Texts.Add(modText);
		secondaryButton.Find("Normal").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + _secondary;
		secondaryButton.Find("Highlighted").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + _secondary;
		modText = secondaryButton.Find("Normal").Find("TextScalerMod1").GetComponentInChildren<Text>();
		modText.text = mod1String;
		mod1Texts.Add(modText);
		modText = secondaryButton.Find("Normal").Find("TextScalerMod2").GetComponentInChildren<Text>();
		modText.text = mod2String;
		mod2Texts.Add(modText);
		modText = secondaryButton.Find("Normal").Find("TextScalerMod3").GetComponentInChildren<Text>();
		modText.text = mod3String;
		mod3Texts.Add(modText);
		modText = secondaryButton.Find("Highlighted").Find("TextScalerMod1").GetComponentInChildren<Text>();
		modText.text = mod1String;
		mod1Texts.Add(modText);
		modText = secondaryButton.Find("Highlighted").Find("TextScalerMod2").GetComponentInChildren<Text>();
		modText.text = mod2String;
		mod2Texts.Add(modText);
		modText = secondaryButton.Find("Highlighted").Find("TextScalerMod3").GetComponentInChildren<Text>();
		modText.text = mod3String;
		mod3Texts.Add(modText);
		primaryButton.Find("Normal").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = _mod1Primary;
		primaryButton.Find("Highlighted").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = _mod1Primary;
		secondaryButton.Find("Normal").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = _mod1Secondary;
		secondaryButton.Find("Highlighted").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = _mod1Secondary;
		primaryButton.Find("Normal").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = _mod2Primary;
		primaryButton.Find("Highlighted").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = _mod2Primary;
		secondaryButton.Find("Normal").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = _mod2Secondary;
		secondaryButton.Find("Highlighted").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = _mod2Secondary;
		primaryButton.Find("Normal").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = _mod3Primary;
		primaryButton.Find("Highlighted").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = _mod3Primary;
		secondaryButton.Find("Normal").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = _mod3Secondary;
		secondaryButton.Find("Highlighted").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = _mod3Secondary;
		primaryButton.GetComponentInChildren<Button>().onClick.AddListener(delegate {BeginSetKeyBinding(0, _actionMethodName, primaryButton); });
		secondaryButton.GetComponentInChildren<Button>().onClick.AddListener(delegate {BeginSetKeyBinding(1, _actionMethodName, secondaryButton); }); 
		return keybindComposite;
	}
	
	public void RefreshKeybindComposite(GameObject _keybindComposite, string _primary, bool _mod1Primary, bool _mod2Primary, bool _mod3Primary, string _secondary, bool _mod1Secondary, bool _mod2Secondary, bool _mod3Secondary){
		Transform primaryButton = _keybindComposite.transform.Find("PrimaryButton");
		Transform secondaryButton = _keybindComposite.transform.Find("SecondaryButton");
		primaryButton.Find("Normal").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + _primary;
		primaryButton.Find("Highlighted").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + _primary;
		secondaryButton.Find("Normal").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + _secondary;
		secondaryButton.Find("Highlighted").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + _secondary;
		primaryButton.Find("Normal").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = _mod1Primary;
		primaryButton.Find("Highlighted").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = _mod1Primary;
		secondaryButton.Find("Normal").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = _mod1Secondary;
		secondaryButton.Find("Highlighted").Find("TextScalerMod1").GetComponentInChildren<Text>().enabled = _mod1Secondary;
		primaryButton.Find("Normal").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = _mod2Primary;
		primaryButton.Find("Highlighted").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = _mod2Primary;
		secondaryButton.Find("Normal").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = _mod2Secondary;
		secondaryButton.Find("Highlighted").Find("TextScalerMod2").GetComponentInChildren<Text>().enabled = _mod2Secondary;
		primaryButton.Find("Normal").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = _mod3Primary;
		primaryButton.Find("Highlighted").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = _mod3Primary;
		secondaryButton.Find("Normal").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = _mod3Secondary;
		secondaryButton.Find("Highlighted").Find("TextScalerMod3").GetComponentInChildren<Text>().enabled = _mod3Secondary;
	}
	
	public void RefreshModComposites(){
		Transform primaryButton;
		Transform secondaryButton;
		for(int i=0;i<=2;i++){
			primaryButton = modComposites[i].transform.Find("PrimaryButton");
			secondaryButton = modComposites[i].transform.Find("SecondaryButton");
			primaryButton.Find("Normal").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + Controller.controller.inputModifiers[i, 0];
			primaryButton.Find("Highlighted").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + Controller.controller.inputModifiers[i, 0];
			secondaryButton.Find("Normal").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + Controller.controller.inputModifiers[i, 1];
			secondaryButton.Find("Highlighted").Find("TextScaler").GetComponentInChildren<Text>().text = "\n" + Controller.controller.inputModifiers[i, 1];
			if(i==0) mod1String = Controller.controller.inputModifiers[0, 0].ToString() + "\n\n";
			else if(i==1) mod2String = Controller.controller.inputModifiers[1, 0].ToString() + "\n\n";
			else if(i==2) mod3String = Controller.controller.inputModifiers[2, 0].ToString() + "\n\n";
			UpdateModTexts(i+1);
		}
	}
}