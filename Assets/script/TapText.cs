using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapText: MonoBehaviour {

	public float spawnTime = 0.5f;
	public Text textTap;
	private float updateSpawnTime;

	private void OnEnable() {
		updateSpawnTime = spawnTime;
	}

	private void Update() {
		updateSpawnTime -= Time.unscaledDeltaTime;
		if(updateSpawnTime <= 0f) {
			gameObject.SetActive( false );
		} else {
			textTap.CrossFadeAlpha( 0f, 0.5f, false );
			if(textTap.color.a == 0f) {
				gameObject.SetActive( false );
			}
		}
	}
}
