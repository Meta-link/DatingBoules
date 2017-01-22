using UnityEngine;
using UnityEngine.SceneManagement;

public class Interstate : MonoBehaviour {

    public string nextScene;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.anyKeyDown)
        {
            SceneManager.LoadScene(nextScene);
        }
	}
}
