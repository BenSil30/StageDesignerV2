using UnityEngine;

public class StageManager : MonoBehaviour
{
	public GameObject[] stages;
	public int currentStage = 0;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
	{
		foreach (var stage in stages)
		{
			stage.SetActive(false);
		}
	}

	public void SwitchStage(string stage)
	{
		switch (stage)
		{
			case "Default":
				currentStage = 0;
				stages[currentStage].SetActive(true);
				break;

			case "Paramount":
				currentStage = 1;
				stages[currentStage].SetActive(true);
				break;

			case "Ogden":
				currentStage = 2;
				stages[currentStage].SetActive(true);
				break;

			case "AllOff":
				foreach (var staged in stages)
				{
					staged.SetActive(false);
					currentStage = -1;
				}
				break;
		}
	}
}