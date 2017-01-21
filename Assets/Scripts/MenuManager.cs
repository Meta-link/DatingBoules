using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    
	void Start () {
		
	}
	
	void Update () {
		
	}

    public void startGame()
    {
        SceneManager.LoadScene("Niveau 1");
    }
}
