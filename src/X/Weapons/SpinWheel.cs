﻿using System;
using System.Collections.Generic;

namespace MMXOnline;

public class SpinWheel : Weapon {
	public static SpinWheel netWeapon = new();

	public SpinWheel() : base() {
		shootSounds = new string[] { "spinWheel", "spinWheel", "spinWheel", "spinWheelCharged" };
		fireRate = 60;
		index = (int)WeaponIds.SpinWheel;
		weaponBarBaseIndex = 12;
		weaponBarIndex = weaponBarBaseIndex;
		weaponSlotIndex = 12;
		killFeedIndex = 20 + (index - 9);
		weaknessIndex = (int)WeaponIds.StrikeChain;
		damage = "1/1*8";
		effect = "Inflicts Slowdown. Doesn't destroy on hit.\nUncharged won't give assists.";
		hitcooldown = "0.2/0";
		Flinch = "0/26";
		maxAmmo = 16;
		ammo = maxAmmo;
	}

	public override float getAmmoUsage(int chargeLevel) {
		if (chargeLevel >= 3) { return 4; }
		return 1;
	}

	public override void shoot(Character character, int[] args) {
		int chargeLevel = args[0];
		Point pos = character.getShootPos();
		int xDir = character.getShootXDir();
		Player player = character.player;

		if (chargeLevel < 3) {
			new SpinWheelProj(this, pos, xDir, player, player.getNextActorNetId(), true);
		} else {
			new SpinWheelProjChargedStart(this, pos, xDir, player, player.getNextActorNetId(), true);
		}
	}
}

public class SpinWheelProj : Projectile {
	int started;
	float startedTime;
	public Anim? sparks;
	float soundTime;
	float startMaxTime = 2.5f;
	float lastHitTime;
	const float hitCooldown = 0.2f;
	float maxTimeProj = 2.5f;

	public SpinWheelProj(
		Weapon weapon, Point pos, int xDir, 
		Player player, ushort netProjId, bool rpc = false
	) : base(
		weapon, pos, xDir, 0, 1, player, "spinwheel_start", 
		0, hitCooldown, netProjId, player.ownedByLocalPlayer
	) {
		destroyOnHit = false;
		projId = (int)ProjIds.SpinWheel;
		maxTimeProj = startMaxTime;
		maxTime = startMaxTime;

		if (rpc) {
			rpcCreate(pos, player, netProjId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new SpinWheelProj(
			SpinWheel.netWeapon, arg.pos, arg.xDir, arg.player, arg.netId
		);
	}

	public override void update() {
		base.update();
		if (ownedByLocalPlayer && time >= maxTimeProj) {
			destroySelf();
			return;
		}
		if (collider != null) {
			collider.isTrigger = false;
			collider.wallOnly = true;
		}
		if (started == 0 && sprite.isAnimOver()) {
			started = 1;
			changeSprite("spinwheel_proj", true);
			useGravity = true;
			if (collider != null) {
				collider.isTrigger = false;
				collider.wallOnly = true;
			}
		}
		if (started == 1) {
			startedTime += Global.spf;
			if (startedTime > 1) {
				started = 2;
				maxTimeProj = startMaxTime;
			}
		}
		if (started == 2) {
			vel.x = xDir * 250;
			if (lastHitTime > 0) vel.x = xDir * 4;
			Helpers.decrementTime(ref lastHitTime);
			if (Global.level.checkTerrainCollisionOnce(this, 0, -1) == null) {
				var collideData = Global.level.checkTerrainCollisionOnce(this, xDir, 0, vel);
				if (collideData != null && collideData.hitData != null &&
					collideData.hitData.normal != null && !((Point)collideData.hitData.normal).isAngled()
				) {
					xDir *= -1;
					if (sparks != null) sparks.xDir *= -1;
					maxTimeProj = startMaxTime;
					if (ownedByLocalPlayer) {
						startMaxTime -= 0.2f;
					}
				}
			}
			soundTime += Global.spf;
			if (soundTime > 0.15f) {
				soundTime = 0;
				//playSound("spinWheelLoop");
			}
		}
		if (started > 0 && grounded && !destroyed) {
			if (sparks == null) {
				sparks = new Anim(pos, "spinwheel_sparks", xDir, null, false);
				playSound("spinWheelGround", forcePlay: true);
			}
			sparks.pos = pos.addxy(-xDir * 10, 10);
			sparks.visible = true;
		} else {
			if (sparks != null) {
				sparks.visible = false;
			}
		}
	}

	public override void onDestroy() {
		if (sparks != null) {
			sparks.destroySelf();
		}
	}

	public override void onHitDamagable(IDamagable damagable) {
		if (damagable is CrackedWall) {
			damager.hitCooldownSeconds = hitCooldown;
			return;
		}

		lastHitTime = hitCooldown;

		var chr = damagable as Character;
		if (chr != null && chr.ownedByLocalPlayer && !chr.isSlowImmune()) {
			chr.vel = Point.lerp(chr.vel, Point.zero, Global.spf * 10);
			chr.slowdownTime = 0.25f;
		}

		base.onHitDamagable(damagable);
	}
}

public class SpinWheelProjChargedStart : Projectile {
	public SpinWheelProjChargedStart(
		Weapon weapon, Point pos, int xDir, 
		Player player, ushort netProjId, bool rpc = false
	) : base(
		weapon, pos, xDir, 0, 4, player, "spinwheel_charged_start", 
		Global.defFlinch, 0.5f, netProjId, player.ownedByLocalPlayer
	) {
		projId = (int)ProjIds.SpinWheelChargedStart;

		if (rpc) {
			rpcCreate(pos, player, netProjId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new SpinWheelProjChargedStart(
			SpinWheel.netWeapon, arg.pos, arg.xDir, arg.player, arg.netId
		);
	}

	public override void update() {
		base.update();
		if (isAnimOver()) {
			if (ownedByLocalPlayer) {
				for (int i = 0; i < 8; i++) {
					new SpinWheelProjCharged(
						weapon, pos, 1, damager.owner, i,
						damager.owner.getNextActorNetId(), rpc: true
					);
				}
			}
			destroySelf();
		}
	}
}

public class SpinWheelProjCharged : Projectile {

	public SpinWheelProjCharged(
		Weapon weapon, Point pos, int xDir, Player player, 
		int type, ushort netProjId, bool rpc = false
	) : base(
		weapon, pos, 1, 200, 1, player, "spinwheel_charged", 
		Global.defFlinch, 0, netProjId, player.ownedByLocalPlayer
	) {
		projId = (int)ProjIds.SpinWheelCharged;
		maxTime = 0.75f;
		vel = Point.createFromByteAngle(type * 32).times(speed);

		if (vel.x < 0) {
			xDir *= -1;
			xScale *= -1;
		}
		if (vel.y > 0) yScale *= -1;

		if (type % 2 != 0) changeSprite("spinwheel_charged_diag", true);
		else if (type is 2 or 6) changeSprite("spinwheel_charged_up", true);

		if (rpc) {
			rpcCreate(pos, player, netProjId, xDir, new byte[] {(byte)type});
		}
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new SpinWheelProjCharged(
			SpinWheel.netWeapon, arg.pos, arg.xDir, 
			arg.player, arg.extraData[0], arg.netId
		);
	}
}
