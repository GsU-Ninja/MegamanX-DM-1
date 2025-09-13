using System;
using System.Collections.Generic;
using System.Linq;

namespace MMXOnline;

public class BusterZero : Character {
	public bool upHeld => player.input.isHeld(Control.Up, player);
	public bool downHeld => player.input.isHeld(Control.Down, player);
	public bool WLeftPressed => player.input.isPressed(Control.WeaponLeft, player);
	public bool WRightPressed => player.input.isPressed(Control.WeaponRight, player);
	public float zSaberCooldown;
	public float lemonCooldown;
	public bool isBlackZero;
	public int stockedBusterLv;
	public bool stockedSaber;
	public List<DZBusterProj> zeroLemonsOnField = new();
	public YammarkOption? YammarkOptionOnField;
	public ZBusterSaber meleeWeapon = new();
	public BZTTriThunderWeapon TriThunderWeapon = new();
	public BZYammarkOption YammarkOptionWeapon = new();
	public BZLightningWeapon LightningWeapon = new();
	public const int UnlockTree = 1;
	public int shootPressTime;
	public int specialPressTime;
	public int swingPressTime;
	public bool shootPressed => (shootPressTime > 0);
	public bool specialPressed => (specialPressTime > 0);
	public bool specialDownPressed => (specialDownTime > 0);
	public bool specialUpPressed => (specialUpTime > 0);
	public int specialUpTime;
	public int specialDownTime;
	public float BZFishFangC, BZBubbleSplashC;
	public float noDamageTime;
	public float timetest;
	public int maxTotalChipHealAmount = 24;
	public int totalChipHealAmount;
	public float rechargeHealthTime;
	public float MagnetMineCooldown;
	public BZMagnetMineProjCharged? BZMagnetMineProjCharged;
	public BoomerangShield? boomerangShield;
	public YammarkOptionClass? yammarkOptionClass;
	public BZParasiteBombClass? parasiteBombClass;
	public BZBeeCursorAnim? parasiteBombAnim;
	public float YammarkShotCooldown;
	public float shootCooldown;

	public BusterZero(
		Player player, float x, float y, int xDir,
		bool isVisible, ushort? netId, bool ownedByLocalPlayer, bool isWarpIn = true
	) : base(
		player, x, y, xDir, isVisible, netId, ownedByLocalPlayer, isWarpIn
	) {
		charId = CharIds.BusterZero;
		altCtrlsLength = 1;
		if (!player.weapons.Any(w => w is ZeroBuster)) {
			player.weapons.Add(new ZeroBuster());
		}
	}

	public override void update() {
		if (stockedBusterLv > 0 || stockedSaber) {
			var renderGfx = stockedBusterLv switch {
				_ when stockedSaber || stockedBusterLv == 2 => RenderEffectType.ChargeGreen,
				1 => RenderEffectType.ChargePink,
				2 => RenderEffectType.ChargeOrange,
				_ => RenderEffectType.ChargeBlue
			};
			addRenderEffect(renderGfx, 0.033333f, 0.1f);
		}
		if (!ownedByLocalPlayer) {
			return;
		}
		// Hypermode music.
		if (!Global.level.isHyper1v1()) {
			if (isBlackZero && ownedByLocalPlayer) {
				if (musicSource == null) {
					addMusicSource("zero_X3", getCenterPos(), true);
				}
			} else {
				destroyMusicSource();
			}
		}
		// Cooldowns.
		inputUpdate();
		Helpers.decrementFrames(ref zSaberCooldown);
		Helpers.decrementFrames(ref lemonCooldown);
		Helpers.decrementFrames(ref BZFishFangC);
		Helpers.decrementFrames(ref BZBubbleSplashC);
		Helpers.decrementFrames(ref shootCooldown);
		Helpers.decrementTime(ref YammarkShotCooldown);
		Helpers.decrementTime(ref MagnetMineCooldown);
		if (yammarkOptionClass != null) {
			yammarkOptionClass.update();
		}
		if (parasiteBombClass != null) {
			parasiteBombClass.update();
		}
		if (parasiteBombAnim != null) {
			parasiteBombAnim.update();
		}
		YammarkOptionWeapon.update();
		YammarkOptionWeapon.charLinkedUpdate(this, true);
		TriThunderWeapon.update();
		TriThunderWeapon.charLinkedUpdate(this, true);
		LightningWeapon.update();
		LightningWeapon.charLinkedUpdate(this, true);
		player.changeWeaponControls();
		base.update();
		// For the shooting animation.
		Helpers.decrementTime(ref shootAnimTime);
		if (shootAnimTime <= 0 && charState.attackCtrl && !charState.isGrabbing) {
			changeSpriteFromName(charState.defaultSprite, false);
			if (charState is WallSlide) {
				frameIndex = sprite.totalFrameNum - 1;
			}
		}
		// Charge and release charge logic.
		chargeLogic(shoot);
		if (player.HelmetAutoCharge && getChargeLevel() > 1 && shootPressed) shoot(getChargeLevel());
		else if (shootPressed && !player.HelmetAutoCharge) shoot(getChargeLevel());
		IhidmyselfwhileIrepairedmyself();
		TractorShot();
		TreeFixes();
		YammarkVoid();
		TriThunderVoid();
		LightingVoid();
	}
	public override void chargeGfx() {
		if (ownedByLocalPlayer) {
			chargeEffect.stop();
		}
		if (isCharging()) {
			chargeSound.play();
			int chargeType = 1;
			int level = getChargeLevel();
			var renderGfx = RenderEffectType.ChargeBlue;
			renderGfx = level switch {
				1 => RenderEffectType.ChargeBlue,
				2 => RenderEffectType.ChargeYellow,
				3 => RenderEffectType.ChargePink,
				_ => RenderEffectType.ChargeGreen,
			};
			addRenderEffect(renderGfx, 0.033333f, 0.1f);
			chargeEffect.update(getChargeLevel(), chargeType);
		}
	}

	public void setShootAnim() {
		string shootSprite = getSprite(charState.shootSprite);
		if (!Global.sprites.ContainsKey(shootSprite)) {
			if (grounded) { shootSprite = "zero_shoot"; } else { shootSprite = "zero_fall_shoot"; }
		}
		changeSprite(shootSprite, false);
		if (charState is Idle) {
			frameIndex = 0;
			frameTime = 0;
		}
		if (charState is LadderClimb) {
			if (player.input.isHeld(Control.Left, player)) {
				this.xDir = -1;
			} else if (player.input.isHeld(Control.Right, player)) {
				this.xDir = 1;
			}
		}
		shootAnimTime = 0.3f;
	}
	public override bool canCharge() {
		return (stockedBusterLv == 0 && !stockedSaber && !isInvulnerableAttack());
	}
	public override bool normalCtrl() {
		// Handles Standard Hypermode Activations.
		if (player.currency >= Player.zBusterZeroHyperCost &&
			!isBlackZero &&
			player.input.isHeld(Control.Special2, player) &&
			charState is not HyperZeroStart and not WarpIn
		) {
			hyperProgress += Global.spf;
		} else {
			hyperProgress = 0;
		}
		if (hyperProgress >= 1 && player.currency >= Player.zBusterZeroHyperCost) {
			hyperProgress = 0;
			changeState(new HyperBusterZeroStart(), true);
			return true;
		}
		return base.normalCtrl();
	}
	public override bool attackCtrl() {
		bool shootPressed = player.input.isPressed(Control.Shoot, player);
		bool specialPressed = player.input.isPressed(Control.Special1, player);
		bool specialUpPressed = player.input.isPressed(Control.Special1, player) && player.input.isHeld(Control.Up, player);
		bool specialDownPressed = player.input.isPressed(Control.Special1, player) && player.input.isHeld(Control.Down, player);
		if (grounded) {
			if (specialDownPressed && player.ArmorModeRise && zSaberCooldown <= 0) {
				changeState(new BusterZeroHuuX6(), true);
				return true;
			}
			if (isDashing && specialPressed && zSaberCooldown <= 0 && player.ArmorModeActive) {
				changeState(new BusterZeroRollingSlash(), true);
				return true;
			}
			if (isDashing && specialPressed && zSaberCooldown <= 0 && player.BZDash) {
				slideVel = xDir * getDashSpeed() * 1.1f;
				changeState(new BusterZeroDashAttack(), true);
				return true;
			}
			if (WRightPressed && downHeld && player.BZFrostShield) {
				changeState(new BusterZeroFrostShield(), true);
				return true;
			}
			if (WRightPressed && upHeld && player.BZShieldBoomerang && getChargeLevel() > 3) {
				new BoomerangShield(getCenterPos().addxy(52 * xDir, 0), xDir,
				player, player.getNextActorNetId(), true);
				stopCharge();
				return true;
			}
			if (specialUpPressed && zSaberCooldown <= 0 && player.BZTenshouzan) {
				changeState(new BusterZeroTenshouzan(), true);
				return true;
			}
			if (specialUpPressed && BZBubbleSplashC <= 0 && player.BZBubblesplash) {
				changeState(new BusterZeroBubbleSplash(), true);
				return true;
			}
		}
		if (!grounded) {
			if (specialDownPressed && zSaberCooldown <= 0 && player.BZDrillCrush) {
				changeState(new BusterZeroDrillCrush(), true);
				return true;
			}
		}
		if (specialPressed) {
			if (zSaberCooldown == 0) {
				if (stockedSaber) {
					changeState(new BusterZeroTHadangeki2(), true);
					return true;
				}
				if (charState is WallSlide wallSlide) {
					changeState(new BusterZeroMeleeWall(wallSlide.wallDir, wallSlide.wallCollider), true);
				} else {
					changeState(new BusterZeroTHadangeki(), true);
				}
				return true;
			}
		}
		if (!grounded) {
			if (specialPressed && BZFishFangC <= 0 && player.BZFishFang) {
				changeState(new BusterZeroFishFang(), true);
				return true;
			}
		}
		if (!isCharging()) {
			if (shootPressed) {
				lastShootPressed = Global.frameCount;
			}
			int framesSinceLastShootPressed = Global.frameCount - lastShootPressed;
			if (shootPressed || framesSinceLastShootPressed < 6) {
				if (stockedBusterLv == 1) {
					if (charState is WallSlide) {
						Buster2Proj(this);
						shootCooldown = 22;
						stockedBusterLv = 0;
						return true;
					}
					changeState(new BusterZeroDoubleBuster(true, true), true);
					return true;
				}
				if (stockedBusterLv == 2) {
					if (charState is WallSlide) {
						Buster3Proj(0, 3, this);
						shootCooldown = 22;
						stockedBusterLv = 0;
						lastShootPressed = 0;
						return true;
					}
					changeState(new BusterZeroDoubleBuster(true, false), true);
					return true;
				}
				if (stockedSaber) {
					if (charState is WallSlide wsState) {
						changeState(new BusterZeroHadangekiWall(wsState.wallDir, wsState.wallCollider), true);
						return true;
					}
					if (!player.UnlockTree)
						changeState(new BusterZeroTHadangeki(), true);
					else changeState(new BusterZeroTHadangeki2(), true);
					return true;
				}
				if (shootCooldown <= 0) {
					shoot(0);
					return true;
				}
			}
		}
		return base.attackCtrl();
	}

	public void shoot(int chargeLevel) {
		//if (!player.weapon.canShoot(chargeLevel, player)) return;
		if (!canShoot()) return;
		setShootAnim();
		float ammoUsage = -player.weapon.getAmmoUsage(chargeLevel);
		shootCooldown = player.weapon.fireRate;
		player.weapon.addAmmo(ammoUsage, player);
		player.weapon.bZeroShoot(this, new int[] { chargeLevel });
		string sound = "";
		Point shootPos = getShootPos();
		int xDir = getShootXDir();
		if (chargeLevel == 0) {
			for (int i = zeroLemonsOnField.Count - 1; i >= 0; i--) {
				if (zeroLemonsOnField[i].destroyed) {
					zeroLemonsOnField.RemoveAt(i);
				}
			}
			if (zeroLemonsOnField.Count >= 4 && player.ArmorModeX) { return; }
			if (zeroLemonsOnField.Count >= 3 && !player.ArmorModeX) { return; }
		}
		if (!charState.BZBugFix) {
			if (chargeLevel == 0) {
				var lemon = new DZBusterProj(
					shootPos, xDir, player, player.getNextActorNetId(), rpc: true
				);
				zeroLemonsOnField.Add(lemon);
				sound = "busterX3";
				shootCooldown = 9;
			} else if (chargeLevel == 1) {
				Buster2Proj(this);
				shootCooldown = 22;
			} else if (chargeLevel == 2) {
				shootCooldown = 22;
				if (player.BZVShot) {
					Buster3Proj(1, 1, this);
					Buster3Proj(2, 1, this);
				} else {
					Buster3Proj(0, 3, this);
				}
			} else if (chargeLevel == 3) {
				if (charState is WallSlide) {
					stockedBusterLv = 1;
					Buster3Proj(0, 3, this);
					shootCooldown = 22;
					return;
				} else {
					shootAnimTime = 0;
					changeState(new BusterZeroDoubleBuster(false, true), true);
				}
			} else if (chargeLevel >= 4) {
				if (charState is WallSlide) {
					Buster3Proj(0, 3, this);
					stockedBusterLv = 2;
					stockedSaber = true;
					shootCooldown = 22;
					return;
				} else {
					shootAnimTime = 0;
					changeState(new BusterZeroDoubleBuster(false, false), true);
				}
			}
			if (chargeLevel >= 1) {
				stopCharge();
			}
		}
		if (!string.IsNullOrEmpty(sound)) playSound(sound, sendRpc: true);
	}
	public override void destroySelf(
		string spriteName = "", string fadeSound = "",
		bool disableRpc = false, bool doRpcEvenIfNotOwned = false,
		bool favorDefenderProjDestroy = false
	) {
		base.destroySelf(spriteName, fadeSound, disableRpc, doRpcEvenIfNotOwned, favorDefenderProjDestroy);
		yammarkOptionClass?.destroy();
	}
	public void Buster3Proj(int type, int dmg, Character character) {
		Point shootPos = character.getShootPos();
		int xDir = character.getShootXDir();
		Player player = character.player;
		character.playSound("buster3X3");
		new DZBuster3Proj(player.ArmorModeUltimate ? dmg + 1 : dmg, type, shootPos, xDir, player, player.getNextActorNetId(), rpc: true);
	}
	public void Buster2Proj(Character character) {
		Point shootPos = character.getShootPos();
		int xDir = character.getShootXDir();
		Player player = character.player;
		character.playSound("buster2X3");
		new DZBuster2Proj(player.ArmorModeUltimate ? 3 : 2, shootPos, xDir, player, player.getNextActorNetId(), 0, rpc: true);
	}
	public override bool canShoot() {
		if (isInvulnerableAttack()) return false;
		if (shootCooldown > 0) return false;
		if (invulnTime > 0) return false;
		return base.canShoot();
	}
	public void TreeFixes() {
		if (player.ArmorModeActive && player.ArmorModeProto &&
			player.ArmorModeRise && player.ArmorModeX && player.ArmorModeDefense &&
			player.ArmorModeErase && player.ArmorModePower && player.ArmorModeEnergy) {
			player.ArmorModeUltimate = true;
		}
		if (!player.ArmorModeActive || !player.ArmorModeProto! ||
			  !player.ArmorModeRise || !player.ArmorModeX! || !player.ArmorModeDefense ||
			 !player.ArmorModeErase || !player.ArmorModePower! || !player.ArmorModeEnergy) {
			player.ArmorModeUltimate = false;
		}
		if (!player.ArmorModeX || !player.ArmorModeActive) player.ArmorModeEnergy = false;
		if (!player.ArmorModeX || !player.ArmorModeProto) player.ArmorModeErase = false;
		if (!player.ArmorModeProto || !player.ArmorModeRise) player.ArmorModeDefense = false;
		if (!player.ArmorModeProto || !player.ArmorModeRise) player.ArmorModeDefense = false;
		if (!player.ArmorModeActive || !player.ArmorModeRise) player.ArmorModePower = false;
	}
	public override int getHitboxMeleeId(Collider hitbox) {
		return (int)(sprite.name switch {
			"zero_projswing" or "zero_projswing_air" or "zero_wall_slide_attack" => MeleeIds.SaberSwing,
			"zero_swing" or "zero_swing_air" => MeleeIds.BusterZeroHadangeki,
			"zero_attack3x6" => MeleeIds.HuuX6,
			"zero_tenshouzan" => MeleeIds.Tenshouzan,
			"zero_attack_dash3" => MeleeIds.DashAttack,
			"zero_drillcrush" => MeleeIds.Drillcrush,
			"zero_bubblesplash" => MeleeIds.Bubblesplash,
			"zero_attack_air2" => MeleeIds.BusterZeroRSlash,
			_ => MeleeIds.None
		});
	}
	public override Projectile? getMeleeProjById(int id, Point projPos, bool addToLevel = true) {
		Projectile? proj = id switch {
			(int)MeleeIds.SaberSwing => new GenericMeleeProj(
				meleeWeapon, projPos, ProjIds.DZMelee, player,
				isBlackZero ? 4 : 3, Global.defFlinch, 0.5f,
				isReflectShield: true, addToLevel: addToLevel
			),
			(int)MeleeIds.HuuX6 => new GenericMeleeProj(
				meleeWeapon, projPos, ProjIds.BusterZHuuX6, player,
				player.ArmorModeProto ? 2.5f : 2, player.ArmorModePower ? 26 : 13,
				0.5f, isReflectShield: player.ArmorModeErase, isDeflectShield: !player.ArmorModeErase,
				addToLevel: addToLevel
			),
			(int)MeleeIds.Tenshouzan => new GenericMeleeProj(
				meleeWeapon, projPos, ProjIds.BusterZTenshouzan, player,
				player.ArmorModeProto ? 2.5f : 2, Global.halfFlinch,
				0.5f, isReflectShield: player.ArmorModeErase, isDeflectShield: !player.ArmorModeErase,
				addToLevel: addToLevel
			),
			(int)MeleeIds.DashAttack => new GenericMeleeProj(
				meleeWeapon, projPos, ProjIds.BusterZDashAttack, player,
				player.ArmorModeProto ? 2.5f : 2, Global.halfFlinch,
				0.5f, addToLevel: addToLevel
			),
			(int)MeleeIds.BusterZeroHadangeki => new GenericMeleeProj(
				meleeWeapon, projPos, ProjIds.BusterZHadangeki, player,
				player.ArmorModeProto ? 2.5f : 2, player.ArmorModePower ? 26 : 13,
				0.75f, isReflectShield: player.ArmorModeErase, isDeflectShield: !player.ArmorModeErase,
				addToLevel: addToLevel
			),
			(int)MeleeIds.Drillcrush => new GenericMeleeProj(
				meleeWeapon, projPos, ProjIds.BusterZDrillCrush, player,
				player.ArmorModeProto ? 2.5f : 2, Global.defFlinch,
				2f, addToLevel: addToLevel
			),
			(int)MeleeIds.Bubblesplash => new GenericMeleeProj(
				meleeWeapon, projPos, ProjIds.BusterZBubbleSplash, player,
				2, Global.defFlinch, 0.5f, addToLevel: addToLevel
			),
			(int)MeleeIds.BusterZeroRSlash => new GenericMeleeProj(
				meleeWeapon, projPos, ProjIds.BusterZRollingSlash, player,
				player.ArmorModeProto ? 1.5f : 1, player.ArmorModePower ? 4 : 0,
				1.25f, isReflectShield: player.ArmorModeErase, isDeflectShield: !player.ArmorModeErase,
				addToLevel: addToLevel
			),
			_ => null
		};
		return proj;
	}
	public enum MeleeIds {
		None = -1,
		SaberSwing,
		HuuX6,
		Tenshouzan,
		DashAttack,
		Drillcrush,
		Bubblesplash,
		BusterZeroHadangeki,
		BusterZeroRSlash
	}
	public override string getSprite(string spriteName) {
		return "zero_" + spriteName;
	}
	public override bool chargeButtonHeld() {
		return player.input.isHeld(Control.Shoot, player) || player.HelmetAutoCharge;
	}
	public override void increaseCharge() {
		float factor = 1;
		if (isBlackZero) factor = 1.5f;
		if (player.ArmorModeX && !isBlackZero) factor = 1.2f;
		if (player.ArmorModeUltimate && !isBlackZero) factor = 1.5f;
		if (player.ArmorModeUltimate && isBlackZero) factor = 2f;
		chargeTime += Global.speedMul * factor;
	}
	public override float getRunSpeed() {
		float runSpeed = 90;
		if (isBlackZero) {
			runSpeed *= 1.15f;
		}
		if (player.BootsFastRun) {
			runSpeed *= 1.15f;
		}
		return runSpeed * getRunDebuffs();
	}
	public override float getDashSpeed() {
		if (flag != null || !isDashing) {
			return getRunSpeed();
		}
		float dashSpeed = 210;
		if (isBlackZero) {
			dashSpeed *= 1.15f;
		}
		return dashSpeed * getRunDebuffs();
	}
	public override bool canAirDash() {
		return dashedInAir == 0
		   || (dashedInAir == 1 && isBlackZero)
		   || (dashedInAir == 1 && player.BootsDash && !isBlackZero)
		   || (dashedInAir == 2 && player.BootsDash && isBlackZero);
	}
	public override bool canAirJump() {
		return dashedInAir == 0
		   || (dashedInAir == 1 && isBlackZero)
		   || (dashedInAir == 1 && player.BootsJump && !isBlackZero)
		   || (dashedInAir == 2 && player.BootsJump && isBlackZero);
	}
	public override float getLabelOffY() {
		if (sprite.name.Contains("_ra_")) {
			return 25;
		}
		return 45;
	}
	public void IhidmyselfwhileIrepairedmyself() {
		Helpers.decrementTime(ref timetest);
		if (player.HelmetAutoRecover && totalChipHealAmount < maxTotalChipHealAmount) {
			noDamageTime += Global.spf;
			if ((player.health < player.maxHealth / 1.5) && noDamageTime > 12) {
				rechargeHealthTime -= Global.spf;
				if (rechargeHealthTime <= 0 && timetest <= 0) {
					rechargeHealthTime = 2;
					addHealth(1);
					totalChipHealAmount++;
				}
			}
		}
	}
	public void inputUpdate() {
		if (shootPressTime > 0) {
			shootPressTime--;
		}
		if (specialPressTime > 0) {
			specialPressTime--;
		}
		if (swingPressTime > 0) {
			swingPressTime--;
		}
		if (specialDownTime > 0) {
			specialDownTime--;
		}
		if (specialUpTime > 0) {
			specialUpTime--;
		}
		if (player.input.isPressed(Control.Shoot, player)) {
			shootPressTime = 6;
		}
		if (player.input.isPressed(Control.Special1, player)) {
			specialPressTime = 6;
		}
		if (player.input.isHeld(Control.Up, player)) {
			specialUpTime = 6;
		}
		if (player.input.isHeld(Control.Down, player)) {
			specialDownTime = 6;
		}
	}
	public override bool altCtrl(bool[] ctrls) {
		if (charState is BusterZeroGenericMeleeState zgms) {
			zgms.altCtrlUpdate(ctrls);
		}
		return base.altCtrl(ctrls);
	}
	public override bool changeState(CharState newState, bool forceChange = false) {
		// Save old state.
		CharState oldState = charState;
		// Base function call.
		bool hasChanged = base.changeState(newState, forceChange);
		if (!hasChanged) {
			return false;
		}
		if (!newState.attackCtrl || newState.attackCtrl != oldState.attackCtrl) {
			shootPressTime = 0;
			specialPressTime = 0;
			specialUpTime = 0;
			specialDownTime = 0;
			specialPressTime = 0;
		}
		return true;
	}
	public void TractorShot() {
		if (isCharging() && player.BZTractorShot && MagnetMineCooldown == 0) {
			MagnetMineCooldown = 12f;
			if (BZMagnetMineProjCharged == null) {
				BZMagnetMineProjCharged = new BZMagnetMineProjCharged(getShootPos(),
				xDir, player, player.getNextActorNetId(), rpc: true);
			}
			if (BZMagnetMineProjCharged != null) {
				if (BZMagnetMineProjCharged.timee >= 60f / 60f) {
					BZMagnetMineProjCharged?.destroySelf();
					BZMagnetMineProjCharged = null;
				}
			}
		}
	}
	public void YammarkVoid() {
		if (player.BZYammarkOption && (!player.weapons.Any(w => w is BZYammarkOption))) {
			player.weapons.Add(new BZYammarkOption());
		}
		if (!player.BZYammarkOption) {
			player.weapons.Remove(new BZYammarkOption());
		}
	}
	public void TriThunderVoid() {
		if (player.BZTriThunder && (!player.weapons.Any(w => w is BZTTriThunderWeapon))) {
			player.weapons.Add(new BZTTriThunderWeapon());
		} else if (!player.BZTriThunder) {
			player.weapons.Remove(new BZTTriThunderWeapon());
		}
	}
	public void LightingVoid() {
		if (player.BZZLighting && (!player.weapons.Any(w => w is BZLightningWeapon))) {
			player.weapons.Add(new BZLightningWeapon());
		} else if (!player.BZZLighting) {
			player.weapons.Remove(new BZLightningWeapon());
		}
	}
	public override bool canChangeWeapons() {
		if (charState is BusterZeroLighting) return false;
		if (charState is BusterZeroTriThunder) return false;
		return base.canChangeWeapons();
	}
	public override void addAmmo(float amount) {
		getRefillTargetWeapon()?.addAmmoHeal(amount);
	}
	public override void addPercentAmmo(float amount) {
		getRefillTargetWeapon()?.addAmmoPercentHeal(amount);
	}
	public override bool canAddAmmo() {
		if (player.weapon == null) { return false; }
		if (!player.BZTriThunder && player.weapon is BZTTriThunderWeapon) return false;
		if (!player.BZYammarkOption && player.weapon is BZYammarkOption) return false;
		if (!player.BZZLighting && player.weapon is BZLightningWeapon) return false;
		if (player.weapon is ZeroBuster) return false;
		if (player.weapon.ammo >= player.weapon.maxAmmo) return false;
		bool hasEmptyAmmo = false;
		foreach (Weapon weapon in player.weapons) {
			if (weapon.canHealAmmo && weapon.ammo < weapon.maxAmmo) {
				hasEmptyAmmo = true;
				break;
			}
		}
		return hasEmptyAmmo;
	}
	public Weapon? getRefillTargetWeapon() {
		if (player.weapon.canHealAmmo && player.weapon.ammo < player.weapon.maxAmmo) {
			return player.weapon;
		}
		Weapon? targetWeapon = null;
		float targetAmmo = Int32.MaxValue;
		foreach (Weapon weapon in player.weapons) {
			if (!weapon.canHealAmmo) {
				continue;
			}
			if (weapon != player.weapon &&
				weapon.ammo < weapon.maxAmmo &&
				weapon.ammo < targetAmmo
			) {

			}
		}
		return targetWeapon;
	}
	public override List<ShaderWrapper> getShaders() {
		List<ShaderWrapper> baseShaders = base.getShaders();
		List<ShaderWrapper> shaders = new();
		ShaderWrapper? palette = null;

		if (isBlackZero) {
			palette = player.zeroPaletteShader;
			palette?.SetUniform("palette", 1);
			palette?.SetUniform("paletteTexture", Global.textures["hyperBusterZeroPalette"]);
		}
		if (palette != null) {
			shaders.Add(palette);
		}
		if (shaders.Count == 0) {
			return baseShaders;
		}
		shaders.AddRange(baseShaders);
		return shaders;
	}
	public override List<byte> getCustomActorNetData() {
		List<byte> customData = base.getCustomActorNetData();
		customData.Add((byte)MathF.Floor(TriThunderWeapon.ammo));
		customData.Add(Helpers.boolArrayToByte([
			isBlackZero,
		]));
		return customData;
	}
	public override void updateCustomActorNetData(byte[] data) {
		// Update base arguments.
		base.updateCustomActorNetData(data);
		data = data[data[0]..];
		bool[] flags = Helpers.byteToBoolArray(data[0]);
		isBlackZero = flags[0];
	}
}