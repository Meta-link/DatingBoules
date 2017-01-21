using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

    public Button[] _choices;

	void Start () {

        //EVENTS
        GameManager.OnState += _OnState;
    }

    void OnDestroy()
    {
        GameManager.OnState -= _OnState;
    }

    void Update () {
        switch (GameManager.state)
        {
            case EState.CHOOSE:
                {
                    if(Input.GetButtonDown("A"))
                    {
                        _choices[0].onClick.Invoke();
                    }
                    else if (Input.GetButtonDown("B"))
                    {
                        _choices[1].onClick.Invoke();
                    }
                    else if (Input.GetButtonDown("X"))
                    {
                        _choices[2].onClick.Invoke();
                    }
                    else if (Input.GetButtonDown("Y"))
                    {
                        _choices[3].onClick.Invoke();
                    }
                    break;
                }
        }
    }

    private void _OnState(EState state)
    {
        switch(state)
        {
            case EState.CHOOSE:
                {
                    break;
                }
        }
    }
}
