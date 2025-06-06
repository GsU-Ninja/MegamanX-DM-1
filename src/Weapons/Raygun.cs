﻿using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace MMXOnline;

public class RayGun : AxlWeapon {
	public int laserChargeLevel;

	public RayGun(int altFire) : base(altFire) {
		sprite = "axl_arm_raygun";
		flashSprite = "axl_raygun_flash";
		chargedFlashSprite = "axl_raygun_flash";
		shootSounds = new string[] { "raygun", "raygun", "raygun", "splashLaser" };
		index = (int)WeaponIds.RayGun;
		weaponBarBaseIndex = 30;
		weaponBarIndex = 28;
		weaponSlotIndex = 34;
		killFeedIndex = 33;
		fireRate = 6;
		rechargeAmmoCooldown = 120;
		altRechargeAmmoCooldown = 120;
		if (altFire == 1) {
			shootSounds[3] = "";
		}
	}

	public override float whiteAxlFireRateMod() {
		return 1.5f;
	}

	public override float whiteAxlAmmoMod() {
		return 1;
	}

	public override float getAmmoUsage(int chargeLevel) {
		if (chargeLevel == 0) {
			return 1f;
		} else {
			if (altFire == 1) {
				if (laserChargeLevel == 0) return 0.1f;
				else if (laserChargeLevel == 1) return 0.25f;
				else return 1f;
			} else {
				return 0.5f;
			}
		}
	}

	public override void axlGetProjectile(
	    Weapon weapon, Point bulletPos, int xDir, Player player, float angle,
	 	IDamagable? target, Character? headshotTarget, Point cursorPos, int chargeLevel, ushort netId
	) {
		if (player.character is not Axl axl) {
			return;
		}
		Point bulletDir = Point.createFromAngle(angle);
		Projectile? bullet = null;
		if (chargeLevel < 3) {
			bullet = new RayGunProj(weapon, bulletPos, xDir, player, bulletDir, netId);
		} else {
			if (altFire == 0) {
				bullet = new SplashLaserProj(weapon, bulletPos, player, bulletDir, netId, sendRpc: true);
				bullet = new SplashLaserProj(weapon, bulletPos.add(bulletDir.times(22)), player, bulletDir, player.getNextActorNetId(), sendRpc: true);
				if (player.character != null) {
					RPC.playSound.sendRpc(shootSounds[3], player.character.netId);
				}
				return;
			} else {
				if (axl.rayGunAltProj == null) {
					axl.rayGunAltProj = new RayGunAltProj(weapon, bulletPos, cursorPos, 1, player, netId);
				} else {
					netId = axl.rayGunAltProj.netId.Value;
				}
				bullet = axl.rayGunAltProj;
				laserChargeLevel = axl.rayGunAltProj.getChargeLevel();
			}
		}

		if (player.ownedByLocalPlayer) {
			RPC.axlShoot.sendRpc(player.id, bullet.projId, netId, bulletPos, xDir, angle);
		}
	}
}

public class RayGunProj : Projectile {
	float len = 0;
	float lenDelay = 0;
	//float lastAngle;
	const float maxLen = 50;
	public RayGunProj(Weapon weapon, Point pos, int xDir, Player player, Point bulletDir, ushort netProjId) :
		base(weapon, pos, xDir, 400, 1, player, "axl_raygun_laser", 0, 0f, netProjId, player.ownedByLocalPlayer) {
		reflectable = true;
		if ((player?.character as Axl)?.isWhiteAxl() == true) {
			speed = 525;
			damager.hitCooldown = 0;
			maxTime *= 1.5f;
		}
		vel.x = bulletDir.x * speed;
		vel.y = bulletDir.y * speed;
		maxTime = 0.35f;
		projId = (int)ProjIds.RayGun;
		updateAngle();
		destroyOnHitWall = true;
	}

	public void updateAngle() {
		angle = vel.angle;
	}

	public override void update() {
		base.update();
		if (lenDelay > 0.01f) {
			len += Global.spf * 300;
			if (len > maxLen) len = maxLen;
		}
		lenDelay += Global.spf;
	}

	public void reflectSide() {
		vel.x *= -1;
		len = 0;
		lenDelay = 0;
		updateAngle();
	}

	public override void onReflect() {
		reflectSide();
		time = 0;
	}

	public override void onDeflect() {
		base.onDeflect();
		len = 0;
		lenDelay = 0;
		updateAngle();
	}

	public override void render(float x, float y) {
		var normVel = vel.normalize();
		var col1 = new Color(74, 78, 221);
		var col2 = new Color(61, 113, 255);
		var col3 = new Color(215, 244, 255);
		if (Global.level.gameMode.isTeamMode && damager.owner.alliance == GameMode.redAlliance) {
			col1 = new Color(221, 78, 74);
			col2 = new Color(255, 113, 61);
			col3 = new Color(255, 244, 215);
		}

		float xOff1 = -(normVel.x * len);
		float yOff1 = -(normVel.y * len);

		float sin = MathF.Sin(Global.time * 42.5f);

		if (!Options.main.lowQualityParticles()) {
			DrawWrappers.DrawLine(pos.x + xOff1, pos.y + yOff1, pos.x, pos.y, col1, 4 + sin, 0, true);
			DrawWrappers.DrawLine(pos.x + xOff1, pos.y + yOff1, pos.x, pos.y, col2, 2 + sin, 0, true);
			DrawWrappers.DrawLine(pos.x + xOff1, pos.y + yOff1, pos.x, pos.y, col3, 1 + sin, 0, true);
		} else {
			DrawWrappers.DrawLine(pos.x + xOff1, pos.y + yOff1, pos.x, pos.y, col3, 2 + sin, 0, true);
		}
	}
}

public class RayGunAltProj : Projectile {
	Player? player;
	const float range = 150;
	float soundCooldown;
	float chargeTime;
	float chargeDecreaseCooldown;
	Axl? axl;

	public RayGunAltProj(
		Weapon weapon, Point pos, Point cursorPos, int xDir, Player player, ushort netProjId
	) : base(
		weapon, pos, xDir, 0, 1, player, "axl_raygun_laser", 0, 0.33f, netProjId, player.ownedByLocalPlayer
	) {
		projId = (int)ProjIds.RayGunChargeBeam;
		destroyOnHit = false;
		shouldShieldBlock = false;
		shouldVortexSuck = false;
		this.player = player;
		netcodeOverride = NetcodeModel.FavorAttacker;
		angle = 0;

		axl = (player.character as Axl);

		if (axl != null) {
			if (axl.isWhiteAxl()) {
				damager.damage = 4;
				damager.hitCooldown = 8;
			}
		}
		if (!ownedByLocalPlayer && axl != null) {
			axl.nonOwnerAxlBulletPos = pos;
		}
		canBeLocal = false;

		isMelee = true;
		if (player?.character != null) {
			owningActor = player.character;
		}
	}

	public int getChargeLevel() {
		if (!ownedByLocalPlayer) {
			if (angle == 0) return 0;
			else return 1;
		}

		if (axl?.isWhiteAxl() == true) {
			return 1;
		}
		if (chargeTime >= 1.5f && chargeTime < 3f) return 1;
		else if (chargeTime >= 3f) return 1;
		return 0;
	}

	public override void postUpdate() {
		base.postUpdate();
		Character? chr = player?.character;

		if (ownedByLocalPlayer) {
			if (chr == null || chr.destroyed == true) {
				destroySelf();
				return;
			}
		}

		Helpers.decrementTime(ref soundCooldown);
		if (soundCooldown == 0) {
			string laserSound = "laser";
			soundCooldown = 0.217f;
			int chargeLevel = getChargeLevel();
			if (chargeLevel == 1) {
				laserSound = "laser2";
				soundCooldown = 0.18f;
			} else if (chargeLevel == 2) {
				laserSound = "laser3";
				soundCooldown = 0.14f;
			}

			chr?.playSound(laserSound);
		}

		if (!ownedByLocalPlayer) { return; }
		if (axl == null) { return; }

		Point bulletPos = axl.getAxlBulletPos();
		Point destPos = axl.getAxlHitscanPoint(range);
		var hits = Global.level.raycastAll(bulletPos, destPos, new List<Type>() { typeof(Actor), typeof(Wall) }, isChargeBeam: true);

		CollideData? closestHit = null;
		float bestDist = float.MaxValue;
		foreach (var hit in hits) {
			if (hit.gameObject is IDamagable damagable) {
				if (damagable.canBeDamaged(owner.alliance, player?.id, null)) {
					float dist = bulletPos.distanceTo(hit.hitData.hitPoint.Value);
					if (dist < bestDist) {
						bestDist = dist;
						closestHit = hit;
					}
				}
			}
			if (hit.gameObject is Wall) {
				float dist = bulletPos.distanceTo(hit.hitData.hitPoint.Value);
				if (dist < bestDist) {
					bestDist = dist;
					closestHit = hit;
				}
			}
		}

		Helpers.decrementTime(ref chargeDecreaseCooldown);
		bool chargeIncrease = false;
		if (closestHit != null) {
			destPos = closestHit.hitData.hitPoint.Value;
			if (closestHit.gameObject is IDamagable) {
				chargeIncrease = true;
				chargeTime += Global.spf;
				chargeDecreaseCooldown = 0.1f;
			}
		}

		if (!chargeIncrease && chargeDecreaseCooldown == 0) {
			Helpers.decrementTime(ref chargeTime);
		}

		if (getChargeLevel() == 0) {
			damager.damage = 1;
			damager.hitCooldown = 20;
			angle = 0;
		} else if (getChargeLevel() == 1) {
			damager.damage = 2;
			damager.hitCooldown = 9;
			angle = 90;
		} else if (getChargeLevel() == 2) {
			damager.damage = 4;
			damager.hitCooldown = 8;
			angle = 180;
		}

		changePos(destPos);

		if (Global.level.isSendMessageFrame() && player != null) {
			RPC.syncAxlBulletPos.sendRpc(player.id, bulletPos);
		}
	}

	public override void render(float x, float y) {
		if (axl == null) {
			return;
		}

		var normVel = vel.normalize();
		var col1 = new Color(74, 78, 221);
		var col2 = new Color(61, 113, 255);
		var col3 = new Color(215, 244, 255);
		if (Global.level.gameMode.isTeamMode && damager.owner.alliance == GameMode.redAlliance) {
			col1 = new Color(221, 78, 74);
			col2 = new Color(255, 113, 61);
			col3 = new Color(255, 244, 215);
		}

		float sin = MathF.Sin(Global.time * 30);

		Point origin;
		if (ownedByLocalPlayer) {
			origin = axl.getAxlBulletPos();
		} else {
			origin = axl.nonOwnerAxlBulletPos;
		}

		int chargeFactor = 0;
		if (getChargeLevel() == 1) chargeFactor = 1;
		else if (getChargeLevel() == 2) chargeFactor = 2;

		if (!Options.main.lowQualityParticles()) {
			DrawWrappers.DrawLine(origin.x, origin.y, pos.x, pos.y, col1, 3 + sin + chargeFactor, 0, true);
			DrawWrappers.DrawLine(origin.x, origin.y, pos.x, pos.y, col2, 2 + sin + chargeFactor, 0, true);
			DrawWrappers.DrawLine(origin.x, origin.y, pos.x, pos.y, col3, 1 + sin + chargeFactor, 0, true);
		} else {
			DrawWrappers.DrawLine(origin.x, origin.y, pos.x, pos.y, col3, 2 + sin + chargeFactor, 0, true);
		}
	}
}

public class SplashLaserProj : Projectile {
	public float maxSpeed = 400;
	public SplashLaserProj(
		Weapon weapon, Point pos, Player player, Point bulletDir, ushort netProjId, bool sendRpc = false
	) : base(
		weapon, pos, 1, 400, 1, player, "splashlaser_proj", 0, 0.3f, netProjId, player.ownedByLocalPlayer
	) {
		fadeSprite = "splashlaser_fade";
		projId = (int)ProjIds.SplashLaser;
		maxTime = 0.4f;
		useGravity = false;
		gravityModifier = 0.5f;
		vel.x = bulletDir.x * speed;
		vel.y = bulletDir.y * speed;
		destroyOnHit = true;
		shouldShieldBlock = false;
		updateAngle();
		if (sendRpc) {
			rpcCreateByteAngle(pos, player, netProjId, bulletDir.byteAngle);
		}
		destroyOnHitWall = true;
	}

	public void updateAngle() {
		angle = vel.angle;
	}

	public override void update() {
		base.update();
		updateAngle();
	}

	public override void onCollision(CollideData other) {
		base.onCollision(other);
		if (other.gameObject is Character chr) {
			chr.burnTime = 0;
		}
	}
}

