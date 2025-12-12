using System;
using System.Collections.Generic;
using SFML.Graphics;
using System.Linq;
namespace MMXOnline;
public class Sakuya : Character {
	public bool TauntPressed => player.input.isPressed(Control.Taunt, player);
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
	public bool LeftOrRightPressed => player.input.isPressed(Control.Left, player) || player.input.isPressed(Control.Right, player);
	public bool LeftHeld => player.input.isHeld(Control.Left, player);
	public bool RightHeld => player.input.isHeld(Control.Right, player);
	public float AmmoCooldown;
	public float onAir;
	public int SpecialWeaponSelected = 0;
	public float sakuyaheld;
	public float AttackCooldown, AttackCooldownStunKnife, AttackCooldownChainsaw;
	public float SnailHeld, SnailTime, tapcheck, timeclock, loadouttapcheck;
	public bool OnceSnail, onceloadout;
	public CrystalHunterChargedS? chargedCrystalHunter;
	public SakuyaMeleeW meleeWeapon = new();
	public int autoAimKnifeCount;
	public float autoAimKnifeTime;
	public bool autoAimShoot;
	public Sakuya(
		Player player, float x, float y, int xDir,
		bool isVisible, ushort? netId, bool ownedByLocalPlayer,
		bool isWarpIn = true, int? heartTanks = null, bool isATrans = false
	) : base(
		player, x, y, xDir, isVisible, netId, ownedByLocalPlayer, isWarpIn, heartTanks, isATrans
	) {
		charId = CharIds.Sakuya;
		yScale = 0.7f;
		xScale = 0.7f;
	}
	public override CharState getRunState(bool skipInto = false) => new SakuyaWalk("walk", skipInto);
	public override void update() {
		base.update();
		if (!ownedByLocalPlayer) {
			return;
		}
		Helpers.decrementFrames(ref SnailTime);
		Helpers.decrementFrames(ref AttackCooldown);
		Helpers.decrementFrames(ref AttackCooldownStunKnife);
		Helpers.decrementFrames(ref AttackCooldownChainsaw);
		autoAimSakuyaAttack();
		SakuyaAmmo();
		GlideLogic();
		SakuyaLoadout();
		SnailTimeVoid();
		SakuyaTimeStopVoid();
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
		if (WRightPressed) {
			if (player.SakuyaAmmo >= 16 && SpecialWeaponSelected == 0) {
				changeState(new SakuyaThousandDagger(), true);			
			} else if (player.SakuyaAmmo >= 6 && SpecialWeaponSelected == 3 && AttackCooldownChainsaw <= 0) {
				changeState(new SakuyaAttackChainsaw(), true);
			} else if (player.SakuyaAmmo >= 4 && SpecialWeaponSelected == 2 && AttackCooldownStunKnife <= 0) {
				changeState(new SakuyaAttackStunKnife(), true);
			} else if (player.SakuyaAmmo >= 20 && SpecialWeaponSelected == 1 && autoAimShoot == false) {
				player.SakuyaAmmo -= 20;
				autoAimShoot = true;
			}
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
					changeState(new SakuyaAttack(0), true);
					return true;
				}
			}
			if (grounded) {
				if (shootPressed) {
					if (upHeld) {
						changeState(new SakuyaAttackUP(), true);
						return true;
					}
					if (charState is SakuyaWalk && AttackCooldown <= 0) {
						changeState(new SakuyaAttackRun(0), false);			
						return true;
					}
					if (downHeld) {
						changeState(new SakuyaAttackDown(), true);
						return true;
					}
					if (vel.x <= 0 && AttackCooldown <= 0)
					changeState(new SakuyaAttack(0), true);
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
		CharState idleState = getIdleState();
		dashedInAir = 0;
		changeState(idleState, true);
	}
	public override bool canAirJump() {
		return dashedInAir == 0;
	}
	public override float getRunSpeed() {
		float runSpeed = 2.25f;
		return runSpeed * getRunDebuffs();
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
		if (player.SakuyaAmmo > player.SakuyaMaxAmmo) {
			player.SakuyaAmmo = player.SakuyaMaxAmmo;
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
		if (player.input.isHeld(Control.WeaponRight, player)) {
			if (sakuyaheld < 2)
			sakuyaheld += Global.spf*4;	
			if (sakuyaheld >= 1) {
				if (!onceloadout) {
					onceloadout = true;
					playSound("weaponopen", true, true);
				}
				if (upPressed) {
				SpecialWeaponSelected++;
				playSound("weaponswap", true, true);
				if (SpecialWeaponSelected >= 6) {
					SpecialWeaponSelected = 0;
				}
			}
			}
		} else if (sakuyaheld > 0) {
			sakuyaheld -= Global.spf*4;	
			if (sakuyaheld <= 0) {
				onceloadout = false;
				playSound("weaponout", true, true);
			}
		}
	}
	public void autoAimSakuyaAttack() {
		int[] randomType0 = { 0, 353, 7 };
		int[] randomType1 = { 180, 173, 183 };
		if (autoAimShoot) {
			autoAimKnifeTime += Global.speedMul;
			if (autoAimKnifeCount <= 18) {
				if (autoAimKnifeTime >= Helpers.randomRange(7, 11)) {
					autoAimKnifeTime = 0;
					autoAimKnifeCount++;
					if (xDir == 1) {
						new SakuyaAutoAimKnife(
							getCenterPos().addxy(Helpers.randomRange(-30, 25), Helpers.randomRange(-40, 15)),
							1, this, player, player.getNextActorNetId(),
							randomType0[Helpers.randomRange(0, 2)], rpc: true
						);
					} else if (xDir == -1) {
						new SakuyaAutoAimKnife(
							getCenterPos().addxy(Helpers.randomRange(-30, 25), Helpers.randomRange(-40, 15)),
							1, this, player, player.getNextActorNetId(),
							randomType1[Helpers.randomRange(0, 2)], rpc: true
						);
					}
				}
			}
		}
		if (autoAimKnifeCount == 18) {
			autoAimShoot = false;
			autoAimKnifeCount = 0;
		}
	}
	public void SnailTimeVoid() {
		if (isWarpIn() || charState is Die) {
			OnceSnail = true;
		}
		if (SnailHeld >= 4) {
			OnceSnail = false;
		}
		if (shootHeld && SnailHeld < 4 && !(isWarpIn() || isWarpOut())) {
			SnailHeld += Global.spf * 5;
			if (SnailHeld >= 4) {
				playSound("snailready", true, true);
			}
		} else if (!shootHeld && SnailHeld <= 4 && SnailHeld > 0.1 && !OnceSnail && !(isWarpIn() || isWarpOut())) {
			SnailHeld = 0;
			OnceSnail = true;
			chargedCrystalHunter = new CrystalHunterChargedS(getCenterPos(), player, player.getNextActorNetId(), player.ownedByLocalPlayer, sendRpc: true);
			if (grounded)
				changeSpriteFromName("snail", true);
			else changeSpriteFromName("snail_air", true);
		}
		if (chargedCrystalHunter != null) {
			if (chargedCrystalHunter.destroyed) {
				chargedCrystalHunter = null;
			} else if (charState is Die) {
				chargedCrystalHunter.destroySelf();
			} else {
				chargedCrystalHunter.changePos(getCenterPos());
			}
		}
		if (!shootHeld) {
			Helpers.decrementFrames(ref SnailHeld);
		}
	}
	public void SakuyaTimeStopVoid() {
		if (isSakuyaTimeStopped) {
			SakuyaTimeStopClock -= Global.spf*1.25f;
		} else if (SakuyaTimeStopClock <= 30) {
			SakuyaTimeStopClock += Global.spf;
		}
		if (SakuyaTimeStopClock >= 30) {
			SakuyaTimeStopClock = 30;
		}
		if (SakuyaTimeStopClock > 1 && specialPressed && SakuyaTimeStopProj == null) {
			isSakuyaTimeStopped = true;
			SakuyaTimeStopProj = new SakuyaTimeStop(getCenterPos().addxy(0,-59), xDir, player, player.getNextActorNetId(), true);
			changeState(new SakuyaTimeStopSprite(), true);
		}
		if (specialPressed) {
			tapcheck++;
		}
		if (tapcheck <= 0) {
			tapcheck = 0;
		} 
		tapcheck -= Global.spf/2;
		if (SakuyaTimeStopProj != null) {
			timeclock += Global.spf;
			if (timeclock >= 1) {
				timeclock = 0;
				playSound("timestopclock", true, true);
			}
			if (specialPressed && tapcheck > 2) {
				tapcheck = 0;
				isSakuyaTimeStopped = false;
				SakuyaTimeStopProj.destroySelf();
				SakuyaTimeStopProj = null;
			}
			else if (SakuyaTimeStopClock <= 0) {
				isSakuyaTimeStopped = false;
				SakuyaTimeStopProj.destroySelf();
				SakuyaTimeStopProj = null;
			} else if (charState is Die) {
				SakuyaTimeStopProj?.destroySelf();
				SakuyaTimeStopProj = null;
				isSakuyaTimeStopped = false;
			} else {
				SakuyaTimeStopProj?.changePos(getCenterPos().addxy(0,-59));
			}
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
public class CrystalHunterChargedS : Actor {
	public float time;
	public Player owner;
	Sakuya? sakuya;
	public ShaderWrapper? timeSlowShader;
	public const int radius = 384;
	public float drawRadius = 384;
	public float drawAlpha = 64;
	float maxTime = 8;
	bool once;
	public CrystalHunterChargedS(
		Point pos, Player owner, ushort? netId, bool ownedByLocalPlayer, bool sendRpc = false
	) : base(
		"empty", pos, netId, ownedByLocalPlayer, false
	) {
		useGravity = false;
		this.owner = owner;
		netOwner = owner;
		canBeLocal = true;
		netActorCreateId = NetActorCreateId.CrystalHunterChargedS;
		Global.level.chargedCrystalHuntersS.Add(this);
		if (sendRpc) {
			createActorRpc(owner.id);
		}
		if (Options.main.enablePostProcessing) {
			timeSlowShader = owner.timeSlowShader;
		}
	}
	public override void update() {
		base.update();
		var screenCoords = new Point(pos.x - Global.level.camX, pos.y - Global.level.camY);
		var normalizedCoords = new Point(screenCoords.x / Global.viewScreenW, 1 - screenCoords.y / Global.viewScreenH);
		if (!once) {
			playSound("snaileffect");
			once = true;
		}
		if (timeSlowShader != null) {
			timeSlowShader.SetUniform("x", normalizedCoords.x);
			timeSlowShader.SetUniform("y", normalizedCoords.y);
			timeSlowShader.SetUniform("t", Global.time);
			if (Global.viewSize == 1) timeSlowShader.SetUniform("r", 0.75f);
			else timeSlowShader.SetUniform("r", 0.5f);
		}
		if (timeSlowShader == null) {
			drawRadius = 384 + 0.5f * MathF.Sin(Global.time * 10);
			drawAlpha = 64f + 32f * MathF.Sin(Global.time * 10);
		}
		time += Global.spf;
		if (time > maxTime) {
			destroySelf(disableRpc: true);
		}
	}
	public override void onDestroy() {
		base.onDestroy();
		Global.level.chargedCrystalHuntersS.Remove(this);
	}
	public override void render(float x, float y) {
		base.render(x, y);
		if (timeSlowShader == null) {
			var fillColor = new Color(96, 80, 240, Helpers.toByte(drawAlpha));
			var lineColor = new Color(208, 200, 240, Helpers.toByte(drawAlpha));
			if (owner.alliance == GameMode.redAlliance && Global.level?.gameMode?.isTeamMode == true) {
				fillColor = new Color(240, 80, 96, Helpers.toByte(drawAlpha));
			}
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
public class SakuyaTimeStop : Projectile {
	public float timeclock;
	public Character? character;
	public ShaderWrapper? STimeStop;
	public float radius = 384;
	public float attackRadius => (radius + 80);
	public Sakuya? sakuya;
	public bool once;
	public SakuyaTimeStop(
		 Point pos, int xDir, Player player, ushort netId, bool rpc = false
	) : base(
		SakuyaProjW.netWeapon, pos, xDir, 0, 0, player, "empty", 0, 0f, netId, player.ownedByLocalPlayer
	) {
		if (sakuya != null) {
			maxTime = sakuya.SakuyaTimeStopClock;
		}
		setIndestructableProperties();
		useGravity = false;
		canBeLocal = true;
		projId = (int)ProjIds.SakuyaTimeStop;
		Global.level.SakuyaTimeStop.Add(this);
		STimeStop = player.STimeStop;
		updateShader();
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}
	public override void onHitDamagable(IDamagable damagable) {
		base.onHitDamagable(damagable);
		if (character != null)
		character.isSakuyaTimeStopped = true;
	}
	public override void update() {
		var rect = new Rect(0, 0, Global.windowW, Global.windowH);
		globalCollider = new Collider(rect.getPoints(), true, this, false, false, 0, new Point(0, 0));
		updateShader();
		base.update();
	}
	public override void onDestroy() {
		base.onDestroy();
		Global.level.SakuyaTimeStop.Remove(this);
		playSound("timestopend", true, true);
	}
	public override void render(float x, float y) {
		if (Global.showHitboxes && collider != null) {
			DrawWrappers.DrawRect(0, 0, Global.windowW, Global.windowH, true, new Color(255,0,0, 40), 1, ZIndex.Default + 1);
		}
		base.render(x, y);
	}
	public void updateShader() {
		if (STimeStop != null) {
			var screenCoords = new Point(
				pos.x - Global.level.camX,
				pos.y - Global.level.camY
			);
			var normalizedCoords = new Point(
				screenCoords.x / Global.viewScreenW,
				1 - screenCoords.y / Global.viewScreenH
			);
			float ratio = Global.screenW / (float)Global.screenH;
			float normalizedRadius = (radius / Global.screenH);

			STimeStop.SetUniform("ratio", ratio);
			STimeStop.SetUniform("x", normalizedCoords.x);
			STimeStop.SetUniform("y", normalizedCoords.y);

			STimeStop.SetUniform("r", normalizedRadius);
			
		}
	}
}
