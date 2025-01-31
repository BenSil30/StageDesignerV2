using System;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
	public GameObject SelectedObject;
	public Camera MainCamera;

	public LightProperties CurrentLightProperties;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			ClearSelection();
		}
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			DeleteSelectedObject();
		}
		if (Input.GetMouseButtonDown(0))
		{
			SelectObjectUnderMouse();
		}
	}

	private void SelectObjectUnderMouse()
	{
		if (Input.GetKey(KeyCode.LeftShift) | Input.GetMouseButton(2)) return;
		Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		ClearSelection();

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			GameObject hitObject = hit.collider.gameObject;
			if (hitObject.CompareTag("LightInScene") || hitObject.CompareTag("SelectableLightingPrefab"))
			{
				SelectedObject = hitObject;
				UpdateSelectedLightProperties();
			}
		}
	}

	private void DeleteSelectedObject()
	{
		if (SelectedObject != null)
		{
			Destroy(SelectedObject);
			CurrentLightProperties = null;
		}
	}

	private void ClearSelection()
	{
		if (SelectedObject != null)
		{
			if (CurrentLightProperties != null)
				CurrentLightProperties.RemoveListeners();
			SelectedObject = null;
		}
	}

	//private void SelectObject()
	//{
	//	// Create a ray from the center of the screen
	//	Ray ray = MainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
	//	RaycastHit hit;

	//	// Check if the ray hits any object
	//	if (!Physics.Raycast(ray, out hit)) return;
	//	// Check if the hit object is valid (e.g., has a certain tag)
	//	if (hit.collider == null) return;
	//	// do not allow user to select multiple objects at once

	//	if (SelectedObject != null)
	//	{
	//		Debug.Log("Object automatically deselected");
	//		ClearSelection();
	//		return;
	//	}

	//	CurrentLightProperties.RemoveListeners();
	//	// if not copy pasting
	//	if (hit.collider.CompareTag("LightInScene"))
	//	{
	//		IsSpawnableObject = false;
	//		SelectedObject = hit.collider.gameObject;

	//		UpdateSelectedLightProperties();

	//		Debug.Log("Selected Object: " + SelectedObject.name);
	//		GameObject.FindObjectOfType<UIManager>().SelectedObjectTitle.text = "Selected: " + SelectedObject.name;
	//	}
	//	// if its a copy paste object
	//	else if (hit.collider.CompareTag("SelectableLightingPrefab"))
	//	{
	//		IsSpawnableObject = true;
	//		SelectedObject = hit.collider.gameObject;

	//		Debug.Log("Selected Object: " + SelectedObject.name);
	//		GameObject.FindObjectOfType<UIManager>().SelectedObjectTitle.text = "Selected: " + SelectedObject.name + " to spawn";
	//	}
	//	else
	//	{
	//		Debug.Log("Clicked on a non-selectable object: " + hit.collider.gameObject.name);
	//	}
	//}

	private void UpdateSelectedLightProperties()
	{
		if (SelectedObject == null) return;
		CurrentLightProperties = SelectedObject.GetComponent<LightProperties>();
		CurrentLightProperties.CurrentLightIndex = 0;
		CurrentLightProperties.SelectedLight = CurrentLightProperties.LightsOnPrefab[0];
		CurrentLightProperties.UpdateSliderValues();
		CurrentLightProperties.AddListeners();
	}
}