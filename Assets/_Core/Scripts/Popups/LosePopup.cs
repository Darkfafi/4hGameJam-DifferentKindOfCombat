using System;
using UnityEngine;

public class LosePopup : MonoBehaviour
{
	private Action _playAgainAction;

	public void Show(Action playAgainAction)
	{
		_playAgainAction = playAgainAction;
		gameObject.SetActive(true);
	}

	public void OnPressedPlayAgain()
	{
		_playAgainAction.Invoke();
	}
}