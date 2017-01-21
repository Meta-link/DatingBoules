using UnityEngine;
using System;

public enum EState
{
    START,
    CHOOSE,
    DOWNING,
    TRANSFER,
    ANSWER,
    WIN,
    LOOSE
}

[Serializable]
public struct Boule
{
    public GameObject objet;
    public BouleProperties chara;
}

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public static EState state;

    public int pointsToWin = 5;
    public int pointsToLoose = -5;
    public float transfertTimer = 1f;
    [Space(10)]
    public Boule[] boules;
    [Space(15)]
    public BouleProperties bouleEnd;

    private int _score = 0;
    private GameObject _start;
    private Animator _startAnimator;
    private GameObject _end;
    private Animator _endAnimator;
    private int _currentBoule = 0;

    private bool _lastContent = false;
    private float _timer;

    public delegate void EventState(EState state);
    public static event EventState OnState;

    void Start () {
        _start = GameObject.FindGameObjectWithTag("BouleStart");
        _startAnimator = _start.GetComponent<Animator>();
        _end = GameObject.FindGameObjectWithTag("BouleEnd");
        _endAnimator = _end.GetComponent<Animator>();

        foreach (Boule g in boules)
        {
            g.objet.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
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
                        _timer = transfertTimer;
                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[1].enabled = true;
                        _changeState(EState.TRANSFER);
                    }
                    break;
                }
            case EState.TRANSFER:
                {
                    _timer -= Time.deltaTime;
                    if(_timer <= 0)
                    {
                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
                        _currentBoule++;
                        if (_currentBoule >= boules.Length)
                        {
                            foreach(Boule b in boules)
                            {
                                b.objet.GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
                            }

                            _endAnimator.SetTrigger("Starting");

                            if(_lastContent)
                            {
                                _end.GetComponentsInChildren<SpriteRenderer>()[0].sprite = Resources.Load<Sprite>("Boules/BouleContente");
                            }
                            else
                            {
                                _end.GetComponentsInChildren<SpriteRenderer>()[0].sprite = Resources.Load<Sprite>("Boules/BoulePasContente");
                            }

                            _changeState(EState.ANSWER);
                        }
                        else
                        {
                            _timer = transfertTimer;
                            boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[1].enabled = true;
                        }
                    }
                    break;
                }
            case EState.ANSWER:
                {
                    if (_endAnimator.GetCurrentAnimatorStateInfo(0).IsName("Bouncing"))
                    {
                        _end.GetComponentsInChildren<SpriteRenderer>()[0].sprite = Resources.Load<Sprite>("Boules/BouleNeutral");
                        if (_score >= pointsToWin)
                        {
                            _changeState(EState.WIN);
                        }
                        else if (_score <= pointsToLoose)
                        {
                            _changeState(EState.LOOSE);
                        }
                        else
                        {
                            _startAnimator.SetTrigger("Uping");
                            _currentBoule = 0;
                            _changeState(EState.CHOOSE);
                        }
                    }
                    break;
                }
            case EState.WIN:
                {
                    break;
                }
            case EState.LOOSE:
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
            foreach(Boule b in boules)
            {
                choice = b.chara.getReaction(choice);
                b.objet.GetComponentsInChildren<SpriteRenderer>()[1].sprite = Resources.Load<Sprite>("Icons/" + choice);
            }

            if(bouleEnd.getContent(choice))
            {
                _score++;
                _lastContent = true;
            }
            else
            {
                _score--;
                _lastContent = false;
            }
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
