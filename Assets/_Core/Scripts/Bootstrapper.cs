using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
	public GameStats GameStats
	{
		get; private set;
	}

	protected void Awake()
	{
		GameStats = new GameStats();
	}
}
