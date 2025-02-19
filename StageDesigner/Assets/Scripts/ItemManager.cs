using System.Collections.Generic;
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
		switch (itemName)
		{
			case "Spotlight":
				selectedItem = AvailableItems[0];
				newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
				newLight.tag = "LightInScene";
				SpawnedItems.Add(newLight);
				break;

			case "Blue laser":
				selectedItem = AvailableItems[1];
				newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
				newLight.tag = "LightInScene";
				SpawnedItems.Add(newLight);
				break;

			case "Green laser":
				selectedItem = AvailableItems[2];
				newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
				newLight.tag = "LightInScene";
				SpawnedItems.Add(newLight);
				break;

			case "Purple laser":
				selectedItem = AvailableItems[3];
				newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
				newLight.tag = "LightInScene";
				SpawnedItems.Add(newLight);
				break;

			case "Red laser":
				selectedItem = AvailableItems[4];
				newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
				newLight.tag = "LightInScene";
				SpawnedItems.Add(newLight);
				break;

			case "Three light":
				selectedItem = AvailableItems[5];
				newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
				newLight.tag = "LightInScene";
				SpawnedItems.Add(newLight);
				break;

			case "Discoball":
				selectedItem = AvailableItems[6];
				newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
				newLight.tag = "LightInScene";
				SpawnedItems.Add(newLight);
				break;

			case "Fire":
				selectedItem = AvailableItems[7];
				newLight = Instantiate(selectedItem, SpawnPoint.transform.position, Quaternion.identity);
				newLight.tag = "LightInScene";
				SpawnedItems.Add(newLight);
				break;

			default:
				Debug.Log("Item not found: " + itemName);
				break;
		}
	}
}