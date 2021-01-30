using System.Collections.Generic;
using UnityEngine;

public static class ListUtils
{
	public static void Shuffle<T>(this List<T> myList)
	{
		int n = myList.Count;
		while(n > 1)
		{
			int k = Random.Range(0, n--);
			T kQuest = myList[k];
			myList[k] = myList[n];
			myList[n] = kQuest;
		}
	}
}
