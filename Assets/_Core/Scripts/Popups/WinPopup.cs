using System;
using UnityEngine;

public class WinPopup : MonoBehaviour
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
