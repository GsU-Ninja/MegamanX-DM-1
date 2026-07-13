using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace MMXOnline;

public enum AirSpecialType {
	Kuuenzan,
	FSplasher,
	Hyoroga
}

public class KuuenzanWeapon : Weapon {
	public static KuuenzanWeapon staticWeapon = new();

	public KuuenzanWeapon() : base() {
		//damager = new Damager(player, 3, 0, 0.5f);
		index = (int)WeaponIds.Kuuenzan;
		weaponBarBaseIndex = 21;
		weaponBarIndex = weaponBarBaseIndex;
		weaponSlotIndex = 48;
		killFeedIndex = 121;
		type = (int)AirSpecialType.Kuuenzan;
		displayName = "Kuuenzan";
		description = new string[] { "Standard spin attack in the air." };
		damage = "1";
		hitcooldown = "0.125";
		flinch = "0";
		effect = "";
	}

	public static Weapon getWeaponFromIndex(int index) {
		return index switch {
			(int)AirSpecialType.Kuuenzan => new KuuenzanWeapon(),
			(int)AirSpecialType.FSplasher => new FSplasherWeapon(),
			(int)AirSpecialType.Hyoroga => new HyorogaWeapon(),
			_ => throw new Exception("Invalid Zero air special weapon index!")
		};
	}
}

public class FSplasherWeapon : Weapon {
	public static FSplasherWeapon staticWeapon = new();

	public FSplasherWeapon() : base() {
		//damager = new Damager(player, 3, Global.defFlinch, 0.06f);
		index = (int)WeaponIds.FSplasher;
		killFeedIndex = 109;
		type = (int)AirSpecialType.FSplasher;
		displayName = "Hisuishou";
		description = new string[] { "A Mobile aerial slightly faster Dash." };
		damage = "2";
		hitcooldown = "0.5";
		flinch = "0";
		effect = "Can be canceled by other attacks.";
	}

	public override void attack(Character character) {
		if (character is not Zero zero) {
			return;
		}
		if (zero.fSplasherCooldown > 0) return;
		if (zero.fSplasherUses > 0) return;
		if (zero.airRisingUses > 0) return;
		zero.fSplasherCooldown = 50;
		zero.fSplasherUses++;
		zero.airRisingUses++;
		character.changeState(new FSplasherState(), true);
	}
}

public class FSplasherState : ZeroState {
	public float dashTime = 0;
	public Projectile? fSplasherProj;
	public Anim? ProjVisible;

	public FSplasherState() : base("dash") {
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		ProjVisible = new Anim(
			character.pos, "fsplasher_proj", character.xDir,
			player.getNextActorNetId(), false, sendRpc: true
		);
		character.isDashing = true;
		character.useGravity = false;
		character.vel = new Point(0, 0);
		character.dashedInAir++;
		fSplasherProj = new FSplasherProj(
			character.pos, character.xDir, zero,
			player, player.getNextActorNetId(), sendRpc: true
		);
		zero.playSound("fsplasher", false, true);
	}

	public override void onExit(CharState? newState) {
		character.useGravity = true;
		if (fSplasherProj != null) {
			fSplasherProj.destroySelf();
			fSplasherProj = null;
		}
		if (ProjVisible != null) {
			ProjVisible.destroySelf();
			ProjVisible = null;
		}
		base.onExit(newState);
	}

	public override bool canEnter(Character character) {
		if (!base.canEnter(character)) return false;
		if (character.charState is WallSlide) return false;
		return character.flag == null;
	}

	public override void update() {
		base.update();
		float upSpeed = 0;
		var inputDir = player.input.getInputDir(player);
		if (inputDir.y < 0) upSpeed = -1;
		else if (inputDir.y > 0) upSpeed = 1;
		else upSpeed = 0;

		if (fSplasherProj != null) {
			fSplasherProj.changePos(character.pos);
		}
		if (ProjVisible != null) {
			ProjVisible.changePos(character.pos);
		}
		CollideData? collideData = Global.level.checkTerrainCollisionOnce(
			character, character.xDir, upSpeed
		);
		if (collideData != null) {
			character.changeState(character.getFallState(), true);
			return;
		}

		float modifier = 1f;
		dashTime += Global.spf;
		if (dashTime > 0.6) {
			character.changeState(character.getFallState());
			return;
		}

		var move = new Point(0, 0);
		move.x = character.getDashSpeed() * character.xDir * modifier;
		move.y = upSpeed * 1.65f;
		character.movePoint(move);
		if (stateTime > 0.1) {
			stateTime = 0;
		}
	}
}

public class FSplasherProj : Projectile {
	public FSplasherProj(
		Point pos, int xDir, Actor owner, Player player, ushort? netId, bool sendRpc = false
	) : base(
		pos, xDir, owner, "fsplasher_proj", netId, player
	) {
		weapon = FSplasherWeapon.staticWeapon;
		damager.damage = 2;
		damager.hitCooldown = 30;
		projId = (int)ProjIds.FSplasher;
		setIndestructableProperties();
		visible = false;
		if (sendRpc) {
			rpcCreate(pos, owner, ownerPlayer, netId, xDir);
		}
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new FSplasherProj(
			args.pos, args.xDir, args.owner, args.player, args.netId
		);
	}
}

public class HyorogaWeapon : Weapon {
	public static HyorogaWeapon staticWeapon = new();

	public HyorogaWeapon() : base() {
		//damager = new Damager(player, 3, Global.defFlinch, 0.06f);
		index = (int)WeaponIds.Hyoroga;
		killFeedIndex = 108;
		type = (int)AirSpecialType.Hyoroga;
		displayName = "Hyoroga";
		description = new string[] { "Cling to ceilings and rain down icicles with ATTACK." };
		damage = "4-3";
		hitcooldown = "0.25-0.15";
		flinch = "0-13";
		effect = "Can Freeze enemies on contact.";
	}

	public override void attack(Character character) {
		//if (character.charState is Fall) return;
		for (int i = 1; i <= 4; i++) {
			CollideData? collideData = Global.level.checkTerrainCollisionOnce(character, 0, -12 * i, autoVel: true);
			if (collideData != null && collideData.gameObject is Wall wall
				&& !wall.isMoving && !wall.topWall && collideData.isCeilingHit()
			) {
				character.changeState(new HyorogaStartState(), true);
				return;
			}
		}
	}
}

public class HyorogaStartState : CharState {
	public HyorogaStartState() : base("hyoroga_rise") {
		specialId = SpecialStateIds.HyorogaStart;
	}

	public override void update() {
		base.update();
		if (stateTime > 60f / 60f) {
			character.changeToIdleOrFall();
		}
		if (character.sprite.name == "zero_hyoroga_rise") {
			if (character.deltaPos.isCloseToZero()) {
				character.changeSprite("zero_hyoroga_start", true);
				character.gravityModifier = -1;
				character.useGravity = true;
			}
		} else if (character.sprite.name == "zero_hyoroga_start") {
			if (character.isAnimOver()) {
				character.changeState(new HyorogaState(), true);
			}
		}
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		character.vel = new Point(0, 0);
		character.gravityModifier = -1;
		character.dashedInAir = 0;

	}

	public override void onExit(CharState? newState) {
		character.useGravity = true;
		character.gravityModifier = 1;
		base.onExit(newState);
	}
}

public class HyorogaState : ZeroState {
	public HyorogaState() : base("hyoroga") {
		normalCtrl = true;
	}
	public override void update() {
		hyorogaReuse(zero);
		base.update();
		if (player.input.isPressed(Control.Special1, player)) {
			character.changeState(new HyorogaStateA(), true);
		}
		if (player.input.isPressed(Control.Shoot, player) && zero.isCharging()) {
			character.changeState(new HyorogaStateB(), true);
		}
		if (player.input.isPressed(Control.Jump, player)) {
			character.changeState(character.getFallState(), true);
		}
		character.turnToInput(player.input, player);
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		hyorogaReuse(zero);
		zero.airRisingUses = 0;
		zero.fallSaberAnimationCounter = 0;
	}
	public override void onExit(CharState? newState) {
		character.useGravity = true;
		character.gravityModifier = 1;
		base.onExit(newState);
	}
	public static void hyorogaReuse(Character character) {
		character.vel = new Point(0, 0);
		character.useGravity = false;
		character.gravityModifier = 0;
	}
}
public class HyorogaStateA : ZeroState {
	public HyorogaStateA() : base("hyoroga_attack") {
	}
	public override void update() {
		HyorogaState.hyorogaReuse(zero);
		base.update();
		var pois = character.sprite.getCurrentFrame().POIs;
		if (pois != null && pois.Length > 0 && !once) {
			once = true;
			var poi = character.getFirstPOIOrDefault();
			new HyorogaProj(
				poi, 0, 1, zero,
				player, player.getNextActorNetId(), sendRpc: true
			);
			new HyorogaProj(
				poi, 1, 1, zero,
				player, player.getNextActorNetId(), sendRpc: true
			);
			new HyorogaProj(
				poi, -1, 1, zero,
				player, player.getNextActorNetId(), sendRpc: true
			);
			if (zero.isAwakened && !zero.isGenmuZero && zero.hadangekiCooldown <= 0) {
				zero.hadangekiCooldown = 60;
				new ZSaberProj(
					player.input.isHeld(Control.Left, player) ? poi.addxy(-20,-40) :
					player.input.isHeld(Control.Right, player) ? poi.addxy(20,-40) 
					: poi.addxy(0,-40),
					character.xDir, isAZ: zero.isAwakened ? true : false,
					zero, player, player.getNextActorNetId(), rpc: true
				);
			} else if (zero.isGenmuZero && zero.genmureiCooldown <= 0) {
				zero.genmureiCooldown = 120;
				new GenmuHyorogaProj(
					character.pos.addxy(-55, 10), 1,
					isAZ: zero.isAwakened ? true : false,
					zero, player, player.getNextActorNetId(), rpc: true
				);
			}
		}
		if (character.isAnimOver()) {
			character.changeState(new HyorogaState(), true);
		}
	}
	public override void onExit(CharState? newState) {
		character.useGravity = true;
		character.gravityModifier = 1;
		base.onExit(newState);
	}
}
public class HyorogaStateB : ZeroState {
	public bool fired;

	public HyorogaStateB() : base("hyoroga_shoot") {
		normalCtrl = true;
		attackCtrl = true;
	}

	public override void update() {
		HyorogaState.hyorogaReuse(zero);
		base.update();
		if (!fired && character.frameIndex == 3) {
			fired = true;
			if (zero.isAwakened) {
				zero.chargeLogic(zero.shootDonuts);
			} else {
				zero.chargeLogic(zero.shoot);
			}
		}
		if (character.isAnimOver() && fired) {
			character.changeState(new HyorogaState(), true);
		}
	}
	public override void onExit(CharState? newState) {
		character.useGravity = true;
		character.gravityModifier = 1;
		base.onExit(newState);
	}
}
public class HyorogaProj : Projectile {
	public HyorogaProj(
		Point pos, int velDir, int xDir, Actor owner, Player player, ushort? netId, bool sendRpc = false
	) : base(
		pos, xDir, owner, "hyoroga_proj", netId, player
	) {
		weapon = HyorogaWeapon.staticWeapon;
		damager.damage = 3;
		damager.hitCooldown = 9;
		damager.flinch = Global.halfFlinch;
		projId = (int)ProjIds.HyorogaProj;
		destroyOnHit = true;
		destroyOnHitWall = true;
		this.vel.x = velDir * 250 * 0.375f;
		this.vel.y = 250;
		maxTime = 0.4f;

		if (sendRpc) {
			rpcCreate(pos, owner, ownerPlayer, netId, xDir, (byte)(velDir + 128));
		}
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new HyorogaProj(
			args.pos, args.extraData[0] - 128, args.xDir, args.owner, args.player, args.netId
		);
	}

	public override void onDestroy() {
		base.onDestroy();
		playSound("iceBreak");
		Anim.createGibEffect("hyoroga_proj_pieces", getCenterPos(), owner);
	}
}
public class GenmuHyorogaProj : Projectile {
	public float initY = 0;
	public float initX = 0;

	public GenmuHyorogaProj(
		Point pos, int xDir, bool isAZ, Actor owner, Player player, ushort? netId, bool rpc = false
	) : base(
		pos, xDir, owner, "genmu_proj2", netId, player
	) {
		weapon = Genmu.netWeapon;
		damager.damage = 6;
		damager.hitCooldown = 30;
		damager.flinch = Global.defFlinch;
		vel = new Point(0, 300);
		initX = owner.pos.x;
		maxTime = 0.45f;
		destroyOnHit = false;
		xScale = 0.75f;
		yScale = 0.75f;
		projId = (int)ProjIds.GenmuHyorogaProj;
		angle = 90;
		if (isAZ) {
			genericShader = player.zeroAzPaletteShader;
		}
		if (rpc) {
			rpcCreate(pos, player, netId, xDir, 
			new byte[] { isAZ ? (byte)1 : (byte)0}
			);
		}
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new GenmuHyorogaProj(
			args.pos, args.xDir, args.extraData[0] == 1, args.owner, args.player, args.netId
		);
	}
	public List<Point> getPoints() {
		Point pos1 = new Point(45, 30);
		Point pos2 = new Point(100, 5);
		var points = new List<Point> {
			pos,
			pos.add(pos1),
			pos.add(pos2),
		};
		return points;
	}


	public override void update() {
		base.update();
		if (globalCollider == null) {
			globalCollider = new Collider(getPoints(), true, null, false, false, 0, new Point(0, 0));
		} else {
			changeGlobalCollider(getPoints());
		}
		float x = 0;
		x = initX + (MathF.Sin(-time * 8) * 50) - 20;
		changePos(new Point(x, pos.y));
	}
	public override void render(float x, float y) {
		base.render(x, y);
	}
}