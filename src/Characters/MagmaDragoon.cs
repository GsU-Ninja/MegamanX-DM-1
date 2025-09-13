using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace MMXOnline;
public class MagmaDragoon : Character {
public bool isBoosted;
public float hyperModeTimer = 5;
public int shootPressTime;
public int specialPressTime;
public bool shootPressed => (shootPressTime > 0);
public bool specialPressed => (specialPressTime > 0);
public bool upHeld => player.input.isHeld(Control.Up, player);
public bool downHeld => player.input.isHeld(Control.Down, player);
public bool shootHeld => player.input.isHeld(Control.Shoot, player);
public bool specialHeld => player.input.isHeld(Control.Special1, player);
public bool WLeftPressed => player.input.isHeld(Control.WeaponLeft, player); 
public bool WRightPressed => player.input.isHeld(Control.WeaponRight, player); 
public bool stockedPower;
public float HadokenCooldown;
public float DashCooldown;
public float ShoryukenCooldown;
public float HibashiraCooldown;
public float KickCooldown;
public float RagingDemonCooldown;
public float FireAnimTime, Fire2AnimTime;
public float LandProjCooldown;
public MagmaDragoonDefensiveGiga DefensiveGiga = new();
public MagmaDragoonMeleeW meleeWeapon = new();
public MagmaDragoon(
		Player player, float x, float y, int xDir,
		bool isVisible, ushort? netId, bool ownedByLocalPlayer,
		bool isWarpIn = true, int? heartTanks = null, bool isATrans = false
	) : base(
		player, x, y, xDir, isVisible, netId, ownedByLocalPlayer, isWarpIn, heartTanks, isATrans
	) {
		charId = CharIds.MagmaDragoon;	
	}
	public override void update() {
		base.update();
		if (!ownedByLocalPlayer) {
			return;
		}
		if (player.OffensiveGigaAmmo >= player.OffensiveGigaMaxAmmo) {
			weaponHealAmount = 0;
		}
		if (weaponHealAmount > 0 && player.health > 0) {
			weaponHealTime += Global.spf;
			if (weaponHealTime > 0.05) {
				weaponHealTime = 0;
				weaponHealAmount--;
				player.OffensiveGigaAmmo = Helpers.clampMax(player.OffensiveGigaAmmo + 1, player.OffensiveGigaMaxAmmo);
				playSound("heal", forcePlay: true);
			}
		}
		inputUpdate();
		Helpers.decrementTime(ref HadokenCooldown);
		Helpers.decrementTime(ref DashCooldown);
		Helpers.decrementTime(ref ShoryukenCooldown);
		Helpers.decrementTime(ref HibashiraCooldown);
		Helpers.decrementTime(ref KickCooldown);
		Helpers.decrementTime(ref RagingDemonCooldown);
		Helpers.decrementTime(ref LandProjCooldown);
		DefensiveGiga.update();
		DefensiveGiga.charLinkedUpdate(this, true);
		if (isBoosted && hyperModeTimer > 0) {
			Helpers.decrementTime(ref hyperModeTimer);
		}
		if (hyperModeTimer <= 0) isBoosted = false;
		if (!isBoosted) hyperModeTimer = 5;
		if (isBoosted) {
			FireAnimTime += Global.spf;
			Fire2AnimTime += Global.spf;
			int random = Helpers.randomRange(-48, 48);
			int random2 = Helpers.randomRange(-32, 32);
			int random3 = Helpers.randomRange(-16, 16);
			int random4 = Helpers.randomRange(-20, 20);
			if (FireAnimTime > 0.2f) {
				FireAnimTime = 0;
				new Anim(getCenterPos().addxy(random,random2), "flamethrower_whk_fade",
				xDir, player.getNextActorNetId(), true, true);
			}
			if (Fire2AnimTime > 0.3f) {
				Fire2AnimTime = 0;
				new Anim(getCenterPos().addxy(random3,random4), "fire_wave_fade",
				xDir, player.getNextActorNetId(), true, true);
			}
		}
		if (sprite.name is "mdragoon_land" && sprite.frameIndex <= 0 && LandProjCooldown <= 0) {
			LandProjCooldown = isBoosted ? 1.5f : 3f;
			new Anim(getCenterPos().addxy(0,12), "mdragoon_land_proj_trail",
			xDir, player.getNextActorNetId(), true, true, zIndex: ZIndex.Character);
			new MagmaDragoonLandProjectile(getCenterPos().addxy(0,18), xDir, 
			player, player.getNextActorNetId(), true);
		}
		if (player.OffensiveGigaAmmo < 0) {
			player.OffensiveGigaAmmo = 0;
		}
	}
	public override bool normalCtrl() {
		if (player.dashPressed(out string dashControl) && DashCooldown <= 0
			&& (grounded || charState is WallKick)) {
			changeState(new MagmaDragoonDash(), true);
			return true;			
		}
		if (player.input.isPressed(Control.Dash, player) && DashCooldown <= 0
			&& (!grounded)) {
			changeState(new MagmaDragoonDashAir(), true);
			return true;			
		}
		if (DefensiveGiga.ammo >= DefensiveGiga.maxAmmo && player.input.isHeld(Control.Special2, player) 
		) {
			hyperProgress += Global.spf * 2;
		} else {
			hyperProgress = 0;
		}
		if (hyperProgress >= 1) {
			hyperProgress = 0;
			isBoosted = true;
			hyperModeTimer = 12;
			DefensiveGiga.ammo -= DefensiveGiga.maxAmmo;
			return true;
		}
		bool changedState = base.normalCtrl();
		if (changedState) {
			return true;
		}
		
		return base.normalCtrl();
	}
	public override bool attackCtrl() {
		if (WLeftPressed && player.OffensiveGigaAmmo >= 24
		    && isBoosted && RagingDemonCooldown <= 0) {
			changeState(new MagmaDragoonShungokusatsu(), true);
			charState.isGrabbing = true;
			RagingDemonCooldown = 2;
			return true;
		}
		if (upHeld && WRightPressed && grounded && player.OffensiveGigaAmmo >= 14) {
			changeState(new MagmaDragoonHonodanState(), true);
			return true;
		}
		if (WRightPressed && grounded && player.OffensiveGigaAmmo >= 14) {
			changeState(new MagmaDragoonKaenKogekiStateStart(), true);
			return true;
		}
		if (upHeld && shootPressed && grounded && ShoryukenCooldown <= 0) {
			changeState(new MagmaDragoonRaijinguFaia(), true);
			return true;
		}
		if (downHeld && shootPressed && !grounded && KickCooldown <= 0) {
			changeState(new MagmaDragoonKyukokaState(), true);
			return true;
		}
		if (shootPressed && grounded) {
			changeState(new MagmaDragoonHadoken(), true);
			if (downHeld) {
				changeState(new MagmaDragoonHadokenDown(), true);
			}
			return true;
		}
		if (specialPressed && grounded && HibashiraCooldown <= 0) {
			changeState(new MagmaDragoonHibashiraKogekiState(), true);
			return true;
		}
		return base.attackCtrl();
	}
	public override string getSprite(string spriteName) {
		return "mdragoon_" + spriteName;
	}
	public override bool altCtrl(bool[] ctrls) {
		if (charState is MagmaDragoonGenericMeleeState mmgs) {
			mmgs.altCtrlUpdate(ctrls);
		}
		return base.altCtrl(ctrls);
	}
	public override bool canAirJump() {
		return dashedInAir == 0;
	}
	public override bool canDash() {
		return false;
	}
	public override float getRunSpeed() {
		float runSpeed = Physics.WalkSpeed * 1.75f;
		if (isBoosted) {
			runSpeed *= 1.3f;
		}
		return runSpeed * getRunDebuffs();
	}
	public override void addAmmo(float amount) {
		DefensiveGiga.addAmmoHeal(amount);
		weaponHealAmount += amount;
	}
	public override void addPercentAmmo(float amount) {
		DefensiveGiga.addAmmoPercentHeal(amount);
		weaponHealAmount += amount * 0.32f;
	}
	public override bool canAddAmmo() {
		return (DefensiveGiga.ammo < DefensiveGiga.maxAmmo) || (player.OffensiveGigaAmmo < player.OffensiveGigaMaxAmmo);
	}
	public override bool canEnterRideArmor() {
		return false;
	}
	public override bool canEnterRideChaser() {
		return false;
	}
	public void inputUpdate() {
		if (shootPressTime > 0) {
			shootPressTime--;
		}
		if (specialPressTime > 0) {
			specialPressTime--;
		}
		if (player.input.isPressed(Control.Shoot, player)) {
			shootPressTime = 6;
		}
		if (player.input.isPressed(Control.Special1, player)) {
			specialPressTime = 6;
		}
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
		Kick,
		Shoryuken,
		RagingDemon,
		Dash,
	}
	public override int getHitboxMeleeId(Collider hitbox) {
		return (int)(sprite.name switch {
			"mdragoon_kick" => MeleeIds.Kick,
			"mdragoon_shoryuken" => MeleeIds.Shoryuken,
			"mdragoon_shungoku" => MeleeIds.RagingDemon,
			"mdragoon_dash" => MeleeIds.Dash,
			_ => MeleeIds.None
		});
	}
	public override Projectile? getMeleeProjById(int id, Point projPos, bool addToLevel = true) {
		return id switch {
			(int)MeleeIds.Kick => new GenericMeleeProj(
				meleeWeapon, projPos, ProjIds.MagmaDragoonKick, player, isBoosted ? 3 : 2, 
				isBoosted ? 26 : 13, 0.5f, addToLevel: addToLevel
			),
			(int)MeleeIds.Dash => new GenericMeleeProj(
				meleeWeapon, projPos, ProjIds.MagmaDragoonDash, player, isBoosted ? 2 : 1,
				isBoosted ? 13 : 6, 1.5f, addToLevel: addToLevel
			),
			(int)MeleeIds.Shoryuken => new GenericMeleeProj(
				meleeWeapon, projPos, ProjIds.MagmaDragoonShoryuken, player, isBoosted ? 3 : 2,
				26, 0.5f, addToLevel: addToLevel
			),
			(int)MeleeIds.RagingDemon => new GenericMeleeProj(
				meleeWeapon, projPos, ProjIds.MagmaDragoonRagingDemon, player,
				0, 0, 1f, isDeflectShield: true, addToLevel: addToLevel
			),
			_ => null
		};
	}
	public override bool canCrouch() {
		return false;
	}
	public override Collider getGlobalCollider() {
		Rect rect = new Rect(0, 0, 18, 48);
		return new Collider(rect.getPoints(), false, this, false, false, HitboxFlag.Hurtbox, new Point(0, 0));
	}
	public override List<byte> getCustomActorNetData() {
		List<byte> customData = base.getCustomActorNetData();
		customData.Add((byte)MathF.Floor(DefensiveGiga.ammo));
		customData.Add((byte)MathF.Floor(player.OffensiveGigaAmmo));

		customData.Add(Helpers.boolArrayToByte([
			isBoosted
		]));

		return customData;
	}
	public override void updateCustomActorNetData(byte[] data) {
		// Update base arguments.
		base.updateCustomActorNetData(data);
		data = data[data[0]..];

		// Per-player data.
		DefensiveGiga.ammo = data[0];
		player.OffensiveGigaAmmo = data[1];
		bool[] flags = Helpers.byteToBoolArray(data[2]);
		isBoosted = flags[0];
	}
}
public class MagmaDragoonOffensiveGiga : Weapon {
	public static MagmaDragoonOffensiveGiga netWeapon = new();
	public MagmaDragoonOffensiveGiga() : base() {
		//damager = new Damager(player, 4, Global.defFlinch, 0.5f);
		ammo = 0;
		maxAmmo = 28;
		fireRate = 1;
		index = (int)WeaponIds.MagmaDragoonGigaO;
		weaponBarBaseIndex = 27;
		weaponBarIndex = 35;
		killFeedIndex = 16;
		weaponSlotIndex = 51;
		displayName = "";
		description = new string[] { "" };
		drawGrayOnLowAmmo = true;
		drawRoundedDown = true;
	}

	public override float getAmmoUsage(int chargeLevel) {
		return 1;
	}
}
public class MagmaDragoonDefensiveGiga : Weapon {
	public static MagmaDragoonDefensiveGiga netWeapon = new();
	public MagmaDragoonDefensiveGiga() : base() {
		//damager = new Damager(player, 4, Global.defFlinch, 0.5f);
		ammo = 0;
		maxAmmo = 8;
		fireRate = 1;
		index = (int)WeaponIds.MagmaDragoonGigaD;
		weaponBarBaseIndex = 27;
		weaponBarIndex = 35;
		killFeedIndex = 16;
		weaponSlotIndex = 51;
		displayName = "";
		description = new string[] { "" };
		drawGrayOnLowAmmo = true;
		drawRoundedDown = true;
		canHealAmmo = true;
	}

	public override float getAmmoUsage(int chargeLevel) {
		return 8;
	}
}
public class MagmaDragoonMeleeW : Weapon {
	public static MagmaDragoonMeleeW netWeapon = new();

	public MagmaDragoonMeleeW() : base() {
		index = (int)WeaponIds.MagmaDragoonMeleeW;
		killFeedIndex = 4;
		weaponBarBaseIndex = 0;
		weaponBarIndex = weaponBarBaseIndex;
		weaponSlotIndex = 0;
		shootSounds = new string[] { "", "", "", "" };
		fireRate = 0.15f;
		description = new string[] { "" };
	}
}
public class MagmaDragoonProjW : Weapon {
	public static MagmaDragoonProjW netWeapon = new();

	public MagmaDragoonProjW() : base() {
		index = (int)WeaponIds.MagmaDragoonProjW;
		killFeedIndex = 4;
		weaponBarBaseIndex = 0;
		weaponBarIndex = weaponBarBaseIndex;
		weaponSlotIndex = 0;
		shootSounds = new string[] { "", "", "", "" };
		fireRate = 0.15f;
		description = new string[] { "" };
	}
}
public abstract class MagmaDragoonGenericMeleeState : CharState {
	public MagmaDragoon dragoon = null!;

	public int comboFrame = Int32.MaxValue;

	public string sound = "";
	public bool soundPlayed;
	public int soundFrame = Int32.MaxValue;
	public bool exitOnOver = true;

	public MagmaDragoonGenericMeleeState(string spr) : base(spr) {
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
		dragoon = character as MagmaDragoon ?? throw new NullReferenceException();
	}

	public virtual bool altCtrlUpdate(bool[] ctrls) {
		return false;
	}
}
public class MagmaDragoonHadoken : MagmaDragoonGenericMeleeState {
	public bool fired;
	public MagmaDragoonHadoken() : base("shoot") {
		sound = "ryuenjin";
		soundFrame = 2;
		comboFrame = 2;
	}
	public override void update() {
		if (character.frameIndex == 2 && !fired) {
			fired = true;
			new MagmaDragoonHadookenTrail(character.getCenterPos().addxy(20*character.xDir, -6),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			Global.level.delayedActions.Add( new DelayedAction(() => {
				new MagmaDragoonHadookenTrail(character.getCenterPos().addxy(44	*character.xDir, -6),
				character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);}, 3/60f));
			Global.level.delayedActions.Add( new DelayedAction(() => {
				new MagmaDragoonHadookenTrail(character.getCenterPos().addxy(70*character.xDir, -6),
				character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);}, 5/60f));
			Global.level.delayedActions.Add( new DelayedAction(() => {
				new MagmaDragoonHadookenTrail(character.getCenterPos().addxy(98*character.xDir, -6),
				character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);}, 8/60f));
		}
		if (character.frameSeconds >= 0.14) {
			character.turnToInput(player.input, player);
		}
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (dragoon.shootPressed) {
			dragoon.changeState(new MagmaDragoonHadokenDown(), true);
			return true;
		}
		return false;
	}
}
public class MagmaDragoonHadokenDown : MagmaDragoonGenericMeleeState {
	public bool fired;
	public MagmaDragoonHadokenDown() : base("shoot_down") {
		sound = "ryuenjin";
		soundFrame = 2;
	}
	public override void update() {
		if (character.frameIndex == 2 && !fired) {
			fired = true;
			new MagmaDragoonHadookenTrail(character.getCenterPos().addxy(20*character.xDir, 8),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			Global.level.delayedActions.Add( new DelayedAction(() => {
				new MagmaDragoonHadookenTrail(character.getCenterPos().addxy(44	*character.xDir, 8),
				character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);}, 3/60f));
			Global.level.delayedActions.Add( new DelayedAction(() => {
				new MagmaDragoonHadookenTrail(character.getCenterPos().addxy(70*character.xDir, 8),
				character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);}, 5/60f));
			Global.level.delayedActions.Add( new DelayedAction(() => {
				new MagmaDragoonHadookenTrail(character.getCenterPos().addxy(98*character.xDir, 8),
				character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);}, 8/60f));
		}			
		base.update();
	}
}
public class MagmaDragoonHadookenTrail : Projectile {
	float time2;
	bool once;
	public MagmaDragoonHadookenTrail(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		MagmaDragoonProjW.netWeapon, pos, xDir,
		275, 1, player, "mdragoon_hadouken_trail", 0, 0.2f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = true;
		reflectable = false;
		shouldShieldBlock = false; 
		maxTime = 0.5f;
		projId = (int)ProjIds.MagmaDragoonHadoken;
		destroyOnHitWall = true;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public override void update() {	
		base.update();
	}
	public override void onHitDamagable(IDamagable damagable) {
		if (damagable is not FrostShieldProjGround or FrostShieldProjAir or FrostShieldProjCharged
			or FrostShieldProj or FrostShieldProjPlatform or FrostShieldProjChargedGround 
			or GaeaShieldProj or ChillPIceProj) {
			base.onHitDamagable(damagable);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new MagmaDragoonHadookenTrail(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class MagmaDragoonRaijinguFaia : MagmaDragoonGenericMeleeState {
	Projectile? Raijingu;
	float timeInWall;
	public MagmaDragoonRaijinguFaia() : base("shoryuken") {
		sound = "ryuenjin";
		soundFrame = 1;
		comboFrame = 1;
	}
	public override void update() {
		if (character.currentFrame.POIs.Length > 0) {
			Point poi = character.currentFrame.POIs[0];
			Point firePos = character.pos.addxy(poi.x * character.xDir, poi.y);
			if (character.frameIndex == 1 && !character.isUnderwater() && !once) {
				once = true;
				Raijingu = new MagmaDragoonRaijinguFaiaProj(firePos, 
				character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			}	
			if (Raijingu != null) {
				Raijingu.changePos(firePos);
				Raijingu.xDir = character.xDir;
			}	
			if (character.isUnderwater() && Raijingu != null) {
				Raijingu.destroySelf();
				Raijingu = null;
			}
			if (character.isAnimOver() && Raijingu != null) {
				Raijingu.destroySelf();
				Raijingu = null;
			}
		}
		if (character.sprite.frameTime == 8) {
			float ySpeedMod = 1.1f;
			character.vel.y = -character.getJumpPower() * ySpeedMod;
		}
		if (character.sprite.frameIndex >= 1) {
			float speed = 250;
			character.move(new Point(character.xDir * speed, 0));
		}
		var wallAbove = Global.level.checkTerrainCollisionOnce(character, 0, -10);
		if (wallAbove != null && wallAbove.gameObject is Wall) {
			timeInWall += Global.spf;
			if (timeInWall > 0.1f) {
				character.changeState(new Fall());
				if (Raijingu != null) {
					Raijingu.destroySelf();
					Raijingu = null;
				}
				return;
			}
		}	
		base.update();
	}
	public override void onExit(CharState newstate) {
		dragoon.ShoryukenCooldown = 2f;
		base.onExit(newstate);
	}
}
public class MagmaDragoonRaijinguFaiaProj : Projectile {
	public MagmaDragoonRaijinguFaiaProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		MagmaDragoonProjW.netWeapon, pos, xDir,
		100, 2, player, "mdragoon_shoryuken_proj", 0, 0.25f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = false;
		reflectable = false;
		shouldShieldBlock = false; 
		maxTime = 0.7f;
		canBeLocal = false;
		projId = (int)ProjIds.MagmaDragoonShoryukenProj;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public override void update() {
		base.update();
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new MagmaDragoonRaijinguFaiaProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class MagmaDragoonKyukokaState : MagmaDragoonGenericMeleeState {
	Projectile? Kyukoka;
	public MagmaDragoonKyukokaState() : base("kick") {
		sound = "ryuenjin";
		soundFrame = 1;
		comboFrame = 1;
		exitOnLanding = true;
		useDashJumpSpeed = true;
	}
	public override void update() {
		if (character.currentFrame.POIs.Length > 0) {
			Point poi = character.currentFrame.POIs[0];
			Point firePos = character.pos.addxy(poi.x * character.xDir, poi.y+4);
			if (character.frameIndex == 1 && !character.isUnderwater() && !once) {
				once = true;
				Kyukoka = new MagmaDragoonKyukokaProj(firePos*character.xDir, 
				character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			}	
			if (Kyukoka != null) {
				Kyukoka.changePos(firePos);
				Kyukoka.xDir = character.xDir;
				if (character.isAnimOver()) {
					Kyukoka.destroySelf();
					Kyukoka = null;
				}
				else if (character.isUnderwater()) {
					Kyukoka.destroySelf();
					Kyukoka = null;
				}
				else if (character.grounded) {
					character.changeToIdleOrFall("land");
					Kyukoka.destroySelf();
					Kyukoka = null;
				}
			}	
		}
		if (character.sprite.frameIndex >= 1) {
			float speed = 350;
			character.move(new Point(character.xDir * speed, 100));
		}
		if (character.charState.stateTime >= 2f) {
			character.changeToIdleOrFall();
		}
		base.update();
	}
	public override void onExit(CharState newstate) {
		dragoon.KickCooldown = 2f;
		base.onExit(newstate);
	}
}
public class MagmaDragoonKyukokaProj : Projectile {
	public MagmaDragoonKyukokaProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		MagmaDragoonProjW.netWeapon, pos, xDir,
		0, 1, player, "mdragoon_kick_proj", 0, 0.25f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = false;
		reflectable = false;
		shouldShieldBlock  = false; 
		maxTime = 0.575f;
		canBeLocal = false;
		projId = (int)ProjIds.MagmaDragoonKickProj;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public override void update() {
		base.update();
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new MagmaDragoonKyukokaProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class MagmaDragoonHibashiraKogekiState : MagmaDragoonGenericMeleeState {
	Projectile? KogekiProj;
	Anim? Kogeki;
	bool once1;
	public MagmaDragoonHibashiraKogekiState() : base("hibashira") {
		sound = "ryuenjin";
		soundFrame = 1;
		comboFrame = 1;
	}
	public override void update() {
		if (character.frameIndex == 0 && !once) {
			once = true;
			Kogeki = new Anim(character.getShootPos(), "mdragoon_hibashira_proj",
			character.getShootXDir(), player.getNextActorNetId(), false, true);
		}	
		if (character.frameIndex >= 2 && Kogeki != null) {
			Kogeki.destroySelf();
			Kogeki = null;
		}	
		if (character.frameIndex == 1 && !once1) {
			once1 = true;
			KogekiProj = new MagmaDragoonHibashiraKogekiProj(character.getShootPos(), 
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
		}
		base.update();
	}
	public override void onExit(CharState newstate) {
		dragoon.HibashiraCooldown = 16f;
		Kogeki?.destroySelf();
		Kogeki = null;
		base.onExit(newstate);
	}
}
public class MagmaDragoonHibashiraKogekiProj : Projectile {
	public MagmaDragoonHibashiraKogekiProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		MagmaDragoonProjW.netWeapon, pos, xDir,
		200, 1, player, "mdragoon_hibashira_proj", 0, 0.1f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = false;
		reflectable = false;
		shouldShieldBlock = false; 
		maxTime = 0.85f;
		vel.y = -100;
		projId = (int)ProjIds.MagmaDragoonHibashira;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}
	

	public override void update() {
		base.update();
		vel.y += 10;
	}
	public override void onDestroy() {
		new MagmaDragoonHibashiraVolcano(pos, xDir, damager.owner,
		damager.owner.getNextActorNetId(), rpc: true); 
		
		base.onDestroy();
	}
	public override void onHitDamagable(IDamagable damagable) {
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new MagmaDragoonHibashiraKogekiProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class MagmaDragoonKaenKogekiStateStart : MagmaDragoonGenericMeleeState {
	public int loop;
	public MagmaDragoonKaenKogekiStateStart() : base("kaenkogeki") {
		superArmor = true;
	}
	public override void update() {
		if (character.frameIndex == 8 && loop < 5) {
			character.frameIndex = 7;
			character.shakeCamera(sendRpc: true);
			loop++;
		}
		if (character.isAnimOver()) {
			character.changeState(new MagmaDragoonKaenKogekiStateShoot(), true);
		}
		base.update();
	}
	public override void onEnter(CharState oldState) {
		player.OffensiveGigaAmmo -= 14;
		base.onEnter(oldState);
	}
}
public class MagmaDragoonKaenKogekiStateShoot : MagmaDragoonGenericMeleeState {
	public int loop;
	public float shootTime;
	public MagmaDragoonKaenKogekiStateShoot() : base("kaenkogeki_shoot") {
		superArmor = true;
	}
	public override void update() {
		base.update();
		if (character.frameIndex == 1 && loop < 16) {
			character.frameIndex = 0;
			once = false;
			character.shakeCamera(sendRpc: true);
			loop++;
		}
		shootTime += Global.spf;
		if (shootTime > 0.075f && character.frameIndex == 0) {
			shootTime = 0;
			character.playSound("firewave", sendRpc: true);
			new MagmaDragoonKaenkogekiProj(character.getShootPos(), 
			character.xDir,player,player.getNextActorNetId(),rpc: true);
		}
	}
}
public class MagmaDragoonKaenkogekiProj : Projectile {	
	public MagmaDragoonKaenkogekiProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		MagmaDragoonProjW.netWeapon, pos, xDir,
		400, 1, player, "mdragoon_kaenkogeki_proj", 0, 0.01f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = true;
		reflectable = false;
		shouldShieldBlock = false; 
		maxTime = 0.65f;
		vel.y = 0;
		projId = (int)ProjIds.MagmaDragoonKaenkogeki;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public override void update() {
		base.update();
		vel.y = Helpers.randomRange(-200,200);
	}
	public override void onHitDamagable(IDamagable damagable) {
		if (damagable is not FrostShieldProjGround or FrostShieldProjAir or FrostShieldProjCharged
			or FrostShieldProj or FrostShieldProjPlatform or FrostShieldProjChargedGround
			or GaeaShieldProj or ChillPIceProj) {
			base.onHitDamagable(damagable);
		}
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new MagmaDragoonKaenkogekiProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class MagmaDragoonHonodanState : MagmaDragoonGenericMeleeState {
	public int loop;
	Anim? honodan;
	public float shootTime;
	public MagmaDragoonHonodanState() : base("honodan") {
		superArmor = true;
	}
	public override void update() {
		base.update();
		if (!once && character.frameIndex == 1) {
			once = true;
			honodan = new Anim(character.getCenterPos().addxy(0,15), "",
			character.getShootXDir(), player.getNextActorNetId(), true, true);
		}
		if (character.frameIndex == 2 && loop < 24) {
			character.frameIndex = 1;
			character.shakeCamera(sendRpc: true);
			loop++;
			
		}
		shootTime += Global.spf;
		if (shootTime > 0.08f && character.frameIndex >= 1) {
			shootTime = 0;
			character.playSound("firewave", sendRpc: true);
			new MagmaDragoonHonodanProj(character.getShootPos(), 
			character.xDir,player,player.getNextActorNetId(),rpc: true);
		}
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
	}
	public override void onExit(CharState newState) {
		honodan?.destroySelf();
		player.OffensiveGigaAmmo -= 14;
		base.onExit(newState);
	}
}
public class MagmaDragoonHonodanProj : Projectile {
	public float velx;
	public float vely;
	
	public MagmaDragoonHonodanProj(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		MagmaDragoonProjW.netWeapon, pos, xDir,
		0, 1, player, "", 4, 8f/60f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = false;
		reflectable = false;
		shouldShieldBlock = false; 
		maxTime = 1.2f;
		vel.y = -200;
		projId = (int)ProjIds.MagmaDragoonHonodan;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public override void update() {
		base.update();
		vel.x = Helpers.randomRange(-200,200);
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new MagmaDragoonHonodanProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class MagmaDragoonDash : MagmaDragoonGenericMeleeState {
	public int loop;
	public MagmaDragoonDash() : base("dash") {
		enterSound = "jump";
		exitOnLanding = true;
		useDashJumpSpeed = true;
		airMove = true;
		attackCtrl = true;
		normalCtrl = true;
	}
	public override void update() {
		base.update();
		if (character.grounded && stateTime > 0.05f) {
			exitOnLanding = true;
			return;
		}
		if (Global.level.checkTerrainCollisionOnce(character, 0, -1) != null && character.vel.y < 0) {
			character.vel.y = 0;
		}
		character.move(new Point(
			dragoon.isBoosted ? character.xDir * 250 : character.xDir * 200
		, 0));
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		character.vel.y = -character.getJumpPower() * 0.75f;
	}
	public override void onExit(CharState newState) {
		dragoon.DashCooldown = 1f;
		base.onExit(newState);
	}
}
public class MagmaDragoonDashAir : MagmaDragoonGenericMeleeState {
	public int loop;
	public MagmaDragoonDashAir() : base("dash") {
		enterSound = "jump";
		exitOnLanding = true;
		useDashJumpSpeed = true;
		airMove = true;
		attackCtrl = true;
		normalCtrl = true;
	}
	public override void update() {
		base.update();
		if (character.grounded && stateTime > 0.05f) {
			exitOnLanding = true;
			return;
		}
		if (Global.level.checkTerrainCollisionOnce(character, 0, -1) != null && character.vel.y < 0) {
			character.vel.y = 0;
		}
		character.move(new Point(
			dragoon.isBoosted ? character.xDir * 200 : character.xDir * 150
			, 0));
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		character.dashedInAir++;
		dragoon.DashCooldown = 1f;
		character.vel.y = -character.getJumpPower() * 0.45f;
	}
	public override void onExit(CharState newState) {
		base.onExit(newState);
	}
}
public class MagmaDragoonShungokusatsu : CharState {
	public int loop;
	public float speed;
	public MagmaDragoonShungokusatsu() : base("shungoku") {

	}
	public override void update() {
		base.update();
		speed += Global.speedMul*40;
		if (character.frameIndex == 0) {
			character.move(new Point(character.xDir * speed, 0));
		}
		if (character.isAnimOver()) {
			character.changeToIdleOrFall();
		}
	}
	public override void onEnter(CharState oldState) {
		character.useGravity = false;
		base.onEnter(oldState);
	}
	public override void onExit(CharState newState) {
		base.onExit(newState);
		character.useGravity = true;
	}
}
public class MagmaDragoonGrabWeapon : Weapon {
	public MagmaDragoonGrabWeapon() : base() {
		fireRate = 0.25f;
		index = (int)WeaponIds.MagmaDragoonGrab;
		killFeedIndex = 63;
	}
}
public class MagmaDragoonGrabState : CharState {
	public Character? victim;
	Anim? sprites;
	float leechTime = 0;
	public bool victimWasGrabbedSpriteOnce;
	public MagmaDragoonGrabState(Character? victim) : base("grab", "", "", "") {
		this.victim = victim;
		grabTime = MagmaDragoonGrabbed.maxGrabTime;
		invincible = true;
	}

	public override void update() {
		base.update();
		grabTime -= Global.spf;
		leechTime += Global.spf;
		if (victimWasGrabbedSpriteOnce && !victim.sprite.name.EndsWith("_grabbed")) {
			character.changeToIdleOrFall();
			return;
		}

		if (victim.sprite.name.EndsWith("_grabbed") || victim.sprite.name.EndsWith("_die")) {
			victimWasGrabbedSpriteOnce = true;
		}
		int random = Helpers.randomRange(-16, 32);
		int random2 = Helpers.randomRange(-24, 12);
		victim.charState.superArmor = true;
		if (character.frameIndex >= 4) {
			if (leechTime > 0.08f) {
				leechTime = 0;
				switch (Helpers.randomRange(1,3)) {
				case 1:
					sprites = new Anim(victim.getCenterPos().addxy(random,random2) , "sword_sparks_horizontal",
					victim.getShootXDir(), player.getNextActorNetId(), true, true);
					break;
				case 2:
					sprites = new Anim(victim.getCenterPos().addxy(random,random2) , "sword_sparks_vertical",
					victim.getShootXDir(), player.getNextActorNetId(), true, true);
					break;
				case 3:
					sprites = new Anim(victim.getCenterPos().addxy(random,random2) , "sword_sparks_angled",
					victim.getShootXDir(), player.getNextActorNetId(), true, true);
					break;
				}
				character.playSound("hurt", true, true);
			}
		}
		if (grabTime <= 2) {
			character.changeState(new MagmaDragoonShungokusatsuend(), true);
			return;
		}
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		if (character.ownedByLocalPlayer) {
			new ShungokuEffect(character.pos, character);
		}
	}

	public override void onExit(CharState newState) {
		base.onExit(newState);
		sprites?.destroySelf();
		sprites = null;
		var damager = new Damager(player, 6, 0, 0);
		damager.applyDamage(victim , false, new MagmaDragoonGrabWeapon(),
		character, (int)ProjIds.MagmaDragoonRagingDemon);
		victim?.changeSpriteFromName("hurt", false);
		player.OffensiveGigaAmmo -= 28;
		if (newState is not MagmaDragoonGrabState && victim != null) {
			victim.grabInvulnTime = 2;
			victim.stunInvulnTime = 1;
			victim?.releaseGrab(character, true);
			victim.charState.superArmor = true;
		}
	}
}
public class MagmaDragoonGrabbed : GenericGrabbedState {
	public const float maxGrabTime = 4;
	public MagmaDragoonGrabbed(Character? grabber) : base(grabber, maxGrabTime, "mdragoon_shungoku") {
	}
	public override void update() {
		base.update();
	}
	public override void onExit(CharState newState) {
		base.onExit(newState);
	}
}
public class MagmaDragoonShungokusatsuend : CharState {
	public MagmaDragoonShungokusatsuend() : base("shungokuend") {
		invincible = true;
	}
	public override void update() {
		base.update();
		if (character.isAnimOver()) {
			character.changeToIdleOrFall();
		}
	}
	public override void onEnter(CharState oldState) {
		character.playSound("ching", true, true);
		base.onEnter(oldState);
	}
	public override void onExit(CharState newState) {
		base.onExit(newState);
	}
}
public class ShungokuEffect : Effect {
	public Character rootChar;

	public ShungokuEffect(Point pos, Character character) : base(pos) {
		rootChar = character;
	}

	public override void update() {
		base.update();
		if (effectTime > 120f/60f) {
			destroySelf();
		}
	}

	public override void render(float offsetX, float offsetY) {
		float transparecy = 100;
		if (effectTime < 0.2) {
			transparecy = effectTime * 500f;
		}
		if (effectTime > 3) {
			transparecy = 100f - ((effectTime - 3f) * 500f);
		}

		DrawWrappers.DrawRect(
			Global.level.camX, Global.level.camY,
			Global.level.camX + 1000, Global.level.camY + 1000,
			true, new Color(0, 0, 0, (byte)System.MathF.Round(transparecy)), 1, ZIndex.Backwall
		);
	}
}
public class MagmaDragoonLandProjectile : Projectile {
	public MagmaDragoonLandProjectile(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		MagmaDragoonProjW.netWeapon, pos, xDir,
		0, 0, player, "mdragoon_land_proj", 0, 3f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = false;
		zIndex = ZIndex.Character;
		maxTime = 1;
		projId = (int)ProjIds.MagmaDragoonLandProjectile;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public override void update() {
		base.update();
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new MagmaDragoonLandProjectile(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class MagmaDragoonHibashiraVolcano : Projectile {
	bool once;
	int loop;
	float time1;
	float time2;
	float time3;
	float time4;
	public MagmaDragoonHibashiraVolcano(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		MagmaDragoonProjW.netWeapon, pos, xDir,
		0, 1, player, "mdragoon_volcano", 26, 2f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = false;
		reflectable = false;
		shouldShieldBlock  = false; 
		maxTime = 8;
		vel.y = 0;
		projId = (int)ProjIds.MagmaDragoonHibashiraVolcano;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public override void update() {
		base.update();
		time1 += Global.spf;
		if (time1 >= 0.571f && loop < 8) {
			time1 = 0;
			loop++;
			playSound("volcanostart",true, true);
		}
		if (loop >= 8) {
			time2 += Global.spf;
			vel.y = -150;
			if (vel.y == -150) {
				time3 += Global.spf;
				time4 += Global.spf;
				if (time3 >= 1.5) {
					vel.y = 0;
				}
				if (time4 >= 0.14 && vel.y != 0) {
					time4 = 0;
					new MagmaDragoonHibashiraVolcano2(pos, xDir, damager.owner,
					damager.owner.getNextActorNetId(), rpc: true); 
				}
			}
			if (time2 >= 0.3) {
				time2 = 0;
				shakeCamera(true);
			}
			if (!once) {
				once = true;
				playSound("volcanoshot",true, true);
			}
		}
	}
	public override void onDestroy() {
		base.onDestroy();
	}
	public override void onHitDamagable(IDamagable damagable) {
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new MagmaDragoonHibashiraVolcano(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
public class MagmaDragoonHibashiraVolcano2 : Projectile {
	public MagmaDragoonHibashiraVolcano2(
		Point pos, int xDir, Player player, ushort? netId, bool rpc = false
	) : base(
		MagmaDragoonProjW.netWeapon, pos, xDir,
		0, 1, player, "mdragoon_volcanosprite", 26, 2f,
		netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "";
		fadeOnAutoDestroy = true;
		destroyOnHit = false;
		reflectable = false;
		shouldShieldBlock = false; 
		maxTime = 2f;
		zIndex = ZIndex.Character;
		projId = (int)ProjIds.MagmaDragoonHibashiraVolcano2;
		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public override void update() {
		base.update();
		
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new MagmaDragoonHibashiraVolcano2(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
