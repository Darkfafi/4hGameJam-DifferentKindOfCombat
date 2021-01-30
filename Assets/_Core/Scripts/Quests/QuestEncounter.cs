using System;

public class QuestEncounter
{
	public delegate void ActionHandler(QuestEncounter encounter, ActionType actionType);
	public event ActionHandler ActionPerformedEvent;

	public readonly string Title;
	public readonly string Description;
	public readonly string LeftText;
	public readonly string RightText;

	private Action<QuestEncounter> _leftAction;
	private Action<QuestEncounter> _rightAction;

	public Quest Quest
	{
		get; private set;
	}

	public QuestEncounter(string title, string description, string leftText, Action<QuestEncounter> leftAction, string rightText, Action<QuestEncounter> rightAction)
	{
		Title = title;
		Description = description;
		LeftText = leftText;
		RightText = rightText;

		_leftAction = leftAction;
		_rightAction = rightAction;
	}

	public void Open(Quest parent)
	{
		Quest = parent;
	}

	public void Close()
	{
		Quest = null;
	}

	public void DoAction(ActionType actionType)
	{
		switch(actionType)
		{
			case ActionType.Left:
				_leftAction?.Invoke(this);
				break;
			case ActionType.Right:
				_rightAction?.Invoke(this);
				break;
		}
		ActionPerformedEvent?.Invoke(this, actionType);
	}

	public QuestEncounter[] Duplicate(int amount = 1)
	{
		QuestEncounter[] encounters = new QuestEncounter[amount];
		for(int i = 0; i < amount; i++)
		{
			encounters[i] = new QuestEncounter(Title, Description, LeftText, _leftAction, RightText, _rightAction);
		}
		return encounters;
	}

	public enum ActionType
	{
		Left,
		Right
	}
}
