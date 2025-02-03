using System.Collections;
using System.Collections.Generic;
using SFB;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour
{
	public UIDocument HUDDoc;
	public UIDocument ItemsPanelDoc;
	public UIDocument PauseStartDoc;
	public UIDocument StageSelectionDoc;
	public UIDocument MusicUploaderDoc;
	public UIDocument LightsAnimationDoc;
	public UIDocument GraphicsSettingsDoc;

	public bool CursorVisible;
	public bool IsPaused;
	public bool HUDVisible;
	public bool ItemsPanelVisible;
	public bool MusicUploaderVisible;
	public bool LightsPanelVisible;
	public bool PauseStartVisible;
	public bool StageSelectionVisible;
	public bool GraphicsSettingsVisible;

	#region HUD UI Elements

	private Button _pauseMenuButton;
	private Button _itemsMenuButton;

	#endregion HUD UI Elements

	#region Items UI Elements

	private Button _backButtonItems;
	private Button _item0Button;
	private Button _item1Button;
	private Button _item2Button;
	private Button _item3Button;
	private Button _item4Button;
	private Button _item5Button;
	private Button _item6Button;
	private Button _item7Button;
	private Button _item8Button;

	#endregion Items UI Elements

	#region Pause/Start UI Elements

	private Button _startResumeButtonP;

	private Button _goToStageP;
	private Button _exportButtonP;
	private Button _graphicsSettignsButtonP;

	#endregion Pause/Start UI Elements

	#region Stage Selection UI Elements

	private Button _defaultStageButton;

	private Button _paramountStageButton;
	private Button _ogdenStageButton;
	private Button _backButton;

	#endregion Stage Selection UI Elements

	#region Uploader UI Elements

	private Button _importButton;

	private Label _songTitleLabel;
	private Button _playPauseButton;
	private Button _restartButton;
	private Slider _scrubberSlider;

	public AudioSource AudioSource;
	public AudioClip AudioClip;
	public bool MusicIsPlaying = false;

	#endregion Uploader UI Elements

	#region Animation settings UI Elements

	public Label SelectedObjectTitle;
	public Button NextLightButton;

	public SliderInt IntensitySlider;

	public SliderInt RedSlider;
	public SliderInt GreenSlider;
	public SliderInt BlueSlider;

	public SliderInt XPosSlider;
	public SliderInt YPosSlider;
	public SliderInt ZPosSlider;

	public SliderInt XRotSlider;
	public SliderInt YRotSlider;
	public SliderInt ZRotSlider;

	public SliderInt XScaleSlider;

	public SliderInt PulseRateSlider;
	public SliderInt RotSpeedSlider;

	public Button StartStopAnimationButton;

	#endregion Animation settings UI Elements

	#region Graphics settings UI Elements

	private Button _bloomToggleButton;

	private Button _fogToggleButton;

	private Button _filmGrainToggleButton;

	private Button _motionBlurButton;

	private Label _bloomLabel;

	private Label _fogLabel;

	private Label _filmGrainLabel;

	private Label _motionBlurLabel;

	public GameObject FogVolume;

	public GameObject PostProcessingVolume;

	#endregion Graphics settings UI Elements

	private Coroutine musicLoadingCoroutine;

	// Start is called before the first frame update
	private void Start()
	{
		#region HUD UI elements

		VisualElement HUDRoot = HUDDoc.rootVisualElement;
		_pauseMenuButton = HUDRoot.Q<Button>("PauseMenuButton");
		_itemsMenuButton = HUDRoot.Q<Button>("ItemMenuButton");
		_pauseMenuButton.clicked += PauseMenuButtonClicked;
		_itemsMenuButton.clicked += ItemsMenuButtonClicked;

		#endregion HUD UI elements

		#region Items UI elements

		VisualElement ItemsPanelRoot = ItemsPanelDoc.rootVisualElement;
		_backButtonItems = ItemsPanelRoot.Q<Button>("BackButtonItemsPanel");
		_item0Button = ItemsPanelRoot.Q<Button>("Item0Button");
		_item1Button = ItemsPanelRoot.Q<Button>("Item1Button");
		_item2Button = ItemsPanelRoot.Q<Button>("Item2Button");
		_item3Button = ItemsPanelRoot.Q<Button>("Item3Button");
		_item4Button = ItemsPanelRoot.Q<Button>("Item4Button");
		_item5Button = ItemsPanelRoot.Q<Button>("Item5Button");
		_item6Button = ItemsPanelRoot.Q<Button>("Item6Button");
		_item7Button = ItemsPanelRoot.Q<Button>("Item7Button");
		_item8Button = ItemsPanelRoot.Q<Button>("Item8Button");

		_backButtonItems.clicked += BackButtonItemsPanelClicked;
		_item0Button.clicked += () => ItemSelectedFromPanel(_item0Button);
		_item1Button.clicked += () => ItemSelectedFromPanel(_item1Button);
		_item2Button.clicked += () => ItemSelectedFromPanel(_item2Button);
		_item3Button.clicked += () => ItemSelectedFromPanel(_item3Button);
		_item4Button.clicked += () => ItemSelectedFromPanel(_item4Button);
		_item5Button.clicked += () => ItemSelectedFromPanel(_item5Button);
		_item6Button.clicked += () => ItemSelectedFromPanel(_item6Button);
		_item7Button.clicked += () => ItemSelectedFromPanel(_item7Button);
		_item8Button.clicked += () => ItemSelectedFromPanel(_item8Button);

		#endregion Items UI elements

		#region Pause/Start UI Elements

		VisualElement PauseStartRoot = PauseStartDoc.rootVisualElement;
		_startResumeButtonP = PauseStartRoot.Q<Button>("StageButton");
		_goToStageP = PauseStartRoot.Q<Button>("ChooseStageButton");
		_exportButtonP = PauseStartRoot.Q<Button>("ExportButton");
		_graphicsSettignsButtonP = PauseStartRoot.Q<Button>("GraphicsSettingsButton");

		_startResumeButtonP.clicked += StartResumeClicked;
		_goToStageP.clicked += GoToStageClicked;
		_exportButtonP.clicked += ExportClicked;
		_graphicsSettignsButtonP.clicked += GraphicsSettingsButtonClicked;

		#endregion Pause/Start UI Elements

		#region Stage Selection UI Elements

		VisualElement StageSelectionRoot = StageSelectionDoc.rootVisualElement;
		_defaultStageButton = StageSelectionRoot.Q<Button>("DefaultButton");
		_paramountStageButton = StageSelectionRoot.Q<Button>("ParamountButton");
		_ogdenStageButton = StageSelectionRoot.Q<Button>("TheOgdenButton");
		_backButton = StageSelectionRoot.Q<Button>("BackButton");

		_defaultStageButton.clicked += DefaultStageClicked;
		_paramountStageButton.clicked += ParamountStageClicked;
		_ogdenStageButton.clicked += OgdenStageClicked;
		_backButton.clicked += BackClicked;

		#endregion Stage Selection UI Elements

		#region Music Uploader UI Elements

		VisualElement MusicUploaderRoot = MusicUploaderDoc.rootVisualElement;
		_importButton = MusicUploaderRoot.Q<Button>("UploadButton");
		_songTitleLabel = MusicUploaderRoot.Q<Label>("SongTitle");
		_playPauseButton = MusicUploaderRoot.Q<Button>("PlayPauseButton");
		_restartButton = MusicUploaderRoot.Q<Button>("RestartButton");
		_scrubberSlider = MusicUploaderRoot.Q<Slider>("ScrubberSlider");

		_importButton.clicked += ImportClicked;
		_playPauseButton.clicked += PlayPauseMusicClicked;
		_restartButton.clicked += RestartMusicClicked;

		#endregion Music Uploader UI Elements

		#region Aniimation settings UI Elements

		VisualElement LightsAnimationRoot = LightsAnimationDoc.rootVisualElement;
		SelectedObjectTitle = LightsAnimationRoot.Q<Label>("SelectedObject");
		NextLightButton = LightsAnimationRoot.Q<Button>("NextLightButton");
		NextLightButton.clicked += NextLightButtonHit;

		IntensitySlider = LightsAnimationRoot.Q<SliderInt>("LightIntensitySlider");

		RedSlider = LightsAnimationRoot.Q<SliderInt>("RedColorSlider");
		GreenSlider = LightsAnimationRoot.Q<SliderInt>("GreenColorSlider");
		BlueSlider = LightsAnimationRoot.Q<SliderInt>("BlueColorSlider");

		XPosSlider = LightsAnimationRoot.Q<SliderInt>("XPosSlider");
		YPosSlider = LightsAnimationRoot.Q<SliderInt>("YPosSlider");
		ZPosSlider = LightsAnimationRoot.Q<SliderInt>("ZPosSlider");

		XRotSlider = LightsAnimationRoot.Q<SliderInt>("XRotSlider");
		YRotSlider = LightsAnimationRoot.Q<SliderInt>("YRotSlider");
		ZRotSlider = LightsAnimationRoot.Q<SliderInt>("ZRotSlider");
		//_xScaleSlider = LightsAnimationRoot.Q<SliderInt>("XScaleSlider");
		PulseRateSlider = LightsAnimationRoot.Q<SliderInt>("PulseRateSlider");
		RotSpeedSlider = LightsAnimationRoot.Q<SliderInt>("RotSpeedSlider");
		StartStopAnimationButton = LightsAnimationRoot.Q<Button>("ToggleAnimationButton");

		#endregion Aniimation settings UI Elements

		#region Graphics settings UI Elements

		VisualElement GraphicsSettingsRoot = GraphicsSettingsDoc.rootVisualElement;
		_bloomToggleButton = GraphicsSettingsRoot.Q<Button>("BloomButton");
		_fogToggleButton = GraphicsSettingsRoot.Q<Button>("FogButton");
		_filmGrainToggleButton = GraphicsSettingsRoot.Q<Button>("FilmGrainButton");
		_motionBlurButton = GraphicsSettingsRoot.Q<Button>("MotionBlurButton");

		_bloomLabel = GraphicsSettingsRoot.Q<Label>("BloomLabel");
		_fogLabel = GraphicsSettingsRoot.Q<Label>("FogLabel");
		_filmGrainLabel = GraphicsSettingsRoot.Q<Label>("FilmGrainLabel");
		_motionBlurLabel = GraphicsSettingsRoot.Q<Label>("MotionBlurLabel");

		_bloomToggleButton.clicked += BloomButtonClicked;
		_fogToggleButton.clicked += FogButtonClicked;
		_filmGrainToggleButton.clicked += FilmGrainButtonClicked;
		_motionBlurButton.clicked += MotionBlurButtonClicked;

		#endregion Graphics settings UI Elements

		TogglePanelVisibility("PauseStart");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			TogglePanelVisibility("MusicUploader");
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			TogglePanelVisibility("LightsAnimation");
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePanelVisibility("AllOff");
		}

		// update scrubber slider
		if (MusicUploaderVisible && MusicIsPlaying)
		{
			if (AudioSource.clip != null) _scrubberSlider.value = AudioSource.time;
		}
	}

	private void TogglePanelVisibility(string panelName)
	{
		if (MusicIsPlaying & panelName != "ItemsPanel")
		{
			PlayPauseMusicClicked();
		}
		switch (panelName)
		{
			case "PauseStart":
				PauseStartVisible = true;
				Time.timeScale = 0f;

				HUDVisible = false;
				ItemsPanelVisible = false;
				StageSelectionVisible = false;
				MusicUploaderVisible = false;
				LightsPanelVisible = false;
				GraphicsSettingsVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				PauseStartDoc.rootVisualElement.style.visibility = Visibility.Visible;
				PauseStartDoc.rootVisualElement.focusable = true;
				StageSelectionDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StageSelectionDoc.rootVisualElement.focusable = false;
				MusicUploaderDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderDoc.rootVisualElement.focusable = false;
				LightsAnimationDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				LightsAnimationDoc.rootVisualElement.focusable = false;
				GraphicsSettingsDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				GraphicsSettingsDoc.rootVisualElement.focusable = false;

				break;

			case "ItemsPanel":
				ItemsPanelVisible = true;

				PauseStartVisible = false;
				HUDVisible = false;
				StageSelectionVisible = false;
				MusicUploaderVisible = false;
				LightsPanelVisible = false;
				GraphicsSettingsVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Visible;
				ItemsPanelDoc.rootVisualElement.focusable = true;
				PauseStartDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				PauseStartDoc.rootVisualElement.focusable = false;
				StageSelectionDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StageSelectionDoc.rootVisualElement.focusable = false;
				MusicUploaderDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderDoc.rootVisualElement.focusable = false;
				LightsAnimationDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				LightsAnimationDoc.rootVisualElement.focusable = false;
				GraphicsSettingsDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				GraphicsSettingsDoc.rootVisualElement.focusable = false;
				break;

			case "StageSelection":
				StageSelectionVisible = true;
				Time.timeScale = 0f;

				HUDVisible = false;
				ItemsPanelVisible = false;
				PauseStartVisible = false;
				MusicUploaderVisible = false;
				LightsPanelVisible = false;
				GraphicsSettingsVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				PauseStartDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				PauseStartDoc.rootVisualElement.focusable = false;
				StageSelectionDoc.rootVisualElement.style.visibility = Visibility.Visible;
				StageSelectionDoc.rootVisualElement.focusable = true;
				MusicUploaderDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderDoc.rootVisualElement.focusable = false;
				LightsAnimationDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				LightsAnimationDoc.rootVisualElement.focusable = false;
				GraphicsSettingsDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				GraphicsSettingsDoc.rootVisualElement.focusable = false;

				break;

			case "MusicUploader":
				MusicUploaderVisible = true;

				HUDVisible = false;
				ItemsPanelVisible = false;
				PauseStartVisible = false;
				StageSelectionVisible = false;
				LightsPanelVisible = false;
				GraphicsSettingsVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				PauseStartDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				PauseStartDoc.rootVisualElement.focusable = false;
				StageSelectionDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StageSelectionDoc.rootVisualElement.focusable = false;
				MusicUploaderDoc.rootVisualElement.style.visibility = Visibility.Visible;
				MusicUploaderDoc.rootVisualElement.focusable = true;
				LightsAnimationDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				LightsAnimationDoc.rootVisualElement.focusable = false;
				GraphicsSettingsDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				GraphicsSettingsDoc.rootVisualElement.focusable = false;

				break;

			case "LightsAnimation":
				LightsPanelVisible = true;

				HUDVisible = false;
				ItemsPanelVisible = false;
				PauseStartVisible = false;
				StageSelectionVisible = false;
				MusicUploaderVisible = false;
				GraphicsSettingsVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				PauseStartDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				PauseStartDoc.rootVisualElement.focusable = false;
				StageSelectionDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StageSelectionDoc.rootVisualElement.focusable = false;
				MusicUploaderDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderDoc.rootVisualElement.focusable = false;
				LightsAnimationDoc.rootVisualElement.style.visibility = Visibility.Visible;
				LightsAnimationDoc.rootVisualElement.focusable = true;
				GraphicsSettingsDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				GraphicsSettingsDoc.rootVisualElement.focusable = false;

				break;

			case "GraphicsSettings":
				GraphicsSettingsVisible = true;
				Time.timeScale = 0f;

				HUDVisible = false;
				ItemsPanelVisible = false;
				PauseStartVisible = false;
				StageSelectionVisible = false;
				MusicUploaderVisible = false;
				LightsPanelVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				PauseStartDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				PauseStartDoc.rootVisualElement.focusable = false;
				StageSelectionDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StageSelectionDoc.rootVisualElement.focusable = false;
				MusicUploaderDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderDoc.rootVisualElement.focusable = false;
				LightsAnimationDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				LightsAnimationDoc.rootVisualElement.focusable = false;
				GraphicsSettingsDoc.rootVisualElement.style.visibility = Visibility.Visible;
				GraphicsSettingsDoc.rootVisualElement.focusable = true;

				break;

			case "AllOff": // shows the HUD only
				HUDVisible = true;
				ItemsPanelVisible = false;
				PauseStartVisible = false;
				StageSelectionVisible = false;
				MusicUploaderVisible = false;
				LightsPanelVisible = false;
				GraphicsSettingsVisible = false;
				Time.timeScale = 1f;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Visible;
				HUDDoc.rootVisualElement.focusable = true;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				PauseStartDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				PauseStartDoc.rootVisualElement.focusable = false;
				StageSelectionDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StageSelectionDoc.rootVisualElement.focusable = false;
				MusicUploaderDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderDoc.rootVisualElement.focusable = false;
				LightsAnimationDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				LightsAnimationDoc.rootVisualElement.focusable = false;
				GraphicsSettingsDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				GraphicsSettingsDoc.rootVisualElement.focusable = false;
				break;
		}
	}

	private void NextLightButtonHit()
	{
		SelectionManager sm = FindObjectOfType<SelectionManager>();
		if (sm.SelectedObject != null | sm.CurrentLightProperties.LightsOnPrefab.Length == 1) return;
		if (sm.CurrentLightProperties.CurrentLightIndex >= sm.CurrentLightProperties.LightsOnPrefab.Length - 1)
		{
			sm.CurrentLightProperties.CurrentLightIndex = 0;
		}
		else
		{
			sm.CurrentLightProperties.CurrentLightIndex++;
		}
		sm.CurrentLightProperties.SelectedLight = sm.CurrentLightProperties.LightsOnPrefab[sm.CurrentLightProperties.CurrentLightIndex];
		sm.CurrentLightProperties.UpdateSliderValues();
	}

	#region HUD Methods

	private void PauseMenuButtonClicked()
	{
		TogglePanelVisibility("PauseStart");
	}

	private void ItemsMenuButtonClicked()
	{
		TogglePanelVisibility("ItemsPanel");
	}

	#endregion HUD Methods

	#region Items Methods

	private void BackButtonItemsPanelClicked()
	{
		TogglePanelVisibility("AllOff");
	}

	private void ItemSelectedFromPanel(Button buttonPressed)
	{
		if (buttonPressed != null)
		{
			Debug.Log(buttonPressed.name + " was pressed");
			FindObjectOfType<ItemManager>().SpawnItem(buttonPressed.text);
		}
		TogglePanelVisibility("AllOff");
	}

	#endregion Items Methods

	#region PauseStart Methods

	private void StartResumeClicked()
	{
		Debug.Log("Game Paused/Started");
		Time.timeScale = 1f;
		TogglePanelVisibility("AllOff");
	}

	private void GoToStageClicked()
	{
		TogglePanelVisibility("StageSelection");
	}

	private void ExportClicked()
	{
		// todo export stuff lmao
	}

	private void GraphicsSettingsButtonClicked()
	{
		TogglePanelVisibility("GraphicsSettings");
	}

	#endregion PauseStart Methods

	#region StageSelection Methods

	private void DefaultStageClicked()
	{
		Time.timeScale = 1f;
		TogglePanelVisibility("AllOff");
		FindObjectOfType<StageManager>().SwitchStage("AllOff");
		FindObjectOfType<StageManager>().SwitchStage("Default");
	}

	private void ParamountStageClicked()
	{
		Time.timeScale = 1f;
		TogglePanelVisibility("AllOff");
		FindObjectOfType<StageManager>().SwitchStage("AllOff");
		FindObjectOfType<StageManager>().SwitchStage("Paramount");
	}

	private void OgdenStageClicked()
	{
		Time.timeScale = 1f;
		TogglePanelVisibility("AllOff");
		FindObjectOfType<StageManager>().SwitchStage("AllOff");
		FindObjectOfType<StageManager>().SwitchStage("Ogden");
	}

	private void BackClicked()
	{
		TogglePanelVisibility("PauseStart");
	}

	#endregion StageSelection Methods

	#region MusicUploader Methods

	private void ImportClicked()
	{
		// Open file browser and let the user select an audio file
		string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Music", "", "mp3", false);

		if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
		{
			// If a coroutine is already running, stop it first
			if (musicLoadingCoroutine != null)
			{
				StopCoroutine(musicLoadingCoroutine);
			}

			// Start a new coroutine to load and play the music
			musicLoadingCoroutine = StartCoroutine(LoadMusic(paths[0]));
		}
		else
		{
			_songTitleLabel.text = "No File Uploaded";
		}
	}

	[System.Obsolete]
	private IEnumerator LoadMusic(string path)
	{
		using (WWW www = new WWW("file:///" + path))
		{
			yield return www;

			if (www.error == null)
			{
				AudioClip clip = www.GetAudioClip(false, true);
				if (clip != null)
				{
					AudioSource.clip = clip;
					AudioClip = clip;
					//audioSource.Play();
					_songTitleLabel.text = "Selected: " + Path.GetFileName(path);
					_scrubberSlider.highValue = clip.length;
				}
				else
				{
					_songTitleLabel.text = "Failed to load audio file!";
				}
			}
			else
			{
				_songTitleLabel.text = "Error: " + www.error;
			}
		}
	}

	private void PlayPauseMusicClicked()
	{
		if (AudioSource.clip != null)
		{
			if (MusicIsPlaying)
			{
				AudioSource.Pause();
				MusicIsPlaying = false;
			}
			else
			{
				AudioSource.Play();
				MusicIsPlaying = true;
			}
		}
	}

	private void RestartMusicClicked()
	{
		if (AudioSource.clip != null)
		{
			AudioSource.Stop();
			AudioSource.Play();
			_scrubberSlider.value = 0;
		}
	}

	#endregion MusicUploader Methods

	#region GraphicsSettings Methods

	private void FogButtonClicked()
	{
		FogVolume.SetActive(!FogVolume.activeSelf);
		if (FogVolume.activeSelf)
		{
			_fogLabel.text = "On";
		}
		else
		{
			_fogLabel.text = "Off";
		}
	}

	private void BloomButtonClicked()
	{
		Bloom bloom;
		if (PostProcessingVolume.GetComponent<Volume>().profile.TryGet(out bloom))
		{
			if (bloom != null)
			{
				bloom.active = !bloom.active;
			}
			if (bloom.active)
			{
				_bloomLabel.text = "On";
			}
			else
			{
				_bloomLabel.text = "Off";
			}
		}
	}

	private void FilmGrainButtonClicked()
	{
		FilmGrain grain;
		if (PostProcessingVolume.GetComponent<Volume>().profile.TryGet(out grain))
		{
			if (grain != null)
			{
				grain.active = !grain.active;
			}
			if (grain.active)
			{
				_filmGrainLabel.text = "On";
			}
			else
			{
				_filmGrainLabel.text = "Off";
			}
		}
	}

	private void MotionBlurButtonClicked()
	{
		MotionBlur blur;
		if (PostProcessingVolume.GetComponent<Volume>().profile.TryGet(out blur))
		{
			if (blur != null)
			{
				blur.active = !blur.active;
			}
			if (blur.active)
			{
				_motionBlurLabel.text = "On";
			}
			else
			{
				_motionBlurLabel.text = "Off";
			}
		}
	}

	#endregion GraphicsSettings Methods
}