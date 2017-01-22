using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public enum EState
{
    START,
    CHOOSE,
    DOWNING,
    TRANSFER,
    ANSWER,
    MISS,
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

    public string nextLevel;
    public int pointsToWin = 5;
    public int pointsToLoose = -5;
    public int bonnesReponses = 3;
    public float transfertTimer = 1f;
    public float slowMotionCap = 0.2f;
    public float slowMotionSpeed = 1f;
    public float val = 0.3f;
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
    private string[] _choices;
    private UIManager _UIManager;
    private List<string> _icons;
    private AudioSource _sound;
    private AudioSource _swing;
    private SpriteRenderer _fenetre;
    private float _turn = 0;

    private bool _lastContent = false;
    private float _timer;

    public delegate void EventState(EState state);
    public static event EventState OnState;
    public delegate void EventShake();
    public static event EventShake OnShake;

    void Start () {
        _start = GameObject.FindGameObjectWithTag("BouleStart");
        _startAnimator = _start.GetComponent<Animator>();
        _end = GameObject.FindGameObjectWithTag("BouleEnd");
        _endAnimator = _end.GetComponent<Animator>();
        _UIManager = GameObject.Find("UI").GetComponent<UIManager>();
        _sound = GetComponents<AudioSource>()[0];
        _swing = GetComponents<AudioSource>()[1];
        _choices = new string[boules.Length];
        _fenetre = GameObject.Find("Fenetre").GetComponent<SpriteRenderer>();

        _icons = new List<string>();
        foreach(Reaction r in bouleEnd.reactions)
        {
            _icons.Add(r.action);
        }

        foreach(Boule b in boules)
        {
            b.objet.GetComponentsInChildren<SpriteRenderer>()[2].sprite = b.chara.sprite;
        }
        _end.GetComponentsInChildren<SpriteRenderer>()[2].sprite = bouleEnd.sprite;

        _setNewTexts(bonnesReponses);

        _changeState(EState.START);

        //EVENTS
        UIManager.OnChoice += OnChoice;
        BouleBase.OnChange += _OnStartChange;
    }

    void OnDestroy()
    {
        //EVENTS
        UIManager.OnChoice -= OnChoice;
        BouleBase.OnChange -= _OnStartChange;
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
                    if(Time.timeScale >= slowMotionCap)
                    {
                        Time.timeScale -= slowMotionSpeed * Time.deltaTime;
                        if (Time.timeScale < slowMotionCap)
                            Time.timeScale = slowMotionCap;
                    }

                    if(_startAnimator.GetCurrentAnimatorStateInfo(0).IsName("Bouncing"))
                    {
                        Time.timeScale = 1f;
                        _timer = transfertTimer;
                        foreach(Boule b in boules)
                        {
                            b.objet.GetComponentsInChildren<SpriteRenderer>()[4].sprite = Resources.Load<Sprite>("Icons/Silence");
                        }
                        _score--;
                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[1].sprite = Resources.Load<Sprite>("Boules/neutral");
                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[4].enabled = true;
                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[5].enabled = true;

                        _changeState(EState.MISS);
                    }

                    break;
                }
            case EState.DOWNING:
                {
                    if(_startAnimator.GetCurrentAnimatorStateInfo(0).IsName("Bouncing"))
                    {
                        _start.GetComponentsInChildren<SpriteRenderer>()[3].enabled = false;
                        _start.GetComponentsInChildren<SpriteRenderer>()[4].enabled = false;
                        _timer = transfertTimer;

                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[4].enabled = true;
                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[5].enabled = true;

                        boules[_currentBoule].objet.GetComponent<Animator>().SetTrigger("ShakeR");

                        _setMiddleFace();

                        _playIconSound(_choices[_currentBoule]);

                        if (OnShake != null)
                            OnShake();

                        _changeState(EState.TRANSFER);
                    }
                    break;
                }
            case EState.TRANSFER:
                {
                    _timer -= Time.deltaTime;
                    if(_timer <= 0)
                    {
                        if (OnShake != null)
                            OnShake();
                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[1].sprite = Resources.Load<Sprite>("Boules/face_neutral_boy");
                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[4].enabled = false;
                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[5].enabled = false;
                        _currentBoule++;
                        if (_currentBoule >= boules.Length)
                        {
                            _endAnimator.SetTrigger("Starting");

                            if(_lastContent)
                            {
                                _end.GetComponentsInChildren<SpriteRenderer>()[1].sprite = Resources.Load<Sprite>("Boules/face_happy03");
                            }
                            else
                            {
                                _end.GetComponentsInChildren<SpriteRenderer>()[1].sprite = Resources.Load<Sprite>("Boules/face_sad03_boy");
                            }

                            _changeState(EState.ANSWER);
                        }
                        else
                        {
                            _timer = transfertTimer;
                            boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[4].enabled = true;
                            boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[5].enabled = true;
                            boules[_currentBoule].objet.GetComponent<Animator>().SetTrigger("ShakeR");
                            _setMiddleFace();
                            _playIconSound(_choices[_currentBoule]);
                        }
                    }
                    break;
                }
            case EState.ANSWER:
                {
                    if (_endAnimator.GetCurrentAnimatorStateInfo(0).IsName("Bouncing"))
                    {
                        if (OnShake != null)
                            OnShake();
                        _end.GetComponentsInChildren<SpriteRenderer>()[1].sprite = Resources.Load<Sprite>("Boules/face_neutral_boy");

                        string skin = "face_neutral_boy";
                        if (_score >= 3)
                        {
                            skin = "face_happy03";
                        }
                        else if(_score == 2)
                        {
                            skin = "face_happy02_boy";
                        }
                        else if (_score == 1)
                        {
                            skin = "face_happy01_boy";
                        }
                        else if (_score == -1)
                        {
                            skin = "face_sad01_boy";
                        }
                        else if (_score == -2)
                        {
                            skin = "face_sad02_boy";
                        }
                        else if (_score <= -3)
                        {
                            skin = "face_sad03_boy";
                        }
                        _end.GetComponentsInChildren<SpriteRenderer>()[1].sprite = Resources.Load<Sprite>("Boules/"+skin);

                        if (_score >= pointsToWin)
                        {
                            _changeState(EState.WIN);
                        }
                        else if (_score <= pointsToLoose || _turn >= 10)
                        {
                            _changeState(EState.LOOSE);
                        }
                        else
                        {
                            _startAnimator.SetTrigger("Uping");
                            _currentBoule = 0;
                            _changeState(EState.CHOOSE);
                            _setNewTexts(bonnesReponses);
                            boules[boules.Length-1].objet.GetComponent<Animator>().SetTrigger("ShakeL");
                        }
                    }
                    break;
                }
            case EState.MISS:
                {
                    _timer -= Time.deltaTime;
                    if (_timer <= 0)
                    {
                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[1].sprite = Resources.Load<Sprite>("Boules/face_happy01_boy");
                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[4].enabled = false;
                        boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[5].enabled = false;
                        _currentBoule++;
                        if (_currentBoule >= boules.Length)
                        {
                            _endAnimator.SetTrigger("Starting");
                            _end.GetComponentsInChildren<SpriteRenderer>()[1].sprite = Resources.Load<Sprite>("Boules/neutral");
                            _changeState(EState.ANSWER);
                        }
                        else
                        {
                            _timer = transfertTimer;
                            boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[1].sprite = Resources.Load<Sprite>("Boules/neutral");
                            boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[4].enabled = true;
                            boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[5].enabled = true;
                        }
                    }
                    break;
                }
            case EState.WIN:
                {
                    SceneManager.LoadScene(nextLevel);
                    break;
                }
            case EState.LOOSE:
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    break;
                }
        }
	}

    void OnChoice(string choice)
    {
        if(state ==  EState.CHOOSE)
        {
            Time.timeScale = 1f;

            _start.GetComponentsInChildren<SpriteRenderer>()[3].sprite = Resources.Load<Sprite>("Icons/" + choice);
            _start.GetComponentsInChildren<SpriteRenderer>()[3].enabled = true;
            _start.GetComponentsInChildren<SpriteRenderer>()[4].enabled = true;

            _playIconSound(choice, true);

            _changeState(EState.DOWNING);
            _startAnimator.SetTrigger("Starting");
            for(int i  = 0; i < boules.Length; i++)
            {
                _choices[i] = choice = boules[i].chara.getReaction(choice);
                boules[i].objet.GetComponentsInChildren<SpriteRenderer>()[4].sprite = Resources.Load<Sprite>("Icons/" + choice);
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

            _turn++;
            _fenetre.sprite = Resources.Load<Sprite>("Fenetre/" + _turn);
        }
    }

    private void _setNewTexts(int n)
    {
        int b = 1;
        List<string> list = _getChain();
        List<string> remaining = _icons.Except(list).ToList();

        System.Random rnd = new System.Random();

        for(int i = 0; i < n; i++)
        {
            int rdm = rnd.Next(list.Count);
            _UIManager.setButton(b, list[rdm]);
            list.RemoveAt(rdm);
            b++;
        }
        for(int i = b; i <= 4; i++)
        {
            int rdm = rnd.Next(remaining.Count);
            _UIManager.setButton(b, remaining[rdm]);
            remaining.RemoveAt(rdm);
        }
    }

    private List<string> _getChain()
    {
        List<string> actions = new List<string>();
        foreach(Reaction r in bouleEnd.reactions)
        {
            if(r.content)
            {
                actions.AddRange(_getPrevious(r.action, boules.Length-1));
            }
        }

        return actions.Distinct().ToList();
    }

    private List<string> _getPrevious(string action, int i)
    {
        List<string> actions = new List<string>();
        foreach (Reaction r in boules[i].chara.reactions)
        {
            if(r.reaction == action)
            {
                if(i == 0)
                {
                    actions.Add(r.action);
                }
                else
                {
                    actions.AddRange(_getPrevious(r.action, i - 1));
                }
            }
        }
        return actions;
    }

    private void _playIconSound(string c, bool left = false, bool right = false)
    {
        _sound.clip = Resources.Load<AudioClip>("SonsIcons/I_" + c);

        if(left)
        {
            _sound.panStereo = -val;
        }
        else if(right)
        {
            _sound.panStereo = val;
        }
        else
        {
            _sound.panStereo = 0f;
        }

        _sound.Play();
    }

    private void _playSwingSound(bool up, bool slow, bool start)
    {
        string s = "";
        if(up)
        {
            s += "Up_";
        }
        else
        {
            s += "Down_";
        }

        if(slow)
        {
            s += "Slow";
        }
        else
        {
            s += "Fast";
        }

        if (start)
        {
            _swing.panStereo = -val;
        }
        else
        {
            _swing.panStereo = val;
        }

        _swing.clip = Resources.Load<AudioClip>("SonsUpDown/" + s);
        _swing.Play();
    }

    private void _OnStartChange(bool up, bool start)
    {
        _playSwingSound(up, Time.timeScale == 1 ? false : true, start);
    }

    private void _changeState(EState newState)
    {
        state = newState;
        Debug.Log(state);
        if (OnState != null)
            OnState(state);
    }

    private void _setMiddleFace()
    {
        if (boules[_currentBoule].chara.getContent(_choices[_currentBoule]))
        {
            boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[1].sprite = Resources.Load<Sprite>("Boules/face_happy01_boy");
        }
        else
        {
            boules[_currentBoule].objet.GetComponentsInChildren<SpriteRenderer>()[1].sprite = Resources.Load<Sprite>("Boules/face_sad01_boy");
        }
    }
}
