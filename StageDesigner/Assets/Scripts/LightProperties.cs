using Ookii.Dialogs;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class LightProperties : MonoBehaviour
{
	public UIManager UIManager;
	public GameObject LightPrefab;

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

	public void AddListeners()
	{
		UIManager.IntensitySlider.RegisterValueChangedCallback(IntensityUpdated);

		UIManager.RedSlider.RegisterValueChangedCallback(LightColorUpdated);
		UIManager.GreenSlider.RegisterValueChangedCallback(LightColorUpdated);
		UIManager.BlueSlider.RegisterValueChangedCallback(LightColorUpdated);

		UIManager.XPosSlider.RegisterValueChangedCallback(LightPositionUpdated);
		UIManager.YPosSlider.RegisterValueChangedCallback(LightPositionUpdated);
		UIManager.ZPosSlider.RegisterValueChangedCallback(LightPositionUpdated);

		UIManager.XRotSlider.RegisterValueChangedCallback(LightRotationUpdated);
		UIManager.YRotSlider.RegisterValueChangedCallback(LightRotationUpdated);
		UIManager.ZRotSlider.RegisterValueChangedCallback(LightRotationUpdated);

		UIManager.RotSpeedSlider.RegisterValueChangedCallback(RotationSpeedUpdated);
		UIManager.PulseRateSlider.RegisterValueChangedCallback(PulseRateUpdated);

		UIManager.StartStopAnimationButton.clicked += StartStopAnimation;
	}

	public void RemoveListeners()
	{
		UIManager.IntensitySlider.UnregisterValueChangedCallback(IntensityUpdated);

		UIManager.RedSlider.UnregisterValueChangedCallback(LightColorUpdated);
		UIManager.GreenSlider.UnregisterValueChangedCallback(LightColorUpdated);
		UIManager.BlueSlider.UnregisterValueChangedCallback(LightColorUpdated);

		UIManager.XPosSlider.UnregisterValueChangedCallback(LightPositionUpdated);
		UIManager.YPosSlider.UnregisterValueChangedCallback(LightPositionUpdated);
		UIManager.ZPosSlider.UnregisterValueChangedCallback(LightPositionUpdated);

		UIManager.XRotSlider.UnregisterValueChangedCallback(LightRotationUpdated);
		UIManager.YRotSlider.UnregisterValueChangedCallback(LightRotationUpdated);
		UIManager.ZRotSlider.UnregisterValueChangedCallback(LightRotationUpdated);

		UIManager.RotSpeedSlider.UnregisterValueChangedCallback(RotationSpeedUpdated);
		UIManager.PulseRateSlider.UnregisterValueChangedCallback(PulseRateUpdated);

		UIManager.StartStopAnimationButton.clicked -= StartStopAnimation;
	}

	private void IntensityUpdated(ChangeEvent<int> evt)
	{
		if (SelectedLight != null)
		{
			SelectedLight.intensity = evt.newValue;
		}
		else if (LightMaterial != null)
		{
			Color finalColor = LightMaterial.GetColor("_EmissionColor") * evt.newValue;
			LightMaterial.SetColor("_EmissionColor", finalColor);
		}
	}

	private void LightColorUpdated(ChangeEvent<int> evt)
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
	}

	private void LightPositionUpdated(ChangeEvent<int> evt)
	{
		Vector3 newPosition = new Vector3(UIManager.XPosSlider.value, UIManager.YPosSlider.value, UIManager.ZPosSlider.value);
		transform.position = newPosition;
	}

	private void LightRotationUpdated(ChangeEvent<int> evt)
	{
		Vector3 newRotation = new Vector3(UIManager.XRotSlider.value, UIManager.YRotSlider.value, UIManager.ZRotSlider.value);
		transform.rotation = Quaternion.Euler(newRotation);
	}

	private void RotationSpeedUpdated(ChangeEvent<int> evt)
	{
	}

	private void PulseRateUpdated(ChangeEvent<int> evt)
	{
		PulseRate = evt.newValue;
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
		if (LightsOnPrefab.Length >= 0 & SelectedLight != null)
		{
			UIManager.IntensitySlider.value = (int)SelectedLight.intensity;

			// todo add range value to the UI
			//UIManager._rangeSlider.value = (int)SelectedLight.range;

			UIManager.RedSlider.value = (int)SelectedLight.color.r;
			UIManager.GreenSlider.value = (int)SelectedLight.color.g;
			UIManager.BlueSlider.value = (int)SelectedLight.color.b;

			UIManager.XPosSlider.value = (int)SelectedLight.transform.position.x;
			UIManager.YPosSlider.value = (int)SelectedLight.transform.position.y;
			UIManager.ZPosSlider.value = (int)SelectedLight.transform.position.z;

			UIManager.XRotSlider.value = (int)SelectedLight.transform.rotation.eulerAngles.x;
			UIManager.YRotSlider.value = (int)SelectedLight.transform.rotation.eulerAngles.y;
			UIManager.ZRotSlider.value = (int)SelectedLight.transform.rotation.eulerAngles.z;

			UIManager.RotSpeedSlider.value = (int)RotationSpeed;
			UIManager.PulseRateSlider.value = (int)PulseRate;
		}
		else if (LightMaterial != null)
		{
			// todo: update sliders to regulars and not ints, remove casts
			UIManager.IntensitySlider.value = (int)LightMaterial.GetColor("_EmissionColor").maxColorComponent;

			// todo add range value to the UI
			//UIManager._rangeSlider.value = (int)SelectedLight.range;

			UIManager.RedSlider.value = (int)LightMaterial.GetColor("_EmissionColor").r;
			UIManager.GreenSlider.value = (int)LightMaterial.GetColor("_EmissionColor").g;
			UIManager.BlueSlider.value = (int)LightMaterial.GetColor("_EmissionColor").b;

			UIManager.XPosSlider.value = (int)transform.position.x;
			UIManager.YPosSlider.value = (int)transform.position.y;
			UIManager.ZPosSlider.value = (int)transform.position.z;

			UIManager.XRotSlider.value = (int)transform.rotation.eulerAngles.x;
			UIManager.YRotSlider.value = (int)transform.rotation.eulerAngles.y;
			UIManager.ZRotSlider.value = (int)transform.rotation.eulerAngles.z;

			UIManager.RotSpeedSlider.value = (int)RotationSpeed;
			UIManager.PulseRateSlider.value = (int)PulseRate;
		}
	}
}