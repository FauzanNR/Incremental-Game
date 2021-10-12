using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class ResourceController: MonoBehaviour {

	public AudioSource upgradekSFX;
	public AudioSource popUpSFX;
	public Text resDescription;
	public Text resUpgradeCost;
	public Text resUnlockCost;
	private ResourceConfig config;
	public Button resButton;
	public Image resImage;
	public bool isUnlocked {
		get; set;
	}

	private int _index;
	private int level {
		set {
			// Menyimpan value yang di set ke _level pada Progress Data
			UserDataManager.Progress.ResourcesLevels[_index] = value;

			UserDataManager.Save();
		}
		get {
			// Mengecek apakah index sudah terdapat pada Progress Data
			if(!UserDataManager.HasResources( _index )) {
				// Jika tidak maka tampilkan level 1
				return 1;
			}
			// Jika iya maka tampilkan berdasarkan Progress Data
			return UserDataManager.Progress.ResourcesLevels[_index];
		}
	}


	public void setConfig(ResourceConfig conf, int index) {
		_index = index;
		config = conf;
		resDescription.text = $"{ config.Name} Lv.{level}\n+{getOutput().ToString( "0" )}";
		resUpgradeCost.text = $"Upgrade Cost\n{ getUpgradeCost() }";
		resUnlockCost.text = $"Unlock Cost\n{ config.UnlockCost }";
		setUnlocked( config.UnlockCost == 0 || UserDataManager.HasResources( _index ) );
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
		if(UserDataManager.Progress.Gold < upgradeCost) return;
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
		if(UserDataManager.Progress.Gold < unlockCost) return;
		setUnlocked( true );
		popUpSFX.Play();
		GameManager.Instance.ShowNextResource();
		AchievementController.Instance.UnlockAchievement( AchievementType.UnlockResource, config.Name );
	}

	private void setUnlocked(bool unlocked) {
		isUnlocked = unlocked;
		if(unlocked) {
			// Jika resources baru di unlock dan belum ada di Progress Data, maka tambahkan data
			if(!UserDataManager.HasResources( _index )) {
				UserDataManager.Progress.ResourcesLevels.Add( level );
				UserDataManager.Save();
			}
		}

		resImage.color = isUnlocked ? Color.white : Color.grey;
		resUnlockCost.gameObject.SetActive( !unlocked );
		resUpgradeCost.gameObject.SetActive( unlocked );
	}
}

