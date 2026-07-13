using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MMXOnline;
public class XBuster : Weapon {
	public static XBuster netWeapon = new();
	public List<BusterProj> lemonsOnField = new List<BusterProj>();
	public bool isUnpoBuster;

	public XBuster() : base() {
		displayName = "X-Buster";
		index = (int)WeaponIds.Buster;
		killFeedIndex = 0;
		weaponBarBaseIndex = 0;
		weaponBarIndex = weaponBarBaseIndex;
		weaponSlotIndex = 0;
		shootSounds = new string[] { "", "", "", "" };
		fireRate = 9;
		canHealAmmo = false;
		drawAmmo = false;
		drawCooldown = false;
		//effect = "Mega Buster Mark 17";
		hitcooldown = "0/0/0/60";
		damage = "1/2/3/4";
		flinch = "0/0/13/26";
		flinchCD = "0";
	}

	public void setUnpoBuster(MegamanX mmx) {
		isUnpoBuster = true;
		fireRate = 45;
		weaponBarBaseIndex = 70;
		weaponBarIndex = 59;
		weaponSlotIndex = 121;
		killFeedIndex = 180;

		// Ammo variables
		maxAmmo = 12;
		ammo = maxAmmo;
		allowSmallBar = false;
		ammoGainMultiplier = 2;
		canHealAmmo = true;
		drawRoundedDown = true;
		
		// HUD.
		drawAmmo = true;
		drawCooldown = true;
	}

	public static bool isNormalBuster(Weapon weapon) {
		return weapon is XBuster buster && !buster.isUnpoBuster;
	}

	public static bool isWeaponUnpoBuster(Weapon weapon) {
		return weapon is XBuster buster && buster.isUnpoBuster;
	}

	public override bool canShoot(int chargeLevel, Player player) {
		if (!base.canShoot(chargeLevel, player)) return false;
		if (chargeLevel > 1) {
			return true;
		}
		for (int i = lemonsOnField.Count - 1; i >= 0; i--) {
			if (lemonsOnField[i].destroyed) {
				lemonsOnField.RemoveAt(i);
				continue;
			}
		}
		
		return lemonsOnField.Count < 3;
	}

	public override float getAmmoUsage(int chargeLevel) {
		return 0;
	}

	public override void shoot(Character character, int[] args) {
		if (character is not MegamanX mmx) {
			return;
		}
		int chargeLevel = args[0];
		bool isStock = args[1] == 1;
		Point pos = character.getShootPos();
		int xDir = character.getShootXDir();
		Player player = character.player;
		
		string shootSound = "buster";
		shootSound = chargeLevel switch {
			0 => "buster",
			1 => "buster2",
			2 => "buster3",
			_ when isStock || mmx.armArmor == ArmorId.Giga => "buster4X2",
			_ => "buster4"
		};
		if (mmx.armArmor == ArmorId.Giga && !isStock) {
			shootSound = chargeLevel switch {
				0 => "busterX2",
				1 => "buster2X2",
				2 => "buster3X2",
				3 => "buster4X2",
				_ => shootSound
			};
		} else if (mmx.armArmor == ArmorId.Max && !isStock) {
			shootSound = chargeLevel switch {
				0 => "busterX3",
				1 => "buster2X3",
				2 => "buster3X3",
				3 => "buster3X3",
				_ => shootSound
			};
		}

		if (mmx.hasUltimateArmor && chargeLevel >= 3 && !isStock && mmx.armArmor != ArmorId.Max) {
			shootPlasmaShot(mmx, pos, xDir);
			return;
		} else if (mmx.stockedMaxBusterLv >= 1) {
			if (mmx.charState.attackCtrl && mmx.charState.normalCtrl) {
				if (mmx.charState is not WallSlide) {
					mmx.changeState(new X3ChargeShot(null));
				} else {
					shootMaxBuster3(mmx, pos, xDir);
					character.playSound("buster3X3", sendRpc: true);
					mmx.stockedMaxBusterLv--;
				}
				return;
			}
		}
		else if (chargeLevel == 0) {
			lemonsOnField.Add(new BusterProj(pos, xDir, mmx, player, player.getNextActorNetId(), true));
		} else if (chargeLevel == 1) {
			shootBuster2(mmx, pos, xDir);
		} else if (chargeLevel == 2) {
			if (mmx.armArmor == ArmorId.Light || mmx.armArmor == ArmorId.None) {
				shootLightBuster3(mmx, pos, xDir);
			} else if (mmx.armArmor == ArmorId.Giga) {
				shootGigaBuster3(mmx, pos, xDir);
			} else if (mmx.armArmor == ArmorId.Max) {
				shootMaxBuster3(mmx, pos, xDir);
			}
		} else if (chargeLevel >= 3) {
			if (isStock) {
				shootGigaBuster4Stock(mmx, pos, xDir);
			}
			else if (mmx.armArmor == ArmorId.Max) {
				mmx.stockedMaxBusterLv += 2;
				if (mmx.charState is not WallSlide) {
					mmx.changeState(new X3ChargeShot(null));
				} else {
					shootMaxBuster4(mmx, pos, xDir);
					mmx.stockedMaxBusterLv--;
				}
			} else if (mmx.armArmor == ArmorId.Giga) {
				shootGigaBuster4(mmx, pos, xDir);
			} else {
				shootLightBuster4(mmx, pos, xDir);
			}
		}
		if (shootSound != "") {
			character.playSound(shootSound, sendRpc: true);	
		}
	}

	public void shootLightBuster4(MegamanX mmx, Point pos, int xDir) {
		Player player = mmx.player;
		new Anim(pos.addxy(-14*xDir, 17), "buster4_muzzle_flash", xDir, null, true);
		//Create the buster effect
		int xOff = xDir * -5;
		player.setNextActorNetId(player.getNextActorNetId());
		// Create first line instantly.
		createBuster4Line(pos.x + xOff, pos.y, xDir, mmx, 0f);
		// Create 2nd with a delay.
		Global.level.delayedActions.Add(new DelayedAction(delegate {
			createBuster4Line(pos.x + xOff, pos.y, xDir, mmx, 10);
		}, 2.8f / 60f));
		// Use smooth spawn on the 3rd.
		Global.level.delayedActions.Add(new DelayedAction(delegate {
			createBuster4Line(pos.x + xOff, pos.y, xDir, mmx, 5, true);
		}, 5.8f / 60f));
	}
	
	public void createBuster4Line(
		float x, float y, int xDir, MegamanX mmx,
		float offsetTime, bool smoothStart = false
	) {
		Player player = mmx.player;

		new Buster4Proj(
			new Point(x + xDir, y), xDir, mmx,
			player, 0, offsetTime,
			player.getNextActorNetId(allowNonMainPlayer: true), smoothStart, true
		);
		Global.level.delayedActions.Add(new DelayedAction(delegate {
			new Buster4Proj(
				new Point(x + xDir, y), xDir, mmx,
				player, 1, offsetTime,
				player.getNextActorNetId(allowNonMainPlayer: true), smoothStart, true
			);
		}, 1.8f / 60f
		));
		Global.level.delayedActions.Add(new DelayedAction(delegate {
			new Buster4Proj(
				new Point(x + xDir, y), xDir, mmx,
				player, 2, offsetTime,
				player.getNextActorNetId(allowNonMainPlayer: true), smoothStart, true
			);
		}, 3.8f / 60f
		));
		Global.level.delayedActions.Add(new DelayedAction(delegate {
			new Buster4Proj(
				new Point(x + xDir, y), xDir, mmx,
				player, 2, offsetTime,
				player.getNextActorNetId(allowNonMainPlayer: true), smoothStart, true
			);
		}, 5.8f / 60f
		));
		Global.level.delayedActions.Add(new DelayedAction(delegate {
			new Buster4Proj(
				new Point(x + xDir, y), xDir,mmx,
				player, 3, offsetTime,
				player.getNextActorNetId(allowNonMainPlayer: true), smoothStart, true
			);
		}, 7.8f / 60f
		));
	}

	public static void shootBuster2(MegamanX mmx, Point pos, int xDir) {
		Player player = mmx.player;
		new Buster2Proj(pos, xDir, mmx, player, player.getNextActorNetId(), true);
	}
	public static void shootLightBuster3(MegamanX mmx, Point pos, int xDir) {
		Player player = mmx.player;
		new Buster3LightProj(pos, xDir, mmx, player, player.getNextActorNetId(), true);
	}
	public static void shootGigaBuster3(MegamanX mmx, Point pos, int xDir) {
		Player player = mmx.player;
		new Buster3GigaProj(pos, xDir, mmx, player, player.getNextActorNetId(), true);
	}
	public static void shootMaxBuster3(MegamanX mmx, Point pos, int xDir) {
		Player player = mmx.player;
		new Buster3MaxProj(pos, xDir, mmx, player, player.getNextActorNetId(), true);
	}
	public static void shootGigaBuster4(MegamanX mmx, Point pos, int xDir) {
		Player player = mmx.player;
		new Buster4GigaProj(pos, xDir, mmx, player, player.getNextActorNetId(), true);
	}
	public static void shootGigaBuster4Stock(MegamanX mmx, Point pos, int xDir) {
		Player player = mmx.player;
		new Buster4Giga2Proj(pos, xDir, mmx, player, player.getNextActorNetId(), true);
	}
	public static void shootMaxBuster4(MegamanX mmx, Point pos, int xDir) {
		Player player = mmx.player;
		new BusterX3Proj1(pos, xDir, 0, mmx, player, player.getNextActorNetId(), rpc: true);
	}
	public static void shootPlasmaShot(MegamanX mmx, Point pos, int xDir) {
		Player player = mmx.player;
		new Anim(pos.addxy(-14 * xDir, 17), "buster4_muzzle_flash", xDir, player.getNextActorNetId(), true);
		new BusterPlasmaProj(pos, xDir, mmx, player, player.getNextActorNetId(), true);
		mmx.playSound("plasmaShot", sendRpc: true);
	}

}
