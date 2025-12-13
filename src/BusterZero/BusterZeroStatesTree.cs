using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using SFML.Graphics;

namespace MMXOnline;
public abstract class BusterZeroGenericMeleeState : CharState {
	public BusterZeroTree zero = null!;
	public int comboFrame = Int32.MaxValue;
	public string sound = "";
	public bool soundPlayed;
	public int soundFrame = Int32.MaxValue;
	public bool exitOnOver = true;
	public BusterZeroGenericMeleeState(string spr) : base(spr) {
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
	public void ZSaberExtend(int x, int y, bool follow, int xF, int yF) {
		if (player.BZZSaberExtend || zero.isBlackZero) 
			new BZZSaberExtend(
				follow, xF, yF, character.pos.addxy(x * character.xDir, y), character.xDir,
				player, player.getNextActorNetId(), rpc: true
			);
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		character.turnToInput(player.input, player);
		zero = character as BusterZeroTree ?? throw new NullReferenceException();
	}

	public virtual bool altCtrlUpdate(bool[] ctrls) {
		return false;
	}
}
public class BusterZeroDoubleBusterTree : BusterZeroGenericMeleeState {
	public bool fired1;
	public bool fired2;
	public bool isSecond;
	public bool isPinkCharge;
	public bool shootPressedAgain;
	public int startStockLevel;

	public BusterZeroDoubleBusterTree(bool isSecond, int startstockLevel) : base("doublebuster") {
		this.isSecond = isSecond;
		this.startStockLevel = startstockLevel;
		useDashJumpSpeed = true;
		airMove = true;
		superArmor = false;
		canStopJump = true;
		canJump = true;
		landSprite = "doublebuster";
		airSprite = "doublebuster_air";
	}

	public override void update() {
		base.update();
		if (player.input.isPressed(Control.Shoot, player)) {
			shootPressedAgain = true;
		}
		if (!fired1 && character.frameIndex == 3) {
			fired1 = true;
			zero.Buster3Proj(0, 3, zero);
			zero.stockedTime = 0;
		}
		if (!fired2 && character.frameIndex == 7) {
			fired2 = true;
			if (!isPinkCharge) {
				zero.stockedBusterLv = 0;
				zero.Buster3Proj(0, 3, zero);
			} else {
				zero.stockedBusterLv = 0;
				zero.Buster2Proj(zero);
			}
			zero.stockedTime = 0;
		}
		if (character.isAnimOver()) {
			character.changeToIdleOrFall();
		} else if (!isSecond && character.frameIndex >= 4 && !shootPressedAgain) {
			character.changeToIdleOrFall();
		}
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		// For the starting buster;
		if (startStockLevel is 1 or 3) {
			isPinkCharge = true;
		}
		// Non-full charge.
		if (isPinkCharge) {
			zero.stockedBusterLv = 1;
			isPinkCharge = true;
		}
		// Full charge.
		else {
			// We add Z-Saber charge if we fire the full charge and we were at 0 charge before.
			if (startStockLevel == 4 || !isSecond) {
				zero.stockedSaber = true;
			}
			zero.stockedBusterLv = 2;
		}
		if (!character.grounded || character.vel.y < 0) {
			sprite = "doublebuster_air";
			character.changeSpriteFromName(sprite, true);
		}
		// For halfway shot.
		if (startStockLevel <= 2) {
			character.frameIndex = 4;
			fired1 = true;
		}
	}

	public override void onExit(CharState? newState) {
		zero.stockedTime = 0;
		base.onExit(newState);
		// We check if we fired the second shot. If not we add the stocked charge.
		if (!fired2) {
			if (isPinkCharge) {
				zero.stockedBusterLv = 1;
			} else {
				zero.stockedBusterLv = 2;
				zero.stockedSaber = true;
			}
		}
		if (!fired1) {
			if (isPinkCharge) {
				zero.stockedBusterLv = 3;
			} else {
				zero.stockedBusterLv = 4;
				zero.stockedSaber = true;
			}
		}
	}
}
public class BusterZeroHuuX6 : BusterZeroGenericMeleeState {
	public BusterZeroHuuX6() : base("attack3x6") {
		sound = "saber3";
		soundFrame = 6;
		comboFrame = 9;
	}
	public override void update() {
		base.update();
		if (character.sprite.frameIndex >= comboFrame && zero.isCharging() && zero.shootPressed) {
			attackCtrl = true;
		}
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		zero.zSaberCooldown = 60;
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (zero.specialUpPressed && player.BZTenshouzan) {
			zero.changeState(new BusterZeroTenshouzan(), true);
			return true;
		}
		if (zero.isCharging() && zero.shootPressed) {
			zero.shoot(zero.getChargeLevel());
			return true;
		}
		return false;
	}
	public override void onExit(CharState oldState) {
		base.onExit(oldState);
	}
}
public class BusterZeroTHadangeki : BusterZeroGenericMeleeState {
	public BusterZeroTHadangeki() : base("swing") {
		sound = "zerosaberx3";
		soundFrame = 4;
		landSprite = "swing";
		airSprite = "swing_air";
		airMove = true;
		canJump = true;
		canStopJump = true;
	}
	public override void update() {
		if (character.frameIndex == 6 && !once) {
			once = true;
			ZSaberExtend(70,-19, false,0,0);
		}
		base.update();
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		if (!character.grounded || character.vel.y < 0) {
			sprite = "zero_swing_air";
			character.changeSpriteFromName(sprite, true);
		}
	}
	public override void onExit(CharState oldState) {
		base.onExit(oldState);
		zero.zSaberCooldown = 42;
	}
}
public class BusterZeroDashAttack : BusterZeroGenericMeleeState {
	public BusterZeroDashAttack() : base("attack_dash3") {
		sound = "saber1";
		soundFrame = 4;
		normalCtrl = true;
	}
	public override void update() {
		base.update();
		if (!once && character.frameIndex == 3) {
			once = true;
			ZSaberExtend(65, -17, true, 65, 0);
		}
	}
	public override void onExit(CharState oldState) {
		base.onExit(oldState);
		zero.zSaberCooldown = 24;
	}
}
public class BusterZeroDrillCrush : BusterZeroGenericMeleeState {
	public BusterZeroDrillCrush() : base("drillcrush") {
		sound = "saber3";
		soundFrame = 2;
		exitOnLanding = true;
		airMove = true;
	}
	public override void update() {
		base.update();
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		if (character.vel.y < 0) {
			character.vel.y = 0;
		}
	}
	public override void onExit(CharState oldState) {
		base.onExit(oldState);
		zero.zSaberCooldown = 50;
		sprite = "quakeblazer_land";
		normalCtrl = false;
		character.changeSpriteFromName(sprite, true);
		if (player.BZDrillCrushSpark) {
			new ElectricSparkProj(
				character.pos, character.xDir, character,
				player, 1, player.getNextActorNetId(),
				rpc: true);
			new ElectricSparkProj(
				character.pos, character.xDir *-1, character,
				player, 1, player.getNextActorNetId(),
				rpc: true);
		}
	}
}
public class BusterZeroFishFang : BusterZeroGenericMeleeState {
	public bool fired, fired2;
	public BusterZeroFishFang() : base("fishfang") {
		sound = "saber3";
		soundFrame = 4;
		exitOnLanding = true;
		useDashJumpSpeed = true;
		canStopJump = true;
	}
	public override void update() {
		base.update();
		Point shootPos = character.getShootPos();
		int xDir = character.getShootXDir();
		if (character.frameIndex == 3 && !fired) {
			fired = true;
			new FishFangProj(shootPos, xDir, player, player.getNextActorNetId(), rpc: true);			
		}	
		if (character.frameIndex == 4 && !fired2) {
			fired2 = true;
			new FishFangProj(shootPos, xDir, player, player.getNextActorNetId(), rpc: true);			
		}
		if (character.frameIndex >= 3) {
			character.vel.y = 0;
		}	
	}
	public override void onExit(CharState oldState) {
		base.onExit(oldState);
		zero.BZFishFangC = 52;
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		return false;
	}
}
public class BusterZeroLighting : BusterZeroGenericMeleeState {
	public BusterZeroLighting() : base("lighting") {
		sound = "wolfSigmaThunderX1";
		soundFrame = 1;
		BZBugFix = true;
	}
	public override void update() {
		base.update();
		int xDir = character.getShootXDir();
		if (!once && character.frameIndex == 1) {
			once = true;
			new BZLightingProj(
				character.getCenterPos().addxy(30 * xDir, -200),
				1, player, player.getNextActorNetId(), true
			);
		}
	}
	public override bool canEnter(Character character) {
		if (player.weapon.ammo < 16) return false;
		if (character.isCharging()) return false;
		if (player.BZZLighting == false) return false;
		return base.canEnter(character);
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		character.stopMoving();
		character.useGravity = false;
		player.weapon.addAmmo(-16, player);
	}
	public override void onExit(CharState oldState) {
		base.onExit(oldState);
		character.useGravity = true;
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		return false;
	}
}
public class BusterZeroLightingCharged : BusterZeroGenericMeleeState {
	public BusterZeroLightingCharged() : base("lighting") {
		sound = "wolfSigmaThunderX1";
		soundFrame = 1;
		BZBugFix = true;
	}
	public override void update() {
		base.update();
		int xDir = character.getShootXDir();
		if (!once && character.frameIndex == 1) {
			once = true;
			new BZLightingProj(
				character.getCenterPos().addxy(30 * xDir, -200),
				1, player, player.getNextActorNetId(), true
			);
			new BZLightingProj(
				character.getCenterPos().addxy(-30 * xDir, -200),
				1, player, player.getNextActorNetId(), true
			);
		}
	}
	public override bool canEnter(Character character) {
		if (player.weapon.ammo < 16) return false;
		if (player.BZZLighting == false) return false;
		return base.canEnter(character);
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		character.stopMoving();
		character.useGravity = false;
		player.weapon.addAmmo(-16, player);
	}
	public override void onExit(CharState oldState) {
		base.onExit(oldState);
		character.useGravity = true;
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		return false;
	}
}
public class BusterZeroFrostShield : BusterZeroGenericMeleeState {
	public BusterZeroFrostShield() : base("frostshield") {
		sound = "frostShieldCharged";
		soundFrame = 4;
	}
	public override void update() {
		if (!once && character.frameIndex == 5) {
			new BZFrostShield(
				character.getCenterPos().addxy(30,-5), character.xDir,
				player, player.getNextActorNetId(), rpc: true
			);
			once = true;
		}
		base.update();
	}
	public override void onExit(CharState oldState) {
		base.onExit(oldState);
		zero.zSaberCooldown = 60;
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		return false;
	}
}
public class BusterZeroTenshouzan : BusterZeroGenericMeleeState {
	bool jumpedYet;
	public BusterZeroTenshouzan() : base("tenshouzan") {
		sound = "saber3";
		soundFrame = 7;
		comboFrame = 9;
	}
	public override void update() {
		base.update();
		if (character.sprite.frameIndex >= 6 && !jumpedYet) {
			jumpedYet = true;
			character.dashedInAir++;
			float ySpeedMod = 1.15f;
			character.vel.y += -character.getJumpPower() * ySpeedMod;
			character.vel.x += 35 * character.xDir;
			ZSaberExtend(45, -40, true, 55, -35);
		}
		if (zero.shootPressed && character.sprite.frameIndex >= 7) {
			zero.changeState(new Fall());
			character.vel.y = 0;
		}
	}
	public override void onExit(CharState oldState) {
		base.onExit(oldState);
		zero.zSaberCooldown = 60;
		character.vel.x = 0;
	}
	public override bool altCtrlUpdate(bool[] ctrls) {		
		return false;
	}
}
public class BusterZeroTriThunder : BusterZeroGenericMeleeState {
	public bool fired;
	public int loop;
	public float time;
	public BusterZeroTriThunder() : base("trithunder") {
		sound = "crashX3";
		soundFrame = 11;
		invincible = true;
		comboFrame = 11;
	}
	public override void update() {
		base.update();
		Point shootPos = character.getShootPos();
		int xDir = character.getShootXDir();
		if (character.frameIndex == 13 && loop < 18) {
			character.frameIndex = 11;
			loop++;
		}
		if (character.sprite.frameIndex >= comboFrame && zero.isCharging() && zero.shootPressed) {
			attackCtrl = true;
		}
		if (character.frameIndex == 11 && !fired) {
			time += Global.spf;
			if (time > 0.135f) {
				time = 0;
				character.playSound("crashX3", sendRpc: true);
				character.shakeCamera(sendRpc: true);
				new BZTTriThunder(shootPos, xDir, player, player.getNextActorNetId(), rpc: true);
			}
		}
	}
	public override bool canEnter(Character character) {
		if (player.weapon.ammo < 28) return false;
		if (character.isCharging()) return false;
		if (player.BZTriThunder == false) return false;
		return base.canEnter(character);
	}
	public override void onEnter(CharState oldState) {
		player.weapon.addAmmo(-28, player);
		base.onEnter(oldState);
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		return false;
	}
}
public class BusterZeroChargedTriThunder : BusterZeroGenericMeleeState {
	public bool fired;
	public int loop;
	public float time;
	public BusterZeroChargedTriThunder() : base("trithunder") {
		sound = "crashX3";
		soundFrame = 11;
		invincible = true;
		comboFrame = 11;
	}
	public override void update() {
		base.update();
		character.sprite.frameSpeed = 2;
		Point shootPos = character.getShootPos();
		int xDir = character.getShootXDir();
		if (character.sprite.frameIndex >= comboFrame && zero.isCharging() && zero.shootPressed) {
			attackCtrl = true;
		}
		if (character.frameIndex == 11 && !fired) {
			character.playSound("crashX3", sendRpc: true);
			character.shakeCamera(sendRpc: true);
			new BZTTriThunderCharged(0, shootPos, xDir, player, player.getNextActorNetId(), rpc: true);
			new BZTTriThunderCharged(1, shootPos, xDir, player, player.getNextActorNetId(), rpc: true);
			new BZTTriThunderCharged(2, shootPos, xDir, player, player.getNextActorNetId(), rpc: true);
			new BZTTriThunderCharged(3, shootPos, xDir, player, player.getNextActorNetId(), rpc: true);
			new BZTTriThunderCharged(4, shootPos, xDir, player, player.getNextActorNetId(), rpc: true);
			fired = true;
		}
	}
	public override bool canEnter(Character character) {
		if (player.weapon.ammo < 28) return false;
		if (player.BZTriThunder == false) return false;
		return base.canEnter(character);
	}
	public override void onEnter(CharState oldState) {
		player.weapon.addAmmo(-28, player);
		base.onEnter(oldState);
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		return false;
	}
}

public class BusterZeroBubbleSplash : BusterZeroGenericMeleeState {
	bool jumpedYet;
	public BusterZeroBubbleSplash() : base("bubblesplash") {
		sound = "bubbleSplashCharged";
		soundFrame = 5;
	}
	public override void update() {
		base.update();
		if (character.sprite.frameIndex >= 6 && !jumpedYet) {
			jumpedYet = true;
			character.dashedInAir++;
			float ySpeedMod = 1.35f;
			character.vel.y += -character.getJumpPower() * ySpeedMod;
			if (character.isUnderwater()) character.vel.x += 60 * character.xDir;
			else character.vel.x += 35 * character.xDir;
		}
		if (zero.shootPressed && character.sprite.frameIndex >= 7) {
			zero.changeState(new Fall());
			character.vel.y = 0;
		}
		if (character.isUnderwater()) {
			airMove = true;
			normalCtrl = true;
		}
	}
	public override void onExit(CharState oldState) {
		base.onExit(oldState);
		zero.BZBubbleSplashC = 60;
		character.vel.x = 0;
	}
	public override bool altCtrlUpdate(bool[] ctrls) {

		return false;
	}
}
public class BZTTriThunder : Projectile {
	public bool fall;

	public BZTTriThunder(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		BZTTriThunderWeapon.netWeapon, pos, xDir,
		0, 2, player, "triadthunder_charged", 0, 0,
		netId, player.ownedByLocalPlayer
	) {
		reflectable = true;
		maxTime = 2.5f;
		projId = (int)ProjIds.BusterZTriThunderProj;
		useGravity = false;
		damager.flinch = Helpers.randomRange(4, 13);
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public override void update() {
		base.update();
		if (!fall) {
			changePos(new Point(pos.x, pos.y));
			vel.y -= Helpers.randomRange(10, 25);
			vel.x += Helpers.randomRange(-20, 20) * xDir;
		}
		if (vel.y <= -250) fall = true;
		if (fall) vel.y += 5;
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new BZTTriThunder(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class BZTTriThunderCharged : Projectile {
	public bool fall;
	public Point dest;

	public BZTTriThunderCharged(
		int num, Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		BZTTriThunderWeapon.netWeapon, pos, xDir,
		0, 2, player, "triadthunder_charged", 0, 0,
		netId, player.ownedByLocalPlayer
	) {
		maxTime = 1.75f;
		projId = (int)ProjIds.BusterZTriThunderChargedProj;
		useGravity = false;
		damager.flinch = 26;
		damager.damage = 3;
		damager.hitCooldown = 30f / 60f;
		destroyOnHit = false;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
		xScale = 1.35f;
		yScale = 1.35f;
		if (num == 0) dest = pos.addxy(-65, -200);
		if (num == 1) dest = pos.addxy(-35, -200);
		if (num == 2) dest = pos.addxy(-0, -200);
		if (num == 3) dest = pos.addxy(35, -200);
		if (num == 4) dest = pos.addxy(65, -200);
	}

	public override void update() {
		base.update();
		if (!fall) {
			float x = Helpers.lerp(pos.x, dest.x, Global.spf * 10);
			changePos(new Point(x, pos.y));
			vel.y += -40;
		}
		if (vel.y <= -375) fall = true;
		if (vel.y > 100) yDir = -1;
		if (fall) {
			vel.y += 10;
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new BZTTriThunderCharged(
			args.extraData[0], args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class BZTTriThunderWeapon : Weapon {
	public static BZTTriThunderWeapon netWeapon = new();

	public BZTTriThunderWeapon() : base() {
		ammo = 0;
		maxAmmo = 28;
		index = (int)WeaponIds.BZTTriThunderW;
		weaponBarBaseIndex = 19;
		weaponBarIndex = 33;
		killFeedIndex = 42;
		weaponSlotIndex = 51;
		displayName = "TriThunder";
		description = new string[] { "" };
		drawGrayOnLowAmmo = true;
		drawRoundedDown = true;
	}

	public override float getAmmoUsage(int chargeLevel) {
		return 0;
	}
	public override void bZeroShoot(Character character, int[] args) {
		int chargeLevel = args[0];
		base.bZeroShoot(character, args);
		if (character.ownedByLocalPlayer && chargeLevel < 1) {
			character.changeState(new BusterZeroTriThunder(), true);
		} else if (character.ownedByLocalPlayer &&  chargeLevel == 2) {
			character.changeState(new BusterZeroChargedTriThunder(), true);
		}
	}
}
public class FishFangProj : Projectile {
	public Actor? target;
	public float smokeTime = 0;
	public float maxSpeed = 175;
	public FishFangProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		BZTTriThunderWeapon.netWeapon, pos, xDir,
		0, 1, player, "zero_fishfangproj", 4, 0,
		netId, player.ownedByLocalPlayer
	) {
		maxTime = 1.75f;
		fadeOnAutoDestroy = true;
		reflectableFBurner = true;
		netcodeOverride = NetcodeModel.FavorDefender;
		projId = (int)ProjIds.BusterZFishFangProj;
		fadeSprite = "explosion";
		fadeSound = "explosion";
		xScale = 0.85f;
		yScale = 0.85f;
		if (xDir == 1) {
 		angle = 45;
		yDir = 1;
		}
		if (xDir == -1) {
 		angle = -235;
		}
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public override void update() {
		base.update();
		if (ownedByLocalPlayer) {
			if (!Global.level.gameObjects.Contains(target)) {
				target = null;
			}

			if (target != null) {
				if (time < 3f) {
					var dTo = pos.directionTo(target.getCenterPos()).normalize();
					var destAngle = MathF.Atan2(dTo.y, dTo.x) * 180 / MathF.PI;
					destAngle = Helpers.to360(destAngle);
					angle = Helpers.lerpAngle((float)angle, destAngle, Global.spf * 3);
				}
			}
			if (time >= 0.1 && target == null) {
				target = Global.level.getClosestTarget(pos, damager.owner.alliance, false, aMaxDist: Global.screenW);
			}

			vel.x = Helpers.cosd((float)angle) * maxSpeed;
			vel.y = Helpers.sind((float)angle) * maxSpeed;
		}

		smokeTime += Global.spf;
		if (smokeTime > 0.2) {
			smokeTime = 0;
			new Anim(pos, "torpedo_smoke", 1, null, true);
		}
		}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new FishFangProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class BusterZeroRollingSlash : BusterZeroGenericMeleeState {
	public BusterZeroRollingSlash() : base("attack_air2") {
		sound = "saber1";
		soundFrame = 1;
		comboFrame = 7;
		exitOnAirborne = true;
		attackCtrl = true;
		normalCtrl = true;
	}

	public override void update() {
		base.update();
		if (character.frameIndex == 1 && !once && (player.BZZSaberExtend || zero.isBlackZero)) {
			once = true;
			new BZZSaberExtendRolling(
				character.getCenterPos(), character.xDir,
				player, player.getNextActorNetId(), rpc: true
			);
		}
		if (character.frameIndex == 8) once = false;
		
		var move = new Point(0, 0);
		if (player.input.isHeld(Control.Left, player)) {
			character.xDir = -1;
			if (player.character.canMove()) {
				move.x = -character.getDashSpeed();
				character.move(move);
			}
		} else if (player.input.isHeld(Control.Right, player)) {
			character.xDir = 1;
			if (player.character.canMove()) {
				move.x = character.getDashSpeed();
				character.move(move);
			}
		}	
		if (move.magnitude > 0) {
			character.move(move);
		}	
	}
	public override void onExit(CharState oldState) {
		base.onExit(oldState);
	}
	public override bool altCtrlUpdate(bool[] ctrls) {

		return false;
	}
}
public class BZHadangekiProj : Projectile {
	public BZHadangekiProj(
		Point pos, int xDir, bool isBZ, Player player, ushort? netId, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir,
		350, 2, player, "zsaber_shot", 0, 0,
		netId, player.ownedByLocalPlayer
	) {
		fadeOnAutoDestroy = true;
		fadeSprite = "zsaber_shot_fade";
		reflectable = true;
		projId = (int)ProjIds.ZSaberProj;
		maxTime = 0.5f;
		damager.damage = player.ArmorModeUltimate ? damager.damage+1 : 2;
		if (isBZ) {
			genericShader = player.zeroPaletteShader;
		}
		if (rpc) {
			rpcCreate(pos, player, netId, xDir, (isBZ ? (byte)0 : (byte)1));
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new DZHadangekiProj(
			args.pos, args.xDir, args.extraData[0] == 1, args.owner, args.player, args.netId
		);
	}
}
public class BusterZeroTHadangeki2 : BusterZeroGenericMeleeState {
	bool fired;
	public BusterZeroTHadangeki2() : base("swing") {
		landSprite = "swing";
		airSprite = "swing_air";
		airMove = true;
		superArmor = true;
		airMove = true;
		canJump = true;
		canStopJump = true;
	}

	public override void update() {
		base.update();
		if (character.frameIndex >= 7 && !fired) {
			character.playSound("zerosaberx3", sendRpc: true);
			zero.stockedSaber = false;
			fired = true;
			new BZHadangekiProj(
				character.pos.addxy(30 * character.xDir, -20), character.xDir,
				zero.isBlackZero, player, player.getNextActorNetId(), rpc: true
			);
			new BZSlamProj(
				character.pos.addxy(50 * character.xDir, -40), character.xDir,
				zero.isBlackZero, player, player.getNextActorNetId(), rpc: true
			); 
			ZSaberExtend(70,-19, false, 0, 0);
		}
		if (character.isAnimOver()) {
			character.changeToIdleOrFall();
		} else {
			if ((character.grounded || character.canAirJump() && character.flag == null) &&
				player.input.isPressed(Control.Jump, player)
			) {
				if (!character.grounded) {
					character.dashedInAir++;
				}
				character.vel.y = -character.getJumpPower();
				sprite = "swing_air";
				character.changeSpriteFromName(sprite, false);
			}
		}
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		if (!character.grounded || character.vel.y < 0) {
			sprite = "swing_air";
			defaultSprite = sprite;
			character.changeSpriteFromName(sprite, true);
		}
	}
	public override void onExit(CharState oldState) {
		base.onExit(oldState);
		zero.zSaberCooldown = 36;
	}
}
public class BZMagnetMineProjCharged : Projectile {
	public float size;
	public BusterZeroTree? zero;
	public float timee;
	public BZMagnetMineProjCharged(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir,
		0, 1, player, "magnetmine_charged", 0, 1,
		netId, player.ownedByLocalPlayer
	) {
		maxTime = 9999;
		destroyOnHit = true;
		shouldShieldBlock = false;
		zero = (player.character as BusterZeroTree);
		if (zero is not null) {
			zero.BZMagnetMineProjCharged = this;
		}
		projId = (int)ProjIds.MagnetMineCharged;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public override void update() {
		base.update();
		if (!(zero?.charState is Die or Idle or Run or Dash or AirDash
			or WallSlide or WallKick or Jump or Fall or Crouch
			or BusterZeroDoubleBuster)) {
			destroySelf();
			zero.BZMagnetMineProjCharged = null;
		}
		if (zero != null) {
			if (zero.isCharging() && vel.x == 0) {
				changePos(zero.getShootPos());
			}
			if (!zero.player.input.isHeld(Control.Shoot, zero.player) && vel.x == 0) {
				if (zero.xDir == 1) {
					vel.x = 200;
				}
				if (zero.xDir == -1) {
					vel.x = -200;
				}
			}
			if (vel.x > 0 || vel.x == -200) {
				timee += Global.spf;
				if (timee >= 60f / 60f) {
					destroySelf();
					zero.BZMagnetMineProjCharged = null;
				}
			}
		}
	}
	public override void onHitDamagable(IDamagable damagable) {
		if (destroyOnHit) {
			if (zero != null || zero == null)
				zero.BZMagnetMineProjCharged = null;
			destroySelf();
		}
	}

	public override void onCollision(CollideData other) {
		base.onCollision(other);
		if (!ownedByLocalPlayer) return;
		var go = other.gameObject;
		if (go is Projectile) {
			var proj = go as Projectile;
			if (proj != null && size <= 64) {
				if (!proj.shouldVortexSuck) return;
				if (proj is BZMagnetMineProjCharged || proj is DZBuster2Proj ||
					proj is DZBuster3Proj || proj is DZBusterProj) return;
				size += proj.damager.damage;
				proj.destroySelfNoEffect(doRpcEvenIfNotOwned: true);
			}
			if (size == 63) {
				playSound("gigaCrushAmmoFull");
			}
			if (size >= 16) {
				updateDamager(size / 4, size >= 64 ? 64 : 0);
				changeSprite("magnetmine_charged3", true);
			} else if (size > 8) {
				updateDamager(2);
				changeSprite("magnetmine_charged2", true);
			}
		}
	}
}

public class BoomerangShield : Projectile {
	public float anglee = 0;
	public float orbitRadius = 52; // Distancia al owner
	public float orbitSpeed = 11f; // Radianes por segundo
	float baseOrbitSpeed = 7f;
	bool once;
	private float radiusX = 50f; // Eje horizontal mayor
	private float radiusY = 36f; // Eje vertical menor	
	public BoomerangShield(
		Point pos, int xDir,
		Player player, ushort netProjId,
		bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir, 350, 2,
		player, "zero_shieldboomerang", 0, 1, netProjId,
		player.ownedByLocalPlayer
	) {
		fadeSprite = "buster2_fade";
		maxTime = 7f;
		destroyOnHit = false;
		projId = (int)ProjIds.Buster2;

		if (rpc) {
			rpcCreate(pos, player, netProjId, xDir);
		}
	}

	public override void update() {
		base.update();
		if (owner == null) return;
		if (!once) {
			owner.character.playSound("saber1", sendRpc: true);
			once = true;
		}

		// Desacelera cuando está al costado (π/2 y 3π/2) usando el coseno
		// Esto crea una oscilación suave: rápido al frente y atrás, lento a los lados
		float speedFactor = 0.5f + 0.5f * MathF.Abs(MathF.Cos(anglee));
		float currentSpeed = baseOrbitSpeed * speedFactor;
		anglee += currentSpeed * Global.spf * xDir;

		if (anglee > MathF.PI * 2) anglee -= MathF.PI * 2;

		// Centro del owner
		var center = owner.character.getCenterPos();

		// Movimiento circular
		unsafePos.x = center.x + (int)(MathF.Cos(anglee) * radiusX);
		unsafePos.y = center.y + (int)(MathF.Sin(anglee) * radiusY);
	}
}
public class BZSlamProj : Projectile {
	public BZSlamProj(
		Point pos, int xDir, bool isBZ, Player player, ushort? netId, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir,
		0, 2, player, "zero_chargedproj", 30, 2,
		netId, player.ownedByLocalPlayer
	) {
		setIndestructableProperties();
		projId = (int)ProjIds.BZSlamProj;
		maxTime = 0.15f;
		if (isBZ) {
			genericShader = player.zeroPaletteShader;
		}
		if (rpc) {
			rpcCreate(pos, player, netId, xDir, (isBZ ? (byte)0 : (byte)1));
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new BZSlamProj(
			args.pos, args.xDir, args.extraData[0] == 1, args.player, args.netId
		);
	}
}
public class BZZSaberExtend : Projectile {
	public bool follow;
	public int xF, yF;
	public BZZSaberExtend(
		bool follow, int xF, int yF, Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir,
		0, 1, player, "zero_zsaberex", 0, 1,
		netId, player.ownedByLocalPlayer
	) {
		this.yF = yF;
		this.xF = xF;
		this.follow = follow;
		setIndestructableProperties();
		projId = (int)ProjIds.BZZSaberExtend;
		maxTime = 0.15f;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}
	public override void update() {
		var center = owner.character.getCenterPos();
		if (follow) {
			unsafePos.x = center.x + xF * xDir;
			unsafePos.y = center.y + yF;
		}
		base.update();
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new BZZSaberExtend(
			args.extraData[0] == 1, args.extraData[2], args.extraData[3], args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class BZZSaberExtendRolling : Projectile {
	public BZZSaberExtendRolling(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir,
		0, 1, player, "zero_zsaberex_k", 0, 1,
		netId, player.ownedByLocalPlayer
	) {
		setIndestructableProperties();
		projId = (int)ProjIds.BZZSaberExtend;
		maxTime = 18f / 60f;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}
	public override void update() {
		var center = owner.character.getCenterPos();
		unsafePos.x = center.x;
		unsafePos.y = center.y - 40;
		base.update();
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new BZZSaberExtendRolling(
		args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class BZFrostShield : Projectile, IDamagable {
	float health = 4;
	public BZFrostShield(
		Point pos, int xDir, Player player,
		ushort netProjId, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir, 0, 0, player,
		"frostshield_charged_platform", 0, 1f, netProjId,
		player.ownedByLocalPlayer
	) {
		maxTime = 8;
		projId = (int)ProjIds.FrostShieldChargedPlatform;
		setIndestructableProperties();
		isShield = true;
		collider.wallOnly = true;
		useGravity = true;
		isPlatform = true;
		if (rpc) {
			rpcCreate(pos, player, netProjId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new BZFrostShield(
			arg.pos, arg.xDir, arg.player, arg.netId
		);
	}

	public override void update() {
		base.update();
		moveWithMovingPlatform();
		if (isAnimOver() && isUnderwater()) {
			move(new Point(0, -100));
		}
	}
	public void applyDamage(float damage, Player? owner, Actor? actor, int? weaponIndex, int? projId) {
		health -= damage;
		if (health <= 0) {
			destroySelf();
		}
	}

	public bool canBeDamaged(int damagerAlliance, int? damagerPlayerId, int? projId) {
		return damagerAlliance != owner.alliance;
	}

	public bool canBeHealed(int healerAlliance) {
		return false;
	}

	public void heal(Player healer, float healAmount, bool allowStacking = true, bool drawHealText = false) {
	}

	public bool isInvincible(Player attacker, int? projId) {
		return false;
	}

	public override void onDestroy() {
		base.onDestroy();
		breakFreeze(owner);
	}
	public bool isPlayableDamagable() {
		return false;
	}
}
public class BZBurningShot : Projectile {
	public int type;
	public BZBurningShot(
		int type, Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir,
		0, 0, player, "zero_burningshot", 0, 0,
		netId, player.ownedByLocalPlayer
	) {
		this.type = type;
		maxTime = 8 / 60f;
		if (type == 1) {
			projId = (int)ProjIds.BZBurningShot2;
		} else {
			projId = (int)ProjIds.BZBurningShot;
		}
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}
	public override void onDestroy() {
		base.onDestroy();
		playSound("explosion");
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new BZBurningShot(
			args.extraData[0], args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class BZEspark : Projectile {
	public int type = 0;
	public BZEspark(
		int type, Point pos, int xDir, Player player,
		ushort netProjId, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir, 0, 1, player, "electric_spark",
		Global.miniFlinch, 1, netProjId, player.ownedByLocalPlayer
	) {
		projId = (int)ProjIds.BZEspark;
		maxTime = 1.2f;
		fadeSprite = "electric_spark_fade";
		this.type = type;
		reflectable = true;
		destroyOnHit = false;
		if (rpc) {
			rpcCreate(pos, player, netProjId, xDir);
		}
		if (type == 1) {
			vel.y = 150;
		} else if (type == 2) {
			vel.y = -150;
		}
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new BZEspark(
			arg.extraData[0], arg.pos, arg.xDir,
			arg.player, arg.netId
		);
	}
	public override void onHitWall(CollideData other) {
		base.onHitWall(other);
		destroySelf();
	}
	public override void onReflect() {
		vel.y *= -1;
		base.onReflect();
	}
}
public class BZBlizzardArrow : Projectile {
	int state = 0;
	float stateTime;
	public Anim exhaust;
	public BZBlizzardArrow(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir,
		40, 1, player, "frostshield_start", 0, 0,
		netId, player.ownedByLocalPlayer
	) {
		exhaust = new Anim(pos, "frostshield_exhaust", xDir, null, false);
		maxDistance = 200;
		projId = (int)ProjIds.BZBlizzardArrow;
		destroyOnHit = true;
		reflectable = true;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}
	public override void onHitWall(CollideData other) {
		base.onHitWall(other);
		destroySelf();
	}
	public override void onDestroy() {
		base.onDestroy();
		exhaust?.destroySelf();
		breakFreeze(owner);
	}
	public override void update() {
		base.update();
		exhaust.pos = pos;
		exhaust.xDir = xDir;
		if (state == 0) {
			stateTime += Global.speedMul;
			if (stateTime >= 30) {
				state = 1;
				changeSprite("frostshield_proj", true);
			}
		} else if (state == 1) {
			vel.x += Global.spf * 200 * xDir;
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new BZBlizzardArrow(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class BZLightningWeapon : Weapon {
	public static BZLightningWeapon netWeapon = new();
	public BZLightningWeapon() : base() {
		//damager = new Damager(player, 4, Global.defFlinch, 0.5f);
		ammo = 16;
		maxAmmo = 16;
		index = (int)WeaponIds.BZYammarkOption;
		weaponBarBaseIndex = 27;
		weaponBarIndex = 35;
		killFeedIndex = 16;
		weaponSlotIndex = 51;
		displayName = "Lighting";
		description = new string[] { "" };
		drawAmmo = true;
		canHealAmmo = true;
	}

	public override float getAmmoUsage(int chargeLevel) {
		return 0;
	}
	public override void bZeroShoot(Character character, int[] args) {
		base.bZeroShoot(character, args);
		int chargeLevel = args[0];
		if (character.ownedByLocalPlayer && chargeLevel < 1) {
			character.changeState(new BusterZeroLighting(), true);
		} else if (character.ownedByLocalPlayer && chargeLevel > 1) {
			character.changeState(new BusterZeroLightingCharged(), true);
		}
	}
}
public class BZLightingProj : Projectile {
	public BZLightingProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir,
		0, 0, player, "zero_lighting_proj", 0, 0,
		netId, player.ownedByLocalPlayer
	) {
		maxTime = 60f / 60f;
		projId = (int)ProjIds.BusterZLightingProj;
		vel = new Point(0, 400);
		damager.damage = 4;
		damager.flinch = 26;
		damager.hitCooldown = 0.2f;
		destroyOnHit = false;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new BZLightingProj(args.pos, args.xDir, args.player, args.netId);
	}
}
public class BZTimeStopper : Actor {
	public float time;
	public Player owner;
	public ShaderWrapper? timeSlowShader;
	public const int radius = 90;
	public float drawRadius = 100;
	public float drawAlpha = 48;
	public bool isSnails;
	float maxTime = 0.25f;
	float soundTime;
	public BZTimeStopper(
		Point pos, Player owner, ushort? netId, bool ownedByLocalPlayer, bool sendRpc = false
	) : base(
		"empty", pos, netId, ownedByLocalPlayer, false
	) {
		useGravity = false;
		this.owner = owner;
		if (Options.main.enablePostProcessing) timeSlowShader = owner.timeSlowShader;
		Global.level.BZTimeStopper.Add(this);
		netOwner = owner;
		netActorCreateId = NetActorCreateId.CrystalHunterChargedBZ;
		canBeLocal = true;
		if (sendRpc) createActorRpc(owner.id);
	}

	public override void update() {
		base.update();
		var screenCoords = new Point(pos.x - Global.level.camX, pos.y - Global.level.camY);
		var normalizedCoords = new Point(screenCoords.x / Global.viewScreenW, 1 - screenCoords.y / Global.viewScreenH);
		Helpers.decrementFrames(ref soundTime);
		if (soundTime == 0) {
			playSound("csnailSlowLoop");
			soundTime = 65;
		}
		if (timeSlowShader != null) {
			timeSlowShader.SetUniform("x", normalizedCoords.x);
			timeSlowShader.SetUniform("y", normalizedCoords.y);
			timeSlowShader.SetUniform("t", Global.time);
			if (Global.viewSize == 2) timeSlowShader.SetUniform("r", 0.25f);
			else timeSlowShader.SetUniform("r", 0.4f);
		}
		if (timeSlowShader == null) {
			drawRadius = 120 + 0.5f * MathF.Sin(Global.time * 10);
			drawAlpha = 64f + 32f * MathF.Sin(Global.time * 10);
		}
		time += Global.spf;
		if (time > maxTime) {
			destroySelf(disableRpc: true);
		}
	}

	public override void onDestroy() {
		base.onDestroy();
		Global.level.BZTimeStopper.Remove(this);
	}

	public override void render(float x, float y) {
		base.render(x, y);
		if (timeSlowShader == null) {
			var fillColor = new Color(96, 80, 240, Helpers.toByte(drawAlpha));
			var lineColor = new Color(208, 200, 240, Helpers.toByte(drawAlpha));
			if (owner.alliance == GameMode.redAlliance && Global.level?.gameMode?.isTeamMode == true) {
				fillColor = new Color(240, 80, 96, Helpers.toByte(drawAlpha));
			}

			//if (Global.isOnFrameCycle(20))
			{
				DrawWrappers.DrawCircle(pos.x, pos.y, drawRadius, true, fillColor, 0, ZIndex.Character - 1, pointCount: 50u);
				//DrawWrappers.DrawCircle(pos.x, pos.y, drawRadius, false, new Color(208, 200, 240, Helpers.toByte(drawAlpha)), 1f, ZIndex.Character - 1, pointCount: 50u);
			}

			float randY = Helpers.randomRange(-1f, 1f);
			float xLen = MathF.Sqrt(1 - MathF.Pow(randY, 2)) * drawRadius;
			float randThickness = Helpers.randomRange(0.5f, 2f);
			DrawWrappers.DrawLine(pos.x - xLen, pos.y + randY * drawRadius, pos.x + xLen, pos.y + randY * drawRadius, lineColor, randThickness, ZIndex.Character - 1);
		}
	}
}
public class BZYammarkOption : Weapon {
	public static BZYammarkOption netWeapon = new();
	public BZYammarkOption() : base() {
		ammo = 32;
		maxAmmo = 32;
		index = (int)WeaponIds.BZYammarkOption;
		weaponBarBaseIndex = 27;
		weaponBarIndex = 35;
		killFeedIndex = 16;
		weaponSlotIndex = 51;
		displayName = "Yammark Option";
		description = new string[] { "" };
		drawAmmo = true;
		allowSmallBar = true;
	}

	public override float getAmmoUsage(int chargeLevel) {
		return 0;
	}
	public override void bZeroShoot(Character character, int[] args) {
		int chargeLevel = args[0];
		BusterZeroTree bz = character as BusterZeroTree ?? throw new NullReferenceException();
		base.bZeroShoot(character, args);
		if (character.ownedByLocalPlayer && bz.yammarkOptionClass == null && bz.player.weapon.ammo > 0) {
			bz.yammarkOptionClass = new YammarkOptionClass(bz);
		}
		if (character.ownedByLocalPlayer && bz.parasiteBombClass == null && bz.player.weapon.ammo > 0) {
			bz.parasiteBombClass = new BZParasiteBombClass(bz);
		}
	}
}
public class BZYammarkProj : Projectile {
	public BZYammarkProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir,
		200, 1, player, "zero_yammarkproj", 0, 0,
		netId, player.ownedByLocalPlayer
	) {
		yScale = 0.75f;
		xScale = 0.75f;
		maxTime = 40f / 60f;
		reflectable = true;
		destroyOnHitWall = true;
		projId = (int)ProjIds.BZYammarkProj;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new BZYammarkProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}

public class YammarkOptionClass {
	public BusterZeroTree bz;
	public List<YammarkOption> Yams = new List<YammarkOption>();
	Player MainP => Global.level.mainPlayer;
	public YammarkOptionClass(BusterZeroTree bz) {
		this.bz = bz;
		float separation = (MathF.PI * 2) / 3f;
		if (MainP.BZYammarkOption && bz.player.weapon.ammo > 0) {
			Yams = new List<YammarkOption>() {
				new YammarkOption(bz.getCenterPos(), bz.xDir, MainP, MainP.getNextActorNetId(), 0, true),
				new YammarkOption(bz.getCenterPos(), bz.xDir, MainP, MainP.getNextActorNetId(), separation, true),
				new YammarkOption(bz.getCenterPos(), bz.xDir, MainP, MainP.getNextActorNetId(), separation * 2, true),
			};
		}
	}

	public void update() {
		if (shouldDestroy()) {
			destroy();
		}
	}
	public bool shouldDestroy() {
		if (bz.player.weapon is not BZYammarkOption) return true;
		if (bz.player.weapon.ammo <= 0) return true;
		return false;
	}

	public void destroy() {
		foreach (var Yams in Yams) Yams.destroySelf();
		Yams.Clear();
		bz.yammarkOptionClass = null;
	}
}
public class YammarkOption : Projectile, IDamagable {
	public float angle = 0;
	public float orbitRadius = 36;
	float baseOrbitSpeed = 7f;
	float health = 4;
	public float angleOffset = 0;
	public Anim exhaust;
	public YammarkOption(
		Point pos, int xDir, Player player,
		ushort netProjId, float angleOffset = 0f, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir, 0, 1, player,
		"zero_yammark", 0, 0.25f, netProjId,
		player.ownedByLocalPlayer
	) {
		this.angleOffset = angleOffset;
		exhaust = new Anim(pos, "zero_yammark_anim", xDir, null, false);
		fadeSprite = "explosion";
		fadeSound = "explosion";
		maxTime = 999;
		projId = (int)ProjIds.BZYammarkOption;
		if (rpc) {
			rpcCreate(pos, player, netProjId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new YammarkOption(
			arg.pos, arg.xDir, arg.player, arg.netId
		);
	}

	public override void update() {
		base.update();
		if (owner == null) return;
		float currentSpeed = baseOrbitSpeed * 0.5f;
		angle += currentSpeed * Global.spf;
		var center = owner.character.getCenterPos();
		float totalAngle = angle + angleOffset;
		unsafePos.x = center.x + (int)(MathF.Cos(totalAngle) * orbitRadius);
		unsafePos.y = center.y + (int)(MathF.Sin(totalAngle) * orbitRadius);
		if (owner.character is BusterZeroTree busterZero) {
			if (owner.input.isPressed(Control.Shoot, owner) && owner.weapon.ammo > 0 && busterZero.YammarkShotCooldown <= 0) {
				new BZYammarkProj(pos.addxy(20 * xDir, 0), xDir, owner, owner.getNextActorNetId(), true);
				owner.weapon.addAmmo(-1, owner);
			}
		}
	}
	public override void postUpdate() {
		base.postUpdate();
		if (!ownedByLocalPlayer) return;
		if (destroyed) return;
		xDir = owner.character.getShootXDir();
		exhaust.xDir = xDir;
		exhaust.pos = pos.addxy(-20 * xDir, 0);
		if (owner.character is BusterZeroTree busterZero) {
			if (owner.input.isPressed(Control.Shoot, owner)) {
				busterZero.YammarkShotCooldown = 0.20f;
			}
		}
	}
	public override void onDestroy() {
		base.onDestroy();
		exhaust?.destroySelf();
	}
	public void applyDamage(float damage, Player? owner, Actor? actor, int? weaponIndex, int? projId) {
		health -= damage;
		if (health <= 0) {
			destroySelf();
		}
	}

	public bool canBeDamaged(int damagerAlliance, int? damagerPlayerId, int? projId) {
		return damagerAlliance != owner.alliance;
	}

	public bool canBeHealed(int healerAlliance) {
		return false;
	}

	public void heal(Player healer, float healAmount, bool allowStacking = true, bool drawHealText = false) {
	}

	public bool isInvincible(Player attacker, int? projId) {
		return false;
	}
	public bool isPlayableDamagable() {
		return false;
	}

}
public class BZDrone : Projectile, IDamagable {
	public float angle = 0;
	public float orbitRadius = 36;
	float baseOrbitSpeed = 7f;
	float health = 4;
	public float angleOffset = 0;
	public Anim exhaust;
	public BZDrone(
		Point pos, int xDir, Player player,
		ushort netProjId, float angleOffset = 0f, bool rpc = false
	) : base(
		ZeroBuster.netWeapon, pos, xDir, 0, 1, player,
		"zero_yammark", 0, 0.25f, netProjId,
		player.ownedByLocalPlayer
	) {
		this.angleOffset = angleOffset;
		exhaust = new Anim(pos, "zero_yammark_anim", xDir, null, false);
		fadeSprite = "explosion";
		fadeSound = "explosion";
		maxTime = 999;
		projId = (int)ProjIds.BZDrone;
		if (rpc) {
			rpcCreate(pos, player, netProjId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new YammarkOption(
			arg.pos, arg.xDir, arg.player, arg.netId
		);
	}

	public override void update() {
		base.update();
		if (owner == null) return;
		float currentSpeed = baseOrbitSpeed * 0.5f;
		angle += currentSpeed * Global.spf;
		var center = owner.character.getCenterPos();
		float totalAngle = angle + angleOffset;
		unsafePos.x = center.x + (int)(MathF.Cos(totalAngle) * orbitRadius);
		unsafePos.y = center.y + (int)(MathF.Sin(totalAngle) * orbitRadius);
		if (owner.character is BusterZeroTree busterZero) {
			if (owner.input.isPressed(Control.Shoot, owner) && owner.weapon.ammo > 0 && busterZero.YammarkShotCooldown <= 0) {
				new BZYammarkProj(pos.addxy(20 * xDir, 0), xDir, owner, owner.getNextActorNetId(), true);
				owner.weapon.addAmmo(-owner.weapon.getAmmoUsage(0), owner);
			}
		}
	}
	public override void postUpdate() {
		base.postUpdate();
		if (!ownedByLocalPlayer) return;
		if (destroyed) return;
		xDir = owner.character.getShootXDir();
		exhaust.xDir = xDir;
		exhaust.pos = pos.addxy(-20 * xDir, 0);
		if (owner.character is BusterZeroTree busterZero) {
			if (owner.input.isPressed(Control.Shoot, owner)) {
				busterZero.YammarkShotCooldown = 0.20f;
			}
		}
	}
	public override void onDestroy() {
		base.onDestroy();
		exhaust?.destroySelf();
	}
	public void applyDamage(float damage, Player? owner, Actor? actor, int? weaponIndex, int? projId) {
		health -= damage;
		if (health <= 0) {
			destroySelf();
		}
	}

	public bool canBeDamaged(int damagerAlliance, int? damagerPlayerId, int? projId) {
		return damagerAlliance != owner.alliance;
	}

	public bool canBeHealed(int healerAlliance) {
		return false;
	}

	public void heal(Player healer, float healAmount, bool allowStacking = true, bool drawHealText = false) {
	}

	public bool isInvincible(Player attacker, int? projId) {
		return false;
	}
	public bool isPlayableDamagable() {
		return false;
	}
}
public class BZBeeCursorAnim : Anim {
	public int state = 0;
	BusterZeroTree? character;
	Player player;
	public Actor? target;
	public float angle = 0;
	public float orbitRadius = 36;
	float baseOrbitSpeed = 7f;
	float separation = (MathF.PI * 2) / 3f;
	public float angleOffset = 0;
	public BZBeeCursorAnim(Point pos, Character character)
		: base(pos, "parasite_cursor_start", 1, character.player.getNextActorNetId(), false, true, character.ownedByLocalPlayer) {
		this.character = character as BusterZeroTree;
		player = character.player;
	}

	public override void update() {
		base.update();
		if (!ownedByLocalPlayer) return;
		if (target == null) {
			angleOffset = separation * 1.5f;
			float currentSpeed = baseOrbitSpeed * 0.5f;
			angle += currentSpeed * Global.spf;
			if (character != null) {
				var center = character.getCenterPos();
				float totalAngle = angle + angleOffset;
				unsafePos.x = center.x + (int)(MathF.Cos(totalAngle) * orbitRadius);
				unsafePos.y = center.y + (int)(MathF.Sin(totalAngle) * orbitRadius);
			}
		}
		if (state == 0) {
			if (sprite.name == "parasite_cursor_start" && sprite.isAnimOver()) {
				changeSprite("parasite_cursor", true);
				state = 1;
				time = 0;
			}
		} else if (state == 1) {
			if (target != null) {
				state = 2;
			}
		} else if (state == 2) {
			if (target!.destroyed) {
				state = 3;
				return;
			}
			move(pos.directionToNorm(target.getCenterPos()).times(350));
			if (pos.distanceTo(target.getCenterPos()) < 5) {
				state = 3;
				changeSprite("parasite_cursor_lockon", true);
			}
		} else if (state == 3) {
			pos = target!.getCenterPos();

			if (isAnimOver()) {
				state = 4;
				destroySelf();
				if (!target!.destroyed) {
					if (character != null) {
						if (character.charState.attackCtrl) character.setShootAnim();
						player.weapon.addAmmo(-2, player);
						new BZParasiticBombProjCharged(
							new ParasiticBomb(), character.getShootPos(),
							character.pos.x - target.getCenterPos().x < 0 ? 1 : -1,
							character.player, character.player.getNextActorNetId(), target, rpc: true
						);
					}
				}
			}
		}

	}
}
public class BZParasiticBombProjCharged : Projectile, IDamagable {
	public Actor host;
	public Point lastMoveAmount;
	const float maxSpeed = 150;
	public BZParasiticBombProjCharged(
		Weapon weapon, Point pos, int xDir, Player player,
		ushort netProjId, Actor host, bool rpc = false
	) : base(
		weapon, pos, xDir, 0, 1, player, "parasitebomb_bee",
		0, 0.25f, netProjId, player.ownedByLocalPlayer
	) {
		this.weapon = weapon;
		this.host = host;
		fadeSprite = "explosion";
		fadeSound = "explosion";
		maxTime = 3f;
		projId = (int)ProjIds.BZPbomb;
		destroyOnHit = true;
		shouldShieldBlock = true;
		if (rpc) {
			rpcCreate(pos, player, netProjId, xDir);
		}
		canBeLocal = false;
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new BZParasiticBombProjCharged(
			ParasiticBomb.netWeapon, arg.pos, arg.xDir,
			arg.player, arg.netId, null!
		);
	}

	public override void update() {
		base.update();
		updateProjectileCooldown();
		if (!ownedByLocalPlayer) return;

		if (!host.destroyed) {
			Point amount = pos.directionToNorm(host.getCenterPos()).times(150);
			vel = Point.lerp(vel, amount, Global.spf * 4);
			if (vel.magnitude > maxSpeed) vel = vel.normalize().times(maxSpeed);
		} else {
		}
	}

	public void applyDamage(float damage, Player? owner, Actor? actor, int? weaponIndex, int? projId) {
		if (damage > 0) {
			destroySelf();
		}
	}

	public bool canBeDamaged(int damagerAlliance, int? damagerPlayerId, int? projId) {
		return damager.owner.alliance != damagerAlliance;
	}

	public bool isInvincible(Player attacker, int? projId) {
		return false;
	}

	public bool canBeHealed(int healerAlliance) {
		return false;
	}

	public void heal(Player healer, float healAmount, bool allowStacking = true, bool drawHealText = false) {
	}
	public bool isPlayableDamagable() {
		return false;
	}
}
public class BZParasiteBombClass {
	public BusterZeroTree bz;
	public List<BZBeeCursorAnim> Pbomb = new List<BZBeeCursorAnim>();
	Player MainP => Global.level.mainPlayer;
	int currentIndex;
	float currentTime = 0f;
	const float beeCooldown = 1f;
	public BZParasiteBombClass(BusterZeroTree bz) {
		this.bz = bz;
		if (bz.ownedByLocalPlayer) {
			if (MainP.BZParasiteBomb && bz.player.weapon.ammo > 0) {
				Pbomb = new List<BZBeeCursorAnim>() {
					new BZBeeCursorAnim(bz.getCenterPos(), bz),
				};
			}
		}
	}

	public Actor? getAvailableTarget() {
		Point centerPos = bz.getCenterPos();
		var targets = Global.level.getTargets(centerPos, bz.player.alliance, true, 120);
		foreach (var target in targets) {
			if (Pbomb.Any(b => b.target == target)) {
				continue;
			}
			return target;
		}
		return null;
	}
	public void update() {
		if (shouldDestroy()) {
			destroy();
		}
		if (MainP.BZParasiteBomb) {
			currentTime -= Global.spf;
			if (currentTime <= 0) {
				var target = getAvailableTarget();
				if (target != null) {
					Pbomb[currentIndex].target = target;
					currentTime = beeCooldown;
					currentIndex = 0;
				}
			}

			for (int i = 0; i < Pbomb.Count; i++) {
				if (Pbomb[i].state < 2) {
					Pbomb[i].pos = Pbomb[i].pos;
				}
				if (Pbomb[i].state == 4) {
					Pbomb[i] = new BZBeeCursorAnim(Pbomb[i].pos, bz);
				}
			}
		}
	}
	public bool shouldDestroy() {
		if (bz.player.weapon is not BZYammarkOption) return true;
		if (bz.player.weapon.ammo <= 0) return true;
		return false;
	}

	public void destroy() {
		foreach (var Pbomb in Pbomb) Pbomb.destroySelf();
		Pbomb.Clear();
		bz.parasiteBombClass = null;
	}
}