using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	public List<GameObject> AvailableItems = new List<GameObject>();
	public List<GameObject> SpawnedItems = new List<GameObject>();
	public GameObject SpawnPoint;

	public bool BudgetEnabled;
	public float StartingBudget;
	public float RemainingBudget;

	public void SpawnItem(string itemName)
	{
		GameObject selectedItem;
		GameObject newLight;

		selectedItem = AvailableItems.Find(x => x.name == itemName);
		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		newLight.tag = "LightInScene";
		SpawnedItems.Add(newLight);
		newLight.name = itemName + " " + SpawnedItems.Count;

		//switch (itemName)
		//{
		//	case "Spotlight":
		//		selectedItem = AvailableItems[0];
		//		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		//		newLight.tag = "LightInScene";
		//		SpawnedItems.Add(newLight);
		//		newLight.name = itemName + " " + SpawnedItems.Count;
		//		break;

		//	case "Blue laser":
		//		selectedItem = AvailableItems[1];
		//		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		//		newLight.tag = "LightInScene";
		//		SpawnedItems.Add(newLight);
		//		newLight.name = itemName + " " + SpawnedItems.Count;
		//		break;

		//	case "Green laser":
		//		selectedItem = AvailableItems[2];
		//		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		//		newLight.tag = "LightInScene";
		//		SpawnedItems.Add(newLight);
		//		newLight.name = itemName + " " + SpawnedItems.Count;
		//		break;

		//	case "Purple laser":
		//		selectedItem = AvailableItems[3];
		//		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		//		newLight.tag = "LightInScene";
		//		SpawnedItems.Add(newLight);
		//		newLight.name = itemName + " " + SpawnedItems.Count;
		//		break;

		//	case "Red laser":
		//		selectedItem = AvailableItems[4];
		//		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		//		newLight.tag = "LightInScene";
		//		SpawnedItems.Add(newLight);
		//		newLight.name = itemName + " " + SpawnedItems.Count;
		//		break;

		//	case "Three light":
		//		selectedItem = AvailableItems[5];
		//		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		//		newLight.tag = "LightInScene";
		//		SpawnedItems.Add(newLight);
		//		newLight.name = itemName + " " + SpawnedItems.Count;
		//		break;

		//	case "Discoball":
		//		selectedItem = AvailableItems[6];
		//		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		//		newLight.tag = "LightInScene";
		//		SpawnedItems.Add(newLight);
		//		newLight.name = itemName + " " + SpawnedItems.Count;
		//		break;

		//	case "Blue Fire":
		//		selectedItem = AvailableItems[7];
		//		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		//		newLight.tag = "LightInScene";
		//		SpawnedItems.Add(newLight);
		//		newLight.name = itemName + " " + SpawnedItems.Count;
		//		break;

		//	case "Green Fire":
		//		selectedItem = AvailableItems[8];
		//		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		//		newLight.tag = "LightInScene";
		//		SpawnedItems.Add(newLight);
		//		newLight.name = itemName + " " + SpawnedItems.Count;
		//		break;

		//	case "Purple Fire":
		//		selectedItem = AvailableItems[9];
		//		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		//		newLight.tag = "LightInScene";
		//		SpawnedItems.Add(newLight);
		//		newLight.name = itemName + " " + SpawnedItems.Count;
		//		break;

		//	case "Red Fire":
		//		selectedItem = AvailableItems[10];
		//		newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
		//		newLight.tag = "LightInScene";
		//		SpawnedItems.Add(newLight);
		//		newLight.name = itemName + " " + SpawnedItems.Count;
		//		break;

		//	default:
		//		Debug.Log("Item not found: " + itemName);
		//		break;
		//}
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
	}

	// todo: add stage selection and maybe music file to export
	public void ExportAllKeyframes()
	{
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

		string json = JsonUtility.ToJson(keyframeCollection, true); Debug.Log(json);
		string filePath = Path.Combine(Application.persistentDataPath, "LightKeyframes.json");
		File.WriteAllText(filePath, json);

		Debug.Log($"Keyframes exported to {filePath}");
	}

	// todo: fix this method, the keyframelights need to be properly set or changed
	public void ImportKeyframes()
	{
		string filePath = Path.Combine(Application.persistentDataPath, "LightKeyframes.json");
		if (!File.Exists(filePath))
		{
			Debug.LogError("Keyframe file not found!");
			return;
		}

		string json = File.ReadAllText(filePath);
		KeyframeCollection keyframeCollection = JsonUtility.FromJson<KeyframeCollection>(json);

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

		Debug.Log("Keyframes imported successfully!");
		FindFirstObjectByType<UIManager>().TogglePanelVisibility("AllOff");
	}
}