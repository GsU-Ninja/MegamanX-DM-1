using System;
using System.Collections.Generic;
using SFML.Graphics;
using System.Linq;
using MMXOnline;
namespace MMXOnline;
public class SakuyaKnifeProj : Projectile {
	public bool landed;
	public SakuyaKnifeProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		SakuyaProjW.netWeapon, pos, xDir,
		300, 0.35f, player, "sakuya_knife", 0, 0f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = true;
		maxTime = 0.75f;
		projId = (int)ProjIds.SakuyaKnifeProj;
		destroyOnHitWall = false;
		xScale = 0.75f;
		yScale = 0.75f;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}
	public override void update() {	
		if (collider != null) {
			collider.isTrigger = false;
			collider.wallOnly = true;
		}
		base.update();
	}
	public override void onHitDamagable(IDamagable damagable) {
		if (damagable is not FrostShieldProjGround or FrostShieldProjAir or FrostShieldProjCharged
			or FrostShieldProj or FrostShieldProjPlatform or FrostShieldProjChargedGround
			or GaeaShieldProj or ChillPIceProj) {
			base.onHitDamagable(damagable);
		}
	}
	public override void onCollision(CollideData other) {
		base.onCollision(other);
		if (!ownedByLocalPlayer) return;
		if (!landed && other.gameObject is Wall) {
			landed = true;
			vel = new Point();
			playSound("knife_land");
			maxTime = 2;
			var triggers = Global.level.getTriggerList(this, 0, 0);
			if (triggers.Any(t => t.gameObject is SakuyaKnifeProj)) {
				incPos(new Point(Helpers.randomRange(-2, 2), Helpers.randomRange(-2, 2)));
			}
		}
		var character = other.gameObject as Character;
		if (time > 0.2f && character != null && character.player == damager.owner) {	
			 character.player.SakuyaAmmo += 0.5f;
			 destroySelf();
		}
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new SakuyaKnifeProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class SakuyaKnifeUpProj : Projectile {
	public bool landed;
	public SakuyaKnifeUpProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		SakuyaProjW.netWeapon, pos, xDir,
		0, 0.35f, player, "sakuya_knife_up", 0, 0f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = true;
		maxTime = 0.75f;
		projId = (int)ProjIds.SakuyaKnifeUpProj;
		destroyOnHitWall = false;
		xScale = 0.75f;
		yScale = 0.75f;
		vel.y = -200f;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}
	public override void update() {	
		if (collider != null) {
			collider.isTrigger = false;
			collider.wallOnly = true;
		}
		base.update();
	}
	public override void onHitDamagable(IDamagable damagable) {
		if (damagable is not FrostShieldProjGround or FrostShieldProjAir or FrostShieldProjCharged
			or FrostShieldProj or FrostShieldProjPlatform or FrostShieldProjChargedGround 
			or GaeaShieldProj or ChillPIceProj) {
			base.onHitDamagable(damagable);
		}
	}
	public override void onCollision(CollideData other) {
		base.onCollision(other);
		if (!ownedByLocalPlayer) return;
		if (!landed && other.gameObject is Wall) {
			landed = true;
			vel = new Point();
			playSound("knife_land");
			maxTime = 2;
			var triggers = Global.level.getTriggerList(this, 0, 0);
			if (triggers.Any(t => t.gameObject is SakuyaKnifeProj)) {
				incPos(new Point(Helpers.randomRange(-2, 2), Helpers.randomRange(-2, 2)));
			}
		}
		var character = other.gameObject as Character;
		if (time > 0.2f && character != null && character.player == damager.owner) {	
			 character.player.SakuyaAmmo += 0.5f;
			 destroySelf();
		}
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new SakuyaKnifeUpProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class SakuyaKnifeDProj : Projectile {
	public bool landed;
	public SakuyaKnifeDProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		SakuyaProjW.netWeapon, pos, xDir,
		180, 0.35f, player, "sakuya_knife", 0, 0f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = true;
		maxTime = 0.5f;
		projId = (int)ProjIds.SakuyaKnifeDProj;
		destroyOnHitWall = false;
		xScale = 0.75f;
		yScale = 0.75f;
		vel.y = 250;
		angle = 40*xDir;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}
	public override void update() {	
		if (collider != null) {
			collider.isTrigger = false;
			collider.wallOnly = true;
		}
		base.update();
	}
	public override void onHitDamagable(IDamagable damagable) {
		if (damagable is not FrostShieldProjGround or FrostShieldProjAir or FrostShieldProjCharged
			or FrostShieldProj or FrostShieldProjPlatform or FrostShieldProjChargedGround 
			or GaeaShieldProj or ChillPIceProj) {
			base.onHitDamagable(damagable);
		}
	}
	public override void onCollision(CollideData other) {
		base.onCollision(other);
		if (!ownedByLocalPlayer) return;
		if (!landed && other.gameObject is Wall) {
			landed = true;
			vel = new Point();
			playSound("knife_land");
			maxTime = 2;
			var triggers = Global.level.getTriggerList(this, 0, 0);
			if (triggers.Any(t => t.gameObject is SakuyaKnifeProj)) {
				incPos(new Point(Helpers.randomRange(-2, 2), Helpers.randomRange(-2, 2)));
			}
		}
		var character = other.gameObject as Character;
		if (time > 0.2f && character != null && character.player == damager.owner) {	
			 character.player.SakuyaAmmo += 0.5f;
			 destroySelf();
		}
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new SakuyaKnifeDProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class SakuyaKnifeDownProj : Projectile {
	public bool landed;
	public SakuyaKnifeDownProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		SakuyaProjW.netWeapon, pos, xDir,
		0, 0.35f, player, "sakuya_knife_up", 0, 0f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = true;
		maxTime = 0.75f;
		projId = (int)ProjIds.SakuyaKnifeDownProj;
		destroyOnHitWall = false;
		xScale = 0.75f;
		yScale = 0.75f;
		vel.y = 0f;
		angle = 180;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}
	public override void update() {	
		if (collider != null) {
			collider.isTrigger = false;
			collider.wallOnly = true;
		}
		vel.y += 10;
		base.update();
	}
	public override void onHitDamagable(IDamagable damagable) {
		if (damagable is not FrostShieldProjGround or FrostShieldProjAir or FrostShieldProjCharged
			or FrostShieldProj or FrostShieldProjPlatform or FrostShieldProjChargedGround 
			or GaeaShieldProj or ChillPIceProj) {
			base.onHitDamagable(damagable);
		}
	}
	public override void onCollision(CollideData other) {
		base.onCollision(other);
		if (!ownedByLocalPlayer) return;
		if (!landed && other.gameObject is Wall) {
			landed = true;
			vel = new Point();
			playSound("knife_land");
			maxTime = 2;
			var triggers = Global.level.getTriggerList(this, 0, 0);
			if (triggers.Any(t => t.gameObject is SakuyaKnifeProj)) {
				incPos(new Point(Helpers.randomRange(-2, 2), Helpers.randomRange(-2, 2)));
			}
		}
		var character = other.gameObject as Character;
		if (time > 0.2f && character != null && character.player == damager.owner) {	
			 character.player.SakuyaAmmo += 0.5f;
			 destroySelf();
		}
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new SakuyaKnifeDownProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class SakuyaStunKnifeProj : Projectile {
	public bool once;
	public SakuyaStunKnifeProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		SakuyaProjW.netWeapon, pos, xDir,
		375, 1, player, "sakuya_knife_stun", 0, 1f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = false;
		maxTime = 0.85f;
		projId = (int)ProjIds.SakuyaKnifeStunProj;
		destroyOnHitWall = false;
		xScale = 0.75f;
		yScale = 0.75f;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}
	public override void update() {	
		if (!once) {
			playSound("knifestun", true, true);
			once = true;
		}
		base.update();
	}
	public override void onHitDamagable(IDamagable damagable) {
		if (damagable is not FrostShieldProjGround or FrostShieldProjAir or FrostShieldProjCharged
			or FrostShieldProj or FrostShieldProjPlatform or FrostShieldProjChargedGround 
			or GaeaShieldProj or ChillPIceProj) {
			base.onHitDamagable(damagable);
		}
	}
	public override void onCollision(CollideData other) {
		base.onCollision(other);
		if (!ownedByLocalPlayer) return;
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new SakuyaStunKnifeProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class SakuyaChainSawProj : Projectile {
	public bool once, once2, once3;
	public SakuyaChainSawProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		SakuyaProjW.netWeapon, pos, xDir,
		60, 1, player, "sakuya_chainsaw", 0, 0.15f,
		netId, player.ownedByLocalPlayer
	) {
		weapon = SakuyaProjW.netWeapon;
		vel = new Point(70*xDir, -320);
		damager.damage = 3;
		damager.flinch = 26;
		damager.hitCooldown = 7;
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = false;
		maxTime = 2.15f;
		projId = (int)ProjIds.SakuyaChainSawProj;
		destroyOnHitWall = false;
		gravityModifier = 0.4f;
		useGravity = true;
		xScale = 0.75f;
		yScale = 0.75f;
		angle = 0;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}
	public override void update() {
		if (xDir == -1) {
			angle += speedMul;
		} else {
			angle -= speedMul;
		}
		if (!once) {
			playSound("chainsawsound", true, true);
			once = true;
		}
		base.update();
	}
	public override void onHitDamagable(IDamagable damagable) {
		if (damagable is not FrostShieldProjGround or FrostShieldProjAir or FrostShieldProjCharged
			or FrostShieldProj or FrostShieldProjPlatform or FrostShieldProjChargedGround
			or GaeaShieldProj or ChillPIceProj) {
			base.onHitDamagable(damagable);
		}
	}
	public override void onCollision(CollideData other) {
		base.onCollision(other);
		if (!ownedByLocalPlayer) return;
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new SakuyaStunKnifeProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class SakuyaAutoAimKnife : Projectile {
	Actor? target;
	float kSpeed = 400;
	public SakuyaAutoAimKnife(
		Point pos, int xDir, Actor owner, Player player, ushort? netId, float angle, bool rpc = false
	) : base(
		pos, xDir, owner, "sakuya_knife_autoaim", netId, player
	) {
		weapon = XBuster.netWeapon;
		damager.damage = 1f;
		damager.hitCooldown = 10;
		damager.flinch = 0;
		angle = angle % 360;
		this.angle = angle;
		vel.x = kSpeed * Helpers.cosd(angle) * xDir;
		vel.y = kSpeed * Helpers.sind(angle) * xDir;
		if (target == null) {
			target = Global.level.getClosestTarget(pos, damager.owner.alliance, true, aMaxDist: 150);
		}
		reflectable = true;
		maxTime = 0.7f;
		projId = (int)ProjIds.SakuyaKnifeAutoAim;
		if (rpc) {
			rpcCreate(pos, owner, ownerPlayer, netId, xDir);
		}
		new Anim(
			pos, "sakuya_autoknife_effect", xDir, player.getNextActorNetId(), true, true) {xScale = 0.75f, yScale = 0.75f};
		canBeLocal = false;
		
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new SakuyaAutoAimKnife(
			args.pos, args.xDir, args.owner, args.player, args.netId, args.angle
		);
	}

	public override void update() {
		base.update();
		target = Global.level.getClosestTarget(pos, damager.owner.alliance, true, aMaxDist: 150);
		if (target != null) {
			var dTo = pos.directionTo(target.getCenterPos()).normalize();
			var destAngle = MathF.Atan2(dTo.y, dTo.x) * 180 / MathF.PI;
			destAngle = Helpers.to360(destAngle);
			angle = Helpers.lerpAngle(angle, destAngle, Global.spf * 3);
			vel.x = Helpers.cosd(angle) * kSpeed * xDir;
			vel.y = Helpers.sind(angle) * kSpeed * xDir;
		}
	}
}