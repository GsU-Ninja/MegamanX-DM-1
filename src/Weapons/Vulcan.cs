﻿namespace MMXOnline;

public enum VulcanType {
	None = -1,
	CherryBlast,
	DistanceNeedler,
	BuckshotDance
}
public class VileVulcan : Weapon {
	public float vileAmmoUsage;
	public VileVulcan() : base() {
		index = (int)WeaponIds.Vulcan;
		weaponBarBaseIndex = 26;
		weaponBarIndex = weaponBarBaseIndex;
		killFeedIndex = 62;
		weaponSlotIndex = 44;
	}

	public override void vileShoot(WeaponIds weaponInput, Vile vile) {
		if (shootCooldown > 0 || vile.energy.ammo < vileAmmoUsage) {
			return;
		}
		vile.vulcanLingerTime = fireRate > 9 ? fireRate : 9;
		vile.changeSpriteFromName(vile.charState.shootSpriteEx, false);
		ladderVoid(vile);
		vile.setVileShootTime(this);
		vile.tryUseVileAmmo(vileAmmoUsage, true);
		shoot(vile, []);
	}

	public void ladderVoid(Vile vava) {
        if (vava.charState is LadderClimb) {
			if (vava.player.input.isHeld(Control.Left, vava.player)) {
				vava.xDir = -1;
			}
			if (vava.player.input.isHeld(Control.Right, vava.player)) {
				vava.xDir = 1;
			}
		}
    }
}
public class CherryBlast : VileVulcan {
	public static CherryBlast netWeapon = new();
	public CherryBlast() : base() {
		type = (int)VulcanType.CherryBlast;
		fireRate = 6;
		displayName = "Cherry Blast";
		vileAmmoUsage = 1f;
		vileWeight = 2;
		ammousage = vileAmmoUsage;
		damage = "1";
		effect = "None.";
	}
	
	public override void shoot(Character character, int[] args) {
		character.playSound("vulcan", sendRpc: true);
		new VulcanCherryBlast(
			character.getShootPos(), character.getShootXDir(), character,
			character.player, character.player.getNextActorNetId(), rpc: true
		);
	}
}
public class DistanceNeedler : VileVulcan {
	public static DistanceNeedler netWeapon = new();
	public DistanceNeedler() : base() {
		type = (int)VulcanType.DistanceNeedler;
		fireRate = 15;
		displayName = "Distance Needler";
		vileAmmoUsage = 6f;
		killFeedIndex = 88;
		weaponSlotIndex = 59;
		vileWeight = 2;
		ammousage = vileAmmoUsage;
		damage = "2";
		hitcooldown = "0.2";
		effect = "Won't destroy on hit.";
	}

	public override void shoot(Character character, int[] args) {
		character.playSound("vulcan", sendRpc: true);
		new VulcanDistanceNeedler(
			character.getShootPos(), character.getShootXDir(), character,
			character.player, character.player.getNextActorNetId(), rpc: true
		);
	}
}
public class BuckshotDance : VileVulcan {
	public static BuckshotDance netWeapon = new();
	public static int critRange = 0;
	public static int shootNum = 0;

	public BuckshotDance() : base() {
		type = (int)VulcanType.BuckshotDance;
		fireRate = 8;
		displayName = "Buckshot Dance";
		vileAmmoUsage = 2f;
		killFeedIndex = 89;
		weaponSlotIndex = 60;
		vileWeight = 4;
		ammousage = vileAmmoUsage;
		damage = "1";
		effect = "Splits.";
	}

	public override void shoot(Character character, int[] args) {
		character.playSound("vulcan", sendRpc: true);
		new VulcanBuckshotDance(
			character.getShootPos(), getAngle(character.xDir), character,
			character.player.getNextActorNetId(), sendRpc: true
		);
		if (critRange >= 2f || Helpers.randomRange(0, 6) == 4) {
			critRange -= 2;
			new VulcanBuckshotDance(
				character.getShootPos(), getAngle(character.xDir), character,
				character.player.getNextActorNetId(), sendRpc: true
			);
		} else {
			critRange++;
		}
	}

	public float getAngle(int xDir) {
		float angle = shootNum switch {
			1 => 256 - 14,
			2 => 14,
			_ => 0
		};
		if (xDir == -1) {
			angle += 128;
		}
		shootNum = (shootNum + 1) % 3;

		return angle % 256;
	}
}
public class NoneVulcan : VileVulcan {
	public static NoneVulcan netWeapon = new();
	public NoneVulcan() : base() {
		type = (int)VulcanType.None;
		displayName = "None";
		killFeedIndex = 126;
	}
}
public class VulcanCherryBlast : Projectile {
	public VulcanCherryBlast(
		Point pos, int xDir, Actor owner, Player player, ushort? netId, bool rpc = false
	) : base(
		pos, xDir, owner, "vulcan_proj", netId, player
	) {
		weapon = CherryBlast.netWeapon;
		damager.damage = 1;
		vel = new Point(500 * xDir, 0);
		destroyOnHit = true;
		reflectable = true;
		maxTime = 0.25f;
		projId = (int)ProjIds.Vulcan;
		if (rpc) {
			rpcCreate(pos, owner, ownerPlayer, netId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new VulcanCherryBlast(
			args.pos, args.xDir, args.owner, args.player, args.netId
		);
	}
}
public class VulcanDistanceNeedler : Projectile {
	public VulcanDistanceNeedler(
		Point pos, int xDir, Actor owner, Player player, ushort? netId, bool rpc = false
	) : base(
		pos, xDir, owner, "vulcan_dn_proj", netId, player
	) {
		weapon = DistanceNeedler.netWeapon;
		damager.damage = 2;
		damager.hitCooldown = 12;
		vel = new Point(600 * xDir, 0);
		destroyOnHit = false;
		reflectable = true;
		maxTime = 0.3f;
		projId = (int)ProjIds.DistanceNeedler;
		if (rpc) {
			rpcCreate(pos, owner, ownerPlayer, netId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new VulcanDistanceNeedler(
			args.pos, args.xDir, args.owner, args.player, args.netId
		);
	}
}
public class VulcanBuckshotDance : Projectile {
	public VulcanBuckshotDance(
		Point pos, float byteAngle, Actor owner, ushort? netId,
		bool sendRpc = false, Player? altPlayer = null
	) :	base(
		pos, 1, owner, "vulcan_bd_proj", netId, altPlayer
	) {
		projId = (int)ProjIds.BuckshotDance;
		weapon = BuckshotDance.netWeapon;
		damager.damage = 1;
		destroyOnHit = true;
		reflectable = true;
		maxTime = 15f / 60;
		vel = Point.createFromByteAngle(byteAngle).times(8.4f * 60);

		if (sendRpc) {
			rpcCreateByteAngle(pos, owner, ownerPlayer, netId, byteAngle);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new VulcanBuckshotDance(
			args.pos, args.byteAngle, args.owner, args.netId, altPlayer: args.player
		);
	}
}
/*
public class VulcanMuzzleAnim : Anim {
	Character chr;
	public VulcanMuzzleAnim(Vulcan weapon, Point pos, int xDir, Character chr, ushort? netId = null, bool sendRpc = false, bool ownedByLocalPlayer = true) :
		base(pos, weapon.muzzleSprite ?? "empty", xDir, netId, true, sendRpc, ownedByLocalPlayer) {
		this.chr = chr;
	}

	public override void postUpdate() {
		if (chr.currentFrame.getBusterOffset() != null) {
			changePos(chr.getShootPos());
		}
	}
}

public class Vulcan : Weapon {
	public float vileAmmoUsage;
	public string? muzzleSprite;
	public string? projSprite;
	public static Vulcan netWeaponCB = new Vulcan(VulcanType.CherryBlast);
	public static Vulcan netWeaponDN = new Vulcan(VulcanType.DistanceNeedler);
	public static Vulcan netWeaponBD = new Vulcan(VulcanType.BuckshotDance);
	public Vulcan(VulcanType vulcanType) : base() {
		index = (int)WeaponIds.Vulcan;
		weaponBarBaseIndex = 26;
		weaponBarIndex = weaponBarBaseIndex;
		killFeedIndex = 62;
		weaponSlotIndex = 44;
		type = (int)vulcanType;
		if (vulcanType == VulcanType.None) {
			displayName = "None";
			killFeedIndex = 126;
			ammousage = 0;
			vileAmmoUsage = 0;
			fireRate = 0;
			vileWeight = 0;
			effect = "";
		} else if (vulcanType == VulcanType.NoneCutter) {
			displayName = "None(Cutter)";
			description = new string[] { "Equip Missile." };
			killFeedIndex = 126;
			ammousage = 0;
			vileAmmoUsage = 0;
			fireRate = 0;
			vileWeight = 0;
			effect = "Equip Cutter";
		} else if (vulcanType == VulcanType.NoneMissile) {
			displayName = "None(Missile)";
			description = new string[] { "Equip Missile." };
			killFeedIndex = 126;
			ammousage = 0;
			vileAmmoUsage = 0;
			fireRate = 0;
			vileWeight = 0;
			effect = "Equip Missile";
		} else if (vulcanType == VulcanType.CherryBlast) {
			fireRate = 6;
			displayName = "Cherry Blast";
			vileAmmoUsage = 0.25f;
			muzzleSprite = "vulcan_muzzle";
			projSprite = "vulcan_proj";
			description = new string[] { "With a range of approximately 20 feet,", "this vulcan is easy to use." };
			vileWeight = 2;
			ammousage = vileAmmoUsage;
			damage = "1";
			effect = "None.";
		} else if (vulcanType == VulcanType.DistanceNeedler) {
			fireRate = 15;
			displayName = "Distance Needler";
			vileAmmoUsage = 6f;
			muzzleSprite = "vulcan_dn_muzzle";
			projSprite = "vulcan_dn_proj";
			killFeedIndex = 88;
			weaponSlotIndex = 59;
			description = new string[] { "This vulcan has good range and speed,", "but cannot fire rapidly." };
			vileWeight = 2;
			ammousage = vileAmmoUsage;
			damage = "2";
			hitcooldown = "0.2";
			effect = "Won't destroy on hit.";
		} else if (vulcanType == VulcanType.BuckshotDance) {
			fireRate = 8;
			displayName = "Buckshot Dance";
			vileAmmoUsage = 0.3f;
			muzzleSprite = "vulcan_bd_muzzle";
			projSprite = "vulcan_bd_proj";
			killFeedIndex = 89;
			weaponSlotIndex = 60;
			description = new string[] { "The scattering power of this vulcan", "results in less than perfect aiming." };
			vileWeight = 4;
			ammousage = vileAmmoUsage;
			damage = "1";
			effect = "Splits.";
		}
	}
	public static bool isVulcanTypes(Vile vile) {
		if (vile.vulcanWeapon.type == (int)VulcanType.BuckshotDance) return true;
		if (vile.vulcanWeapon.type == (int)VulcanType.DistanceNeedler) return true;
		return vile.vulcanWeapon.type == (int)VulcanType.CherryBlast;
	}
	public static bool isNotVulcanTypes(Vile vile) {
		if (vile.vulcanWeapon.type == (int)VulcanType.NoneMissile) return true;
		return vile.vulcanWeapon.type == (int)VulcanType.NoneCutter;
	}
	public static bool ladderVoid(Vile vile) {
		if (vile.charState is LadderClimb) {
			if (vile.player.input.isHeld(Control.Left, vile.player)) {
				vile.xDir = -1;
				return true;
			}
			if (vile.player.input.isHeld(Control.Right, vile.player)) {
				vile.xDir = 1;
				return true;
			}
		}
		return false;
	}

	public override void vileShoot(WeaponIds weaponInput, Vile vile) {
		if (vile.vulcanWeapon.type == (int)VulcanType.None) return;
		if (type == (int)VulcanType.DistanceNeedler && shootCooldown > 0) return;
		if (string.IsNullOrEmpty(vile.charState.shootSpriteEx)) return;
		if (isVulcanTypes(vile) && vile.tryUseVileAmmo(vileAmmoUsage, true)) {
			vile.changeSpriteFromName(vile.charState.shootSpriteEx, false);
			ladderVoid(vile);
			shootVulcan(vile);
		} else if (isNotVulcanTypes(vile) && vile.tryUseVileAmmo(vileAmmoUsage, false)) {
			shootVulcan(vile);
			vile.setVileShootTime(this);
		}
	}

	public void shootVulcan(Vile vile) {
		Player player = vile.player;
		if (vile.vulcanWeapon.type == (int)VulcanType.NoneMissile) {
			vile.missileWeapon.vileShoot(WeaponIds.ElectricShock, vile);		
			vile.setVileShootTime(this);
		} else if (vile.vulcanWeapon.type == (int)VulcanType.NoneCutter) {
			vile.cutterWeapon.vileShoot(WeaponIds.VileCutter, vile);		
			vile.setVileShootTime(this);
		} else {
			if (shootCooldown <= 0) {
				vile.vulcanLingerTime = 0f;
				new VulcanMuzzleAnim(this, vile.getShootPos(), vile.getShootXDir(), vile, player.getNextActorNetId(), true, true);
				if (vile.vulcanWeapon.type == (int)VulcanType.CherryBlast) {
					new VulcanCherryBlast(
						vile.getShootPos(), vile.getShootXDir(), vile,
						player, player.getNextActorNetId(), rpc: true
					);
				} else if (vile.vulcanWeapon.type == (int)VulcanType.DistanceNeedler) {
					new VulcanDistanceNeedler(
						vile.getShootPos(), vile.getShootXDir(), vile,
						player, player.getNextActorNetId(), rpc: true
					);
				} else if (vile.vulcanWeapon.type == (int)VulcanType.BuckshotDance) {
					new VulcanBuckshotDance(
						vile.getShootPos(), vile.getShootXDir(), vile,
						player, player.getNextActorNetId(), rpc: true
					);
					if (Global.isOnFrame(3)) {
						new VulcanBuckshotDance(
							vile.getShootPos(), vile.getShootXDir(), vile,
							player, player.getNextActorNetId(), rpc: true
						);
					}
				}
				vile.playSound("vulcan", sendRpc: true);
				shootCooldown = fireRate;
			}
		}
	}
}
*/
