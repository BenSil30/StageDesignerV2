using System;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
	public GameObject SelectedObject;
	public Camera MainCamera;

	public Material SelectionMat;
	public Material SelectedItemStorageMat;

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

				SelectedItemStorageMat = SelectedObject.GetComponentInChildren<Renderer>().material;
				SelectedObject.GetComponentInChildren<Renderer>().material = SelectionMat;
			}
		}
	}

	private void DeleteSelectedObject()
	{
		if (SelectedObject != null)
		{
			Destroy(SelectedObject);
			CurrentLightProperties = null;
			SelectedItemStorageMat = null;
		}
	}

	private void ClearSelection()
	{
		if (SelectedObject != null)
		{
			if (CurrentLightProperties != null) CurrentLightProperties.RemoveListeners();

			SelectedObject.GetComponentInChildren<Renderer>().material = SelectedItemStorageMat;
			SelectedItemStorageMat = null;
			SelectedObject = null;
		}
	}

	private void UpdateSelectedLightProperties()
	{
		if (SelectedObject == null) return;
		CurrentLightProperties = SelectedObject.GetComponent<LightProperties>();
		CurrentLightProperties.CurrentLightIndex = 0;
		if (CurrentLightProperties.LightsOnPrefab.Length > 0)
			CurrentLightProperties.SelectedLight = CurrentLightProperties.LightsOnPrefab[0];
		CurrentLightProperties.UpdateSliderValues();
		CurrentLightProperties.AddListeners();
	}
}