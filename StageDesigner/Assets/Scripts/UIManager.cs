using System.Collections;
using System.Collections.Generic;
using System.IO;
using SFB;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
	public SelectionManager sm;
	public BudgetController bc;

	public UIDocument HUDDoc;
	public UIDocument ItemsPanelDoc;
	public UIDocument StartMenuDoc;
	public UIDocument MusicUploaderStartMenuDoc;
	public UIDocument StageSelectionDoc;
	public UIDocument MusicUploaderDoc;
	public UIDocument LightsAnimationDoc;
	public UIDocument GraphicsSettingsDoc;

	public bool CursorVisible;
	public bool IsPaused;
	public bool HUDVisible;
	public bool ItemsPanelVisible;
	public bool MusicUploaderVisible;
	public bool AnimationPanelVisible;
	public bool StartVisible;
	public bool MusicUploaderStartVisible;
	public bool StageSelectionVisible;
	public bool GraphicsSettingsVisible;

	#region HUD UI Elements

	private VisualElement _musicHudRoot;
	private Button _pauseMenuButton;
	private Button _itemsMenuButton;
	private Button _musicMenuButton;
	private Button _lightsMenuButton;
	private Button _stageSelectionButton;

	private Button _addKeyframeButton;
	private Button _playPauseMusicHUDButton;

	public Slider TimelineSlider;
	private Label _musicDurationLabel;

	#endregion HUD UI Elements

	#region Items UI Elements

	private Button _backButtonItems;
	public Label BudgetLabel;

	#endregion Items UI Elements

	#region Start UI Elements

	private Button _playCampaignButton;
	private Button _playSandboxButton;
	public bool StartClicked = false;

	private Button _graphicsSettignsButtonP;
	private Button _exportButtonP;
	private Button _importButtonP;

	#endregion Start UI Elements

	#region Music Uploader Start Panel UI Elements

	private Label _fileNameLabel;

	private Button _chooseFileButton;
	private Button _goToSelectStageFromMusicUploader;
	private Button _goBackToStartFromMusicUploader;

	#endregion Music Uploader Start Panel UI Elements

	#region Stage Selection UI Elements

	private Button _defaultStageButton;

	private Button _paramountStageButton;
	private Button _ogdenStageButton;
	private Button _backButton;

	#endregion Stage Selection UI Elements

	#region Music Uploader Panel UI Elements

	private Button _importButton;

	private Label _songTitleLabel;
	private Button _restartButton;
	private Button _exitMusicUploaderPanelButton;

	public AudioSource AudioSource;
	public AudioClip AudioClip;
	public bool MusicIsPlaying = false;

	#endregion Music Uploader Panel UI Elements

	#region Animation settings UI Elements

	public Label SelectedObjectTitle;
	public Button NextLightButton;

	public Slider IntensitySlider;

	public Slider RedSlider;
	public Slider GreenSlider;
	public Slider BlueSlider;

	public Slider XScaleSlider;

	public Slider PulseRateSlider;
	public Slider RotSpeedSlider;

	public Button AddKeyframeAnimationButton;
	private Button _backButtonAnimation;

	public ScrollView KeyframeVisualList;

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

	private Button _backButtonGraphicsMenu;

	public GameObject FogVolume;

	public GameObject PostProcessingVolume;

	#endregion Graphics settings UI Elements

	private Coroutine musicLoadingCoroutine;

	// Start is called before the first frame update
	private void Start()
	{
		#region HUD UI elements

		VisualElement HUDRoot = HUDDoc.rootVisualElement;
		_musicHudRoot = HUDRoot.Q<VisualElement>("MusicHudRoot");

		_pauseMenuButton = HUDRoot.Q<Button>("PauseMenuButton");
		_itemsMenuButton = HUDRoot.Q<Button>("ItemMenuButton");
		_musicMenuButton = HUDRoot.Q<Button>("MusicMenuButton");
		_lightsMenuButton = HUDRoot.Q<Button>("LightsMenuButton");
		_stageSelectionButton = HUDRoot.Q<Button>("StageSelectionButton");
		_addKeyframeButton = HUDRoot.Q<Button>("AddKeyframeButton");

		_playPauseMusicHUDButton = HUDRoot.Q<Button>("PlayPauseMusicHudButton");

		TimelineSlider = HUDDoc.rootVisualElement.Q<Slider>("SongProgressSliderHud");
		_musicDurationLabel = HUDDoc.rootVisualElement.Q<Label>("SongDurationIndicatorHud");

		_pauseMenuButton.clicked += () => TogglePanelVisibility("PauseStart");
		_itemsMenuButton.clicked += () => TogglePanelVisibility("ItemsPanel");
		_musicMenuButton.clicked += () => TogglePanelVisibility("MusicUploader");
		_stageSelectionButton.clicked += () => TogglePanelVisibility("StageSelection");
		_lightsMenuButton.clicked += () => TogglePanelVisibility("LightsAnimation");
		_addKeyframeButton.clicked += () => AddKeyframeClicked();
		_playPauseMusicHUDButton.clicked += PlayPauseMusicClicked;
		// update keyframes and properties to reflect where the timelines lider is
		TimelineSlider.RegisterValueChangedCallback(evt =>
		{
			if (AudioSource.clip != null)
			{
				if (!AudioSource.isPlaying) AudioSource.time = evt.newValue;
				FindFirstObjectByType<ItemManager>().AnimateAllKeyframes();
				AddKeyframeAnimationButton.style.backgroundColor = new StyleColor(new Color(0.7372549f, 0.7372549f, 0.7372549f, 1f));
				//_addKeyframeButton.style.backgroundColor = new StyleColor(new Color(0.7372549f, 0.7372549f, 0.7372549f, 1f));
			}
		});
		//when dragging the slider, subscibe a method
		TimelineSlider.RegisterCallback<MouseDownEvent>(evt =>
		{
			FindFirstObjectByType<ItemManager>().AnimateAllKeyframes();
		});

		#endregion HUD UI elements

		#region Items UI elements

		VisualElement ItemsPanelRoot = ItemsPanelDoc.rootVisualElement;
		_backButtonItems = ItemsPanelRoot.Q<Button>("BackButtonItemsPanel");
		ScrollView ButtonsRoot = ItemsPanelRoot.Q<ScrollView>("ButtonScroller");
		ItemManager im = FindFirstObjectByType<ItemManager>();
		for (int i = 0; i < im.AvailableItems.Count; i++)
		{
			string name = im.AvailableItems[i].name;
			var newButton = new Button(() => ItemSelectedFromPanel(name))
			{
				// todo don't add price to button name if sandbox mode is enabled
				text = $"{name} - ${im.AvailableItems[i].GetComponent<LightProperties>().ItemCost}"
			};
			// change the size of newButton to be 200px wide with a font size of 32
			newButton.style.fontSize = 32;
			newButton.style.width = 250;
			ButtonsRoot.Add(newButton);
		}

		BudgetLabel = HUDDoc.rootVisualElement.Q<Label>("BudgetLabel");
		_backButtonItems.clicked += () => TogglePanelVisibility("AllOff");

		#endregion Items UI elements

		#region Start UI Elements

		VisualElement PauseStartRoot = StartMenuDoc.rootVisualElement;
		_playCampaignButton = PauseStartRoot.Q<Button>("PlayCampaignButton");
		_playSandboxButton = PauseStartRoot.Q<Button>("PlaySandboxButton");
		_graphicsSettignsButtonP = PauseStartRoot.Q<Button>("GraphicsSettingsButton");
		_exportButtonP = PauseStartRoot.Q<Button>("ExportButton");
		_importButtonP = PauseStartRoot.Q<Button>("ImportButton");

		_playCampaignButton.clicked += () => TogglePanelVisibility("MusicUploaderStartMenu");
		_playCampaignButton.clicked += () => bc.SandboxModeEnabled = false;

		_playSandboxButton.clicked += () => TogglePanelVisibility("MusicUploaderStartMenu");
		_playSandboxButton.clicked += () => bc.SandboxModeEnabled = true;

		_graphicsSettignsButtonP.clicked += () => TogglePanelVisibility("GraphicsSettings");
		_exportButtonP.clicked += FindFirstObjectByType<ItemManager>().ExportAllKeyframes;
		_importButtonP.clicked += FindFirstObjectByType<ItemManager>().ImportKeyframes;

		#endregion Start UI Elements

		#region Music Uploader Start Panel UI Elements

		VisualElement MusicUploadStart = MusicUploaderStartMenuDoc.rootVisualElement;
		_fileNameLabel = MusicUploadStart.Q<Label>("FileNameLabel");
		_chooseFileButton = MusicUploadStart.Q<Button>("UploadFile");
		_goToSelectStageFromMusicUploader = MusicUploadStart.Q<Button>("GoToSelectStageFromMusic");
		_goBackToStartFromMusicUploader = MusicUploadStart.Q<Button>("GoToStartFromMusic");

		_chooseFileButton.clicked += ImportClicked;
		_goToSelectStageFromMusicUploader.clicked += () => TogglePanelVisibility("StageSelection");
		_goToSelectStageFromMusicUploader.clicked += () =>
		{
			if (!StartClicked)
				StartClicked = true;
		};
		_goBackToStartFromMusicUploader.clicked += () => TogglePanelVisibility("PauseStart");

		#endregion	Music Uploader Start Panel UI Elements

		#region Stage Selection UI Elements

		VisualElement StageSelectionRoot = StageSelectionDoc.rootVisualElement;
		_defaultStageButton = StageSelectionRoot.Q<Button>("DefaultButton");
		_paramountStageButton = StageSelectionRoot.Q<Button>("ParamountButton");
		_ogdenStageButton = StageSelectionRoot.Q<Button>("TheOgdenButton");
		_backButton = StageSelectionRoot.Q<Button>("BackButton");

		_defaultStageButton.clicked += DefaultStageClicked;
		_paramountStageButton.clicked += ParamountStageClicked;
		_ogdenStageButton.clicked += OgdenStageClicked;
		_backButton.clicked += () => TogglePanelVisibility("PauseStart");
		;

		#endregion Stage Selection UI Elements

		#region Music Uploader UI Elements

		VisualElement MusicUploaderRoot = MusicUploaderDoc.rootVisualElement;
		_importButton = MusicUploaderRoot.Q<Button>("UploadButton");
		_songTitleLabel = MusicUploaderRoot.Q<Label>("SongTitle");
		_restartButton = MusicUploaderRoot.Q<Button>("RestartButton");
		_exitMusicUploaderPanelButton = MusicUploaderRoot.Q<Button>("ExitMusicPanelButton");

		_importButton.clicked += ImportClicked;
		_restartButton.clicked += RestartMusicClicked;
		_exitMusicUploaderPanelButton.clicked += () => TogglePanelVisibility("AllOff");

		#endregion Music Uploader UI Elements

		#region Animation settings UI Elements

		VisualElement LightsAnimationRoot = LightsAnimationDoc.rootVisualElement;
		NextLightButton = LightsAnimationRoot.Q<Button>("NextLightButton");
		NextLightButton.clicked += NextLightButtonHit;

		IntensitySlider = LightsAnimationRoot.Q<Slider>("LightIntensitySlider");

		RedSlider = LightsAnimationRoot.Q<Slider>("RedColorSlider");
		GreenSlider = LightsAnimationRoot.Q<Slider>("GreenColorSlider");
		BlueSlider = LightsAnimationRoot.Q<Slider>("BlueColorSlider");

		//_xScaleSlider = LightsAnimationRoot.Q<Slider>("XScaleSlider");
		PulseRateSlider = LightsAnimationRoot.Q<Slider>("PulseRateSlider");
		RotSpeedSlider = LightsAnimationRoot.Q<Slider>("RotSpeedSlider");
		AddKeyframeAnimationButton = LightsAnimationRoot.Q<Button>(
			"AddKeyframeAnimationPanelButton"
		);
		_backButtonAnimation = LightsAnimationRoot.Q<Button>("BackButtonAnimation");
		KeyframeVisualList = LightsAnimationRoot.Q<ScrollView>("KeyframeVisualList");
		AddKeyframeAnimationButton.clicked += AddKeyframeClicked;
		_backButtonAnimation.clicked += () => TogglePanelVisibility("AllOff");

		#endregion Animation settings UI Elements

		#region Graphics settings UI Elements

		VisualElement GraphicsSettingsRoot = GraphicsSettingsDoc.rootVisualElement;
		_bloomToggleButton = GraphicsSettingsRoot.Q<Button>("BloomButton");
		_fogToggleButton = GraphicsSettingsRoot.Q<Button>("FogButton");
		_filmGrainToggleButton = GraphicsSettingsRoot.Q<Button>("FilmGrainButton");
		_motionBlurButton = GraphicsSettingsRoot.Q<Button>("MotionBlurButton");
		_backButtonGraphicsMenu = GraphicsSettingsRoot.Q<Button>("BackButtonGraphics");

		_bloomLabel = GraphicsSettingsRoot.Q<Label>("BloomLabel");
		_fogLabel = GraphicsSettingsRoot.Q<Label>("FogLabel");
		_filmGrainLabel = GraphicsSettingsRoot.Q<Label>("FilmGrainLabel");
		_motionBlurLabel = GraphicsSettingsRoot.Q<Label>("MotionBlurLabel");

		_bloomToggleButton.clicked += BloomButtonClicked;
		_fogToggleButton.clicked += FogButtonClicked;
		_filmGrainToggleButton.clicked += FilmGrainButtonClicked;
		_motionBlurButton.clicked += MotionBlurButtonClicked;
		_backButtonGraphicsMenu.clicked += () => TogglePanelVisibility("PauseStart");

		#endregion Graphics settings UI Elements

		TogglePanelVisibility("PauseStart");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePanelVisibility("AllOff");
		}
		// change start/pause menu to account for sandbox/campaign mode resumption
		if (StartClicked)
		{
			if (!bc.SandboxModeEnabled)
			{
				_playCampaignButton.text = "Resume Campaign";
				_playCampaignButton.clicked -= () => TogglePanelVisibility("MusicUploaderStartMenu");
				_playCampaignButton.clicked += () => TogglePanelVisibility("AllOff");
				_playSandboxButton.style.display = DisplayStyle.None;
			}
			else
			{
				_playSandboxButton.text = "Resume Sandbox";
				_playSandboxButton.clicked -= () => TogglePanelVisibility("MusicUploaderStartMenu");
				_playSandboxButton.clicked += () => TogglePanelVisibility("AllOff");
				_playCampaignButton.style.display = DisplayStyle.None;
			}
		}
		// hide add keyframe button if nothing selected
		if (sm.SelectedObject == null || !HUDVisible)
		{
			_addKeyframeButton.style.visibility = Visibility.Hidden;
			_lightsMenuButton.style.visibility = Visibility.Hidden;
		}
		else if (sm.SelectedObject != null)
		{
			_addKeyframeButton.style.visibility = Visibility.Visible;
			_lightsMenuButton.style.visibility = Visibility.Visible;
		}

		// hide scrubber if no music is playing
		if (AudioSource.clip == null)
		{
			_musicHudRoot.style.visibility = Visibility.Hidden;
			_musicHudRoot.focusable = false;
		}
		// update scrubber slider
		else if (AudioSource.clip != null)
		{
			_musicHudRoot.style.visibility = Visibility.Visible;
			_musicHudRoot.focusable = true;
			if (MusicIsPlaying)
			{
				TimelineSlider.value = AudioSource.time;
			}
		}

		if (bc.SandboxModeEnabled)
		{
			BudgetLabel.style.visibility = Visibility.Hidden;
			BudgetLabel.focusable = false;
			BudgetLabel.style.display = DisplayStyle.None;
		}
		else
		{
			BudgetLabel.text = $"Remaining Budget: ${bc.RemainingBudget}";
		}
	}

	#region Keyframe Methods

	private void AddKeyframeClicked()
	{
		if (sm.SelectedObject != null)
		{
			sm.CurrentLightProperties.AddKeyframe(TimelineSlider.value);
		}
		ShowToastNotification("Keyframe Added", 1f);
		RefreshKeyframeList();
	}

	public void ShowToastNotification(string msg, float duration)
	{
		Label toastLabel = new Label(msg);
		VisualElement HUDroot = HUDDoc.rootVisualElement;
		VisualElement AnimationRoot = LightsAnimationDoc.rootVisualElement;
		toastLabel.style.position = Position.Absolute;
		toastLabel.style.top = 20;
		toastLabel.style.left = 50;
		toastLabel.style.paddingLeft = 10;
		toastLabel.style.paddingRight = 10;
		toastLabel.style.backgroundColor = new Color(0, 0, 0, 0.8f);
		toastLabel.style.color = Color.white;
		toastLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
		toastLabel.style.fontSize = 30;
		toastLabel.style.opacity = 1;
		toastLabel.style.borderBottomLeftRadius = 5;
		toastLabel.style.borderBottomRightRadius = 5;
		toastLabel.style.borderTopLeftRadius = 5;
		toastLabel.style.borderTopRightRadius = 5;
		toastLabel.style.paddingTop = 5;
		toastLabel.style.paddingBottom = 5;

		if (HUDVisible)
		{
			HUDroot.Add(toastLabel);
			StartCoroutine(FadeOutAndRemove(toastLabel, duration, HUDroot));
		}
		else if (AnimationPanelVisible)
		{
			AnimationRoot.Add(toastLabel);
			StartCoroutine(FadeOutAndRemove(toastLabel, duration, AnimationRoot));
		}
	}

	private IEnumerator FadeOutAndRemove(Label toastLabel, float duration, VisualElement root)
	{
		yield return new WaitForSeconds(duration);

		float fadeTime = 0.5f;
		float elapsedTime = 0;
		while (elapsedTime < fadeTime)
		{
			elapsedTime += Time.deltaTime;
			toastLabel.style.opacity = 1 - (elapsedTime / fadeTime);
			yield return null;
		}

		root.Remove(toastLabel);
	}

	public void RefreshKeyframeList()
	{
		KeyframeVisualList.Clear();
		if (sm.CurrentLightProperties != null)
		{
			foreach (var keyframe in sm.CurrentLightProperties.KeyframesOnPrefab)
			{
				// Create a label for each keyframe
				// also show color and selected light index
				var keyframeEntry = new VisualElement();
				keyframeEntry.style.flexDirection = FlexDirection.Row;
				keyframeEntry.AddToClassList("keyframe-entry");

				var keyframeLabel = new Label($"Time: {keyframe.KeyframeTime} | Light #: {keyframe.KeyframeLightIndex} | Position: {keyframe.KeyframePosition} | Intensity: {keyframe.KeyframeIntensity}");
				keyframeLabel.style.color = Color.white;
				keyframeEntry.Add(keyframeLabel);

				var jumpButton = new Button(() => JumpToKeyframe(keyframe.KeyframeTime))
				{
					text = "Jump"
				};
				var deleteButton = new Button(() => DeleteKeyframe(keyframe.KeyframeTime))
				{
					text = "Delete"
				};

				keyframeEntry.Add(jumpButton);
				keyframeEntry.Add(deleteButton);

				KeyframeVisualList.Add(keyframeEntry);
			}
		}
	}

	public void DeleteKeyframe(float time)
	{
		if (sm.SelectedObject != null)
		{
			sm.CurrentLightProperties.DeleteKeyframe(time);
		}
		ShowToastNotification($"Keyframe Deleted at {time}", 1f);
		RefreshKeyframeList();
	}

	public void JumpToKeyframe(float time)
	{
		AudioSource.time = time;
		TimelineSlider.value = time;
		FindFirstObjectByType<ItemManager>().AnimateAllKeyframes();
	}

	#endregion Keyframe Methods

	// todo: clean this shit up
	public void TogglePanelVisibility(string panelName)
	{
		if (MusicIsPlaying & panelName != "ItemsPanel")
		{
			PlayPauseMusicClicked();
		}
		switch (panelName)
		{
			case "PauseStart":
				StartVisible = true;
				Time.timeScale = 0f;

				HUDVisible = false;
				ItemsPanelVisible = false;
				StageSelectionVisible = false;
				MusicUploaderVisible = false;
				AnimationPanelVisible = false;
				GraphicsSettingsVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				StartMenuDoc.rootVisualElement.style.visibility = Visibility.Visible;
				StartMenuDoc.rootVisualElement.focusable = true;
				MusicUploaderStartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderStartMenuDoc.rootVisualElement.focusable = false;
				MusicUploaderDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderDoc.rootVisualElement.focusable = false;
				StageSelectionDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StageSelectionDoc.rootVisualElement.focusable = false;
				MusicUploaderDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderDoc.rootVisualElement.focusable = false;
				LightsAnimationDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				LightsAnimationDoc.rootVisualElement.focusable = false;
				GraphicsSettingsDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				GraphicsSettingsDoc.rootVisualElement.focusable = false;

				break;

			case "MusicUploaderStartMenu":
				MusicUploaderStartVisible = true;
				Time.timeScale = 0f;

				HUDVisible = false;
				ItemsPanelVisible = false;
				StartVisible = false;
				StageSelectionVisible = false;
				AnimationPanelVisible = false;
				GraphicsSettingsVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				StartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StartMenuDoc.rootVisualElement.focusable = false;
				StageSelectionDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderStartMenuDoc.rootVisualElement.style.visibility = Visibility.Visible;
				MusicUploaderStartMenuDoc.rootVisualElement.focusable = true;
				MusicUploaderDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderDoc.rootVisualElement.focusable = false;
				StageSelectionDoc.rootVisualElement.focusable = false;
				LightsAnimationDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				LightsAnimationDoc.rootVisualElement.focusable = false;
				GraphicsSettingsDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				GraphicsSettingsDoc.rootVisualElement.focusable = false;
				break;

			case "ItemsPanel":
				ItemsPanelVisible = true;

				StartVisible = false;
				MusicUploaderVisible = false;
				HUDVisible = false;
				StageSelectionVisible = false;
				MusicUploaderVisible = false;
				AnimationPanelVisible = false;
				GraphicsSettingsVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Visible;
				ItemsPanelDoc.rootVisualElement.focusable = true;
				StartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StartMenuDoc.rootVisualElement.focusable = false;
				MusicUploaderStartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderStartMenuDoc.rootVisualElement.focusable = false;
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
				StartVisible = false;
				MusicUploaderVisible = false;
				MusicUploaderVisible = false;
				AnimationPanelVisible = false;
				GraphicsSettingsVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				StartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StartMenuDoc.rootVisualElement.focusable = false;
				MusicUploaderStartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderStartMenuDoc.rootVisualElement.focusable = false;
				MusicUploaderStartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderStartMenuDoc.rootVisualElement.focusable = false;
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
				StartVisible = false;
				MusicUploaderVisible = false;
				StageSelectionVisible = false;
				AnimationPanelVisible = false;
				GraphicsSettingsVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				StartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StartMenuDoc.rootVisualElement.focusable = false;
				MusicUploaderDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderDoc.rootVisualElement.focusable = false;
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
				AnimationPanelVisible = true;

				HUDVisible = false;
				ItemsPanelVisible = false;
				StartVisible = false;
				MusicUploaderVisible = false;
				StageSelectionVisible = false;
				MusicUploaderVisible = false;
				GraphicsSettingsVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				StartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StartMenuDoc.rootVisualElement.focusable = false;
				MusicUploaderStartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderStartMenuDoc.rootVisualElement.focusable = false;
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
				StartVisible = false;
				MusicUploaderVisible = false;
				StageSelectionVisible = false;
				MusicUploaderVisible = false;
				AnimationPanelVisible = false;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				HUDDoc.rootVisualElement.focusable = false;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				StartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StartMenuDoc.rootVisualElement.focusable = false;
				MusicUploaderStartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderStartMenuDoc.rootVisualElement.focusable = false;
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
				StartVisible = false;
				MusicUploaderVisible = false;
				StageSelectionVisible = false;
				MusicUploaderVisible = false;
				AnimationPanelVisible = false;
				GraphicsSettingsVisible = false;
				Time.timeScale = 1f;

				HUDDoc.rootVisualElement.style.visibility = Visibility.Visible;
				HUDDoc.rootVisualElement.focusable = true;
				ItemsPanelDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				ItemsPanelDoc.rootVisualElement.focusable = false;
				StartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				StartMenuDoc.rootVisualElement.focusable = false;
				MusicUploaderStartMenuDoc.rootVisualElement.style.visibility = Visibility.Hidden;
				MusicUploaderStartMenuDoc.rootVisualElement.focusable = false;
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
		if (sm.SelectedObject != null | sm.CurrentLightProperties.LightsOnPrefab.Length == 1)
			return;
		if (
			sm.CurrentLightProperties.CurrentLightIndex
			>= sm.CurrentLightProperties.LightsOnPrefab.Length - 1
		)
		{
			sm.CurrentLightProperties.CurrentLightIndex = 0;
		}
		else
		{
			sm.CurrentLightProperties.CurrentLightIndex++;
		}
		sm.CurrentLightProperties.SelectedLight = sm.CurrentLightProperties.LightsOnPrefab[
			sm.CurrentLightProperties.CurrentLightIndex
		];
		sm.CurrentLightProperties.UpdateSliderValues();
	}

	#region Items Methods

	private void ItemSelectedFromPanel(string name)
	{
		Debug.Log(name + " was pressed");
		FindFirstObjectByType<ItemManager>().SpawnItem(name);
		TogglePanelVisibility("AllOff");
	}

	#endregion Items Methods

	#region StageSelection Methods

	private void DefaultStageClicked()
	{
		Time.timeScale = 1f;
		TogglePanelVisibility("AllOff");
		FindFirstObjectByType<StageManager>().SwitchStage("AllOff");
		FindFirstObjectByType<StageManager>().SwitchStage("Default");
	}

	private void ParamountStageClicked()
	{
		Time.timeScale = 1f;
		TogglePanelVisibility("AllOff");
		FindFirstObjectByType<StageManager>().SwitchStage("AllOff");
		FindFirstObjectByType<StageManager>().SwitchStage("Paramount");
	}

	private void OgdenStageClicked()
	{
		Time.timeScale = 1f;
		TogglePanelVisibility("AllOff");
		FindFirstObjectByType<StageManager>().SwitchStage("AllOff");
		FindFirstObjectByType<StageManager>().SwitchStage("Ogden");
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
					_fileNameLabel.text = "Selected: " + Path.GetFileName(path);
					TimelineSlider.highValue = clip.length;
					_musicDurationLabel.text = clip.length.ToString();
				}
				else
				{
					_songTitleLabel.text = "Failed to load audio file!";
					_fileNameLabel.text = "Failed to load audio file!";
				}
			}
			else
			{
				_songTitleLabel.text = "Error: " + www.error;
				_fileNameLabel.text = "Error: " + www.error;
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
			TimelineSlider.value = 0;
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