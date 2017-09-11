using System;
using System.Collections.Generic;
using UnityEngine;

public class GuidManager
{
	private static List<PathGame> games;
	private static List<PathGame> Games
	{
		get
		{
			if(games == null)
			{
				games = new List<PathGame> ();

				foreach(PathGame pg in UnityEngine.Object.FindObjectsOfTypeIncludingAssets(typeof(PathGame)))
				{
					games.Add (pg);
				}
			}
			return games;
		}
	}


	public static int GetItemGUID()
	{
		int r = UnityEngine.Random.Range(0,Int32.MaxValue);


		foreach (PathGame inspectedgame in Games) {
				foreach (Param p in inspectedgame.parameters) {
					if (p.paramGUID == r) {
						return GetItemGUID ();
					}
				}
		}
		return r;
	}

	public static Param GetItemByGuid(int aimParamGuid)
	{
		foreach (PathGame inspectedgame in Games) {
			foreach (Param p in inspectedgame.parameters) {
				if (p.paramGUID == aimParamGuid) {
					return p;
				}
			}
		}
		return null;
	}
}