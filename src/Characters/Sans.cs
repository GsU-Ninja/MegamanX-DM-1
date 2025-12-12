namespace MMXOnline;
using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
public class Sans : Character {
	public bool downPressed => player.input.isPressed(Control.Down, player);
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
	public float BoneAttackCooldown;
	public float GasterBlasterCooldown;
	public float DodgeCooldown;
	public List<BoneAttackProjectile> BoneAttackProjectileOnField = new();
	
	public Sans(
		Player player, float x, float y, int xDir,
		bool isVisible, ushort? netId, bool ownedByLocalPlayer,
		bool isWarpIn = true, int? heartTanks = null, bool isATrans = false
	) : base(
		player, x, y, xDir, isVisible, netId, ownedByLocalPlayer, isWarpIn, heartTanks, isATrans
	) {
		charId = CharIds.Sans;
	}
	public override void update() {
		base.update();
		chargeLogic(Shoot);
		Helpers.decrementFrames(ref BoneAttackCooldown);
		Helpers.decrementFrames(ref GasterBlasterCooldown);
		Helpers.decrementFrames(ref DodgeCooldown);

	}
	public override bool normalCtrl() {
		if (player.input.isPressed(Control.Dash, player) 
			&& DodgeCooldown <= 0) {
			changeState(new SansDodge(), true);
			DodgeCooldown = 50;
		}
		return base.normalCtrl();
	}
	public override bool canDash() {
		return false;
	}

	public override bool canWallClimb() {
		return false;
	}
	public override bool attackCtrl() {
		bool shoot = shootPressed || specialPressed || upPressed;	
		if (shoot && canCharge()) {
			Shoot(getChargeLevel());
			stopCharge();
			return true;			
		}
		return base.attackCtrl();
	}
	public void Shoot(int chargeLevel) {
		for (int i = BoneAttackProjectileOnField.Count - 1; i >= 0; i--) {
			if (BoneAttackProjectileOnField[i].destroyed) {
				BoneAttackProjectileOnField.RemoveAt(i);
			}
		}
		if (BoneAttackProjectileOnField.Count >= 4) { return; }	
		Point shootPosNext = new Point(pos.x + 30 * getShootXDir(), pos.y);
		int xDir = getShootXDir();
		
		if (specialPressed) {
			switch (chargeLevel) {
				case 1:
					new GasterBlaster(getShootPos(), player, xDir, 1, 
					player.getNextActorNetId(), ownedByLocalPlayer, rpc: true);
					break;
				case 2:
					
					break;
			}
		} 
		if (upPressed && grounded) {
			switch (chargeLevel) {
				case 1:
					var gbone = new GroundBoneAttackProjectile(
					shootPosNext, xDir, player, player.getNextActorNetId(), rpc: true);
					break;
				case 2:
					var gbbone = new BlueGroundBoneAttackProjectile(
					shootPosNext, xDir, player, player.getNextActorNetId(), rpc: true);
					break;
			}
		}
		if (shootPressed) {
			if (chargeLevel == 0) {
				shootbone(1, 13);
			} else if (chargeLevel == 1) {
				shootbone(2, 13);
				DelayActionBone(1, 13, 26f / 60, false);
			} else if (chargeLevel == 2) {
				shootbone(3, 30);
				DelayActionBone(2, 30, 26f / 60f, false);
				DelayActionBone(2, 30, 42f / 60f, false);
			} else if (chargeLevel == 3) {
				shootbluebone(1, 30);
				DelayActionBone(2, 30, 26f / 60f, false);
				DelayActionBone(1, 30, 42f / 60f, true);
				DelayActionBone(2, 30, 58f / 60f, false);
			}
		}
	}
	public void DelayActionBone(int type, int CD, float delaytime, bool isblue) {
		if (isblue) 
		Global.level.delayedActions.Add(new DelayedAction(delegate { shootbluebone(type, CD);}, delaytime));
		else 
		Global.level.delayedActions.Add(new DelayedAction(delegate { shootbone(type, CD);}, delaytime));
	}
	public void shootbone(int type, int CD) {
		Point shootPos = getCenterPos();
		BoneAttackCooldown = CD;
		var bone = new BoneAttackProjectile(
		shootPos, xDir, player, type, player.getNextActorNetId(), rpc: true); 
		BoneAttackProjectileOnField.Add(bone);
	}
	public void shootbluebone(int type, int CD) {
		Point shootPos = getCenterPos();
		BoneAttackCooldown = CD;
		var bluebone = new BlueBoneAttackProjectile(
		shootPos, xDir, player, type, player.getNextActorNetId(), rpc: true); 
	}
	public override bool chargeButtonHeld() {
		return shootHeld || specialHeld;
	}
	public override void increaseCharge() {
		float factor = 1;
		chargeTime += Global.speedMul * factor;
	}
	public override bool canCharge() {
		return BoneAttackCooldown <= 0;
	}
	public override string getSprite(string spriteName) {
		return "sans_" + spriteName;
	}
}
public enum SansAttackLoadoutType {
	BoneAttackWeapon,
	GasterBlasterWeapon

}
public class BoneAttackWeapon : Weapon {
public static BoneAttackWeapon netWeapon = new();
	public BoneAttackWeapon() : base() {
		index = (int)WeaponIds.BoneAttackWeapon;
		killFeedIndex = 0;
		weaponBarBaseIndex = 0;
		weaponBarIndex = weaponBarBaseIndex;
		weaponSlotIndex = 0;
		shootSounds = new string[] { "", "", "", "" };
		fireRate = 0.1f;
		displayName = "Bone Attack";
		description = new string[] { "Shoot bones with ATTACK." };
		type = (int)SansAttackLoadoutType.BoneAttackWeapon;
	}
}
public class GasterBlasterWeapon : Weapon {
public static GasterBlasterWeapon netWeapon = new();
	public GasterBlasterWeapon() : base() {
		index = (int)WeaponIds.GasterBlasterWeapon;
		killFeedIndex = 0;
		weaponBarBaseIndex = 0;
		weaponBarIndex = weaponBarBaseIndex;
		weaponSlotIndex = 0;
		shootSounds = new string[] { "", "", "", "" };
		fireRate = 0.1f;
		displayName = "Gaster Blaster";
		description = new string[] { "Blast your enemies" };
		type = (int)SansAttackLoadoutType.GasterBlasterWeapon;
	}
}
public class BoneAttackProjectile : Projectile {
	public Sprite? spriteStart1, spriteStart2, spriteStart3, spriteStart4;
	public Sprite? spriteMid1, spriteMid2, spriteMid3, spriteMid4;
	public Sprite? spriteEnd;
	public int type;

	public BoneAttackProjectile(Point pos, int xDir, Player player, int type, ushort? netId, bool rpc = false)
	 : base(BoneAttackWeapon.netWeapon,pos, xDir, -100, 1, player, "sans_bonepiece2", 0, 0, netId, player.ownedByLocalPlayer) {
		damager.hitCooldown = 3;
		maxTime = 120f/60f;
		projId = (int)ProjIds.BoneAttackWeaponTestID;
		destroyOnHit = false;
		shouldShieldBlock = false;
		sprite.visible = false;
		this.type = type;
		spriteEnd = new Sprite("sans_bonepiece1"); 
		if (type == 1) {
		spriteStart1 = new Sprite("sans_bonepiece");
		spriteMid1 = new Sprite("sans_bonepiece2");
		}
		if (type == 2) {
		spriteStart2 = new Sprite("sans_bonepiece");
		spriteMid2 = new Sprite("sans_bonepiece2");
		}
		if (type == 3) {
		spriteStart3 = new Sprite("sans_bonepiece");
		spriteMid3 = new Sprite("sans_bonepiece2");
		}
		if (rpc) rpcCreate(pos, player, netId, xDir);	
	}
	public override void render(float x, float y) {
		base.render(x, y);
		switch(type) {
			case 1:
				spriteStart1?.draw(frameIndex, pos.x + x, pos.y + 5, xDir, yDir, getRenderEffectSet(), 1, 1, 1, ZIndex.Character);
				spriteMid1?.draw(frameIndex, pos.x + x + x, pos.y + 10, xDir, yDir, getRenderEffectSet(), 1, 1, 1, ZIndex.Character);
				spriteEnd?.draw(frameIndex, pos.x + x + y, pos.y + 15, xDir, yDir, getRenderEffectSet(), 1, 1, 1, ZIndex.Character);		
				break;
			case 2:
				spriteStart2?.draw(frameIndex, pos.x + x, pos.y + -5, xDir, yDir, getRenderEffectSet(), 1, 1, 1, ZIndex.Character);
				spriteEnd?.draw(frameIndex, pos.x + x + y, pos.y + 15, xDir, yDir, getRenderEffectSet(), 1, 1, 1, ZIndex.Character);	
				for(int i = 0; i <= 10;	i += 5) 
					spriteMid2?.draw(frameIndex, pos.x + x + x, pos.y + i, xDir, yDir, getRenderEffectSet(), 1, 1, 1, ZIndex.Character);					
				break;
			case 3:
				spriteStart3?.draw(frameIndex, pos.x + x, pos.y + -20, xDir, yDir, getRenderEffectSet(), 1, 1, 1, ZIndex.Character);
				spriteEnd?.draw(frameIndex, pos.x + x + y, pos.y + 15, xDir, yDir, getRenderEffectSet(), 1, 1, 1, ZIndex.Character);	
				for(int i = -15; i <= 10; i += 5) 
					spriteMid3?.draw(frameIndex, pos.x + x + x, pos.y + i, xDir, yDir, getRenderEffectSet(), 1, 1, 1, ZIndex.Character);	
				break;
		}
	}
	public override void update() {
		base.update();
		if (type == 1) {
			var rect1 = new Rect(11, 15, 0, 10);
			globalCollider = new Collider(rect1.getPoints(), true, this, false, false, 0, new Point(0, 0));
		} else if (type == 2) {
			var rect2 = new Rect(11, 20, 0, 6);
			globalCollider = new Collider(rect2.getPoints(), true, this, false, false, 0, new Point(0, 0));
		} else if (type == 3) {
			var rect3 = new Rect(11, 28, 0, -2);
			globalCollider = new Collider(rect3.getPoints(), true, this, false, false, 0, new Point(0, 0));
		} 
		if (type == 1 || type == 2 || type == 3) {
			if (vel.x <= 150) {
				vel.x += 5;
			}
			if (xDir == -1 && vel.x >= -150) {
				vel.x -= 10;	
			}
		}
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new BoneAttackProjectile(
			args.pos, args.xDir, args.player, args.extraData[1], args.netId);
	}
}
public class BlueBoneAttackProjectile : Projectile {
	public Sprite? spriteStart, spriteMid, spriteEnd;
	public int type;

	public BlueBoneAttackProjectile(Point pos, int xDir, Player player, int type, ushort? netId, bool rpc = false)
	: base(BoneAttackWeapon.netWeapon,pos, xDir, -100, 0, player, "sans_bonepieceb2", 0, 0, netId, player.ownedByLocalPlayer) {
		damager.hitCooldown = 3f/60f;
		maxTime = 120f/60f;
		projId = (int)ProjIds.BlueBoneAttackWeaponTestID;
		destroyOnHit = false;
		shouldShieldBlock = false;
		sprite.visible = false;
		this.type = type;
		spriteEnd = new Sprite("sans_bonepieceb1");
		if (type == 1) {
		spriteStart =new Sprite("sans_bonepieceb");
		spriteMid = new Sprite("sans_bonepieceb2");
		}
		if (rpc) rpcCreate(pos, player, netId, xDir);
	}
	public override void render(float x, float y) {
		base.render(x, y);
		switch(type) {
			case 1:
				spriteStart?.draw(frameIndex, pos.x + x, pos.y + -20, xDir, yDir, getRenderEffectSet(), 1, 1, 1, ZIndex.Character);
				for(int i = -15; i <= 10; i += 5) 
					spriteMid?.draw(frameIndex, pos.x + x + x, pos.y + i, xDir, yDir, getRenderEffectSet(), 1, 1, 1, ZIndex.Character);	
				break;
		} 
		spriteEnd?.draw(frameIndex, pos.x + x + y, pos.y + 15, xDir, yDir, getRenderEffectSet(), 1, 1, 1, ZIndex.Character);	
	}
	public override void update() {
		base.update();
		var rect3 = new Rect(11, 28, 0, -2);
		globalCollider = new Collider(rect3.getPoints(), true, this, false, false, 0, new Point(0, 0));
		if (vel.x <= 150) {
			vel.x += 5;
		}
		if (xDir == -1 && vel.x >= -150) {
			vel.x -= 10;	
		}
	}
	public override void onHitDamagable(IDamagable damagable) {
		var chr = damagable as Character;
		if (chr == null) return;
		if (chr.charState.invincible) return;
		if (chr.player.input.isHeld(Control.Dash, chr.player) ||
			chr.player.input.isPressed(Control.Dash, chr.player) ||
			chr.player.input.isPressed(Control.Shoot, chr.player) ||
			chr.player.input.isHeld(Control.Shoot, chr.player) ||
			chr.player.input.isPressed(Control.Special1, chr.player) ||
			chr.player.input.isHeld(Control.Special1, chr.player) ||
			chr.player.input.isHeld(Control.Up, chr.player) ||
			chr.player.input.isHeld(Control.Down, chr.player) ||
			chr.player.input.isHeld(Control.Left, chr.player) ||
			chr.player.input.isHeld(Control.Right, chr.player) ||
			chr.player.input.isHeld(Control.Jump, chr.player) ||
			chr.player.input.isPressed(Control.Jump, chr.player)) {
				damager.damage = 1;
		}	
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new BlueBoneAttackProjectile(
			args.pos, args.xDir, args.player, args.extraData[1], args.netId);
	}
}
public class GroundBoneAttackProjectile : Projectile {
	public GroundBoneAttackProjectile(Point pos, int xDir, Player player, ushort? netId, bool rpc = false)
	: base(BoneAttackWeapon.netWeapon,pos, xDir,  0, 1, player, "sans_boneup", 0, 5f/60f, netId, player.ownedByLocalPlayer) {
		projId = (int)ProjIds.BoneAttackWeaponTestID;
		destroyOnHit = false;
		shouldShieldBlock = false;
		if (rpc) rpcCreate(pos, player, netId, xDir);
	}
	public override void update() {
		base.update();
		if (isAnimOver()) destroySelf();	
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new GroundBoneAttackProjectile(
			args.pos, args.xDir, args.player, args.netId);
	}
}
public class BlueGroundBoneAttackProjectile : Projectile {
	public BlueGroundBoneAttackProjectile(Point pos, int xDir, Player player, ushort? netId, bool rpc = false)
	: base(BoneAttackWeapon.netWeapon,pos, xDir,  0, 0, player, "sans_boneupb", 0, 3f/60f, netId, player.ownedByLocalPlayer) {
		projId = (int)ProjIds.BlueBoneAttackWeaponTestID;
		destroyOnHit = false;
		shouldShieldBlock = false;
		if (rpc) rpcCreate(pos, player, netId, xDir);
	}
	public override void update() {
		base.update();
		if (isAnimOver()) destroySelf();	
	}
	public override void onHitDamagable(IDamagable damagable) {
		var chr = damagable as Character;
		if (chr == null) return;
		if (chr.charState.invincible) return;
		if (chr.player.input.isHeld(Control.Dash, chr.player) ||
			chr.player.input.isPressed(Control.Dash, chr.player) ||
			chr.player.input.isPressed(Control.Shoot, chr.player) ||
			chr.player.input.isHeld(Control.Shoot, chr.player) ||
			chr.player.input.isPressed(Control.Special1, chr.player) ||
			chr.player.input.isHeld(Control.Special1, chr.player) ||
			chr.player.input.isHeld(Control.Up, chr.player) ||
			chr.player.input.isHeld(Control.Down, chr.player) ||
			chr.player.input.isHeld(Control.Left, chr.player) ||
			chr.player.input.isHeld(Control.Right, chr.player) ||
			chr.player.input.isHeld(Control.Jump, chr.player) ||
			chr.player.input.isPressed(Control.Jump, chr.player)) {
				damager.damage = 1;
		}	
	}
	public static Projectile rpcInvoke(ProjParameters args) {
		return new BlueGroundBoneAttackProjectile(
			args.pos, args.xDir, args.player, args.netId);
	}
}
public class GasterBlaster : Actor {
	public Player player;
	int type;
	public float time1, time2, time3;
	public GasterBlaster(Point pos, Player player, int xDir, int type, ushort netId, bool ownedByLocalPlayer, bool rpc = false) :
	base("sans_gasterblaster", pos, netId, ownedByLocalPlayer, false) {
		zIndex = ZIndex.Character;
		useGravity = false;
		this.player = player;
		this.type = type;
		if (type == 1) { xScale = 0.5f; yScale = 0.75f; }
		if (type == 2) { xScale = 1f; yScale = 1f; }
		if (type == 3) { xScale = 1.5f; yScale = 1.5f; }
		if (xDir == 1) { angle = -270; } else { angle = Helpers.randomRange(0, 359); }
	}
	public override void update() {
		base.update();
		if (!ownedByLocalPlayer) return;
		time1 += Global.spf;
		time2 += Global.spf;
		if (angle <= -80 && xDir == 1) {
			angle += 20;
			move(new Point(-50 * xDir, -100));
		}
		if (angle >= 45 && xDir == -1) {
			angle -= 20;
		}
		if (time1 > 60f / 60f && time2 > 24f/60f) {
			time2 = 0;
			new GasterBlasterProjectile(
			this.pos, xDir, player, 1, player.getNextActorNetId(), rpc: true); 
		}
		if (time1 > 120f / 60f) {
			destroySelf();
		}
	}
}
public class GasterBlasterProjectile : Projectile {
	int type;
	public GasterBlasterProjectile(Point pos, int xDir, Player player, int type, ushort? netId, bool rpc = false)
	: base(GasterBlasterWeapon.netWeapon, pos, xDir, 0, 1, player, "sans_gasterblasterprojectile", 0, 0, netId, player.ownedByLocalPlayer) {
		maxTime = 240f / 60f;
		projId = (int)ProjIds.GasterBlasterWeaponTestID;
		destroyOnHit = false;
		shouldShieldBlock = false;
		this.type = type;
		if (type == 1) { xScale = 0.5f; yScale = 0.75f; }
		if (type == 2) { xScale = 1f; yScale = 1f; }
		if (type == 3) { xScale = 1.5f; yScale = 1.5f; }	
		if (player?.character != null) {
			owningActor = player.character;
		}
	}
	public override void update() {
		base.update();
		
	}
	public override void onHitDamagable(IDamagable damagable) {
		var chr = damagable as Character;
		if (chr == null) return;
		if (chr.charState.invincible) return;
		if (type == 1) damager.hitCooldown = 7f/60f;	
		if (type == 2) damager.hitCooldown = 5f/60f;
		if (type == 3) damager.hitCooldown = 3f/60f;
		
	}
	public override void render(float x, float y) {
		base.render(x, y);
	}
}
public class SansDodge : CharState {
	//quenosenoteqestarerobao
	public bool onceTeleportInSound;
	bool isInvisible;
	Actor? clone;
	Actor? cloneG;
	Rect teleportCollider = new Rect(0f, 0f, 18, 30);
	int width = 18;

	public SansDodge() : base("sans_idle") {
		invincible = true;
	}

	public override void update() {
		base.update();

		if (!isInvisible && stateFrames >= 16 && stateFrames < 22) {
			isInvisible = true;
			specialId = SpecialStateIds.XTeleport;
			character.useGravity = false;
		}
		if (isInvisible && stateFrames >= 22) {
			isInvisible = false;
			specialId = SpecialStateIds.None;
			character.useGravity = true;
			if (cloneG != null && canChangePos(cloneG)) {
				Point prevCamPos = player.character.getCamCenterPos();
				player.character.stopCamUpdate = true;
				character.changePos(cloneG.pos);
			}
			clone?.destroySelf();
			clone = null;
		}
		if (clone != null && !clone.destroyed && cloneG != null) {
			int xDir = player.input.getXDir(player);
			float moveAmount = xDir * 6 * Global.speedMul;

			CollideData hitWall = Global.level.checkTerrainCollisionOnce(clone, moveAmount, -2);
			if (hitWall != null && hitWall.getNormalSafe().y == 0) {
				float rectW = hitWall.otherCollider.shape.getRect().w();
				if (rectW < 75) {
					float wallClipAmount = moveAmount + xDir * (rectW + width);
					CollideData hitWall2 = Global.level.checkTerrainCollisionOnce(clone, wallClipAmount, -2);
					if (hitWall2 == null && clone.pos.x + wallClipAmount > 0 &&
						clone.pos.x + wallClipAmount < Global.level.width
					) {
						clone.incPos(new Point(wallClipAmount, 0));
						clone.visible = true;
					}
				} else if (xDir != 0) {
					CollideData hitWall2 = Global.level.checkTerrainCollisionOnce(clone, moveAmount, -16);
					float wallY = MathInt.Floor(hitWall.otherCollider.shape.minY);
					if (hitWall2 == null) {
						clone.changePos(new Point(clone.pos.x + moveAmount, clone.pos.y - 64));
						clone.visible = true;
					} else if (clone.pos.y - wallY <= 36 && clone.pos.y - wallY > 0) {
						clone.changePos(new Point(clone.pos.x + moveAmount, wallY - 1));
						clone.visible = true;
					}
				}
			} else {
				if (MathF.Abs(moveAmount) > 0) {
					clone.visible = true;
				}
				clone.move(new Point(moveAmount, 0), useDeltaTime: false);
			}
			if (!canChangePos(clone)) {
				int widthH = MathInt.Ceiling(width / 2.0);
				List<CollideData> hits = Global.level.raycastAllSorted(
					clone.getCenterPos().addxy(-widthH, 0),
					clone.getCenterPos().addxy(-widthH, 200),
					new List<Type> { typeof(Wall) }
				);
				List<CollideData> hits2 = Global.level.raycastAllSorted(
					clone.getCenterPos().addxy(widthH, 0),
					clone.getCenterPos().addxy(widthH, 200),
					new List<Type> { typeof(Wall) }
				);
				CollideData? hit = hits.FirstOrDefault();
				CollideData? hit2 = hits2.FirstOrDefault();
				if (hit != null && (
					hit2 == null ||
					hit2 != null && hit.otherCollider.shape.minY < hit2.otherCollider.shape.minY
				)) {
					cloneG.visible = true;
					clone.visible = false;
					cloneG.changePos(new Point(clone.pos.x, hit.getHitPointSafe().y));
				} else if (hit2 != null) {
					cloneG.visible = true;
					clone.visible = false;
					cloneG.changePos(new Point(clone.pos.x, hit2.getHitPointSafe().y));
				}
			} else {
				cloneG.visible = false;
				cloneG.changePos(clone.pos);
			}
			if (!canChangePos(clone) && !canChangePos(cloneG)) {
				Point redXPos;
				if (cloneG.visible) {
					redXPos = cloneG.getCenterPos();
				} else {
					redXPos = clone.getCenterPos();
				}
				DrawWrappers.DrawLine(
					redXPos.x - 10, redXPos.y - 10, redXPos.x + 10, redXPos.y + 10, Color.Red, 2, ZIndex.HUD
				);
				DrawWrappers.DrawLine(
					redXPos.x - 10, redXPos.y + 10, redXPos.x + 10, redXPos.y - 10, Color.Red, 2, ZIndex.HUD
				);
			}
		}

		if (stateFrames < 16) {
			character.visible = Global.isOnFrameCycle(5);
		} else if (stateFrames >= 16 && stateFrames < 22) {
			character.visible = false;
		} else if (stateFrames >= 22) {
			if (!onceTeleportInSound) {
				onceTeleportInSound = true;
				character.playSound("boomerkTeleport", sendRpc: true);
			}
			character.visible = Global.isOnFrameCycle(5);
		}
		if (stateFrames >= 34) {
			character.changeState(new Idle());
		}
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		character.playSound("boomerkTeleport", sendRpc: true);
		clone = createClone();
		clone.useGravity = true;
		cloneG = createClone();
		character.sprite.frameIndex = 0;
	}
	public override void onExit(CharState newState) {
		base.onExit(newState);
		character.visible = true;
		character.useGravity = true;
		if (clone != null) {
			clone.destroySelf();
		}
		if (cloneG != null) {
			cloneG.destroySelf();
		}
		specialId = SpecialStateIds.None;
	}
	public bool canChangePos(Actor actor) {
		if (Global.level.checkTerrainCollisionOnce(actor, 0, 2) == null) {
			return false;
		}
		List<CollideData> hits = Global.level.getTriggerList(actor, 0, 2, null, new Type[] { typeof(KillZone) });
		if (hits.Count > 0) {
			return false;
		}
		return true;
	}
	public Actor createClone() {
		Actor tempClone = new Actor(
			"empty",
			new Point(MathInt.Round(character.pos.x), MathInt.Floor(character.pos.y)),
			null, true, false
		);
		Collider col = new Collider(
			teleportCollider.getPoints(), false, tempClone, false, false, 0, new Point(0, 0)
		);
		tempClone.changeSprite("sans_dodge", false);
		tempClone.globalCollider = col;
		tempClone.alpha = 0.5f;
		tempClone.xDir = character.xDir;
		tempClone.visible = false;
		tempClone.useGravity = false;
		tempClone.sprite.frameIndex = 0;

		return tempClone;
	}
}

