using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class GameManager: MonoBehaviour {

	private static GameManager instance = null;
	[Range( 0f, 1f )]
	public float autoCollectPercentage = 0.1f;
	public ResourceConfig[] resourcesConfigs;
	public Transform resourcesParent;
	public ResourceController resourcePrefab;
	public Text goldInfo;
	public Text autoCollectInfo;
	private List<ResourceController> activeResources = new List<ResourceController>();
	private float collectSecond;
	public double totalGold;
	public TapText tapTextPrefab;
	public Transform coinIcon;
	private List<TapText> tapTextPool = new List<TapText>();
	public Sprite[] resourcesSprites;

	[System.Serializable]
	public struct ResourceConfig {
		public string Name;
		public double UnlockCost;
		public double UpgradeCost;
		public double Output;
	}

	public static GameManager Instance {
		get {
			if(instance == null) {
				instance = FindObjectOfType<GameManager>();
			}
			return instance;
		}
	}

	private void Start() {
		AddAllResources();
	}

	private void Update() {
		collectSecond += Time.unscaledDeltaTime;
		if(collectSecond >= 1f) {
			collectPerSecond();
			collectSecond = 0f;
		}
		checkResourceCost();
		coinIcon.transform.localScale = Vector3.LerpUnclamped( coinIcon.transform.localScale, Vector3.one * 2f, 0.15f );
		coinIcon.transform.Rotate( 0f, 0f, Time.deltaTime * -100 );
	}

	private void checkResourceCost() {

		foreach(ResourceController resource in activeResources) {
			bool isBuyable = false;
			if(resource.isUnlocked) {
				isBuyable = totalGold >= resource.getUpgradeCost();
			} else {
				isBuyable = totalGold >= resource.getUnlockCost();
			}

			resource.resImage.sprite = resourcesSprites[isBuyable ? 1 : 0];
		}
	}

	private void AddAllResources() {

		bool showResource = true;
		foreach(ResourceConfig config in resourcesConfigs) {
			GameObject obj = Instantiate( resourcePrefab.gameObject, resourcesParent, false );
			ResourceController resource = obj.GetComponent<ResourceController>();
			resource.setConfig( config );
			activeResources.Add( resource );

			obj.gameObject.SetActive( showResource );
			if(showResource && !resource.isUnlocked) {
				showResource = false;
			}
		}
	}

	private void collectPerSecond() {
		double output = 0;
		foreach(ResourceController resource in activeResources) {
			if(resource.isUnlocked) output += resource.getOutput();
		}
		output *= autoCollectPercentage;
		autoCollectInfo.text = $"Auto Collect: { output.ToString( "F1" ) } / second";
		addGold( output );
	}

	public void addGold(double value) {
		totalGold += value;
		goldInfo.text = $"Gold: { totalGold.ToString( "0" ) }";
	}

	public void collectByTap(Vector3 tapPosition, Transform parent) {
		double output = 0;
		foreach(ResourceController resource in activeResources) {
			if(resource.isUnlocked) output += resource.getOutput();
		}

		TapText tapText = getOrCreateTapText();
		tapText.transform.SetParent( parent, false );
		tapText.transform.position = tapPosition;
		tapText.textTap.text = $"+{ output.ToString( "0" ) }";
		tapText.gameObject.SetActive( true );
		coinIcon.transform.localScale = Vector3.one * 1.75f;
		addGold( output );
	}

	private TapText getOrCreateTapText() {
		TapText tapText = tapTextPool.Find( t => !t.gameObject.activeSelf );
		if(tapText == null) {
			tapText = Instantiate( tapTextPrefab ).GetComponent<TapText>();
			tapTextPool.Add( tapText );
		}
		return tapText;
	}

	public void ShowNextResource() {
		foreach(ResourceController resource in activeResources) {
			if(!resource.gameObject.activeSelf) {
				resource.gameObject.SetActive( true );
				break;
			}
		}
	}
}
