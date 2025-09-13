/*
using System;
using System.Collections.Generic;
using SFML.Graphics;
using System.Linq;
using MMXOnline;
namespace MMXOnline;
public class Sakuya : Character {
public bool upPressed => player.input.isPressed(Control.Up, player);
public bool upHeld => player.input.isHeld(Control.Up, player);
public bool downHeld => player.input.isHeld(Control.Down, player);
public bool shootHeld => player.input.isHeld(Control.Shoot, player);
public bool specialHeld => player.input.isHeld(Control.Special1, player);
public bool specialPressed => player.input.isPressed(Control.Special1, player);
public bool WLeftPressed => player.input.isPressed(Control.WeaponLeft, player); 
public bool WRightPressed => player.input.isPressed(Control.WeaponRight, player); 
public bool shootPressed => player.input.isPressed(Control.Shoot, player);
public bool LeftOrRightHeld => player.input.isHeld(Control.Left, player) || player.input.isHeld(Control.Right, player);
public float AmmoCooldown;
public float onAir;
public int SpecialWeaponSelected = 0;
public float sakuyaheld;
public SakuyaMeleeW meleeWeapon = new();
public Sakuya(
		Player player, float x, float y, int xDir,
		bool isVisible, ushort? netId, bool ownedByLocalPlayer,
		bool isWarpIn = true
	) : base(
		player, x, y, xDir, isVisible, netId, ownedByLocalPlayer, isWarpIn, false, false
	) {
		charId = CharIds.Sakuya;	
		xScale = 0.685f;
		yScale = 0.685f;

	}
	public override void update() {
		base.update();
		if (!ownedByLocalPlayer) {
			return;
		}
		SakuyaAmmo();
		GlideLogic();
		SakuyaLoadout();
	}
	public override bool normalCtrl() {
		if (grounded) {
			if (player.input.isPressed(Control.Jump, player) && canJump()) {
				vel.y = -getJumpPower();
				changeState(new Jump());
				return true;
			} 
			if (player.isCrouchHeld() && canCrouch() && charState is not Crouch) {
				changeState(new Crouch());
				return true;
			}
			if (player.input.isPressed(Control.Taunt, player)) {
				changeState(new Taunt());
				return true;
			}
		}
		// Air normal states.
		else {
			if (canAirJump() && flag == null) {
				if (player.input.isPressed(Control.Jump, player) && canJump()) {
					lastJumpPressedTime = Global.time;
				}
				if ((player.input.isPressed(Control.Jump, player) ||
					Global.time - lastJumpPressedTime < 0.1f) &&
					wallKickTimer <= 0 && flag == null  && canJump()
				) {
					lastJumpPressedTime = 0;
					dashedInAir++;
					vel.y = -getJumpPower();
					changeState(new SakuyaJump(), true);
					return true;
				}
			} else {
				lastJumpPressedTime = 0;
			}
		}
		return false;
	}
	public override bool attackCtrl() {
		switch (player.SakuyaAmmo) {
			case >= 16:
				if (WRightPressed)
				changeState(new SakuyaThousandDagger(), true);
				break;
			case >= 10:
				break;
			case >= 6:
				break;
			case >= 4:
				break;
			case >= 1:
				break;
		}
		if (player.SakuyaAmmo > 0) {
			if (!grounded) {
				if (shootPressed) {
					if (upHeld) {
						changeState(new SakuyaAttackUpAir(), true);
						return true;
					}
					if (downHeld) {				
						changeState(new SakuyaAttackAirDown(), true);
						return true;
					}
					changeState(new SakuyaAttackAir(), true);
					return true;
				}
			}
			if (grounded) {
				if (shootPressed) {
					if (upHeld) {
						changeState(new SakuyaAttackUP(), true);
						return true;
					}
					if (charState is SakuyaWalk) {
						changeState(new SakuyaAttackRun(), false);			
						return true;
					}
					if (downHeld) {
						changeState(new SakuyaAttackDown(), true);
						return true;
					}
					if (vel.x <= 0)
					changeState(new SakuyaAttack(), true);
					return true;
				}
			}
		}
		return base.attackCtrl();
	}
	public override string getSprite(string spriteName) {
		return "sakuya_" + spriteName;
	}
	public override bool canCharge() {
		return false;
	}
	public override bool canEnterRideArmor() {
		return false;
	}
	public override bool canEnterRideChaser() {
		return false;
	}
	public override bool canDash() {
		return false;
	}
	public override bool canWallClimb() {
		return false;
	}
	public override bool canUseLadder() {
		return false;
	}
	public override void landingCode(bool useSound = true) {
		if (useSound) {
			playSound("sakuyaland", sendRpc: true);
		}
		dashedInAir = 0;
		changeState(new Idle("land"), true);
	}
	public override bool canAirJump() {
		return dashedInAir == 0;
	}
	public override float getRunSpeed() {
		float runSpeed = 2.25f * 60f;
		return runSpeed * getRunDebuffs();
	}
	public override float getJumpPower() {
		float jp = 5.5f * 60f;

		return jp * getJumpModifier();
	}
	public override void addAmmo(float amount) {
		weaponHealAmount += amount;
	}
	public override void addPercentAmmo(float amount) {
		weaponHealAmount += amount * 0.32f;
	}
	public override bool canAddAmmo() {
		return player.SakuyaAmmo < player.SakuyaMaxAmmo;
	}
	public override bool isAttacking() {
		return sprite.name.Contains("attack");
	}
	public override Collider getGlobalCollider() {
		Rect rect = new Rect(0, 0, 18, 27);
		if (sprite.name.Contains("attack_down")) {
			rect.y2 = 14;
		}
		return new Collider(rect.getPoints(), false, this, false, false, HitboxFlag.Hurtbox, new Point(0, 0));
	}
	public override Collider getCrouchingCollider() {
		var rect = new Rect(0, 0, 18, 14);
		return new Collider(rect.getPoints(), false, this, false, false, HitboxFlag.Hurtbox, new Point(0, 0));
	}
	public override Collider? getTerrainCollider() {
		Collider? overrideGlobalCollider = null;
		if (spriteToColliderMatch(sprite.name, out overrideGlobalCollider)) {
			return overrideGlobalCollider;
		}
		if (physicsCollider == null) {
			return null;
		}
		return new Collider(
			new Rect(0f, 0f, 18, 36).getPoints(),
			false, this, false, false,
			HitboxFlag.Hurtbox, new Point(0, 0)
		);
	}
	public void SakuyaAmmo() {
		if (player.SakuyaAmmo >= player.SakuyaMaxAmmo) {
			weaponHealAmount = 0;
		}
		if (weaponHealAmount > 0 && player.health > 0) {
			weaponHealTime += Global.spf;
			if (weaponHealTime > 0.05) {
				weaponHealTime = 0;
				weaponHealAmount--;
				player.SakuyaAmmo = Helpers.clampMax(player.SakuyaAmmo + 1, player.SakuyaMaxAmmo);
				AmmoCooldown = 0;
				playSound("heal", forcePlay: true);
			}
		}
		if (AmmoCooldown <= 0 && player.SakuyaAmmo <= player.SakuyaMaxAmmo) {
			player.SakuyaAmmo += Global.spf*1.5f;
		}
		if (isAttacking() && AmmoCooldown <= 2) {
			AmmoCooldown += Global.spf*2;
		}
		if (player.SakuyaAmmo < 0) {
			player.SakuyaAmmo = 0;
		}
		Helpers.decrementTime(ref AmmoCooldown);
	}
	public void GlideLogic() {
		if (!grounded) {
			onAir += Global.spf*10;
		}
		if (grounded || vel.y < 0 || charState is WarpIn) {
			onAir = 0;
			gravityModifier = 1;
		}
		if (onAir >= 1 && !isInvulnerable() && !isWarpOut() && charState is not SakuyaHurt) {
			if (player.input.isHeld(Control.Jump, player)) {
				gravityModifier = 0.1f;
				if (!isAttacking()) {
					changeSpriteFromName("gliding", false);
				}
			} else { 
				gravityModifier = 1;
				onAir = 0;
			}
		}
	}
	public void SakuyaLoadout() {
		//will do it better later
		if (specialHeld) {
			if (sakuyaheld < 2)
			sakuyaheld++;	
			if (sakuyaheld <= 1) {
				playSound("weaponopen", true, true);
			}
			if (upPressed) {
				SpecialWeaponSelected++;
				playSound("weaponswap", true, true);
				if (SpecialWeaponSelected >= 6) {
					SpecialWeaponSelected = 0;
				}
			}
		} else if (sakuyaheld > 0) {
			sakuyaheld--; 
			if (sakuyaheld == 0)
			playSound("weaponout", true, true);
		}
	}
	public override bool altCtrl(bool[] ctrls) {
		if (charState is SakuyaGenericMeleeState sgms) {
			sgms.altCtrlUpdate(ctrls);
		}
		return base.altCtrl(ctrls);
	}
	public override Projectile? getProjFromHitbox(Collider hitbox, Point centerPoint) {
		int meleeId = getHitboxMeleeId(hitbox);
		if (meleeId == -1) {
			return null;
		}
		Projectile? proj = getMeleeProjById(meleeId, centerPoint);
		if (proj == null) {
			return null;
		}
		// Assing data variables.
		proj.meleeId = meleeId;
		proj.owningActor = this;
		// Damage based on fall speed.
		return proj;
	}
	public enum MeleeIds {
		None = -1,
		KnifeDown,
	}
	public override int getHitboxMeleeId(Collider hitbox) {
		return (int)(sprite.name switch {
			"sakuya_attack_down" => MeleeIds.KnifeDown,
			_ => MeleeIds.None
		});
	}
	public override Projectile? getMeleeProjById(int id, Point projPos, bool addToLevel = true) {
		return id switch {
			(int)MeleeIds.KnifeDown => new GenericMeleeProj(meleeWeapon, projPos, 
			ProjIds.SakuyaKnifeDown, player, 2, 4, 0.5f, addToLevel: addToLevel),
			_ => null
		};
	}
	public override List<byte> getCustomActorNetData() {
		List<byte> customData = base.getCustomActorNetData();
		customData.Add((byte)MathF.Floor(player.SakuyaAmmo));

		customData.Add(Helpers.boolArrayToByte([
		]));

		return customData;
	}
	public override void updateCustomActorNetData(byte[] data) {
		// Update base arguments.
		base.updateCustomActorNetData(data);
		data = data[data[0]..];

		// Per-player data.
		player.SakuyaAmmo = data[0];
		bool[] flags = Helpers.byteToBoolArray(data[2]);
	}
}
public class SakuyaMeleeW : Weapon {
	public static SakuyaMeleeW netWeapon = new();
	public SakuyaMeleeW() : base() {
		index = (int)WeaponIds.SakuyaMeleeW;
		killFeedIndex = 4;
		weaponBarBaseIndex = 0;
		weaponBarIndex = weaponBarBaseIndex;
		weaponSlotIndex = 0;
		shootSounds = new string[] { "", "", "", "" };
		fireRate = 0.15f;
		description = new string[] { "" };
	}
}
public class SakuyaProjW : Weapon {
	public static SakuyaProjW netWeapon = new();
	public SakuyaProjW() : base() {
		index = (int)WeaponIds.SakuyaProjW;
		killFeedIndex = 4;
		weaponBarBaseIndex = 0;
		weaponBarIndex = weaponBarBaseIndex;
		weaponSlotIndex = 0;
		shootSounds = new string[] { "", "", "", "" };
		fireRate = 0.15f;
		description = new string[] { "" };
	}
}
public class SakuyaWalk : CharState {
	public bool skip;
	public SakuyaWalk(string transitionSprite = "", bool skip = false) : base("walk", transitionSprite: skip ? "walk" : "walk_start") {
		accuracy = 5;
		exitOnAirborne = true;
		attackCtrl = true;
		normalCtrl = true;
	}

	public override void update() {
		base.update();
		
		var move = new Point(0, 0);
		float runSpeed = character.getRunSpeed();
		bool pressed = player.input.isPressed(Control.Left, player) || player.input.isPressed(Control.Right, player);
		if (stateFrames <= 4) {
			runSpeed = 60 * character.getRunDebuffs();
		}
		if (player.input.isHeld(Control.Left, player)) {
			character.xDir = -1;
			if (player.character.canMove()) move.x = -runSpeed;
		} else if (player.input.isHeld(Control.Right, player)) {
			character.xDir = 1;
			if (player.character.canMove()) move.x = runSpeed;
		}
		if (pressed && move.magnitude > 0) {
			character.changeState(new SakuyaWalkBack());
		}
		if (move.magnitude > 0) {
			character.move(move);
		} else {
			character.changeToIdleOrFall("run_stop");
		}
	}
}
public class SakuyaWalkBack : CharState {
	public SakuyaWalkBack() : base("walk_back") {
		accuracy = 5;
		exitOnAirborne = true;
		attackCtrl = true;
		normalCtrl = true;
	}

	public override void update() {
		base.update();
		var move = new Point(0, 0);
		float runSpeed = character.getRunSpeed();
		if (stateFrames <= 4) {
			runSpeed = 60 * character.getRunDebuffs();
		}
		if (player.input.isHeld(Control.Left, player)) {
			character.xDir = -1;
			if (player.character.canMove()) move.x = -runSpeed;
		} else if (player.input.isHeld(Control.Right, player)) {
			character.xDir = 1;
			if (player.character.canMove()) move.x = runSpeed;
		}
		if (move.magnitude > 0) {
			character.move(move);
		} else {
			character.changeToIdleOrFall("run_stop");
		}
		if (character.isAnimOver()) {
			character.changeState(new SakuyaWalk(skip: true));		
		}
	}
}
public class SakuyaJump : CharState {
	public bool fired;
	public SakuyaJump() : base("jump_double") {
		accuracy = 5;
		exitOnLanding = true;
		useDashJumpSpeed = true;
		airMove = true;
		canStopJump = true;
		attackCtrl = true;
		normalCtrl = true;
	}

	public override void update() {
		base.update();
		if (character.frameIndex == 0 && !fired) {
			fired = true;
			character.playSound("knife", true, true);
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f, yScale = 0.75f, angle = 90};
			Global.level.delayedActions.Add( new DelayedAction(() => {
				new Anim(character.getShootPos().addxy(0,12), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.45f, yScale = 0.45f, angle = 90 };}, 4/60f));
			Global.level.delayedActions.Add( new DelayedAction(() => {
				new Anim(character.getShootPos().addxy(0,24), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.25f, yScale = 0.25f, angle = 90 };}, 12/60f));

			new SakuyaKnifeDownProj(character.getShootPos().addxy(5,-10),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeDownProj(character.getShootPos().addxy(-10,-10),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);;
		}
		if (character.vel.y > 0) {
			character.changeState(new Fall());
			return;
		}
	}
}
public abstract class SakuyaGenericMeleeState : CharState {
	public Sakuya sakuya = null!;
	public int comboFrame = Int32.MaxValue;
	public string sound = "";
	public bool soundPlayed;
	public int soundFrame = Int32.MaxValue;
	public bool exitOnOver = true;
	public SakuyaGenericMeleeState(string spr) : base(spr) {
	}
	public override void update() {
		base.update();
		if (character.sprite.frameIndex >= soundFrame && !soundPlayed) {
			character.playSound(sound, forcePlay: false, sendRpc: true);
			soundPlayed = true;
		}
		if (character.sprite.frameIndex >= comboFrame) {
			altCtrls[0] = true;
		}
		if (exitOnOver && character.isAnimOver()) {
			character.changeToIdleOrFall();
		}
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		sakuya = character as Sakuya ?? throw new NullReferenceException();
	}
	public virtual bool altCtrlUpdate(bool[] ctrls) {
		return false;
	}
}
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
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new SakuyaKnifeDownProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class SakuyaAttack : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttack() : base("attack") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 3;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f,
				yScale = 0.75f
			};
			new SakuyaKnifeProj(character.getShootPos(),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-16,8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-12,-8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttack2(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttack2 : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttack2() : base("attack2") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 3;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f,
				yScale = 0.75f
			};
			new SakuyaKnifeProj(character.getShootPos(),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-16,8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-12,-8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttack3(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttack3 : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttack3() : base("attack3") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 3;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f,
				yScale = 0.75f
			};
			new SakuyaKnifeProj(character.getShootPos(),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-16,8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-12,-8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttack4(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttack4 : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttack4() : base("attack4") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 3;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f,
				yScale = 0.75f
			}; 
			new SakuyaKnifeProj(character.getShootPos(),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-16,8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-12,-8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttack(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackDown : SakuyaGenericMeleeState {
	int loop;
	public SakuyaAttackDown() : base("attack_down") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 3;
	}
	public override void update() {
		if (character.frameIndex == 3 && loop < 12) {
			loop++;
		}
		if (loop >= 12) {
			character.changeToIdleOrFall();
		}
		character.move(new Point(character.xDir * 200, 0));
		base.update();
	}
}
public class SakuyaAttackRun : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackRun() : base("attack_run") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 4;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f,
				yScale = 0.75f
			};
			new SakuyaKnifeProj(character.getShootPos(),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-16,8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-12,-8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		if (character.isAnimOver()) {
			character.changeState(new SakuyaWalk(skip: false), true);
		}
		var move = new Point(0, 0);
		float runSpeed = character.getRunSpeed();
		if (player.input.isHeld(Control.Left, player)) {
			character.xDir = -1;
			if (player.character.canMove()) move.x = -runSpeed;
		} else if (player.input.isHeld(Control.Right, player)) {
			character.xDir = 1;
			if (player.character.canMove()) move.x = runSpeed;
		}
		if (move.magnitude > 0) {
			character.move(move);
		}
		if (move.magnitude <= 0) {
			character.changeToIdleOrFall("run_stop");
		}
		
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackRun2(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackRun2 : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackRun2() : base("attack_run2") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 3;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f,
				yScale = 0.75f
			};
			new SakuyaKnifeProj(character.getShootPos(),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-16,8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-12,-8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		if (character.isAnimOver()) {
			character.changeState(new SakuyaWalk(skip: false), true);
		}
		var move = new Point(0, 0);
		float runSpeed = character.getRunSpeed();
		if (player.input.isHeld(Control.Left, player)) {
			character.xDir = -1;
			if (player.character.canMove()) move.x = -runSpeed;
		} else if (player.input.isHeld(Control.Right, player)) {
			character.xDir = 1;
			if (player.character.canMove()) move.x = runSpeed;
		}
		if (move.magnitude > 0) {
			character.move(move);
		}
		if (move.magnitude <= 0) {
			character.changeToIdleOrFall("run_stop");
		}
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackRun3(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackRun3 : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackRun3() : base("attack_run3") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 3;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f,
				yScale = 0.75f
			};
			new SakuyaKnifeProj(character.getShootPos(),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-16,8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-12,-8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		if (character.isAnimOver()) {
			character.changeState(new SakuyaWalk(skip: false), true);
		}
		var move = new Point(0, 0);
		float runSpeed = character.getRunSpeed();
		if (player.input.isHeld(Control.Left, player)) {
			character.xDir = -1;
			if (player.character.canMove()) move.x = -runSpeed;
		} else if (player.input.isHeld(Control.Right, player)) {
			character.xDir = 1;
			if (player.character.canMove()) move.x = runSpeed;
		}
		if (move.magnitude > 0) {
			character.move(move);
		}
		if (move.magnitude <= 0) {
			character.changeToIdleOrFall("run_stop");
		}
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackRun4(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackRun4 : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackRun4() : base("attack_run4") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 3;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f,
				yScale = 0.75f
			};
			new SakuyaKnifeProj(character.getShootPos(),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-16,8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-12,-8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		if (character.isAnimOver()) {
			character.changeState(new SakuyaWalk(skip: false), true);
		}
		var move = new Point(0, 0);
		float runSpeed = character.getRunSpeed();
		if (player.input.isHeld(Control.Left, player)) {
			character.xDir = -1;
			if (player.character.canMove()) move.x = -runSpeed;
		} else if (player.input.isHeld(Control.Right, player)) {
			character.xDir = 1;
			if (player.character.canMove()) move.x = runSpeed;
		}
		if (move.magnitude > 0) {
			character.move(move);
		}
		if (move.magnitude <= 0) {
			character.changeToIdleOrFall("run_stop");
		}
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackRun(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackUP : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackUP() : base("attack_up") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 5;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getCenterPos().addxy(0,-10), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f, yScale = 0.75f, angle = 90
			};
			new SakuyaKnifeUpProj(character.getShootPos().addxy(12,26),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeUpProj(character.getShootPos().addxy(0,18),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);;
		}
		
		var move = new Point(0, 0);
		float runSpeed = character.getRunSpeed();
		if (player.input.isHeld(Control.Left, player)) {
			character.xDir = -1;
			if (player.character.canMove()) move.x = -runSpeed;
			if (move.magnitude > 0) character.changeSpriteFromName("sakuya_attack_up_run", false);
		} else if (player.input.isHeld(Control.Right, player)) {
			character.xDir = 1;
			if (player.character.canMove()) move.x = runSpeed;
		}
		if (move.magnitude > 0) {
			character.move(move);
		}
			
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackUP2(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackUPRun : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackUPRun() : base("attack_up_run") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 5;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getCenterPos().addxy(0,-10), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f, yScale = 0.75f, angle = 90
			};
			new SakuyaKnifeUpProj(character.getShootPos().addxy(12*character.xDir,26),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeUpProj(character.getShootPos().addxy(0,18),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);;
		}
		if (character.isAnimOver()) {
			character.changeState(new SakuyaWalk(skip: false), true);
		}
		var move = new Point(0, 0);
		float runSpeed = character.getRunSpeed();
		if (player.input.isHeld(Control.Left, player)) {
			character.xDir = -1;
			if (player.character.canMove()) move.x = -runSpeed;
		} else if (player.input.isHeld(Control.Right, player)) {
			character.xDir = 1;
			if (player.character.canMove()) move.x = runSpeed;
		}
		if (move.magnitude > 0) {
			character.move(move);
		}
		if (move.magnitude <= 0) {
			character.changeToIdleOrFall("run_stop");
		}
		
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackUP2(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackUP2 : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackUP2() : base("attack_up2") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 5;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getCenterPos().addxy(0,-10), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f, yScale = 0.75f, angle = 90
			};
			new SakuyaKnifeUpProj(character.getShootPos().addxy(12*character.xDir,26),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeUpProj(character.getShootPos().addxy(0,18),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		if (character.isAnimOver()) {
			character.changeState(new SakuyaWalk(skip: false), true);
		}
		var move = new Point(0, 0);
		float runSpeed = character.getRunSpeed();
		if (player.input.isHeld(Control.Left, player)) {
			character.xDir = -1;
			if (player.character.canMove()) move.x = -runSpeed;
		} else if (player.input.isHeld(Control.Right, player)) {
			character.xDir = 1;
			if (player.character.canMove()) move.x = runSpeed;
		}
		if (move.magnitude > 0) {
			character.move(move);
		}
		
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackUP(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackUpAir : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackUpAir() : base("attack_up_air") {
		sound = "knife";
		landSprite = "attack_up";
		airSprite = "attack_up_air";
		soundFrame = 1;
		comboFrame = 5;
		airMove = true;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getCenterPos().addxy(0,-10), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f, yScale = 0.75f, angle = 90
			};
			new SakuyaKnifeUpProj(character.getShootPos().addxy(12*character.xDir,26),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeUpProj(character.getShootPos().addxy(0,18),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		if (character.isAnimOver()) {
			character.changeToIdleOrFall();	
		}		
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackUpAir2(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackUpAir2 : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackUpAir2() : base("attack_up2") {
		sound = "knife";
		landSprite = "attack_up2";
		airSprite = "attack_up2";
		soundFrame = 1;
		comboFrame = 5;
		airMove = true;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getCenterPos().addxy(0,-10), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f, yScale = 0.75f, angle = 90
			};
			new SakuyaKnifeUpProj(character.getShootPos().addxy(12*character.xDir,26),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeUpProj(character.getShootPos().addxy(0,18),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		if (character.isAnimOver()) {
			character.changeToIdleOrFall();	
		}		
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackUpAir(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackAir : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackAir() : base("attack_air") {
		sound = "knife";
		landSprite = "attack";
		airSprite = "attack_air";		
		soundFrame = 1;
		comboFrame = 2;
		airMove = true;
		canStopJump = true;
		normalCtrl = true;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f,
				yScale = 0.75f,
			};
			new SakuyaKnifeProj(character.getShootPos(),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-16,8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeProj(character.getShootPos().addxy(-12,-8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		if (sakuya.isAnimOver()) 
		if (sakuya.grounded && sakuya.LeftOrRightHeld) {
			sakuya.changeState(new SakuyaWalk(skip: true), false);
		}
		if (character.isAnimOver()) {
			character.changeToIdleOrFall();	
		}
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0 && !sakuya.grounded) {
			sakuya.changeState(new SakuyaAttackAir(), true);
			return true;
		}
		if (sakuya.shootPressed && player.SakuyaAmmo > 0 && sakuya.grounded && sakuya.LeftOrRightHeld) {
			sakuya.changeState(new SakuyaAttackRun(), true);
			return true;
		}
		if (sakuya.shootPressed && player.SakuyaAmmo > 0 && sakuya.grounded) {
			sakuya.changeState(new SakuyaAttack(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackAirDown : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackAirDown() : base("attack_air_down") {
		sound = "knife";
		soundFrame = 1;
		comboFrame = 3;
		exitOnLanding = true;
		useDashJumpSpeed = true;
		airMove = true;
		attackCtrl = true;
		normalCtrl = true;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			character.vel.y = -sakuya.getJumpPower() * 0.225f;
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f, yScale = 0.75f,angle = 60 * character.xDir	};
			new SakuyaKnifeDProj(character.getShootPos(),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeDProj(character.getShootPos().addxy(-6*character.xDir,12),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			new SakuyaKnifeDProj(character.getShootPos().addxy(4*character.xDir,-8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		if (character.isAnimOver()) {
			character.changeToIdleOrFall();	
		}
		base.update();
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackAirDown(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackStunKnife : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackStunKnife() : base("attack") {
		sound = "knife";
		soundFrame = 1;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			player.SakuyaAmmo -= 4;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f, yScale = 0.75f };
		}
		base.update();
	}
}
public class SakuyaChainSaw : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaChainSaw() : base("attack") {
		sound = "knife";
		soundFrame = 1;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			player.SakuyaAmmo -= 4;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f, yScale = 0.75f };
		}
		base.update();
	}
}
public class SakuyaThousandDagger : SakuyaGenericMeleeState {
	public bool fired;
	public int loop;
	public float shoottime;
	public SakuyaThousandDagger() : base("attack_hold") {
		sound = "knife";
		soundFrame = 1;
	}
	public override void update() {
		if (character.frameIndex == 1 || character.frameIndex == 5
			 || character.frameIndex == 9 || character.frameIndex == 13 && !fired) {
				fired = true;
				new Anim(character.getShootPos(), "sakuya_knife_effect",
				character.getShootXDir(), player.getNextActorNetId(), true, true) {
					xScale = 0.75f, yScale = 0.75f };
				new SakuyaKnifeProj(character.getShootPos(),
				character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
				new SakuyaKnifeProj(character.getShootPos().addxy(-16,8),
				character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
				new SakuyaKnifeProj(character.getShootPos().addxy(-12,-8),
				character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		if (shoottime > 0.4) {
			shoottime = 0;
			character.playSound("knife", true, true);		
		}
		if (character.frameIndex == 13 && loop < 3) {
			character.frameIndex = 0;
			loop++;
			fired = false;
		}
		if (character.isAnimOver()) {
			character.changeToIdleOrFall();
		}
		base.update();
	}
}
public class SakuyaHurt : CharState {
	public float flinchYPos;
	public bool isCombo;
	public int hurtDir;
	public float hurtSpeed;
	public float flinchTime;
	public bool spiked;

	public SakuyaHurt(int dir, int flinchFrames, bool spiked = false, float? oldComboPos = null) : base("hurt") {
		this.flinchTime = flinchFrames;
		hurtDir = dir;
		hurtSpeed = dir * 1.6f;
		flinchTime = flinchFrames;
		this.spiked = spiked;
		if (oldComboPos != null) {
			isCombo = true;
			flinchYPos = oldComboPos.Value;
		}
	}

	public bool isMiniFlinch() {
		return flinchTime <= 25;
	}

	public override bool canEnter(Character character) {
		if (character.isCCImmune()) return false;
		if (character.vaccineTime > 0) return false;
		if (character.rideArmorPlatform != null) return false;
		return base.canEnter(character);
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		if (!spiked) {
			character.vel.y = (-0.125f * (flinchTime - 1)) * 30f;
		}
	}

	public override void update() {
		base.update();
		if (hurtSpeed != 0) {
			hurtSpeed = Helpers.toZero(hurtSpeed, 1.6f / flinchTime  * Global.speedMul, hurtDir);
			character.move(new Point(hurtSpeed * 60f, 0));
		}
		if (isMiniFlinch()) {
			character.changeState(new SakuyaHurt2(), true);
		}

		if (stateFrames >= flinchTime) {
			character.changeToLandingOrFall(false);
		}
	}
	public override void onExit(CharState newState) {
		character.invulnTime = 24/60f;
		base.onExit(newState);
	}
}
public class SakuyaHurt2 : CharState {
	public SakuyaHurt2() : base("hurt2") {	
	}
	public override bool canEnter(Character character) {
		if (character.isCCImmune()) return false;
		if (character.vaccineTime > 0) return false;
		return base.canEnter(character);
	}
	public override void update() {
		base.update();
		if (character.isAnimOver()) {
			character.changeToIdleOrFall();
			return;
		}
	}
	public override void onExit(CharState newState) {
		character.invulnTime = 12f/60f;
		base.onExit(newState);
	}
}
*/
