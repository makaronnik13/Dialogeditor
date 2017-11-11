using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathGame : MonoBehaviour {

	public PathGame testGame;

	[ContextMenu("play game")]
	public void TestGame()
	{
		QuestBookLibrary.Instance.HideLibrary();
		GameCanvasController.Instance.PlayBook(testGame);
	}
}
