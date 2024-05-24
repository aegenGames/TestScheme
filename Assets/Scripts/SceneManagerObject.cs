using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerObject : MonoBehaviour
{
	private void Start()
	{
		Screen.fullScreen = true;
	}

	public void ChangeScene(string sceneName)
	{
		SceneManager.LoadSceneAsync(sceneName);
	}

	public void Exit()
	{
		Application.Quit();
	}
}