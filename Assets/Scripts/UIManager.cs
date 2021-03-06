﻿using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject choices;

    private Animator animator;

    public delegate void EventChoice(string choice);
    public static event EventChoice OnChoice;

    void Start () {
        animator = GetComponent<Animator>();

        //EVENTS
        GameManager.OnState += _OnState;
	}

    void OnDestroy()
    {
        GameManager.OnState -= _OnState;
    }

    public void makeChoice(string choice)
    {
        if (OnChoice != null)
            OnChoice(choice);
        choices.SetActive(false);
    }

    public void setButton(int nb, string s)
    {
        //Debug.Log(s);
        Button b = GameObject.Find("Button " + nb).GetComponent<Button>();
        b.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("Icons/" + s);
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(() => makeChoice(s));
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
                    break;
                }
            case EState.LOOSE:
                {
                    choices.SetActive(false);
                    animator.SetTrigger("Loose");
                    break;
                }
        }
    }
}