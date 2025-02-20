using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#else
using SFB; // Standalone File Browser for standalone builds
#endif

public class ItemManager : MonoBehaviour
{
	public List<GameObject> AvailableItems = new List<GameObject>();
	public List<GameObject> SpawnedItems = new List<GameObject>();
	public GameObject SpawnPoint;

	public bool BudgetEnabled;
	public float StartingBudget;
	public float RemainingBudget;

	private void Update()
	{
		// if the hud is visible and NOT the animatin pane, and the user is not dragging an item, animate every spawned object
		AnimateAllKeyframes();
	}

	public void AnimateAllKeyframes()
	{
		if (FindFirstObjectByType<UIManager>().HUDVisible
					&& !FindFirstObjectByType<UIManager>().AnimationPanelVisible
					&& !Input.GetMouseButton(1)
					&& SpawnedItems.Count > 0)
		{
			foreach (var item in SpawnedItems)
			{
				LightProperties lp = item.GetComponent<LightProperties>();
				if (lp != null)
				{
					lp.AnimateKeyframes();
					Debug.Log("keyframe animated");
				}
			}
		}
	}

	public void SpawnItem(string itemName)
	{
		GameObject selectedItem;
		GameObject newLight;

		selectedItem = AvailableItems.Find(x => x.name == itemName);
		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		newLight.tag = "LightInScene";
		SpawnedItems.Add(newLight);
		newLight.name = itemName + " " + SpawnedItems.Count;
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
		public List<LightKeyframeData> Lights = new List<LightKeyframeData>();
		public int Stage;
		public string SongName;
		public float SongLength;
	}

	// todo: add stage selection and maybe music file to export
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
			Debug.Log(lightName);
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
			FindFirstObjectByType<UIManager>().TimelineSlider.highValue = songLength;
		}

		Debug.Log("Keyframes imported successfully!");
		FindFirstObjectByType<UIManager>().TogglePanelVisibility("AllOff");
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