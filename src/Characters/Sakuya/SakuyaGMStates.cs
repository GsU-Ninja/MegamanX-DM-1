using System;
using System.Collections.Generic;
using SFML.Graphics;
using System.Linq;
using MMXOnline;
namespace MMXOnline;
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
	public void ShootKnife() {
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
			character.playSound("knife", sendRpc: true);
	}
    public void RunLogic() {
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
              character.changeState(new SakuyaTransition(), true);
    	}
    }
}
public class SakuyaAttack : SakuyaGenericMeleeState {
	public bool fired;
    public int frame;
	public SakuyaAttack(int frame) : base("attack") {
        this.frame = frame;
        airMove = true;
        canJump = true;
		canStopJump = true;
		airSprite = "attack_air";		
		comboFrame = 3;
	}
	public override bool canEnter(Character character) {
		if (sakuya?.AttackCooldown <= 0) return false;
		return base.canEnter(character);
	}
	public override void onEnter(CharState oldState) {
		character.frameIndex = frame;
		base.onEnter(oldState);
        if (!character.grounded || character.vel.y < 0) {
			sprite = "attack_air";
			defaultSprite = sprite;
			character.changeSpriteFromName(sprite, true);
		}
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired && frame != 1) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			ShootKnife();
		}
        if (character.grounded) {
            if (character.isAnimOver()) {
                character.changeState(new SakuyaTransition(), true);
            }
            if (player.input.isHeld(Control.Left, player)) {
                character.changeState(new SakuyaAttackRun(character.frameIndex));             
            } else if (player.input.isHeld(Control.Right, player)) {
                character.changeState(new SakuyaAttackRun(character.frameIndex));					
            }
        }
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
            if (character.grounded)
			sakuya.changeState(new SakuyaAttack2(), true);
            else 
			sakuya.changeState(new SakuyaAttack(0), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttack2 : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttack2() : base("attack2") {
        airMove = true;
        canJump = true;
		canStopJump = true;
		airSprite = "attack_air";	
		comboFrame = 3;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			ShootKnife();
		}
		if (character.isAnimOver() && character.grounded) {
			character.changeState(new SakuyaTransition(), true);
		}
		if (player.input.isHeld(Control.Left, player)) {
			character.changeState(new SakuyaAttackRun2(character.frameIndex));		
			
		} else if (player.input.isHeld(Control.Right, player)) {
			character.changeState(new SakuyaAttackRun2(character.frameIndex));					
		}
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			if (character.grounded)
			sakuya.changeState(new SakuyaAttack3(), true);
            else 
			sakuya.changeState(new SakuyaAttack(0), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttack3 : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttack3() : base("attack3") {
        airMove = true;
        canJump = true;
		canStopJump = true;
		airSprite = "attack_air";
		comboFrame = 3;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			ShootKnife();
		}
		if (character.isAnimOver() && character.grounded) {
			character.changeState(new SakuyaTransition(), true);
		}
		if (player.input.isHeld(Control.Left, player)) {
			character.changeState(new SakuyaAttackRun3(character.frameIndex));		
			
		} else if (player.input.isHeld(Control.Right, player)) {
			character.changeState(new SakuyaAttackRun3(character.frameIndex));					
		}
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			if (character.grounded)
			sakuya.changeState(new SakuyaAttack4(), true);
            else 
			sakuya.changeState(new SakuyaAttack(0), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttack4 : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttack4() : base("attack4") {
        airMove = true;
        canJump = true;
		canStopJump = true;
		airSprite = "attack_air";	
		comboFrame = 3;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			ShootKnife();
		}
		if (character.isAnimOver() && character.grounded) {
			character.changeState(new SakuyaTransition(), true);
		}
		if (player.input.isHeld(Control.Left, player)) {
			character.changeState(new SakuyaAttackRun4(character.frameIndex));		
			
		} else if (player.input.isHeld(Control.Right, player)) {
			character.changeState(new SakuyaAttackRun4(character.frameIndex));					
		}
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttack(0), true);
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
	public int frame;
	public SakuyaAttackRun(int frame) : base("attack_run2") {
		this.frame = frame;
        canJump = true;
		canStopJump = true;
		airSprite = "attack_air";
		comboFrame = 3;
	}
	public override bool canEnter(Character character) {
		if (sakuya?.AttackCooldown <= 0) return false;
		return base.canEnter(character);
	}
	public override void onEnter(CharState oldState) {
		character.frameIndex = frame;
		base.onEnter(oldState);
        if (!character.grounded || character.vel.y < 0) {
			sprite = "attack_air";
			defaultSprite = sprite;
			character.changeSpriteFromName(sprite, true);
		}
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired && frame != 1) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			ShootKnife();
		}
		if (character.isAnimOver()) {
			character.changeState(new SakuyaWalk(skip: false), true);
		}		
        RunLogic();
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackRun2(0), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackRun2 : SakuyaGenericMeleeState {
	public bool fired;
	public int frame;
	public SakuyaAttackRun2(int frame) : base("attack_run") {
		this.frame = frame;
        canJump = true;
		canStopJump = true;
		airSprite = "attack_air";
		comboFrame = 4;
	}
	public override void onEnter(CharState oldState) {
		character.frameIndex = frame;
		base.onEnter(oldState);
        if (!character.grounded || character.vel.y < 0) {
			sprite = "attack_air";
			defaultSprite = sprite;
			character.changeSpriteFromName(sprite, true);
		}
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			ShootKnife();
		}
		if (character.isAnimOver()) {
			character.changeState(new SakuyaWalk(skip: false), true);
		}
        RunLogic();
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackRun3(frame), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackRun3 : SakuyaGenericMeleeState {
	public bool fired;
	public int frame;
	public SakuyaAttackRun3(int frame) : base("attack_run3") {
		this.frame = frame;
        canJump = true;
		canStopJump = true;
		airSprite = "attack_air";
		comboFrame = 3;
	}
	public override void onEnter(CharState oldState) {
		character.frameIndex = frame;
		base.onEnter(oldState);
        if (!character.grounded || character.vel.y < 0) {
			sprite = "attack_air";
			defaultSprite = sprite;
			character.changeSpriteFromName(sprite, true);
		}
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			ShootKnife();
		}
		if (character.isAnimOver()) {
			character.changeState(new SakuyaWalk(skip: false), true);
		}
        RunLogic();
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackRun4(0), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackRun4 : SakuyaGenericMeleeState {
	public bool fired;
	public int frame;
	public SakuyaAttackRun4(int frame) : base("attack_run4") {
		this.frame = frame;
        canJump = true;
		canStopJump = true;
		airSprite = "attack_air";
		comboFrame = 3;
	}
	public override void onEnter(CharState oldState) {
		character.frameIndex = frame;
		base.onEnter(oldState);
        if (!character.grounded || character.vel.y < 0) {
			sprite = "attack_air";
			defaultSprite = sprite;
			character.changeSpriteFromName(sprite, true);
		}
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
			ShootKnife();
		}
		if (character.isAnimOver()) {
			character.changeState(new SakuyaWalk(skip: false), true);
		}
        RunLogic();
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0) {
			sakuya.changeState(new SakuyaAttackRun(0), true);
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
		comboFrame = 3;
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
        RunLogic();
		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0 && sakuya.upHeld) {
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
		if (sakuya.shootPressed && player.SakuyaAmmo > 0 && sakuya.upHeld) {
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
		if (sakuya.shootPressed && player.SakuyaAmmo > 0 && sakuya.upHeld) {
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
		if (sakuya.shootPressed && player.SakuyaAmmo > 0 && sakuya.upHeld) {
			sakuya.changeState(new SakuyaAttackUpAir(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackAir : SakuyaGenericMeleeState {
	public bool fired;
    public int frame;
	public SakuyaAttackAir(int frame) : base("attack_air") {
        this.frame = frame;
        airMove = true;	
		comboFrame = 2;
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			sakuya.player.SakuyaAmmo -= 1;
			fired = true;
            ShootKnife();
		}
        if(sakuya.grounded) {
            RunLogic();
        }

		base.update();
	}
	public override bool altCtrlUpdate(bool[] ctrls) {
		if (sakuya.shootPressed && player.SakuyaAmmo > 0 && !sakuya.grounded) {
			sakuya.changeState(new SakuyaAttackAir(0), true);
			return true;
		}
		if (sakuya.shootPressed && player.SakuyaAmmo > 0 && sakuya.grounded && sakuya.LeftOrRightHeld) {
			sakuya.changeState(new SakuyaAttackRun(0), true);
			return true;
		}
		if (sakuya.shootPressed && player.SakuyaAmmo > 0 && sakuya.grounded) {
			sakuya.changeState(new SakuyaAttack(0), true);
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
		if (sakuya.shootPressed && player.SakuyaAmmo > 0 && sakuya.downHeld) {
			sakuya.changeState(new SakuyaAttackAirDown(), true);
			return true;
		}
		return false;
	}
}
public class SakuyaAttackStunKnife : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackStunKnife() : base("attack") {
	}
	public override void onExit(CharState newState) {
		sakuya.AttackCooldownStunKnife = 90;
		base.onExit(newState);
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			new SakuyaStunKnifeProj(character.getShootPos(),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			player.SakuyaAmmo -= 4;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f, yScale = 0.75f };
		}
		base.update();
	}
}
public class SakuyaAttackChainsaw : SakuyaGenericMeleeState {
	public bool fired;
	public SakuyaAttackChainsaw() : base("attack") {
	}
	public override void onExit(CharState newState) {
		sakuya.AttackCooldownChainsaw = 30;
		base.onExit(newState);
	}
	public override void update() {
		if (character.frameIndex == 1 && !fired) {
			new SakuyaChainSawProj(character.getShootPos(),
			character.getShootXDir(), player, player.getNextActorNetId(), rpc: true);
			player.SakuyaAmmo -= 6;
			fired = true;
			new Anim(character.getShootPos(), "sakuya_knife_effect",
			character.getShootXDir(), player.getNextActorNetId(), true, true) {
				xScale = 0.75f, yScale = 0.75f };
		}
		base.update();
	}
}
public class SakuyaThousandDagger : SakuyaGenericMeleeState {
	public bool fired, fired2, fired3, fired4;
	public int loop;
	public float shoottime;
	public SakuyaThousandDagger() : base("attack_hold") {
	}
	public override void update() {
		sakuya.AmmoCooldown = 1;
		shoottime += Global.spf;
		if (character.frameIndex == 1 && !fired) {
			ShootKnife();
			sakuya.player.SakuyaAmmo -= 1;
			fired4 = false;
			fired = true;
		} else if (character.frameIndex == 5 && !fired2) {
			ShootKnife();
			sakuya.player.SakuyaAmmo -= 1;
			fired = false;
			fired2 = true;
		} else if (character.frameIndex == 9 && !fired3) {
			ShootKnife();
			sakuya.player.SakuyaAmmo -= 1;
			fired2 = false;
			fired3 = true;
		} else if (character.frameIndex == 13 && !fired4) {
			ShootKnife();
			sakuya.player.SakuyaAmmo -= 1;
			fired3 = false;
			fired4 = true;
		}		
		if (character.frameIndex == 13 && loop < 3) {
			character.frameIndex = 0;
			loop++;
		}
		if (character.isAnimOver()) {
			character.changeState(new SakuyaTransition(), true);
		}
		base.update();
	}
}