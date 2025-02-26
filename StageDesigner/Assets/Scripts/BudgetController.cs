using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BudgetController : MonoBehaviour
{
	public ItemManager ItemManager;
	public SelectionManager SelectionManager;
	public UIManager UImanager;

	public float Budget;
	public float RemainingBudget;

	public List<float> BudgetLevels = new();

	public int CurrentBudgetLevel;
	public bool SandboxModeEnabled = false;

	public void IncrementBudgetList()
	{
		CurrentBudgetLevel++;
		Budget = BudgetLevels[CurrentBudgetLevel];
		RemainingBudget = Budget;
		UImanager.BudgetLabel.text = $"Remaining Budget: {RemainingBudget}";
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
			UImanager.BudgetLabel.text = $"Remaining Budget: {RemainingBudget}";
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
		UImanager.BudgetLabel.text = $"Remaining Budget: {RemainingBudget}";
	}
}