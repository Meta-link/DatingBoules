using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject choices;
    public GameObject win;
    public GameObject loose;

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
            case EState.WIN:
                {
                    choices.SetActive(false);
                    win.SetActive(true);
                    break;
                }
            case EState.LOOSE:
                {
                    choices.SetActive(false);
                    loose.SetActive(true);
                    break;
                }
        }
    }
}