// Class to save step measure data for toggling between two sets of options

using MiKu.NET;
using UnityEngine;

public class StepSaver {
	private Track.StepSelectorCycleMode savedStepSelectorCycleMode;
	private float savedMBPMIncreaseFactor;
	
	public StepSaver(){
		savedStepSelectorCycleMode = Track.StepSelectorCycleMode.Fours;
		savedMBPMIncreaseFactor = 1f/1f;
	}
	
	public void SaveStepData(Track.StepSelectorCycleMode _mode, float _factor){
		savedStepSelectorCycleMode = _mode;
		savedMBPMIncreaseFactor = _factor;
	}
	
	public Track.StepSelectorCycleMode getSavedCycleMode(){
		return savedStepSelectorCycleMode;
	}
	
	public float getSavedMBPMIncreaseFactor(){
		return savedMBPMIncreaseFactor;
	}
}