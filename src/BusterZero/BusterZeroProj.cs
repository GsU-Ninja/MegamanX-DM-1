using System;
using System.Collections.Generic;
namespace MMXOnline;

public class DZBusterProj : Projectile {
	bool deflected;

	public DZBusterProj(
		Point pos, int xDir, Actor owner, Player player, ushort? netId, bool rpc = false
	) : base(
		pos, xDir, owner, "buster1", netId, player	
	) {
		weapon = ZeroBuster.netWeapon;
		damager.damage = 1;
		vel = new Point(240 * xDir, 0);
		fadeSprite = "buster1_fade";
		reflectable = true;
		maxTime = player.ArmorModeX ? 0.65f : 0.5175f;
		projId = (int)ProjIds.DZBuster;
		if (rpc) {
			rpcCreate(pos, owner, ownerPlayer, netId, xDir);
		}
	}

	public override void update() {
		base.update();
		if (!deflected && System.MathF.Abs(vel.x) < 360) {
			vel.x += Global.spf * xDir * 900f;
			if (System.MathF.Abs(vel.x) >= 360) {
				vel.x = (float)xDir * 360;
			}
		}
	}

	public override void onDeflect() {
		base.onDeflect();
		deflected = true;
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new DZBusterProj(
			args.pos, args.xDir, args.owner, args.player, args.netId
		);
	}
}

public class DZBuster2Proj : Projectile {
	public int type;
	public bool once, once2;
	public bool firstHit, haveHit;
	public int dmg;
	public DZBuster2Proj(
		int dmg, Point pos, int xDir, Actor owner, Player player, ushort? netId, int type, bool rpc = false
	) : base(
		pos, xDir, owner, "zbuster2", netId, player	
	) {
		weapon = ZeroBuster.netWeapon;
		damager.damage = 2;
		vel = new Point(350 * xDir, 0);
		this.dmg = dmg;
		if (player.BZLaserShot || player.BZTripleShot) {
			damager.hitCooldown = 3.6f;
		}
		fadeOnAutoDestroy = true;
		fadeSprite = "buster2_fade";
		projId = (int)ProjIds.DZBuster2;
		if (player.BZVulcan) {
			vel = new Point(400*xDir, 0);
		}
		maxTime = 0.5f;
		if (type == 0) {
			if (player.BZIceJavelin) maxTime = 0.85f;
			damager.damage = player.ArmorModeUltimate ? 3 : 2;
		} else if (type == 1) {
			vel = new Point(player.BZVulcan ? 400 : 350, 0);
		} else if (type == 2) {
			vel.y = player.BZVulcan ? 400 : 350;
			angle = 45 * xDir;
			yDir = -1;
		} else if (type == 3) {
			vel.y = player.BZVulcan ? -400 :-350;
			angle = -45 * xDir;
		}
		this.type = type;
		reflectable = true;
		destroyOnHit = !player.BZLaserShot;

		if (rpc) {
			rpcCreate(pos, owner, ownerPlayer, netId, xDir, (byte)type);
		}
	}
	public override void onHitWall(CollideData other) {
		if (!ownedByLocalPlayer) return;
		base.onHitWall(other);
		var mainPlayer = Global.level.mainPlayer;
		if (mainPlayer.BZBurningShot && type == 0 && !once2) {
			once2 = true;
			new BZBurningShot(0,
				pos, xDir, mainPlayer,
				mainPlayer.getNextActorNetId(), true
			);
			Global.level.delayedActions.Add(new DelayedAction(() => {
				new BZBurningShot(0,
				pos.addxy(Helpers.randomRange(-30, 30), Helpers.randomRange(-30, 30)),
				xDir, mainPlayer, mainPlayer.getNextActorNetId(), true
				);
			}, 6f / 60f));
			Global.level.delayedActions.Add(new DelayedAction(() => {
				new BZBurningShot(0,
				pos.addxy(Helpers.randomRange(-30, 30), Helpers.randomRange(-30, 30)),
				xDir, mainPlayer, mainPlayer.getNextActorNetId(), true
				);
			}, 12f / 60f));
			Global.level.delayedActions.Add(new DelayedAction(() => {
				new BZBurningShot(0,
				pos.addxy(Helpers.randomRange(-30, 30), Helpers.randomRange(-30, 30)),
				xDir, mainPlayer, mainPlayer.getNextActorNetId(), true
				);
			}, 18f / 60f));
			Global.level.delayedActions.Add(new DelayedAction(() => {
				new BZBurningShot(0,
				pos.addxy(Helpers.randomRange(-30, 30), Helpers.randomRange(-30, 30)),
				xDir, mainPlayer, mainPlayer.getNextActorNetId(), true
				);
			}, 24f / 60f));
			Global.level.delayedActions.Add(new DelayedAction(() => {
				new BZBurningShot(0,
				pos.addxy(Helpers.randomRange(-30, 30), Helpers.randomRange(-30, 30)),
				xDir, mainPlayer, mainPlayer.getNextActorNetId(), true
				);
			}, 30f / 60f));
		}
		if (mainPlayer.BZReflectLaser && type == 0) {
				if (!firstHit) {
					firstHit = true;
				} else if (other.isSideWallHit()) {
					if (haveHit) {
						vel.x = -140 * xDir;
						angle = -120 * xDir;
					} else {
						vel.x = -250 * xDir;
						angle = -130 * xDir;
					}
					vel.y = -250;
				} else if (other.isGroundHit()) {
					vel.y *= -1;
					angle = 225;
				} else if (other.isCeilingHit()) {
					vel.y *= -1;
					angle = 135 * xDir;
				}
			}
	}
	public override void onHitDamagable(IDamagable damagable) {
		base.onHitDamagable(damagable);
		var mainPlayer = Global.level.mainPlayer;
		if (mainPlayer.BZIceJavelin && type == 0) {
			vel.x = 140 * xDir;
			haveHit = true;
		}
		if (mainPlayer.BZTripleShot && !once && type == 0) {
			once = true;
			new DZBuster2Proj(mainPlayer.ArmorModeUltimate ? 2 : 1, pos, xDir, this, mainPlayer, mainPlayer.getNextActorNetId(), 2, rpc: true);
			new DZBuster2Proj(mainPlayer.ArmorModeUltimate ? 2 : 1, pos, xDir, this, mainPlayer, mainPlayer.getNextActorNetId(), 3, rpc: true);

		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new DZBuster2Proj(
			args.extraData[0], args.pos, args.xDir, args.owner, args.player, args.netId, args.extraData[0]
		);
	}
}

public class DZBuster3Proj : Projectile {
	float partTime;
	public bool once, once2, once3;
	public int type;
	public int dmg;
	public DZBuster3Proj(
		int dmg, int type, Point pos, int xDir, Actor owner, Player player, ushort? netId, bool rpc = false
	) : base(
		pos, xDir, owner, "zbuster4", netId, player	
	) {
		weapon = ZeroBuster.netWeapon;
		damager.damage = 3;
		damager.flinch = Global.halfFlinch;
		vel = new Point(350 * xDir, 0);
		this.dmg = dmg;
		this.type = type;
		fadeOnAutoDestroy = true;
		fadeSprite = "buster3_fade";
		reflectable = true;
		maxTime = 0.5f;
		damager.damage = player.BZBuster2 ? dmg + 1 : dmg;
		projId = (int)ProjIds.DZBuster3;
		if (rpc) {
			rpcCreate(pos, owner, ownerPlayer, netId, xDir);
		}
	}

	public override void update() {
		base.update();
		BZBlizzardArrow();
		if (type == 1) vel.y += 5;
		if (type == 2) vel.y -= 5;

		partTime += Global.speedMul;
		if (partTime >= 5) {
			partTime = 0;
			new Anim(
				pos.addxy(-20 * xDir, 0).addRand(0, 12),
				"zbuster4_part", 1, null, true
			) {
				vel = vel,
				acc = new Point(-vel.x * 2, 0)
			};
		}
	}
	public override void onHitDamagable(IDamagable damagable) {
		base.onHitDamagable(damagable);
		BZEspark();
		BurningShot();
		BZTimeStopper();
	}
	public void BZTimeStopper() {
		var mainPlayer = Global.level.mainPlayer;
		if (mainPlayer.BZTimeStopper) {
			new BZTimeStopper(
				pos, owner, owner.getNextActorNetId(),
				 owner.ownedByLocalPlayer, sendRpc: true
			);			
		}
	}
	public void BZBlizzardArrow() {
		var mainPlayer = Global.level.mainPlayer;
		if (mainPlayer.BZBlizzardArrow && time >= 0.1 && !once3) {
			once3 = true;
			new BZBlizzardArrow(
				pos.addxy(Helpers.randomRange(-10, 10), Helpers.randomRange(-25, 25)), xDir, mainPlayer,
				mainPlayer.getNextActorNetId(), true
			);
			Global.level.delayedActions.Add(new DelayedAction(() => {
				new BZBlizzardArrow(
				pos.addxy(Helpers.randomRange(-10, 10), Helpers.randomRange(-25, 25)),
				xDir, mainPlayer, mainPlayer.getNextActorNetId(), true
				);
			}, 8f / 60f));
		}
	}
	public void BZEspark() {
		var mainPlayer = Global.level.mainPlayer;
		if (mainPlayer.BZSparkShot && !once2) {
			once2 = true;
			new BZEspark(1,
				pos, xDir, mainPlayer,
				mainPlayer.getNextActorNetId(), true
			);
			new BZEspark(2,
				pos, xDir, mainPlayer,
				mainPlayer.getNextActorNetId(), true
			);
		}
	}
	public void BurningShot() {
		var mainPlayer = Global.level.mainPlayer;
		if (mainPlayer.BZBlastShot && !once) {
			once = true;
			new BZBurningShot(1,
				pos, xDir, mainPlayer,
				mainPlayer.getNextActorNetId(), true
			);
			Global.level.delayedActions.Add(new DelayedAction(() => {
				new BZBurningShot(1,
				pos.addxy(Helpers.randomRange(-30, 30), Helpers.randomRange(-30, 30)),
				xDir, mainPlayer, mainPlayer.getNextActorNetId(), true
				);
			}, 6f / 60f));
			Global.level.delayedActions.Add(new DelayedAction(() => {
				new BZBurningShot(1,
				pos.addxy(Helpers.randomRange(-30, 30), Helpers.randomRange(-30, 30)),
				xDir, mainPlayer, mainPlayer.getNextActorNetId(), true
				);
			}, 12f / 60f));
			Global.level.delayedActions.Add(new DelayedAction(() => {
				new BZBurningShot(1,
				pos.addxy(Helpers.randomRange(-30, 30), Helpers.randomRange(-30, 30)),
				xDir, mainPlayer, mainPlayer.getNextActorNetId(), true
				);
			}, 18f / 60f));
			Global.level.delayedActions.Add(new DelayedAction(() => {
				new BZBurningShot(1,
				pos.addxy(Helpers.randomRange(-30, 30), Helpers.randomRange(-30, 30)),
				xDir, mainPlayer, mainPlayer.getNextActorNetId(), true
				);
			}, 24f / 60f));
			Global.level.delayedActions.Add(new DelayedAction(() => {
				new BZBurningShot(1,
				pos.addxy(Helpers.randomRange(-30, 30), Helpers.randomRange(-30, 30)),
				xDir, mainPlayer, mainPlayer.getNextActorNetId(), true
				);
			}, 30f / 60f));
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new DZBuster3Proj(
			args.extraData[0], args.extraData[1], args.pos, args.xDir, args.owner, args.player, args.netId
		);
	}
}

public class DZHadangekiProj : Projectile {
	public DZHadangekiProj(
		Point pos, int xDir, bool isBZ, Actor owner, Player player, ushort? netId, bool rpc = false
	) : base(
		pos, xDir, owner, "zsaber_shot", netId, player
	) {
		weapon = ZeroBuster.netWeapon;
		damager.damage = 3;
		vel = new Point(350 * xDir, 0);
		fadeOnAutoDestroy = true;
		fadeSprite = "zsaber_shot_fade";
		reflectable = true;
		projId = (int)ProjIds.DZHadangeki;
		maxTime = 0.5f;
		if (isBZ) {
			damager.damage = 4;
			genericShader = player.zeroPaletteShader;
		}
		if (rpc) {
			rpcCreate(pos, owner, ownerPlayer, netId, xDir, isBZ ? (byte)1 : (byte)0);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new DZHadangekiProj(
			args.pos, args.xDir, args.extraData[0] == 1, args.owner, args.player, args.netId
		);
	}
}
