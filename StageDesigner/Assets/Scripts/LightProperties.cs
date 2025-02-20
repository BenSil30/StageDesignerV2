using Ookii.Dialogs;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using Button = UnityEngine.UIElements.Button;

public class LightProperties : MonoBehaviour
{
	public UIManager UIManager;
	public List<KeyframeClass> KeyframesOnPrefab = new List<KeyframeClass>(); // Holds keyframes for this light

	public Light[] LightsOnPrefab;
	public Light SelectedLight = null;
	public int CurrentLightIndex;

	public Material LightMaterial;
	private Color _emissionColor;

	public float RotationSpeed = 0f;
	public float PulseTimer = 0f;
	public float PulseRate = 0f;

	public bool PulseOn = false;
	public bool IsAnimating = false;

	public bool IsANonLightPrefab = false;

	private Coroutine RotationCoroutine;

	// todo: do light selection within prefabs that have multiple lights, has to add a new button to the UI for it

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
	{
		UIManager = GameObject.Find("UI").GetComponent<UIManager>();
		if (LightsOnPrefab.Length > 0)
		{
			SelectedLight = LightsOnPrefab[0];
		}
	}

	// Update is called once per frame
	private void Update()
	{
		StrobeLight();
	}

	public void AddKeyframe(float time)
	{
		if (KeyframesOnPrefab.Exists(x => x.KeyframeTime == time))
		{
			UpdateKeyframe(time);
			return;
		}
		if (SelectedLight != null)
		{
			KeyframeClass newKeyframe = new KeyframeClass(
				time,
				CurrentLightIndex,
				transform.position,
				transform.rotation,
				SelectedLight.intensity,
				SelectedLight.color,
				RotationSpeed,
				PulseRate,
				PulseOn,
				IsAnimating);
			KeyframesOnPrefab.Add(newKeyframe);
			KeyframesOnPrefab.Sort((a, b) => a.KeyframeTime.CompareTo(b.KeyframeTime)); // Ensure keyframes are ordered by time
		}
		else
		{
			KeyframeClass newKeyframe = new KeyframeClass(
				time,
				transform.position,
				transform.rotation,
				LightMaterial.GetColor("_EmissionColor"),
				LightMaterial.color,
				RotationSpeed,
				PulseRate,
				PulseOn,
				IsAnimating
			);
			KeyframesOnPrefab.Add(newKeyframe);
			KeyframesOnPrefab.Sort((a, b) => a.KeyframeTime.CompareTo(b.KeyframeTime)); // Ensure keyframes are ordered by time
		}
		UIManager um = FindFirstObjectByType<UIManager>();
		VisualElement root = um.LightsAnimationDoc.rootVisualElement;
		Button keyframeButton = root.Q<Button>("AddKeyframeAnimationPanelButton");

		// turn the background of the button a light orange
		keyframeButton.style.backgroundColor = new StyleColor(new Color(0.7372549f, 0.7372549f, 0.7372549f, 1f));
		// log with all keyframe added info
		Debug.Log($"Keyframe added at time: {time} - Position: {transform.position} - Rotation: {transform.rotation.eulerAngles} - Intensity: {SelectedLight.intensity} - Color: {SelectedLight.color} - Rotation Speed: {RotationSpeed} - Pulse Rate: {PulseRate} - Pulse On: {PulseOn} - Is Animating: {IsAnimating}");
	}

	public void UpdateKeyframe(float time)
	{
		KeyframeClass keyframeToUpdate = KeyframesOnPrefab.Find(x => x.KeyframeTime == time);
		DeleteKeyframe(time);
		AddKeyframe(time);
		FindFirstObjectByType<UIManager>().ShowKeyframeToasts("Keyframe Updated", 1f);
		KeyframesOnPrefab.Sort((a, b) => a.KeyframeTime.CompareTo(b.KeyframeTime)); // Ensure keyframes are ordered by time
	}

	public void DeleteKeyframe(float time)
	{
		KeyframeClass keyframeToDelete = KeyframesOnPrefab.Find(x => x.KeyframeTime == time);
		KeyframesOnPrefab.Remove(keyframeToDelete);
		KeyframesOnPrefab.Sort((a, b) => a.KeyframeTime.CompareTo(b.KeyframeTime)); // Ensure keyframes are ordered by time
	}

	public void AnimateKeyframes()
	{
		if (KeyframesOnPrefab.Count < 0) return;
		float currentTime = FindFirstObjectByType<UIManager>().TimelineSlider.value;
		KeyframeClass currentKeyframe = null;
		KeyframeClass nextKeyframe = null;

		// find previous and next keyframes
		for (int i = 0; i < KeyframesOnPrefab.Count - 1; i++)
		{
			// check if grabbing correct frames for the time
			if (KeyframesOnPrefab[i].KeyframeTime <= currentTime
				&& KeyframesOnPrefab[i + 1].KeyframeTime >= currentTime)
			{
				currentKeyframe = KeyframesOnPrefab[i];
				nextKeyframe = KeyframesOnPrefab[i + 1];

				// if the prefab has lights and the current keyframe light is not the same as the next keyframe light, iterate until the keyframe light matches
				if (LightsOnPrefab.Length > 0 && currentKeyframe.KeyframeLightIndex != nextKeyframe.KeyframeLightIndex)
				{
					for (int j = i + 1; j < KeyframesOnPrefab.Count; j++)
					{
						// if next keyframe is found, break out of the inner and outer loops
						if (currentKeyframe.KeyframeLightIndex == KeyframesOnPrefab[j].KeyframeLightIndex && KeyframesOnPrefab[j].KeyframeTime >= currentTime)
						{
							nextKeyframe = KeyframesOnPrefab[j];
							break;
						}
					}
					Debug.Log($"Current Keyframe Time: {currentKeyframe.KeyframeTime} - Next Keyframe Time: {nextKeyframe.KeyframeTime}");
					break;
				}
			}
		}

		// log the times of current and next keyframes

		if (currentKeyframe != null && nextKeyframe != null)
		{
			float t = (currentTime - currentKeyframe.KeyframeTime) / (nextKeyframe.KeyframeTime - currentKeyframe.KeyframeTime);
			transform.position = Vector3.Lerp(currentKeyframe.KeyframePosition, nextKeyframe.KeyframePosition, t);
			transform.rotation = Quaternion.Slerp(currentKeyframe.KeyframeRotation, nextKeyframe.KeyframeRotation, t);

			if (LightsOnPrefab.Length > 0)
			{
				LightsOnPrefab[currentKeyframe.KeyframeLightIndex].intensity = Mathf.Lerp(currentKeyframe.KeyframeIntensity, nextKeyframe.KeyframeIntensity, t);
				LightsOnPrefab[currentKeyframe.KeyframeLightIndex].color = Color.Lerp(currentKeyframe.KeyframeColor, nextKeyframe.KeyframeColor, t);
			}
			else
			{
				LightMaterial.SetColor("_EmissionColor", Color.Lerp(currentKeyframe.AlternativeKeyframeIntensity, nextKeyframe.AlternativeKeyframeIntensity, t));
				LightMaterial.color = Color.Lerp(currentKeyframe.KeyframeColor, nextKeyframe.KeyframeColor, t);
			}

			RotationSpeed = Mathf.Lerp(currentKeyframe.KeyframeRotationSpeed, nextKeyframe.KeyframeRotationSpeed, t);
			PulseRate = Mathf.Lerp(currentKeyframe.KeyframePulseRate, nextKeyframe.KeyframePulseRate, t);

			PulseOn = currentKeyframe.KeyframePulseOn;
			IsAnimating = currentKeyframe.KeyframeIsAnimating;
		}
	}

	public void AddListeners()
	{
		UIManager.IntensitySlider.RegisterValueChangedCallback(IntensityUpdated);
		// when intensity is changed, call notify of value change

		UIManager.RedSlider.RegisterValueChangedCallback(LightColorUpdated);
		UIManager.GreenSlider.RegisterValueChangedCallback(LightColorUpdated);
		UIManager.BlueSlider.RegisterValueChangedCallback(LightColorUpdated);

		UIManager.RotSpeedSlider.RegisterValueChangedCallback(RotationSpeedUpdated);
		UIManager.PulseRateSlider.RegisterValueChangedCallback(PulseRateUpdated);

		UIManager.AddKeyframeAnimationButton.clicked += StartStopAnimation;
	}

	public void RemoveListeners()
	{
		UIManager.IntensitySlider.UnregisterValueChangedCallback(IntensityUpdated);

		UIManager.RedSlider.UnregisterValueChangedCallback(LightColorUpdated);
		UIManager.GreenSlider.UnregisterValueChangedCallback(LightColorUpdated);
		UIManager.BlueSlider.UnregisterValueChangedCallback(LightColorUpdated);

		UIManager.RotSpeedSlider.UnregisterValueChangedCallback(RotationSpeedUpdated);
		UIManager.PulseRateSlider.UnregisterValueChangedCallback(PulseRateUpdated);

		UIManager.AddKeyframeAnimationButton.clicked -= StartStopAnimation;
	}

	// todo: still needs to notify when position or rotation are changed
	public void NotifyOfValueChange()
	{
		UIManager um = FindFirstObjectByType<UIManager>();
		um.ShowKeyframeToasts("Value Changed", 1f);
		VisualElement root = um.LightsAnimationDoc.rootVisualElement;
		Button keyframeButton = root.Q<Button>("AddKeyframeAnimationPanelButton");
		// turn the background of the button a light orange
		keyframeButton.style.backgroundColor = new StyleColor(new Color(0.9320754f, 0.6493151f, 0.3288643f, 1f));
	}

	public void IntensityUpdated(ChangeEvent<float> evt)
	{
		if (SelectedLight != null)
		{
			SelectedLight.intensity = evt.newValue * 10000;
		}
		else if (LightMaterial != null)
		{
			Color finalColor = LightMaterial.GetColor("_EmissionColor") * (evt.newValue * 8);
			LightMaterial.SetColor("_EmissionColor", finalColor);
		}
		NotifyOfValueChange();
	}

	public void LightColorUpdated(ChangeEvent<float> evt)
	{
		if (SelectedLight != null)
		{
			Color lightColor = new Color(UIManager.RedSlider.value, UIManager.GreenSlider.value, UIManager.BlueSlider.value);
			SelectedLight.color = lightColor;
		}
		else if (LightMaterial != null)
		{
			Color lightColor = new Color(UIManager.RedSlider.value, UIManager.GreenSlider.value, UIManager.BlueSlider.value) * UIManager.IntensitySlider.value;
			LightMaterial.SetColor("_EmissionColor", lightColor);
		}
		NotifyOfValueChange();
	}

	public void RotationSpeedUpdated(ChangeEvent<float> evt)
	{
		RotationSpeed = evt.newValue;
		NotifyOfValueChange();
	}

	public void PulseRateUpdated(ChangeEvent<float> evt)
	{
		PulseRate = evt.newValue;
		NotifyOfValueChange();
	}

	public void StrobeLight()
	{
		if (SelectedLight != null)
		{
			if (PulseRate > 0)
			{
				PulseTimer += Time.deltaTime;
				if (PulseTimer >= 1f / PulseRate)
				{
					PulseOn = !PulseOn;
					SelectedLight.enabled = PulseOn;
					PulseTimer = 0;
				}
			}
			else
			{
				// Ensure the light stays on when the strobe is off
				SelectedLight.enabled = true;
			}
		}
		else if (LightMaterial != null)
		{
			if (PulseRate > 0)
			{
				PulseTimer += Time.deltaTime;
				if (PulseTimer >= 1f / PulseRate)
				{
					// Toggle flickering state
					PulseOn = !PulseOn;

					// Set emission based on the flickering state
					if (PulseOn)
					{
						// Set emission to the original color (or a defined color)
						// Get the current emission color
						LightMaterial.SetColor("_EmissionColor", _emissionColor); // Set emission to the original color
					}
					else
					{
						// Set emission to black (turn off)
						LightMaterial.SetColor("_EmissionColor", Color.black); // Turn off emission
					}

					PulseTimer = 0; // Reset the timer
				}
			}
		}
	}

	private void StartStopAnimation()
	{
		IsAnimating = !IsAnimating;
		if (IsAnimating)
		{
			RotationCoroutine = StartCoroutine(RotateLightContinually());
		}
		else
		{
			StopCoroutine(RotationCoroutine);
		}
	}

	public IEnumerator RotateLightContinually()
	{
		while (IsAnimating)
		{
			if (LightsOnPrefab.Length > 0)
			{
				foreach (var light in LightsOnPrefab)
				{
					light.transform.Rotate(transform.up, RotationSpeed * Time.deltaTime);
					transform.Rotate(transform.up, RotationSpeed * Time.deltaTime);
				}
				//yield return null;
			}
			else
			{
				transform.Rotate(transform.up, RotationSpeed * Time.deltaTime);
				//Debug.Log($"Rotating object: {this.name} - New Rotation: {transform.rotation.eulerAngles}");
			}
			yield return null;
		}
	}

	public void UpdateSliderValues()
	{
		// if prefab has actual lights
		if (LightsOnPrefab.Length >= 0 & SelectedLight != null)
		{
			UIManager.IntensitySlider.value = SelectedLight.intensity;

			// todo add range value to the UI
			//UIManager._rangeSlider.value = SelectedLight.range;

			UIManager.RedSlider.value = SelectedLight.color.r;
			UIManager.GreenSlider.value = SelectedLight.color.g;
			UIManager.BlueSlider.value = SelectedLight.color.b;

			UIManager.RotSpeedSlider.value = RotationSpeed;
			UIManager.PulseRateSlider.value = PulseRate;
		}
		// if prefab doesn't have any lights but has an emissive material instead
		else if (LightMaterial != null)
		{
			UIManager.IntensitySlider.value = LightMaterial.GetColor("_EmissionColor").maxColorComponent;
			//UIManager._rangeSlider.value = SelectedLight.range;

			UIManager.RedSlider.value = LightMaterial.GetColor("_EmissionColor").r;
			UIManager.GreenSlider.value = LightMaterial.GetColor("_EmissionColor").g;
			UIManager.BlueSlider.value = LightMaterial.GetColor("_EmissionColor").b;

			UIManager.RotSpeedSlider.value = RotationSpeed;
			UIManager.PulseRateSlider.value = PulseRate;
		}
	}
}