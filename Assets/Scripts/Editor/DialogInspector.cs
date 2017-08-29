using UnityEngine;

[System.Serializable]
public class DialogStateLink
{
	public Chain chain;
	public State state;
    public Rect position;

    public DialogStateLink(Chain chain)
    {
        this.chain = chain;
        this.state = chain.StartState;
        position = new Rect(0, 0, 100, 30);
    }

}