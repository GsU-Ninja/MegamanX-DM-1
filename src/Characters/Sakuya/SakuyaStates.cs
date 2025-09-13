using System;
using System.Collections.Generic;
using SFML.Graphics;
using System.Linq;
using MMXOnline;
namespace MMXOnline;
public class SakuyaStates : CharState {
	public Sakuya sakuya = null!;
	public SakuyaStates(
		string sprite, string transitionSprite = ""
	) : base(
		sprite, transitionSprite
	) {
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		sakuya = character as Sakuya ?? throw new NullReferenceException();
	}
}
public class SakuyaWalk : SakuyaStates {
	public bool skip;
	public SakuyaWalk(string transitionSprite = "", bool skip = false) : base("walk", transitionSprite: skip ? "walk" : "walk_start") {
		exitOnAirborne = true;
		attackCtrl = true;
		normalCtrl = true;
	}


	public override void update() {
		base.update();
		bool pressed = player.input.isPressed(Control.Left, player) || player.input.isPressed(Control.Right, player);
		Point move = new Point(0, 0);
		float runSpeed = sakuya.getRunSpeed();
		if (stateFrames <= 4) {
			//runSpeed = 1 * sakuya.getRunDebuffs();
		}
		if (player.input.isHeld(Control.Left, player)) {
			character.xDir = -1;
			if (character.canMove()) move.x = -runSpeed;
		} else if (player.input.isHeld(Control.Right, player)) {
			character.xDir = 1;
			if (character.canMove()) move.x = runSpeed;
		}
		if (move.magnitude > 0) {
			character.movePoint(move);
		} 
		if (pressed && move.magnitude > 0) {
			character.changeState(new SakuyaWalkBack());
		}
		if (move.magnitude > 0) {
			character.move(move);
		} else {
			character.changeState(new SakuyaTransition(), true);
		}
	}
}
public class SakuyaWalkBack : SakuyaStates {
	public SakuyaWalkBack() : base("walk_back") {
		accuracy = 5;
		exitOnAirborne = true;
		attackCtrl = true;
		normalCtrl = true;
	}

	public override void update() {
		base.update();
		Point move = new Point(0, 0);
		float runSpeed = sakuya.getRunSpeed();
		if (stateFrames <= 4) {
			runSpeed = 1 * sakuya.getRunDebuffs();
		}
		if (player.input.isHeld(Control.Left, player)) {
			character.xDir = -1;
			if (character.canMove()) move.x = -runSpeed;
		} else if (player.input.isHeld(Control.Right, player)) {
			character.xDir = 1;
			if (character.canMove()) move.x = runSpeed;
		}
		if (move.magnitude > 0) {
			character.movePoint(move);
		} else {
			character.changeToIdleOrFall();
		}
		if (move.magnitude > 0) {
			character.move(move);
		} else {
			character.changeState(new SakuyaTransition(), true);
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
public class SakuyaTransition : CharState {
	public SakuyaTransition() : base("run_stop") {
		exitOnAirborne = true;
		attackCtrl = true;
		normalCtrl = true;
	}

	public override void update() {
		base.update();
		if (player.input.isHeld(Control.Left, player)) {
			character.changeState(new SakuyaWalk(skip: false));		
			
		} else if (player.input.isHeld(Control.Right, player)) {
			character.changeState(new SakuyaWalk(skip: false));					
		}	
		if (character.isAnimOver()) {
			character.changeState(new Idle());		
		}
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
		if (character.isFlinchImmune()) return false;
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
		if (character.isFlinchImmune()) return false;
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
public class SakuyaTimeStopSprite : CharState {
	public int loop;
	Sakuya? sakuya;
	public SakuyaTimeStopSprite() : base("time") {
		normalCtrl = true;
		attackCtrl = true;
		airMove = true;
		canStopJump = true;
		airSprite = "time_air";
	}
	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
        if (!character.grounded || character.vel.y < 0) {
			sprite = "time_air";
			defaultSprite = sprite;
			character.changeSpriteFromName(sprite, true);
		}
	}
	public override void onExit(CharState newState) {
		base.onExit(newState);
	}
	public override void update() {
		base.update();
		if (character.frameIndex == 3) {
			loop++;
		}
		if (loop >= 4) {
			sakuya?.changeToIdleOrFall();
		}
		character.stopMoving();
	}
}