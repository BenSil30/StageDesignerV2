using System;
using TransformGizmos;
using UnityEngine;
using UnityEngine.UIElements;

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
	public UIManager UImanager;

	private void Update()
	{
		if (UImanager.HUDVisible)
		{
			SelectionLocked = false;
		}
		else if (!UImanager.HUDVisible)
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

		if (Gizmo != null)
		{
			Gizmo.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}
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
				if (SelectedObject != null)
				{
					FindFirstObjectByType<UIManager>().ShowToastNotification("Please deselect item first", 2f);
					return;
				}
				SelectedObject = hitObject;
				UpdateSelectedLightProperties();

				SelectedItemStorageMat = SelectedObject.GetComponentInChildren<Renderer>().material;
				SelectedObject.GetComponentInChildren<Renderer>().material = SelectionMat;

				Gizmo = Instantiate(MoveGizmoPrefab);
				Gizmo.GetComponent<GizmoController>().m_targetObject = SelectedObject;
				Gizmo.transform.position = SelectedObject.transform.position;

				Gizmo.transform.localScale = Vector3.one;
			}
		}
		if (CurrentLightProperties.LightsOnPrefab.Length > 0)
		{
			UImanager.LightsAnimationDoc.rootVisualElement.Q<Button>("NextLightButton").style.display = DisplayStyle.Flex;
		}
		else
		{
			UImanager.LightsAnimationDoc.rootVisualElement.Q<Button>("NextLightButton").style.display = DisplayStyle.None;
		}
	}

	private void DeleteSelectedObject()
	{
		if (SelectedObject != null)
		{
			ItemManager.SpawnedItems.Remove(SelectedObject);
			Destroy(Gizmo);
			Destroy(SelectedObject);
			CurrentLightProperties = null;
			SelectedItemStorageMat = null;
			if (UImanager.AnimationPanelVisible)
			{
				UImanager.TogglePanelVisibility("AllOff");
			}
		}
	}

	private void ClearSelection()
	{
		if (SelectedObject != null)
		{
			if (Gizmo != null) Destroy(Gizmo);
			if (CurrentLightProperties != null) CurrentLightProperties.RemoveListeners();
			CurrentLightProperties = null;
			SelectedObject.GetComponentInChildren<Renderer>().material = SelectedItemStorageMat;
			SelectedItemStorageMat = null;
			SelectedObject = null;
			if (UImanager.AnimationPanelVisible)
			{
				UImanager.TogglePanelVisibility("AllOff");
			}
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
		FindFirstObjectByType<UIManager>().RefreshKeyframeList();
	}
}