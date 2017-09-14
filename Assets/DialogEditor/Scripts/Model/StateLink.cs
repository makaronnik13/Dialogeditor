using UnityEngine;

[System.Serializable]
public class StateLink: ScriptableObject
{
    public Chain chain;
	public State state;
    public Rect position;

    public void Init(Chain chain)
    {
        this.chain= chain;
        this.state = chain.StartState;
        float z = GuidManager.getGameByChain(chain).zoom;
        position = new Rect(0, 0, 100*z, 30*z);
    }
}