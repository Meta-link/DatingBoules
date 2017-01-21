using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject choices;

    public delegate void EventChoice(string choice);
    public static event EventChoice OnChoice;

    void Start () {

        //EVENTS
        GameManager.OnState += _OnState;
	}

    void OnDestroy()
    {
        GameManager.OnState -= _OnState;
    }
	
	void Update () {
		
	}

    public void makeChoice(string choice)
    {
        if (OnChoice != null)
            OnChoice(choice);
        choices.SetActive(false);
    }

    private void _OnState(EState state)
    {
        switch (state)
        {
            case EState.CHOOSE:
                {
                    choices.SetActive(true);
                    break;
                }
        }
    }
}