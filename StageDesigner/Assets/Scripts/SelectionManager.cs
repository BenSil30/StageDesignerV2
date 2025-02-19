using System;
using TransformGizmos;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
	public GameObject SelectedObject;
	public Camera MainCamera;
	public GameObject MoveGizmoPrefab;
	public GameObject Gizmo;

	public bool SelectionLocked;

	public Material SelectionMat;
	public Material SelectedItemStorageMat;

	public LightProperties CurrentLightProperties;
	public ItemManager ItemManager;

	private void Update()
	{
		if (FindAnyObjectByType<UIManager>().HUDVisible)
		{
			SelectionLocked = false;
		}
		else if (!FindAnyObjectByType<UIManager>().HUDVisible)
		{
			SelectionLocked = true;
		}

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

		//if (SelectedObject == null)
		//{
		//	Gizmo.SetActive(false);
		//}
		//else if (SelectedObject != null)
		//{
		//	Gizmo.GetComponent<GizmoController>().m_targetObject = SelectedObject;
		//	Gizmo.SetActive(true);
		//	Gizmo.transform.position = SelectedObject.transform.position;
		//}
	}

	private void SelectObjectUnderMouse()
	{
		if (SelectionLocked) return;
		if (Input.GetKey(KeyCode.LeftShift) | Input.GetMouseButton(2)) return;
		Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			GameObject hitObject = hit.collider.gameObject;
			if (hitObject.CompareTag("LightInScene") || hitObject.CompareTag("SelectableLightingPrefab"))
			{
				ClearSelection();
				SelectedObject = hitObject;
				UpdateSelectedLightProperties();

				SelectedItemStorageMat = SelectedObject.GetComponentInChildren<Renderer>().material;
				SelectedObject.GetComponentInChildren<Renderer>().material = SelectionMat;

				MoveGizmoPrefab.GetComponent<GizmoController>().m_targetObject = SelectedObject;
				Gizmo = Instantiate(MoveGizmoPrefab);
				Gizmo.transform.position = SelectedObject.transform.position;

				//if (ActiveGizmo != null) Destroy(ActiveGizmo);
				//ActiveGizmo = Instantiate(MoveGizmoPrefab);
				//ActiveGizmo.GetComponent<MoveGizmo>().Initialize(SelectedObject.transform);
			}
		}
	}

	private void DeleteSelectedObject()
	{
		if (SelectedObject != null)
		{
			ItemManager.SpawnedItems.Remove(SelectedObject);
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
			CurrentLightProperties = null;
			SelectedObject.GetComponentInChildren<Renderer>().material = SelectedItemStorageMat;
			SelectedItemStorageMat = null;
			SelectedObject = null;

			if (Gizmo != null) Destroy(Gizmo);
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