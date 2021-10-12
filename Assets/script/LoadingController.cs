using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController: MonoBehaviour {
	private void Start() {
		Invoke( "open", 2 );
	}

	void open() {
		UserDataManager.Load();
		SceneManager.LoadScene( 1 );
	}
}
