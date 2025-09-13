using System;
using System.Collections.Generic;

namespace MMXOnline;

public class ZeroBuster : Weapon {
	public static ZeroBuster netWeapon = new();
	public ZeroBuster() : base() {
		index = (int)WeaponIds.Buster;
		killFeedIndex = 160;
		weaponBarBaseIndex = 0;
		weaponBarIndex = weaponBarBaseIndex;
		weaponSlotIndex = 0;
		shootSounds = new string[] { "", "", "", "" };
		displayName = "Z-Buster";
		description = new string[] { "Shoot uncharged Z-Buster with ATTACK." };
		type = (int)ZeroAttackLoadoutType.ZBuster;
		canHealAmmo = false;
		drawAmmo = false;
		drawCooldown = false;
		ammo = 32;
		maxAmmo = 32;
	}
	public override float getAmmoUsage(int chargeLevel) {
		return 0;
	}
	public override void bZeroShoot(Character character, int[] args) {
		
	}
}

public class ZeroBuster2 : Weapon {
	public static ZeroBuster2 netWeapon = new();
	public List<DZBusterProj> zeroLemonsOnField = new();

	public ZeroBuster2() : base() {
		index = (int)WeaponIds.Buster;
		killFeedIndex = 160;
		weaponBarBaseIndex = 0;
		weaponBarIndex = weaponBarBaseIndex;
		weaponSlotIndex = 0;
		shootSounds = new string[] { "", "", "", "" };
		fireRate = 9;
		displayName = "Z-Buster";
		description = new string[] { "Shoot uncharged Z-Buster with ATTACK." };
		type = (int)ZeroAttackLoadoutType.ZBuster;
		canHealAmmo = false;
		drawAmmo = false;
		drawCooldown = false;
	}
	public override float getAmmoUsage(int chargeLevel) {
		return 0;
	}
	public override void bZeroShoot(Character character, int[] args) {
		int chargeLevel = args[0];
		Point shootPos = character.getShootPos();
		int xDir = character.getShootXDir();
		Player player = character.player;
		BusterZero bz = character as BusterZero ?? throw new NullReferenceException();
		string sound = "";

		if (chargeLevel == 0) {
			zeroLemonsOnField.Add(new DZBusterProj(shootPos, xDir, player, player.getNextActorNetId(), rpc: true));
			sound = "busterX3";
			bz.shootCooldown = 11;
		} else if (chargeLevel == 1) {
			Buster2Proj(character);
			bz.shootCooldown = 22;
		} else if (chargeLevel == 2) {
			bz.shootCooldown = 22;
			if (player.BZVShot) {
				Buster3Proj(1, 1, character);
				Buster3Proj(2, 1, character);
			} else {
				Buster3Proj(0, 3, character);
			}
		} else if (chargeLevel == 3) {
			if (bz.charState is WallSlide) {
				bz.stockedBusterLv = 1;
				Buster3Proj(0, 3, character);
				bz.shootCooldown = 22;
				return;
			} else {
				bz.shootAnimTime = 0;
				bz.changeState(new BusterZeroDoubleBuster(false, true), true);
			}
		} else if (chargeLevel >= 4) {
			if (bz.charState is WallSlide) {
				Buster3Proj(0, 3, character);
				bz.stockedBusterLv = 2;
				bz.stockedSaber = true;
				bz.shootCooldown = 22;
				return;
			} else {
				bz.shootAnimTime = 0;
				bz.changeState(new BusterZeroDoubleBuster(false, false), true);
			}
		}
		if (chargeLevel >= 1) {
			bz.stopCharge();
		}
		if (!string.IsNullOrEmpty(sound)) character.playSound(sound, sendRpc: true);

	}
	public void Buster3Proj(int type, int dmg, Character character) {
		Point shootPos = character.getShootPos();
		int xDir = character.getShootXDir();
		Player player = character.player;
		character.playSound("buster3X3");
		new DZBuster3Proj(dmg, type, shootPos, xDir, player, player.getNextActorNetId(), rpc: true);
	}
	public void Buster2Proj(Character character) {
		Point shootPos = character.getShootPos();
		int xDir = character.getShootXDir();
		Player player = character.player;
		character.playSound("buster2X3");
		new DZBuster2Proj(2, shootPos, xDir, player, player.getNextActorNetId(), 0, rpc: true);
	}
	public override bool canShoot(int chargeLevel, Player player) {
		if (!base.canShoot(chargeLevel, player)) return false;
		if (chargeLevel > 1) {
			return true;
		}

		for (int i = zeroLemonsOnField.Count - 1; i >= 0; i--) {
			if (zeroLemonsOnField[i].destroyed) {
				zeroLemonsOnField.RemoveAt(i);
				continue;
			}
		}

		if (zeroLemonsOnField.Count < 4 && player.ArmorModeX) { return true; }
		return zeroLemonsOnField.Count < 3 && !player.ArmorModeX;
	}
}

public class ZBusterSaber : Weapon {
	public static ZBusterSaber netWeapon = new();

	public ZBusterSaber() : base() {
		index = (int)WeaponIds.ZSaberProjSwing;
		killFeedIndex = 9;
	}
}
