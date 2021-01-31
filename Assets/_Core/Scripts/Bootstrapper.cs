using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
	[SerializeField]
	private QuestView _questView;

	[SerializeField]
	private StatsView _statsView;

	[Header("Popups")]
	[SerializeField]
	private WinPopup _winPopup;

	[SerializeField]
	private LosePopup _losePopup;

	public GameStats GameStats
	{
		get; private set;
	}

	public Quest CurrentQuest
	{
		get; private set;
	}

	public GameState State
	{
		get; private set;
	} = GameState.None;

	private QuestsConfig _questsConfig;

	private List<Quest> _quests = new List<Quest>();

	protected void Awake()
	{
		GameStats = new GameStats(20, 15, 20, 30);
		_questsConfig = new QuestsConfig();

		GameStats.StatChangedEvent += OnStatChangedEvent;
		_statsView.SetGameStats(GameStats);

		SetGameState(GameState.Setup);
	}

	protected void Start()
	{
		SetGameState(GameState.Gameplay);
	}

	protected void OnDestroy()
	{
		GameStats.StatChangedEvent -= OnStatChangedEvent;
	}

	private void StartQuest(Quest quest)
	{
		if(CurrentQuest != null)
		{
			CurrentQuest.QuestCompletedEvent -= OnQuestCompletedEvent;
			CurrentQuest.DeInit();
		}

		CurrentQuest = quest;

		_questView.SetQuest(CurrentQuest);

		if(CurrentQuest != null)
		{
			_quests.Remove(quest);
			CurrentQuest.QuestCompletedEvent += OnQuestCompletedEvent;
			CurrentQuest.Init(GameStats, out _);
		}
	}

	private void OnQuestCompletedEvent()
	{
		if(State == GameState.Gameplay)
		{
			if(_quests.Count > 0)
			{
				// Go to next quest
				StartQuest(_quests[0].ShuffleEncounters());
			}
			else
			{
				StartQuest(null);
				_winPopup.Show(() =>
				{
					SetGameState(GameState.Ended);
					SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
				});
			}
		}
	}

	private void ShuffleQuests()
	{
		_quests.Shuffle();
	}

	private void OnStatChangedEvent(int currentValue, int previousValue, GameStats.Stat stat)
	{
		if(State == GameState.Gameplay)
		{
			if(currentValue <= 0 || currentValue >= GameStats.MaxStatValue)
			{
				_losePopup.Show(() =>
				{
					SetGameState(GameState.Ended);
					SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
				});
			}
		}
	}

	private void SetGameState(GameState gameState)
	{
		if(State != gameState)
		{
			State = gameState;
			switch(State)
			{
				case GameState.Setup:
					_quests = new List<Quest>(_questsConfig.GetAllQuests());
					ShuffleQuests();
					break;
				case GameState.Gameplay:
					StartQuest(QuestsConfig.IntroQuest.CreateQuest());
					break;
			}
		}
	}

	public enum GameState
	{
		None,
		Setup,
		Gameplay,
		Ended
	}
}