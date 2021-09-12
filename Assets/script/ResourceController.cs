using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class ResourceController: MonoBehaviour {
	private int level = 1;
	public AudioSource upgradekSFX;
	public AudioSource popUpSFX;
	public Text resDescription;
	public Text resUpgradeCost;
	public Text resUnlockCost;
	private ResourceConfig config;
	public Button resButton;
	public Image resImage;
	public bool isUnlocked { get; set; }

	public void setConfig(ResourceConfig conf) {
		config = conf;
		resDescription.text = $"{ config.Name} Lv.{level}\n+{getOutput().ToString( "0" )}";
		resUpgradeCost.text = $"Upgrade Cost\n{ getUpgradeCost() }";
		resUnlockCost.text = $"Unlock Cost\n{ config.UnlockCost }";
		setUnlocked( config.UnlockCost == 0 );
	}

	public double getUpgradeCost() {
		return config.UpgradeCost * level;
	}
	public double getOutput() {
		return config.Output * level;
	}
	public double getUnlockCost() {
		return config.UnlockCost;
	}

	public void upgradeLevel() {
		double upgradeCost = getUpgradeCost();
		if(GameManager.Instance.totalGold < upgradeCost) return;
		upgradekSFX.Play();
		GameManager.Instance.addGold( -upgradeCost );
		level++;
		resUpgradeCost.text = $"Upgrade Cost\n{ getUpgradeCost() }";
		resDescription.text = $"{ config.Name } Lv. { level }\n+{ getOutput().ToString( "0" ) }";
	}

	private void Start() {
		resButton.onClick.AddListener( () => {
			if(isUnlocked) {
				upgradeLevel();
			} else {
				unlockResources();
			}
		} );
	}

	private void unlockResources() {
		double unlockCost = getUnlockCost();
		if(GameManager.Instance.totalGold < unlockCost) return;
		setUnlocked( true );
		popUpSFX.Play();
		GameManager.Instance.ShowNextResource();
		AchievementController.Instance.UnlockAchievement( AchievementType.UnlockResource, config.Name );
	}

	private void setUnlocked(bool unlocked) {
		isUnlocked = unlocked;
		resImage.color = isUnlocked ? Color.white : Color.grey;
		resUnlockCost.gameObject.SetActive( !unlocked );
		resUpgradeCost.gameObject.SetActive( unlocked );
	}
}

