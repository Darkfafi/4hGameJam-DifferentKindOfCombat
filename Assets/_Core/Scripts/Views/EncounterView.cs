using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EncounterView : MonoBehaviour
{
	[SerializeField]
	private Text _titleLabel;

	[SerializeField]
	private Text _descriptionLabel;

	[SerializeField]
	private Text _leftButtonText;

	[SerializeField]
	private Text _rightButtonText;

	public QuestEncounter EncounterDisplaying
	{
		get; private set;
	}

	public void SetEncounter(QuestEncounter encounter)
	{
		CompleteTweeners();
		EncounterDisplaying = encounter;
		_titleLabel.DOText(EncounterDisplaying?.Title ?? "Home, sweet... ow god", 0.5f);
		_descriptionLabel.DOText(EncounterDisplaying?.Description ?? "Home is filthy, what do you do?", 1f);
		_leftButtonText.DOText(encounter?.LeftText ?? "Dishes", 0.25f);
		_rightButtonText.DOText(encounter?.RightText ?? "Dishes", 0.25f);
	}

	protected void OnDestroy()
	{
		CompleteTweeners();
	}

	public void OnLeftPressed()
	{
		EncounterDisplaying?.DoAction(QuestEncounter.ActionType.Left);
	}

	public void OnRightPressed()
	{
		EncounterDisplaying?.DoAction(QuestEncounter.ActionType.Right);
	}

	private void CompleteTweeners()
	{
		_titleLabel.DOComplete();
		_descriptionLabel.DOComplete();
		_leftButtonText.DOComplete();
		_rightButtonText.DOComplete();
	}
}