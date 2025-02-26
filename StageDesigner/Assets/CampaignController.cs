using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class CampaignController : MonoBehaviour
{
	[System.Serializable]
	public class LevelObjective
	{
		public int CorrespondingLevel;
		public string Description;
		public bool IsComplete;

		public LevelObjective(int level, string levelDescription)
		{
			CorrespondingLevel = level;
			Description = levelDescription;
			IsComplete = false;
		}
	}

	public int CurrentLevel;
	public List<LevelObjective> CurrentObjectives = new();
	public UIManager UImanager;

	public void InitializeCampaign()
	{
		CurrentLevel = 0;

		GetComponent<BudgetController>().SandboxModeEnabled = false;
		GetComponent<BudgetController>().CurrentBudgetLevel = CurrentLevel;
		GetComponent<BudgetController>().Budget = GetComponent<BudgetController>().BudgetLevels[CurrentLevel];
		GetComponent<BudgetController>().RemainingBudget = GetComponent<BudgetController>().Budget;

		LoadLevel(CurrentLevel);
	}

	public void LoadLevel(int level)
	{
		CurrentObjectives.Clear();

		// todo expand on level objectives when added more prefabs
		switch (level)
		{
			case 0:
				CurrentObjectives.Add(new LevelObjective(0, "The band has requested at least 2 spotlights"));
				CurrentObjectives.Add(new LevelObjective(0, "The band has requested at least 2 lasers"));
				break;

			case 1:
				CurrentObjectives.Add(new LevelObjective(1, "The band has requested at least 4 lasers"));
				CurrentObjectives.Add(new LevelObjective(1, "The band has requested at least 2 fires"));
				CurrentObjectives.Add(new LevelObjective(1, "The band has requested at least 2 purple spotlights"));
				break;

			case 2:
				CurrentObjectives.Add(new LevelObjective(2, "The band has requested at least 2 discoballs"));
				CurrentObjectives.Add(new LevelObjective(2, "The band has requested at least 3 fires of different color"));
				CurrentObjectives.Add(new LevelObjective(2, "The band has requested at least 3 moving spotlights"));
				break;

			case 3:
				CurrentObjectives.Add(new LevelObjective(3, "The band has requested at least 3 lasers of different color. All lasers must animate"));
				CurrentObjectives.Add(new LevelObjective(3, "The band has requested at least 2 red spotlights that must animate"));
				CurrentObjectives.Add(new LevelObjective(3, "The band has requested at least 3 strobing/pulsing lights of any kind, with no animation"));
				CurrentObjectives.Add(new LevelObjective(3, "The band has requested a lot of light movement (place at least 20 keyframes across all lights)"));
				break;

			case 4:
				break;
		}
		ShowLevelPrompts();
	}

	private void ShowLevelPrompts()
	{
		foreach (var objective in CurrentObjectives)
		{
			if (!objective.IsComplete)
			{
				UImanager.ShowToastNotification($"New Objective: {objective.Description}", 2f);
			}
		}
	}

	public void CheckForLevelCompletion()
	{
		foreach (var objective in CurrentObjectives)
		{
			if (!objective.IsComplete)
			{
				return;
			}
		}
		CurrentLevel++;
		if (CurrentLevel < 4)
		{
			UImanager.ShowToastNotification("Level Complete!", 5f);
			GetComponent<BudgetController>().IncrementBudgetList();
			LoadLevel(CurrentLevel);
		}
		else
		{
			UImanager.ShowToastNotification("Campaign Complete!", 5f);
		}
	}
}