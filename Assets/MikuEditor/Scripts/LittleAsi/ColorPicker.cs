using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using MiKu.NET;
using MiKu.NET.Charting;

public class ColorPicker : MonoBehaviour {
	private bool active = false;
	[SerializeField]
	public Image m_ColorSpace;
	[SerializeField]
	public Image m_Swatch;
	[SerializeField]
	public Slider m_RedSlider;
	[SerializeField]
	public TMP_InputField m_RedText;
	[SerializeField]
	public Slider m_GreenSlider;
	[SerializeField]
	public TMP_InputField m_GreenText;
	[SerializeField]
	public Slider m_BlueSlider;
	[SerializeField]
	public TMP_InputField m_BlueText;
	[SerializeField]
	public Animator m_ColorPickerWindowAnimator;
	
	Note.NoteType noteType = Note.NoteType.LeftHanded;
	
	private int redValue = 255;
	private int greenValue = 255;
	private int blueValue = 255;
	
	private Color[] colorData;
	public static ColorPicker colorPicker;
	private Color swatchColor;
	private Color selectedColor;
	
	
	
	void Awake(){
		if(colorPicker == null){
			DontDestroyOnLoad(gameObject);
			colorPicker = this;
		} else if(colorPicker != this){
			Destroy(gameObject);
		}
		active = false;
		colorData = m_ColorSpace.sprite.texture.GetPixels();
	}
	
	void Update(){
		if(active){
			Vector3[] corners = new Vector3[4];
			m_ColorSpace.transform.GetComponent<RectTransform>().GetWorldCorners(corners);
			Vector2 pickerMinPos = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
			Vector2 pickerMaxPos = RectTransformUtility.WorldToScreenPoint(null, corners[2]);
			Rect pickerScreenRect = new Rect(pickerMinPos.x, pickerMinPos.y, pickerMaxPos.x-pickerMinPos.x, pickerMaxPos.y-pickerMinPos.y);
			if (pickerScreenRect.Contains(Input.mousePosition)){
				float mouseXOverPicker = Mathf.Min((Input.mousePosition.x-pickerScreenRect.xMin)*(512/pickerScreenRect.width), 511f);
				float mouseYOverPicker = Mathf.Min((Input.mousePosition.y-pickerScreenRect.yMin)*(512/pickerScreenRect.height), 511f);
				Color hoveredColor = m_ColorSpace.sprite.texture.GetPixel((int)mouseXOverPicker, (int)mouseYOverPicker);
				if (Input.GetMouseButtonUp(0)) SetColor(hoveredColor);
				else if (Input.GetMouseButton(0)) PresetColor(hoveredColor);
			}
		}
	}
	
	public void PresetColor(Color _color){
		redValue = Mathf.RoundToInt(_color.r*255f);
		greenValue = Mathf.RoundToInt(_color.g*255f);
		blueValue = Mathf.RoundToInt(_color.b*255f);
		m_Swatch.color = _color;
		m_RedSlider.value = redValue;
		m_RedText.text = redValue.ToString();
		m_GreenSlider.value = greenValue;
		m_GreenText.text = greenValue.ToString();
		m_BlueSlider.value = blueValue;
		m_BlueText.text = blueValue.ToString();
	}
	
	public void SetColor(Color _color){
		PresetColor(_color);
		selectedColor = _color;
	}
	
	public void SetRed(Slider _slider){
		SetRed((int)_slider.value);
	}
		
	public void SetRed(int _value){
		_value = Mathf.RoundToInt(Mathf.Clamp(_value, 0, 255));
		m_RedSlider.value = _value;
		m_RedText.text = _value.ToString();
		redValue = _value;
		SetColor(new Color(redValue/255.0f, greenValue/255.0f, blueValue/255.0f, 1.0f));
	}
	
	public void SetRed(string _value){
		int parsedValue = 0;
		if(int.TryParse(_value, out parsedValue));
		else Debug.Log("Error parsing color code!");
		SetRed(parsedValue);
	}
	
	public void SetGreen(Slider _slider){
		SetGreen((int)_slider.value);
	}
		
	public void SetGreen(int _value){
		_value = Mathf.RoundToInt(Mathf.Clamp(_value, 0, 255));
		m_GreenSlider.value = _value;
		m_GreenText.text = _value.ToString();
		greenValue = _value;
		SetColor(new Color(redValue/255.0f, greenValue/255.0f, blueValue/255.0f, 1.0f));
	}
	
	public void SetGreen(string _value){
		int parsedValue = 0;
		if(int.TryParse(_value, out parsedValue));
		else Debug.Log("Error parsing color code!");
		SetGreen(parsedValue);
	}
	
	public void SetBlue(Slider _slider){
		SetBlue((int)_slider.value);
	}
		
	public void SetBlue(int _value){
		_value = Mathf.RoundToInt(Mathf.Clamp(_value, 0, 255));
		m_BlueSlider.value = _value;
		m_BlueText.text = _value.ToString();
		blueValue = _value;
		SetColor(new Color(redValue/255.0f, greenValue/255.0f, blueValue/255.0f, 1.0f));
	}
	
	public void SetBlue(string _value){
		int parsedValue = 0;
		if(int.TryParse(_value, out parsedValue));
		else Debug.Log("Error parsing color code!");
		SetBlue(parsedValue);
	}
	
	public void ActivateWindow(int _noteType){
		if(_noteType==0) {
			noteType = Note.NoteType.LeftHanded;
			SetColor(Track.LeftHandColor);
		}
		else if(_noteType==1) {
			noteType = Note.NoteType.RightHanded;
			SetColor(Track.RightHandColor);
		}
		else if(_noteType==2) {
			noteType = Note.NoteType.OneHandSpecial;
			SetColor(Track.OneHandColor);
		}
		else if(_noteType==3) {
			noteType = Note.NoteType.BothHandsSpecial;
			SetColor(Track.TwoHandColor);
		}
		active = true;
		Track.ColorPickerWindowOpen = true;
		m_ColorPickerWindowAnimator.Play("Panel In");
	}
	
	public void SaveChanges(){
		active = false;
		Track.ColorPickerWindowOpen = false;
		Color _leftHandColor = Track.LeftHandColor;
		Color _rightHandColor = Track.RightHandColor;
		Color _oneHandColor = Track.OneHandColor;
		Color _twoHandColor = Track.TwoHandColor;
		switch(noteType) {
			case Note.NoteType.LeftHanded:
				_leftHandColor = selectedColor;
				break;
			case Note.NoteType.RightHanded:
				_rightHandColor = selectedColor;
				break;
			case Note.NoteType.OneHandSpecial:
				_oneHandColor = selectedColor;
				break;
			case Note.NoteType.BothHandsSpecial:
				_twoHandColor = selectedColor;
				break;
		}
		Track.SetCustomNoteColors(_leftHandColor, _rightHandColor, _oneHandColor, _twoHandColor);
		m_ColorPickerWindowAnimator.Play("Panel Out");
	}
	
	public void CancelChanges(){
		active = false;
		Track.ColorPickerWindowOpen = false;
		m_ColorPickerWindowAnimator.Play("Panel Out");
	}
	
	public void ResetToDefault(){
		Color defaultColor;
		switch(noteType) {
			case Note.NoteType.LeftHanded:
				ColorUtility.TryParseHtmlString("#019BAB", out defaultColor);
				break;
			case Note.NoteType.RightHanded:
				ColorUtility.TryParseHtmlString("#E32862", out defaultColor);
				break;
			case Note.NoteType.OneHandSpecial:
				ColorUtility.TryParseHtmlString("#4ABC08", out defaultColor);
				break;
			case Note.NoteType.BothHandsSpecial:
				ColorUtility.TryParseHtmlString("#FB9D11", out defaultColor);
				break;
			default:
				ColorUtility.TryParseHtmlString("#019BAB", out defaultColor);
				break;
		}
		SetColor(defaultColor);
	}
}