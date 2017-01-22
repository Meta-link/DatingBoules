using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    private Animator _animator;
    private bool credits = false;

	void Start () {
        _animator = GetComponent<Animator>();
	}
	
	void Update () {
		if(credits && Input.anyKeyDown)
        {
            _animator.SetTrigger("Hide");
            credits = false;
        }
	}

    public void startGame()
    {
        SceneManager.LoadScene("Niveau 1");
    }

    public void enterCredits()
    {
        _animator.SetTrigger("Show");
        credits = true;
    }

    public void quit()
    {
        Application.Quit();
    }
}
