using UnityEngine;

public enum EState
{
    START,
    CHOOSE,
    DOWNING,
    TRANSFER,
    ANSWER,
    END
}

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public static EState state;

    public float transfertTimer = 1f;
    public GameObject[] _boules;

    private GameObject _start;
    private Animator _startAnimator;
    private GameObject _end;
    private Animator _endAnimator;
    private int _currentBoule = 0;

    private float timer;

    public delegate void EventState(EState state);
    public static event EventState OnState;

    void Start () {
        _start = GameObject.FindGameObjectWithTag("BouleStart");
        _startAnimator = _start.GetComponent<Animator>();
        _end = GameObject.FindGameObjectWithTag("BouleEnd");
        _endAnimator = _end.GetComponent<Animator>();

        foreach (GameObject g in _boules)
        {
            g.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
        }

        _changeState(EState.START);

        //EVENTS
        UIManager.OnChoice += OnChoice;
    }

    void OnDestroy()
    {
        //EVENTS
        UIManager.OnChoice -= OnChoice;
    }
	
	void Update () {
		switch(state)
        {
            case EState.START:
                {
                    if(Input.anyKeyDown)
                    {
                        _changeState(EState.CHOOSE);
                    }
                    break;
                }
            case EState.CHOOSE:
                {
                    break;
                }
            case EState.DOWNING:
                {
                    if(_startAnimator.GetCurrentAnimatorStateInfo(0).IsName("Bouncing"))
                    {
                        timer = transfertTimer;
                        _boules[_currentBoule].GetComponentsInChildren<SpriteRenderer>()[1].enabled = true;
                        _changeState(EState.TRANSFER);
                    }
                    break;
                }
            case EState.TRANSFER:
                {
                    timer -= Time.deltaTime;
                    if(timer <= 0)
                    {
                        _boules[_currentBoule].GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
                        _currentBoule++;
                        if (_currentBoule >= _boules.Length)
                        {
                            foreach(GameObject b in _boules)
                            {
                                b.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
                            }
                            _endAnimator.SetTrigger("Starting");
                            _changeState(EState.ANSWER);
                        }
                        else
                        {
                            timer = transfertTimer;
                            _boules[_currentBoule].GetComponentsInChildren<SpriteRenderer>()[1].enabled = true;
                        }
                    }
                    break;
                }
            case EState.ANSWER:
                {
                    if (_endAnimator.GetCurrentAnimatorStateInfo(0).IsName("Bouncing"))
                    {
                        _startAnimator.SetTrigger("Uping");
                        _currentBoule = 0;
                        _changeState(EState.CHOOSE);
                    }
                    break;
                }
            case EState.END:
                {
                    break;
                }
        }
	}

    void OnChoice(string choice)
    {
        if(state ==  EState.CHOOSE)
        {
            _changeState(EState.DOWNING);
            _startAnimator.SetTrigger("Starting");
        }
    }

    private void _changeState(EState newState)
    {
        state = newState;
        Debug.Log(state);
        if (OnState != null)
            OnState(state);
    }
}
