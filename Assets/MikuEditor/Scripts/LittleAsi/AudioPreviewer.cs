using System;
using System.Linq;
using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.UI;
using MiKu.NET;
using System.IO;

public class AudioPreviewer : MonoBehaviour {
	[SerializeField]
	private Sprite m_PlaySprite;
	[SerializeField]
	private Sprite m_PauseSprite;
	[SerializeField]
	private GameObject m_playButtonImage;
	[SerializeField]
	private GameObject m_playButtonHighlightedImage;
	[SerializeField]
	private Transform m_SpectrumHolder;
	[SerializeField]
	private Slider m_Slider;
	[SerializeField]
	private GameObject m_SpectrumPointMarker;
	private AudioSource audioSource;  
	public static AudioPreviewer audioPreviewer;
	bool playing;
	bool audioSourceLoaded;
	SpectralFluxAnalyzer preProcessedSpectralFluxAnalyzer;
	float spectrumHeightMultiplier = 0.1f;	
	private const string spc_cacheFolder = "/SpectrumData/";
	private string spectrumCachePath;
	private const string spc_ext = ".spectrum";
	
	void Awake(){
		if(audioPreviewer == null){
			audioPreviewer = this;
		} else if(audioPreviewer != this){
			Destroy(gameObject);
		}
		audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.loop = false;
        audioSource.playOnAwake = false;
	}
	
	void Start(){
		spectrumCachePath = Application.dataPath+"/../"+spc_cacheFolder;
	}

	void Update(){
		if(audioSourceLoaded){
			if (m_Slider.value > (int)audioSource.clip.length){
				m_playButtonImage.GetComponent<Image>().sprite = m_PlaySprite;
			    m_playButtonHighlightedImage.GetComponent<Image>().sprite = m_PlaySprite;
				playing = false;
				audioSource.time = 0;
				m_Slider.value = 0;
				audioSource.Stop();
			} else if (playing && m_Slider.value < (int)audioSource.clip.length){
				m_Slider.value = audioSource.time;
			}
		}
	}

	public void SliderDown(){
		if(audioSourceLoaded){
			audioSource.Pause();
			playing = false;
			m_playButtonImage.GetComponent<Image>().sprite = m_PlaySprite;
			m_playButtonHighlightedImage.GetComponent<Image>().sprite = m_PlaySprite;
		}
	}

	public void SliderUp(){
		if(audioSourceLoaded){
			if (m_Slider.value < audioSource.clip.length){
				audioSource.time = Mathf.Clamp(m_Slider.value, 0f, audioSource.clip.length);
				audioSource.Play();
				playing = true;
				m_playButtonImage.GetComponent<Image>().sprite = m_PauseSprite;
			    m_playButtonHighlightedImage.GetComponent<Image>().sprite = m_PauseSprite;
			}
			else{
				audioSource.Stop();
				audioSource.time = 0;
				playing = false;
				m_playButtonImage.GetComponent<Image>().sprite = m_PlaySprite;
			    m_playButtonHighlightedImage.GetComponent<Image>().sprite = m_PlaySprite;
			}
		}
	}
	
	public void ToggleAudioPreview(){
		if(audioSourceLoaded){
			if(playing){
			  m_playButtonImage.GetComponent<Image>().sprite = m_PlaySprite;
			  m_playButtonHighlightedImage.GetComponent<Image>().sprite = m_PlaySprite;
			  audioSource.Pause();
			}
			else{
			  m_playButtonImage.GetComponent<Image>().sprite = m_PauseSprite;
			  m_playButtonHighlightedImage.GetComponent<Image>().sprite = m_PauseSprite;
			  audioSource.Play();
			}
			playing = !playing;
		}
	}
	
	public void Enable(AudioClip _loadedClip){
		if(!Serializer.IsExtratingClip && Serializer.ClipExtratedComplete) {
			audioSource.clip = _loadedClip;
			audioSourceLoaded = true;
			m_Slider.maxValue = audioSource.clip.length;
			m_Slider.value = 0;
			if(IsSpectrumCached()) {
				try {
					using(FileStream file = File.OpenRead(spectrumCachePath+Serializer.CleanInput(Serializer.ChartData.AudioName+spc_ext))) {
						System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
						preProcessedSpectralFluxAnalyzer = (SpectralFluxAnalyzer) bf.Deserialize(file);
					}

					EndSpectralAnalyzer();
					Debug.Log("Spectrum loaded from cached");
				} catch(Exception ex) {
					Miku_DialogManager.ShowDialog(Miku_DialogManager.DialogType.Alert, 
						"Error while dezerializing Spectrum "+ex.ToString()
					);
					Debug.Log(ex.ToString());
				}
			} else {
				Debug.Log("Spectrum data not cached!");
			}
		}
	}
	
	private bool IsSpectrumCached() {
		if(Serializer.ChartData != null){
			if (!Directory.Exists(spectrumCachePath)) {
				DirectoryInfo dir = Directory.CreateDirectory(spectrumCachePath);
				dir.Attributes |= FileAttributes.Hidden;
				return false;
			}
			if(File.Exists(spectrumCachePath+Serializer.CleanInput(Serializer.ChartData.AudioName+spc_ext))){ 
				return true;				
			}
		}
		return false;
	}
	
	private void EndSpectralAnalyzer(){
		List<SpectralFluxInfo> flux = preProcessedSpectralFluxAnalyzer.spectralFluxSamples;
		Vector3 targetTransform = Vector3.zero;
		Vector3 targetScale = Vector3.one;
		Transform plotTempInstance;
		Transform childTransform;
		float rectWidth = m_SpectrumHolder.transform.GetComponent<RectTransform>().rect.width;
		float startPos = (float)(m_SpectrumHolder.transform.localPosition.x-(rectWidth*.5));
		for(int i = 0; i < flux.Count; ++i) {
			if(flux[i].spectralFlux > 0) {
				plotTempInstance = Instantiate(m_SpectrumPointMarker).transform;
				plotTempInstance.parent = m_SpectrumHolder;
				targetTransform.x = startPos+(rectWidth*(flux[i].time/audioSource.clip.length));
				targetTransform.y = m_SpectrumHolder.transform.localPosition.y;
				targetTransform.z = m_SpectrumHolder.transform.localPosition.z;
				plotTempInstance.localPosition = targetTransform;
				targetScale = plotTempInstance.localScale;
				targetScale.y = flux[i].spectralFlux * spectrumHeightMultiplier;
				plotTempInstance.localScale = targetScale;
			}	
		}
	}
} 