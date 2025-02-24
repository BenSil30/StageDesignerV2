using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BudgetController : MonoBehaviour
{
	public ItemManager ItemManager;
	public SelectionManager SelectionManager;

	public float Budget;
	public float RemainingBudget;

	public List<float> BudgetLevels = new List<float>();

	public int CurrentBudgetLevel;
	public bool SandboxModeEnabled = false;

	private void Start()
	{
		if (!SandboxModeEnabled)
		{
			CurrentBudgetLevel = 0;
			Budget = BudgetLevels[CurrentBudgetLevel];
			RemainingBudget = Budget;
		}
	}

	// todo increment level switching
	public void IncrementBudgetList()
	{
		if (CurrentBudgetLevel < BudgetLevels.Count - 1)
		{
			CurrentBudgetLevel++;
			Budget = BudgetLevels[CurrentBudgetLevel];
			RemainingBudget = Budget;
			FindFirstObjectByType<UIManager>().BudgetLabel.text = $"Remaining Budget: {RemainingBudget}";
		}
	}

	public bool CheckForBudgetSpace(float cost)
	{
		if (SandboxModeEnabled)
		{
			return true;
		}
		if (RemainingBudget - cost >= 0)
		{
			RemainingBudget -= cost;
			FindFirstObjectByType<UIManager>().BudgetLabel.text = $"Remaining Budget: {RemainingBudget}";
			return true;
		}
		return false;
	}

	public void UpdateBudgetOnLoad()
	{
		foreach (var item in ItemManager.SpawnedItems)
		{
			RemainingBudget -= item.GetComponent<LightProperties>().ItemCost;
		}
		FindFirstObjectByType<UIManager>().BudgetLabel.text = $"Remaining Budget: {RemainingBudget}";
	}
}