using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    private Animator _animator;
    private bool credits = false;
    private AudioSource _start, _back;

	void Start () {
        _animator = GetComponent<Animator>();
        _start = GetComponents<AudioSource>()[0];
        _back = GetComponents<AudioSource>()[1];
    }
	
	void Update () {
		if(credits && Input.anyKeyDown)
        {
            _back.Play();
            _animator.SetTrigger("Hide");
            credits = false;
        }
	}

    public void startGame()
    {
        _start.Play();
        SceneManager.LoadScene("Niveau 1");
    }

    public void enterCredits()
    {
        _start.Play();
        _animator.SetTrigger("Show");
        credits = true;
    }

    public void quit()
    {
        Application.Quit();
    }
}