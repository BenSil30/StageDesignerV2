using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR

using UnityEditor;

#else
using SFB; // Standalone File Browser for standalone builds
#endif

public class ItemManager : MonoBehaviour
{
	public List<GameObject> AvailableItems = new();
	public List<GameObject> SpawnedItems = new();
	public GameObject SpawnPoint;
	public BudgetController BudgetController;
	public CampaignController CampaignController;
	public UIManager UImanager;

	private void Update()
	{
		// if the hud is visible and NOT the animatin pane, and the user is not dragging an item, animate every spawned object
		AnimateAllKeyframes();
	}

	public void AnimateAllKeyframes()
	{
		if (UImanager.HUDVisible
					&& !UImanager.AnimationPanelVisible
					&& !Input.GetMouseButton(1)
					&& SpawnedItems.Count > 0)
		{
			foreach (var item in SpawnedItems)
			{
				LightProperties lp = item.GetComponent<LightProperties>();
				if (lp != null)
				{
					lp.AnimateKeyframes();
				}
			}
		}
	}

	public void SpawnItem(string itemName)
	{
		GameObject selectedItem;
		GameObject newLight;

		selectedItem = AvailableItems.Find(x => x.name == itemName);
		if (!BudgetController.SandboxModeEnabled)
		{
			if (!BudgetController.CheckForBudgetSpace(selectedItem.GetComponent<LightProperties>().ItemCost))
			{
				UImanager.ShowToastNotification("Not enough budget remaining", 3f);
				return;
			}
			else
			{
				UImanager.ShowToastNotification($"Item spawned, remaining budget {BudgetController.RemainingBudget}", 3f);
			}
		}

		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		newLight.tag = "LightInScene";
		SpawnedItems.Add(newLight);
		newLight.name = itemName + " " + SpawnedItems.Count;

		// check for campaign objectives
		CheckForObjectiveCompletion();
	}

	public void CheckForObjectiveCompletion()
	{
		if (!BudgetController.SandboxModeEnabled)
		{
			switch (CampaignController.CurrentLevel)
			{
				// tutorial - level 2 spotlights 2 lasers
				case 0:
					int spolightCount = SpawnedItems.Count(item => item.name.Contains("Spotlight"));
					if (spolightCount >= 2 && !CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 2 spotlights").IsComplete)
					{
						CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 2 spotlights").IsComplete = true;
						UImanager.ShowToastNotification("Objective complete: The band has requested at least 2 spotlights", 3f);
					}
					int laserCount = SpawnedItems.Count(item => item.name.Contains("Laser"));
					if (laserCount >= 2 && !CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 2 lasers").IsComplete)
					{
						CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 2 lasers").IsComplete = true;
						UImanager.ShowToastNotification("Objective complete: The band has requested at least 2 lasers", 3f);
					}
					break;

				// level 1 - 4 lasers 2 fires 2 purple spotlights
				case 1:
					int laserCount1 = SpawnedItems.Count(item => item.name.Contains("Laser"));
					if (laserCount1 >= 4 && !CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 4 lasers").IsComplete)
					{
						CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 4 lasers").IsComplete = true;
						UImanager.ShowToastNotification("Objective complete: The band has requested at least 4 lasers", 3f);
					}

					int fireCount = SpawnedItems.Count(item => item.name.Contains("Fire"));
					if (fireCount >= 2 && !CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 2 fires").IsComplete)
					{
						CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 2 fires").IsComplete = true;
						UImanager.ShowToastNotification("Objective complete: The band has requested at least 2 fires", 3f);
					}

					Color targetPurple = new Color(.6f, .2f, .8f);
					float thresholdPurple = .35f;
					int purpleSpotlightCount = 0;
					foreach (var item in SpawnedItems)
					{
						if (item.name.Contains("Spotlight"))
						{
							LightProperties lp = item.GetComponent<LightProperties>();
							if (lp != null)
							{
								if (LightColorIsWithinThreshold(lp.LightsOnPrefab[0].color, targetPurple, thresholdPurple))
								{
									purpleSpotlightCount++;
								}
							}
						}
					}
					if (purpleSpotlightCount >= 2 && !CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 2 purple spotlights").IsComplete)
					{
						CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 2 purple spotlights").IsComplete = true;
						UImanager.ShowToastNotification("Objective complete: The band has requested at least 2 purple spotlights", 3f);
					}

					break;

				// level 2 - 2 discoballs, 3 different fires, 3 moving spotlights
				case 2:
					int discoballCount = SpawnedItems.Count(item => item.name.Contains("Disco Ball"));
					if (discoballCount >= 2 && !CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 2 discoballs").IsComplete)
					{
						CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 2 discoballs").IsComplete = true;
						UImanager.ShowToastNotification("Objective complete: The band has requested at least 2 discoballs", 3f);
					}

					int uniqueFireCount = SpawnedItems.Where(item => item.name.Contains("Fire"))
						.Select(item => item.name)
						.Distinct().Count();
					if (uniqueFireCount >= 3 && !CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 3 fires of different color").IsComplete)
					{
						CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 3 fires of different color").IsComplete = true;
						UImanager.ShowToastNotification("Objective complete: The band has requested at least 3 fires of different color", 3f);
					}

					int movingSpotlightCount = SpawnedItems.Count(item => item.name.Contains("Spotlight")
												&& item.GetComponent<LightProperties>()?.KeyframesOnPrefab.Count > 0);
					if (movingSpotlightCount >= 3 && !CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 3 moving spotlights").IsComplete)
					{
						CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 3 moving spotlights").IsComplete = true;
						UImanager.ShowToastNotification("Objective complete: The band has requested at least 3 moving spotlights", 3f);
					}
					break;

				// level 3 - 3 lasers of different color that must animate, 2 red spotlights that must animate, 3 strobing/pulsing lights any kind, 20 keyframes
				case 3:
					List<GameObject> uniqueLasers = SpawnedItems.Where(item => item.name.Contains("Laser"))
						.GroupBy(item => item.name)
						.Select(Group => Group.First())
						.ToList();
					int uniqueLaserCount = uniqueLasers.Count;
					bool allLasersAnimating = true;
					foreach (var item in SpawnedItems)
					{
						if (item.name.Contains("Laser"))
						{
							LightProperties lp = item.GetComponent<LightProperties>();
							if (lp != null)
							{
								if (lp.KeyframesOnPrefab.Count >= 0)
								{
									allLasersAnimating = false;
									break;
								}
							}
						}
					}

					if (uniqueLaserCount >= 3 && allLasersAnimating && !CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 3 lasers of different color. All lasers must animate").IsComplete)
					{
						CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 3 lasers of different color. All lasers must animate").IsComplete = true;
						UImanager.ShowToastNotification("Objective complete: The band has requested at least 3 lasers of different color. All lasers must animate", 3f);
					}

					Color targetRed = new Color(0.8f, 0.0f, 0.0f);
					float thresholdRed = .35f;
					int redMovingSpotlightCount = 0;
					foreach (var item in SpawnedItems)
					{
						if (item.name.Contains("Spotlight"))
						{
							LightProperties lp = item.GetComponent<LightProperties>();
							if (lp != null)
							{
								if (LightColorIsWithinThreshold(lp.LightsOnPrefab[0].color, targetRed, thresholdRed)
									&& lp.KeyframesOnPrefab.Count > 0)
								{
									redMovingSpotlightCount++;
								}
							}
						}
					}
					if (redMovingSpotlightCount >= 2 && !CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 2 red spotlights that must animate").IsComplete)
					{
						CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 2 red spotlights that must animate").IsComplete = true;
						UImanager.ShowToastNotification("Objective complete: The band has requested at least 2 red spotlights that must animate", 3f);
					}

					int strobingStationaryLightCount = 0;
					foreach (var item in SpawnedItems)
					{
						LightProperties lp = item.GetComponent<LightProperties>();
						if (lp != null)
						{
							if (lp.PulseRate > 0.0f && lp.KeyframesOnPrefab.Count < 1)
							{
								strobingStationaryLightCount++;
							}
						}
					}
					if (strobingStationaryLightCount >= 3 && !CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 3 strobing/pulsing lights of any kind, with no animation").IsComplete)
					{
						CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested at least 3 strobing/pulsing lights of any kind, with no animation").IsComplete = true;
						UImanager.ShowToastNotification("Objective complete: The band has requested at least 3 strobing/pulsing lights of any kind, with no animation", 3f);
					}

					int keyframeCount = 0;
					foreach (var item in SpawnedItems)
					{
						LightProperties lp = item.GetComponent<LightProperties>();
						if (lp != null)
						{
							keyframeCount += lp.KeyframesOnPrefab.Count;
						}
					}
					if (keyframeCount >= 20 && !CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested a lot of light movement (place at least 20 keyframes across all lights)").IsComplete)
					{
						CampaignController.CurrentObjectives.Find(x => x.Description == "The band has requested a lot of light movement (place at least 20 keyframes across all lights)").IsComplete = true;
						UImanager.ShowToastNotification("Objective complete: The band has requested a lot of light movement (place at least 20 keyframes across all lights)", 3f);
					}
					break;

				case 4:
					break;
			}

			CampaignController.CheckForLevelCompletion();
		}
	}

	private bool LightColorIsWithinThreshold(Color actualColor, Color targetColor, float threshold)
	{
		// log the same value as we are returning
		Debug.Log($"LightColorIsWithinThreshold: {Mathf.Abs(actualColor.r - targetColor.r) < threshold && Mathf.Abs(actualColor.g - targetColor.g) < threshold && Mathf.Abs(actualColor.b - targetColor.b) < threshold}");

		return Mathf.Abs(actualColor.r - targetColor.r) < threshold &&
			   Mathf.Abs(actualColor.g - targetColor.g) < threshold &&
			   Mathf.Abs(actualColor.b - targetColor.b) < threshold;
		// log the same value as we are returning
	}

	[System.Serializable]
	public class LightKeyframeData
	{
		public string LightName;
		public List<KeyframeClass> Keyframes;
	}

	[System.Serializable]
	public class KeyframeCollection
	{
		public List<LightKeyframeData> Lights = new();
		public int Stage;
		public string SongName;
		public float SongLength;
		public bool SandboxModeEnabled;

		public int CurrentLevel;
	}

	public void ExportAllKeyframes()
	{
		string filePath = GetSaveFilePath("Save Keyframes", "json");
		if (string.IsNullOrEmpty(filePath)) return; // User canceled

		KeyframeCollection keyframeCollection = new KeyframeCollection();

		foreach (GameObject lightObj in SpawnedItems)
		{
			LightProperties lightProperties = lightObj.GetComponent<LightProperties>();
			if (lightProperties != null)
			{
				if (lightProperties.KeyframesOnPrefab.Count == 0)
				{
					Debug.LogWarning($"No keyframes found on {lightObj.name}, light will not be saved");
					continue;
				}
				LightKeyframeData data = new LightKeyframeData
				{
					LightName = lightObj.name,
					Keyframes = lightProperties.KeyframesOnPrefab
				};

				keyframeCollection.Lights.Add(data);
			}
		}
		keyframeCollection.Stage = FindFirstObjectByType<StageManager>().currentStage;
		keyframeCollection.SongLength = FindFirstObjectByType<AudioSource>().clip.length;
		keyframeCollection.SandboxModeEnabled = BudgetController.SandboxModeEnabled;
		keyframeCollection.CurrentLevel = CampaignController.CurrentLevel;

		string json = JsonUtility.ToJson(keyframeCollection, true); Debug.Log(json);
		File.WriteAllText(filePath, json);

		Debug.Log($"Keyframes exported to {filePath}");
	}

	// todo: fix this method, the keyframelights need to be properly set or changed
	public void ImportKeyframes()
	{
		string filePath = GetOpenFilePath("Load Keyframes", "json");
		if (string.IsNullOrEmpty(filePath)) return; // User canceled

		if (!File.Exists(filePath))
		{
			Debug.LogError("Keyframe file not found!");
			return;
		}

		string json = File.ReadAllText(filePath);
		KeyframeCollection keyframeCollection = JsonUtility.FromJson<KeyframeCollection>(json);
		int stageIndex = JsonUtility.FromJson<KeyframeCollection>(json).Stage;
		float songLength = JsonUtility.FromJson<KeyframeCollection>(json).SongLength;

		foreach (var lightData in keyframeCollection.Lights)
		{
			string lightName = lightData.LightName;
			lightName = Regex.Replace(lightName, @"\d", "");
			lightName = lightName.Trim();
			// find the object with lightname in the list of available objects and spawn it as newLight
			GameObject newLight = AvailableItems.Find(x => x.name == lightName);
			// log the object found
			Debug.Log($"Found object: {newLight.name}");
			newLight = Instantiate(newLight, SpawnPoint.transform.position, Quaternion.identity);
			newLight.name = lightData.LightName;

			LightProperties lightProperties = newLight.GetComponent<LightProperties>();
			if (lightProperties != null)
			{
				lightProperties.KeyframesOnPrefab = lightData.Keyframes;
			}
			SpawnedItems.Add(newLight);
			FindFirstObjectByType<BudgetController>().UpdateBudgetOnLoad();
		}

		StageManager stageManager = FindFirstObjectByType<StageManager>();
		switch (stageIndex)
		{
			case 0: stageManager.SwitchStage("Default"); break;
			case 1: stageManager.SwitchStage("Paramount"); break;
			case 2: stageManager.SwitchStage("Ogden"); break;
			default: stageManager.SwitchStage("AllOff"); break;
		}

		if (FindFirstObjectByType<AudioSource>().clip != null)
		{
			UImanager.TimelineSlider.highValue = songLength;
		}

		BudgetController.SandboxModeEnabled = keyframeCollection.SandboxModeEnabled;
		CampaignController.CurrentLevel = keyframeCollection.CurrentLevel;
		CampaignController.LoadLevel(CampaignController.CurrentLevel);
		CheckForObjectiveCompletion();

		Debug.Log("Imported successful");

		UImanager.TogglePanelVisibility("AllOff");
		UImanager.StartClicked = true;
	}

	private string GetSaveFilePath(string title, string extension)
	{
#if UNITY_EDITOR
		return EditorUtility.SaveFilePanel(title, Application.persistentDataPath, "LightKeyframes", extension);
#else
        var path = StandaloneFileBrowser.SaveFilePanel(title, Application.persistentDataPath, "LightKeyframes", new[] { new ExtensionFilter("JSON Files", "json") });
        return string.IsNullOrEmpty(path) ? null : path;
#endif
	}

	private string GetOpenFilePath(string title, string extension)
	{
#if UNITY_EDITOR
		return EditorUtility.OpenFilePanel(title, Application.persistentDataPath, extension);
#else
        var paths = StandaloneFileBrowser.OpenFilePanel(title, Application.persistentDataPath, new[] { new ExtensionFilter("JSON Files", "json") }, false);
        return paths.Length > 0 ? paths[0] : null;
#endif
	}
}