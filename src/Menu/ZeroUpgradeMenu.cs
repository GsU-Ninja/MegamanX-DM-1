namespace MMXOnline;
using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Audio;
using SFML.Graphics;



public class ZeroUpgradeMenu : IMainMenu {
	public int selectArrowPosY;
	public IMainMenu prevMenu;

	public int optionPosX = 20;
	public int[] optionPosY;

	public ZeroUpgradeMenu(IMainMenu prevMenu) {
		this.prevMenu = prevMenu;
		optionPosY = new int[] {
			30,
			50,
			70,
			90,
			110,
			130
		};
	}

	public void update() {
		var mainPlayer = Global.level.mainPlayer;

		if (!Global.level.server.disableHtSt && Global.input.isPressedMenu(Control.MenuLeft)) {
			UpgradeMenu.onUpgradeMenu = true;
			Menu.change(new UpgradeMenu(prevMenu));
			return;
		}

		Helpers.menuUpDown(ref selectArrowPosY, 0, 5);
		if (Global.level.ZeroMenuAvailable()) {
			if (Global.input.isPressedMenu(Control.MenuConfirm)) {
				switch (selectArrowPosY) {
					case 0:
						if (!mainPlayer.WinceBought && mainPlayer.currency >= Zero.WinceCost) {
							mainPlayer.WinceBought = true;
							mainPlayer.currency -= Zero.WinceCost;
							Global.playSound("ching");
						}
						break;
					case 1:
						if (!mainPlayer.RootBought && mainPlayer.currency >= Zero.RootCost) {
							mainPlayer.RootBought = true;
							mainPlayer.currency -= Zero.RootCost;
							Global.playSound("ching");
						}
						break;
					case 2:
						if (!mainPlayer.DisarmBought && mainPlayer.currency >= Zero.DisarmCost) {
							mainPlayer.DisarmBought = true;
							mainPlayer.currency -= Zero.DisarmCost;
							Global.playSound("ching");
						}
						break;
					case 3:
						if (!mainPlayer.SpeedsterBought && mainPlayer.currency >= Zero.SpeedsterCost) {
							mainPlayer.SpeedsterBought = true;
							mainPlayer.currency -= Zero.SpeedsterCost;
							Global.playSound("chingX4");
						}
						break;
					case 4:
						if (!mainPlayer.HyperDashBought && mainPlayer.currency >= Zero.HyperDashCost) {
							mainPlayer.HyperDashBought = true;
							mainPlayer.currency -= Zero.HyperDashCost;
							Global.playSound("chingX4");
						}
						break;
					case 5:
						if (!mainPlayer.JumperBought && mainPlayer.currency >= Zero.HyperDashCost) {
							mainPlayer.JumperBought = true;
							mainPlayer.currency -= Zero.JumperCost;
							Global.playSound("chingX4");
						}
						break;
				}
			} 
			if (Global.input.isPressedMenu(Control.MenuAlt)) {
				switch (selectArrowPosY) {
					case 0:
						if (mainPlayer.WinceBought) {
							mainPlayer.WinceBought = false;
							mainPlayer.currency += Zero.WinceCost;
						}
						break;
					case 1:
						if (mainPlayer.RootBought) {
							mainPlayer.RootBought = false;
							mainPlayer.currency += Zero.RootCost;
						}
						break;
					case 2:
						if (mainPlayer.DisarmBought) {
							mainPlayer.DisarmBought = false;
							mainPlayer.currency += Zero.DisarmCost;
						}
						break;
					case 3:
						if (mainPlayer.SpeedsterBought) {
							mainPlayer.SpeedsterBought = false;
							mainPlayer.currency += Zero.SpeedsterCost;
						}
						break;
					case 4:
						if (mainPlayer.HyperDashBought) {
							mainPlayer.HyperDashBought = false;
							mainPlayer.currency += Zero.HyperDashCost;
						}
						break;
					case 5:
						if (mainPlayer.JumperBought) {
							mainPlayer.JumperBought = false;
							mainPlayer.currency += Zero.JumperCost;
						}
						break;
				}
			}
		}
		
		else if (Global.input.isPressedMenu(Control.MenuBack)) {
			Menu.change(prevMenu);
		}
	}

	public void render() {
		var mainPlayer = Global.level.mainPlayer;
		DrawWrappers.DrawTextureHUD(Global.textures["pausemenu"], 0, 0);
		if (mainPlayer.isZero) Global.sprites["menu_x3zero"].drawToHUD(0, 294, 107);
		if (mainPlayer.isblack) Global.sprites["menu_x3zero"].drawToHUD(1, 294, 107);
		if (mainPlayer.isviral) Global.sprites["menu_x3zero"].drawToHUD(2, 294, 107);
		if (mainPlayer.isaz) Global.sprites["menu_x3zero"].drawToHUD(3, 294, 107);

		if (!Global.level.server.disableHtSt && Global.frameCount % 60 < 30) {
			Fonts.drawText(FontType.DarkPurple, "<", 18, Global.halfScreenH, Alignment.Center);
		}

		Global.sprites["cursor"].drawToHUD(0, optionPosX - 6, optionPosY[0] + selectArrowPosY * 20 + 3);

		Fonts.drawText(FontType.Yellow, "Zero Menu", Global.screenW * 0.5f, 10, Alignment.Center);
		Fonts.drawText(FontType.Golden,Global.nameCoins + ": " + mainPlayer.currency,Global.screenW * 0.5f, 20, Alignment.Center);
		//Wince
		Fonts.drawText(FontType.Blue, "Wince", optionPosX, optionPosY[0], selected: selectArrowPosY == 0);
		if (mainPlayer.WinceBought == false) {
			Fonts.drawText(FontType.Purple, $" ({Zero.WinceCost} {Global.nameCoins})",optionPosX + 60, optionPosY[0]);
		} else {
			Fonts.drawText(FontType.Purple, "(Active)",optionPosX + 67, optionPosY[0]);
		}
		Fonts.drawText(FontType.Green, "The Z-Saber now inflicts SlowDown",optionPosX, 40);
		//Root
		Fonts.drawText(FontType.Blue, "Root", optionPosX, optionPosY[1],selected: selectArrowPosY == 1);
		if (mainPlayer.RootBought == false) {
			Fonts.drawText(FontType.Purple, $" ({Zero.RootCost} {Global.nameCoins})",optionPosX + 60, optionPosY[1]);
		} else {
			Fonts.drawText(FontType.Purple, "(Active)",optionPosX + 67, optionPosY[1]);
		}
		Fonts.drawText(FontType.Green, "Hyour, Raijin, Denjin denies movement",optionPosX, 60);
		//Disarm
		Fonts.drawText(FontType.Blue, "Disarm", optionPosX, optionPosY[2],selected: selectArrowPosY == 2);
		Fonts.drawText(FontType.Green, "Ryuenjin and Suiretsusan breaks guard/parry.",optionPosX, 80);
		if (mainPlayer.DisarmBought == false) {
		Fonts.drawText(FontType.Purple, $" ({Zero.DisarmCost} {Global.nameCoins})",optionPosX + 60, optionPosY[2]);
		} else {
			Fonts.drawText(FontType.Purple, "(Active)",optionPosX + 67, optionPosY[2]); 
		}
		//Speedster
		Fonts.drawText(FontType.Blue, "Speedster", optionPosX, optionPosY[3],selected: selectArrowPosY == 3);
		Fonts.drawText(FontType.Green, "Move 10% Faster.",optionPosX, 100);
		if (mainPlayer.SpeedsterBought == false) {
			Fonts.drawText(FontType.Purple, $" ({Zero.SpeedsterCost} {Global.nameCoins})",optionPosX + 60, optionPosY[3]);
		} else {
			Fonts.drawText(FontType.Purple, "(Active)",optionPosX + 67, optionPosY[3]); 
		}
		//Hyper Dash
		Fonts.drawText(FontType.Blue, "Hyper Dash", optionPosX, optionPosY[4],selected: selectArrowPosY == 4);
		Fonts.drawText(FontType.Green, "Ground Dash 10% Faster.",optionPosX, 120);
		if (mainPlayer.HyperDashBought == false) {
		Fonts.drawText(FontType.Purple, $" ({Zero.HyperDashCost} {Global.nameCoins})",optionPosX + 60, optionPosY[4]);
		} else {
			Fonts.drawText(FontType.Purple, "(Active)",optionPosX + 67, optionPosY[4]); 
		}
		//Jumper
		Fonts.drawText(FontType.Blue, "Jumper", optionPosX, optionPosY[5],selected: selectArrowPosY == 5);
		Fonts.drawText(FontType.Green, "Jump 10% Higher.",optionPosX, 140);
		if (mainPlayer.JumperBought == false) {
		Fonts.drawText(FontType.Purple, $" ({Zero.JumperCost} {Global.nameCoins})",optionPosX + 60, optionPosY[5]);
		} else {
			Fonts.drawText(FontType.Purple, "(Active)",optionPosX + 67, optionPosY[5]); 
		}

		Fonts.drawTextEX(FontType.Grey, "[MLEFT]: Change Menu", 20, 188);
		Fonts.drawTextEX(FontType.Grey,
			"[OK]: Buy, [ALT]: Sell, [BACK]: Back", 20, 198
		);
	}

}

public class Skill {
    public string Name = "";
    public Func<Player, bool> IsUnlocked;
    public Func<Player, bool> CanUnlock;
    public Func<Player, bool> CanLock;
    public List<Action<Player>> Unlock;
    public List<Action<Player>> Lock;
    public List<Func<Player, bool>> Requirements = new();
    public string description = "";
    public string description2 = "";
    public string Price = "";

    public bool RequirementsMet(Player p) {
        foreach (var req in Requirements) {
            if (!req(p)) return false;
        }
        return true;
    }
}

public interface IMenuHandler {
    void HandleInput(Player p, int ud, int lr);
    void RenderCursor(int slot, int ud, int lr, AnimData cursor, uint GHW, uint GHH);
    void RenderDescription(Player p, int ud, int lr, uint GHW, uint GHH);
    void RenderIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity);
    void RenderWIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity);
}

public class SaberMenuHandler : IMenuHandler {
    private Dictionary<(int ud, int lr), Skill> saberSkills = new();

    public SaberMenuHandler() {
        saberSkills[(0, 0)] = new Skill {
            Name = "Z-Saber",
            IsUnlocked = p => p.BZZSaber,
            CanUnlock = p => !p.BZZSaber,
            CanLock = p => p.BZZSaber && !HasAnyActive(p,
             "Tenshouzan", "DrillC", "SaberC", "ShieldB",
             "FishF" , "BubbleS" , "ZEX", "ESpark", "FShield", "FWave"),
            Unlock = new List<Action<Player>> { p => p.BZZSaber = true },
            Lock = new List<Action<Player>> { p => p.BZZSaber = false },
            description = "The start of your\nZ-Saber expansion.",
            Price = "1",
        };
        saberSkills[(0, 1)] = new Skill {
            Name = "Tenshouzan",
            IsUnlocked = p => p.BZTenshouzan,
            CanUnlock = p => !p.BZTenshouzan,
            CanLock = p => p.BZTenshouzan && !HasAnyActive(p, "BubbleS", "FishF"),
            Unlock = new List<Action<Player>> { p => p.BZTenshouzan = true },
            Lock = new List<Action<Player>> { p => p.BZTenshouzan = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BZZSaber
            },
            description = "A rising upward slash.",
            description2 = "Input: [MUP] + [SHOOT]",
            Price = "1",
        };
        saberSkills[(0, -1)] = new Skill {
            Name = "Drill Crush",
            IsUnlocked = p => p.BZDrillCrush,
            CanUnlock = p => !p.BZDrillCrush,
            CanLock = p => p.BZDrillCrush && !HasAnyActive(p, "SaberC", "ESpark"),
            Unlock = new List<Action<Player>> { p => p.BZDrillCrush = true },
            Lock = new List<Action<Player>> { p => p.BZDrillCrush = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BZZSaber
            },
            Price = "1",
            description = "Do a downwards thrust while falling.",
            description2 = "Input: [MDOWN] + [SHOOT]"
        };
        saberSkills[(1, 0)] = new Skill {
            Name = "Shield B.",
            IsUnlocked = p => p.BZShieldBoomerang,
            CanUnlock = p => !p.BZShieldBoomerang,
            CanLock = p => p.BZShieldBoomerang,
            Unlock = new List<Action<Player>> { p => p.BZShieldBoomerang = true },
            Lock = new List<Action<Player>> { p => p.BZShieldBoomerang = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BZZSaber,
            },
            Price = "1",
            description = "Spin",
            description2 = "Input: [WeaponR] + [UP] + Full Charge"
        };
        saberSkills[(1, 1)] = new Skill {
            Name = "Fish Fang",
            IsUnlocked = p => p.BZFishFang,
            CanUnlock = p => !p.BZFishFang,
            CanLock = p => p.BZFishFang,
            Unlock = new List<Action<Player>> { p => p.BZFishFang = true },
            Lock = new List<Action<Player>> { p => p.BZFishFang = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BZTenshouzan,
            },
            Price = "1",
            description = "Creates two torpedos on slash.",
            description2 = "Input: [SPC] + On Air"
        };
        saberSkills[(1, -1)] = new Skill {
            Name = "Saber C.",
            IsUnlocked = p => p.BZSaberSlam,
            CanUnlock = p => !p.BZSaberSlam,
            CanLock = p => p.BZSaberSlam,
            Unlock = new List<Action<Player>> { p => p.BZSaberSlam = true },
            Lock = new List<Action<Player>> { p => p.BZSaberSlam = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BZDrillCrush,
            },
            Price = "1",
            description = "A projectile is created on Ground.",
            description2 = "Requires Full Charge"
        };
        saberSkills[(-1, 0)] = new Skill {
            Name = "ZS. Extend",
            IsUnlocked = p => p.BZZSaberExtend,
            CanUnlock = p => !p.BZZSaberExtend,
            CanLock = p => p.BZZSaberExtend && !HasAnyActive(p, "BubbleS", "ESpark", "FWave", "FShield"),
            Unlock = new List<Action<Player>> { p => p.BZZSaberExtend = true },
            Lock = new List<Action<Player>> { p => p.BZZSaberExtend = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BZZSaber,
            },
            description = "Small extension that deals damage.",
            description2 = "Input: [MUP] + [SHOOT]",
            Price = "1",
        };
        saberSkills[(-1, 1)] = new Skill {
            Name = "BS. Option",
            IsUnlocked = p => p.BZBubblesplash,
            CanUnlock = p => !p.BZBubblesplash,
            CanLock = p => p.BZBubblesplash && !HasAnyActive(p, "FWave"),
            Unlock = new List<Action<Player>> { p => p.BZBubblesplash = true },
            Lock = new List<Action<Player>> { p => p.BZBubblesplash = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BZZSaberExtend,
                p => p.BZTenshouzan,
            },
            description = "A rising bubble splash upwards slash.",
            description2 = "Input: [MUP] + [SHOOT]",
            Price = "1",
        };
        saberSkills[(-1, -2)] = new Skill {
            Name = "FS. Option",
            IsUnlocked = p => p.BZFrostShield,
            CanUnlock = p => !p.BZFrostShield,
            CanLock = p => p.BZFrostShield,
            Unlock = new List<Action<Player>> { p => p.BZFrostShield = true },
            Lock = new List<Action<Player>> { p => p.BZFrostShield = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BZDrillCrushSpark,
            },
            Price = "1",
            description = "Hyouretsuzan on DC, Block has 4 HP.",
            description2 = "Input: [WeaponR] + [MDOWN]",
        };
        saberSkills[(-1, -1)] = new Skill {
            Name = "ES. Option",
            IsUnlocked = p => p.BZDrillCrushSpark,
            CanUnlock = p => !p.BZDrillCrushSpark,
            CanLock = p => p.BZDrillCrushSpark && !HasAnyActive(p, "FShield"),
            Unlock = new List<Action<Player>> { p => p.BZDrillCrushSpark = true },
            Lock = new List<Action<Player>> { p => p.BZDrillCrushSpark = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BZZSaberExtend,
                p => p.BZDrillCrush,
            },
            description = "Drill Crush creates \ntwo sparks on ground",
            description2 = "",
            Price = "1",
        };
        saberSkills[(-1, 2)] = new Skill {
            Name = "FW. Option",
            IsUnlocked = p => p.BZFirewave,
            CanUnlock = p => !p.BZFirewave,
            CanLock = p => p.BZFirewave,
            Unlock = new List<Action<Player>> { p => p.BZFirewave = true },
            Lock = new List<Action<Player>> { p => p.BZFirewave = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BZBubblesplash,
            },
            description = "Tenshouzan gets two ticks of Fire\nThe rest one tick of Fire",
            description2 = "",
            Price = "1",
        };
    }
    public void RenderDescription(Player p, int ud, int lr, uint GHW, uint GHH) {
        if (!saberSkills.TryGetValue((ud, lr), out var skill)) return;
        if (string.IsNullOrWhiteSpace(skill.description)) return;
        bool met = skill.RequirementsMet(p);
        FontType font = met ? FontType.Blue : FontType.Grey;
        int wsy = 162;
        DrawWrappers.DrawRect(
            20, wsy - 4, 364, wsy + 18, true, new Color(0, 0, 0, Helpers.toColorByte(Options.main.TreeOpacity / 3)),
            Options.main.TreeOpacity, ZIndex.HUD, false, outlineColor: Color.White
        );
        Fonts.drawTextEX(font, "Name:" + skill.Name, GHW + 70, GHH + 54, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
        Fonts.drawTextEX(font, skill.description2, GHW - 170, GHH + 64, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
        Fonts.drawTextEX(font, "Price: " + skill.Price, GHW + 70, GHH + 64, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
        Fonts.drawTextEX(font, skill.description, GHW - 170, GHH + 54, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
    }

    public void HandleInput(Player p, int ud, int lr) {
        if (!saberSkills.TryGetValue((ud, lr), out var skill)) return;
        if (Global.input.isPressedMenu(Control.MenuConfirm) && skill.CanUnlock(p) && skill.RequirementsMet(p)) {
            foreach (var action in skill.Unlock) action(p);
            Global.playSound("ching");
        } else if (Global.input.isPressedMenu(Control.MenuAlt) && skill.CanLock(p)) {
            foreach (var action in skill.Lock) action(p);
            Global.playSound("busterX3");
        }
    }

    public void RenderCursor(int slot, int ud, int lr, AnimData cursor, uint GHW, uint GHH) {
        Dictionary<(int, int), (float x, float y)> positions = new() {
            {(0, 0), (0, 0)},
            {(0, 1), (50, 0)},
            {(0, -1), (-50, 0)},
            {(1, 0), (0, 30)},
            {(1, 1), (50, 40)},
            {(1, -1), (-50, 40)},
            {(-1, 0), (0, -40)},
            {(-1, 1), (50, -40)},
            {(-1, -1), (-50, -40)},
            {(-1, -2), (-90, -40)},
            {(-1, 2), (90, -40)},
            {(0, -2), (-90, 0)},
            {(0, 2), (90, 0)},
            {(1, -2), (-90, 0)},
            {(1, 2), (90, 0)},

        };
        if (positions.TryGetValue((ud, lr), out var pos))
            cursor.drawToHUD(0, GHW + pos.x, GHH + pos.y);
    }
    public void RenderIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {
        Dictionary<int, Point> zSaberIconPositions = new()
        {
            { 25, new Point(0, 0) },
            { 33, new Point(0, 30) },
            { 27, new Point(0, -40) },
            { 28, new Point(-50, 40) },
            { 31, new Point(-50, 0) },
            { 60, new Point(-90, 0) },
            { 57, new Point(-90, -40) }, //FS
            { 12, new Point(25, -20) },
            { 34, new Point(50, 0) },
            { 30, new Point(50, 40) },
            { 56, new Point(50, -40) },
            { 36, new Point(90, -40) }, //FW
        };
        foreach (var icons in zSaberIconPositions) {
            icon.drawToHUD(icons.Key, GHW + icons.Value.x, GHH + icons.Value.y, opacity);
        }
        icon.drawToHUD(60, 282, 108, opacity);

    }
    public void RenderWIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {
        Dictionary<int, Point> zSaberWeaponIconPositions = new()
        {
            { 48, new Point(-25, -20) },
            { 6, new Point(-50, -40) }
        };
        foreach (var icons in zSaberWeaponIconPositions) {
            icon.drawToHUD(icons.Key, GHW + icons.Value.x, GHH + icons.Value.y, opacity);
        }
    }
    private static readonly Dictionary<string, Func<Player, bool>> dependentsMap = new() {
        { "Saber", p => p.BZZSaber },
        { "Tenshouzan", p => p.BZTenshouzan },
        { "DrillC", p => p.BZDrillCrush },
        { "SaberC", p => p.BZSaberSlam },
        { "ShieldB", p => p.BZShieldBoomerang },
        { "FishF", p => p.BZFishFang },
        { "BubbleS", p => p.BZBubblesplash },
        { "ZEX", p => p.BZZSaberExtend },
        { "ESpark", p => p.BZDrillCrushSpark },
        { "FShield", p => p.BZFrostShield },
        { "SparkShot", p => p.BZSparkShot },
        { "FWave", p => p.BZFirewave },
    };
    private static bool HasAnyActive(Player p, params string[] fields) {
        foreach (var field in fields) {
            if (dependentsMap[field](p)) return true;
        }
        return false;
    }
}

public class SaberMenuHandler2 : IMenuHandler {
    private Dictionary<(int ud, int lr), Skill> saberSkills = new();

    public SaberMenuHandler2() {
        saberSkills[(0, 0)] = new Skill {
            Name = "Saber Dash",
            IsUnlocked = p => p.BZDash,
            CanUnlock = p => !p.BZDash,
            CanLock = p => p.BZDash,
            Unlock = new List<Action<Player>> { p => p.BZDash = true },
            Lock = new List<Action<Player>> { p => p.BZDash = false },
            Price = "1",
            description = "Just a Saber Dash",
            description2 = "Input: [DASH] + [SPC]",
            Requirements = new List<Func<Player, bool>> { p => p.BZZSaber },
        };
        saberSkills[(0, 1)] = new Skill {
            Name = "BZDash2",
            IsUnlocked = p => p.BZDash2,
            CanUnlock = p => !p.BZDash2,
            CanLock = p => p.BZDash2,
            Unlock = new List<Action<Player>> { p => p.BZDash2 = true },
            Lock = new List<Action<Player>> { p => p.BZDash2 = false },
            Requirements = new List<Func<Player, bool>> { p => p.BZZSaber },
            description = "No idea of what Add."
        };
    }
    public void RenderDescription(Player p, int ud, int lr, uint GHW, uint GHH) {
        if (!saberSkills.TryGetValue((ud, lr), out var skill)) return;
        if (string.IsNullOrWhiteSpace(skill.description)) return;
        bool met = skill.RequirementsMet(p);
        FontType font = met ? FontType.Blue : FontType.Grey;
        int wsy = 162;
        DrawWrappers.DrawRect(
            20, wsy - 4, 364, wsy + 18, true, new Color(0, 0, 0, Helpers.toColorByte(Options.main.TreeOpacity / 3)),
            Options.main.TreeOpacity, ZIndex.HUD, false, outlineColor: Color.White
        );
        Fonts.drawTextEX(font, "Name:" + skill.Name, GHW + 70, GHH + 54, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
        Fonts.drawTextEX(font, skill.description2, GHW - 170, GHH + 64, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
        Fonts.drawTextEX(font, "Price: " + skill.Price, GHW+70, GHH+64, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
        Fonts.drawTextEX(font, skill.description, GHW - 170, GHH + 54, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
    }

    public void HandleInput(Player p, int ud, int lr) {
        if (!saberSkills.TryGetValue((ud, lr), out var skill)) return;
        if (p.EXPCurrency >= 1 && Global.input.isPressedMenu(Control.MenuConfirm)
            && skill.CanUnlock(p) && skill.RequirementsMet(p)) {
            foreach (var action in skill.Unlock) action(p);
            p.EXPCurrency--;
            Global.playSound("ching");
        } else if (Global.input.isPressedMenu(Control.MenuAlt) && skill.CanLock(p)) {
            foreach (var action in skill.Lock) action(p);
            p.EXPCurrency++;
            Global.playSound("busterX3");
        }
    }

    public void RenderCursor(int slot, int ud, int lr, AnimData cursor, uint GHW, uint GHH) {
        Dictionary<(int, int), (float x, float y)> positions = new() {
            {(0, 0), (-25, -20)},
            {(0, 1), (25, -20)},
            

        };
        if (positions.TryGetValue((ud, lr), out var pos))
            cursor.drawToHUD(0, GHW + pos.x, GHH + pos.y);
    }
    public void RenderIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {
        

    }
    public void RenderWIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {
        
    }
}

public class BusterMenuHandler : IMenuHandler {
    private Dictionary<(int ud, int lr), Skill> busterSkills = new();

    public BusterMenuHandler() {
        busterSkills[(0, 0)] = new Skill {
            Name = "Z-Buster",
            IsUnlocked = p => p.BZBuster,
            CanUnlock = p => !p.BZBuster,
            CanLock = p => p.BZBuster && !HasAnyActive(p,
            "BusterPlus", "BZVulcan", "ReflectLaser", "BurningShot", "LaserShot", "IceJavelin", "TripleShot",
            "BlastShot", "VShot", "SparkShot", "BlizzardArrow", "YammarkOption", "ParasiteBomb", "ZDrones"),
            Unlock = new List<Action<Player>> { p => p.BZBuster = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.BZBuster = false, p => p.EXPCurrency = p.EXPCurrency + 1 },
            Requirements = new List<Func<Player, bool>> { p => p.EXPCurrency >= 1 },
            description = "The start of your\nZ-Buster expansion.",
            Price = "1",
        };
        busterSkills[(0, 1)] = new Skill {
            Name = "Speed Shoot",
            IsUnlocked = p => p.BZVulcan,
            CanUnlock = p => !p.BZVulcan,
           CanLock = p => p.BZVulcan && !HasAnyActive(p, "BlastShot", "ReflectLaser", "BurningShot"),
            Unlock = new List<Action<Player>> { p => p.BZVulcan = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.BZVulcan = false, p => p.EXPCurrency = p.EXPCurrency + 1 },
            Requirements = new List<Func<Player, bool>> { p => p.BZBuster, p => p.EXPCurrency >= 1 },
            description = "LV 1 Main Projectile is \na little bit faster.",
            description2 = "",
            Price = "1",
        };
        busterSkills[(0, 2)] = new Skill {
            Name = "Reflect Laser",
            IsUnlocked = p => p.BZReflectLaser,
            CanUnlock = p => !p.BZReflectLaser,
            CanLock = p => p.BZReflectLaser && !HasAnyActive(p, "VShot", "BurningShot"),
            Unlock = new List<Action<Player>> { p => p.BZReflectLaser = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.BZReflectLaser = false, p => p.EXPCurrency = p.EXPCurrency + 1 },
            Requirements = new List<Func<Player, bool>> { p => p.BZVulcan, p => p.EXPCurrency >= 1 },
            description = "LV 1 Main Projectile bounces \non contact with a wall.",
            description2 = "",
            Price = "1",
        };
        busterSkills[(0, 3)] = new Skill {
            Name = "Burning Shot",
            IsUnlocked = p => p.BZBurningShot,
            CanUnlock = p => !p.BZBurningShot,
            CanLock = p => p.BZBurningShot,
            Unlock = new List<Action<Player>> { p => p.BZBurningShot = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.BZBurningShot = false, p => p.EXPCurrency = p.EXPCurrency + 1 },
            Requirements = new List<Func<Player, bool>> { p => p.BZReflectLaser, p => p.EXPCurrency >= 1 },
            description = "LV 1 Main Projectile generates \nan explosion on contact with a wall.",
            description2 = "",
            Price = "1",
        };
        busterSkills[(0, -1)] = new Skill {
            Name = "Laser Shot",
            IsUnlocked = p => p.BZLaserShot,
            CanUnlock = p => !p.BZLaserShot,
            CanLock = p => p.BZLaserShot && !HasAnyActive(p, "SparkShot", "IceJavelin", "TripleShot"),
            Unlock = new List<Action<Player>> { p => p.BZLaserShot = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.BZLaserShot = false, p => p.EXPCurrency = p.EXPCurrency + 1 },
            Requirements = new List<Func<Player, bool>> { p => p.BZBuster, p => p.EXPCurrency >= 1 },
            description = "LV 1 Projectiles won't destroy on hit.",
            description2 = "",
            Price = "1",
        };
        busterSkills[(0, -2)] = new Skill {
            Name = "Ice Javelin",
            IsUnlocked = p => p.BZIceJavelin,
            CanUnlock = p => !p.BZIceJavelin,
            CanLock = p => p.BZIceJavelin && !HasAnyActive(p, "BlizzardArrow", "TripleShot"),
            Unlock = new List<Action<Player>> { p => p.BZIceJavelin = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.BZIceJavelin = false, p => p.EXPCurrency = p.EXPCurrency + 1 },
            Requirements = new List<Func<Player, bool>> { p => p.BZLaserShot, p => p.EXPCurrency >= 1 },
            description = "LV 1 Main Projectile time increased. \nDecelerates on contact, adds Freeze.",
            description2 = "",
            Price = "1",
        };
        busterSkills[(0, -3)] = new Skill {
            Name = "Triple Shot",
            IsUnlocked = p => p.BZTripleShot,
            CanUnlock = p => !p.BZTripleShot,
            CanLock = p => p.BZTripleShot,
            Unlock = new List<Action<Player>> { p => p.BZTripleShot = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.BZTripleShot = false, p => p.EXPCurrency = p.EXPCurrency + 1 },
            Requirements = new List<Func<Player, bool>> { p => p.BZIceJavelin, p => p.EXPCurrency >= 1 },
            description = "LV 1 Main Projectile will create \ntwo projectiles on contact.",
            description2 = "",
            Price = "1",
        };
        busterSkills[(1, 0)] = new Skill {
            Name = "Yammark O.",
            IsUnlocked = p => p.BZYammarkOption,
            CanUnlock = p => !p.BZYammarkOption,
            CanLock = p => p.BZYammarkOption && !HasAnyActive(p, "Buster","ParasiteBomb, ZDrones"),
            Unlock = new List<Action<Player>> { p => p.BZYammarkOption = true, p => p.EXPCurrency = p.EXPCurrency - 2 },
            Lock = new List<Action<Player>> { p => p.BZYammarkOption = false, p => p.EXPCurrency = p.EXPCurrency + 2 },
            Requirements = new List<Func<Player, bool>> { p => p.BZBuster, p => p.EXPCurrency >= 2 },
            description = "No way Yammark Option?!.",
            description2 = "",
            Price = "2",
        };
        busterSkills[(1, 1)] = new Skill {
            Name = "Parasite B.",
            IsUnlocked = p => p.BZParasiteBomb,
            CanUnlock = p => !p.BZParasiteBomb,
            CanLock = p => p.BZParasiteBomb,
            Unlock = new List<Action<Player>> { p => p.BZParasiteBomb = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.BZParasiteBomb = false, p => p.EXPCurrency = p.EXPCurrency - +1 },
            Requirements = new List<Func<Player, bool>> { p => p.BZYammarkOption, p => p.EXPCurrency >= 1 },
            description = "Add a Parasite Bomb to the weapon.",
            description2 = "",
            Price = "1",
        };
        busterSkills[(1, -1)] = new Skill {
            Name = "Z-Drone",
            IsUnlocked = p => p.BZZDrones,
            CanUnlock = p => !p.BZZDrones,
            CanLock = p => p.BZZDrones,
            Unlock = new List<Action<Player>> { p => p.BZZDrones = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.BZZDrones = false, p => p.EXPCurrency = p.EXPCurrency + 1 },
            Requirements = new List<Func<Player, bool>> { p => p.BZYammarkOption, p => p.EXPCurrency >= 1 },
            description = "Add a Drone to the weapon.",
            description2 = "",
            Price = "1",
        };
        busterSkills[(-1, 0)] = new Skill {
            Name = "Buster Plus",
            IsUnlocked = p => p.BZBuster2,
            CanUnlock = p => !p.BZBuster2,
            CanLock = p => p.BZBuster2 && !HasAnyActive(p, "BlizzardArrow", "SparkShot", "BlastShot", "VShot"),
            Unlock = new List<Action<Player>> { p => p.BZBuster2 = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.BZBuster2 = false, p => p.EXPCurrency = p.EXPCurrency + 1 },
            Requirements = new List<Func<Player, bool>> { p => p.BZBuster, p => p.EXPCurrency >= 1 },
            description = "LV 2 Projectiles gets one \nextra point of damage.",
            description2 = "",
            Price = "1",
        };
        busterSkills[(-1, 1)] = new Skill {
            Name = "Blast Shot",
            IsUnlocked = p => p.BZBlastShot,
            CanUnlock = p => !p.BZBlastShot,
            CanLock = p => p.BZBlastShot && !HasAnyActive(p, "VShot"),
            Unlock = new List<Action<Player>> { p => p.BZBlastShot = true, p => p.EXPCurrency = p.EXPCurrency - 2 },
            Lock = new List<Action<Player>> { p => p.BZBlastShot = false, p => p.EXPCurrency = p.EXPCurrency + 2 },
            Requirements = new List<Func<Player, bool>> { p => p.BZBuster2, p => p.BZVulcan, p => p.EXPCurrency >= 2 },
            description = "LV 2 Projectiles generates \nan explosion on contact with an enemy.",
            description2 = "",
            Price = "2",
        };
        busterSkills[(-1, 2)] = new Skill {
            Name = "V-Shot",
            IsUnlocked = p => p.BZVShot,
            CanUnlock = p => !p.BZVShot,
            CanLock = p => p.BZVShot,
            Unlock = new List<Action<Player>> { p => p.BZVShot = true, p => p.EXPCurrency = p.EXPCurrency - 2 },
            Lock = new List<Action<Player>> { p => p.BZVShot = false, p => p.EXPCurrency = p.EXPCurrency + 2 },
            Requirements = new List<Func<Player, bool>> { p => p.BZBlastShot, p => p.BZReflectLaser, p => p.EXPCurrency >= 2 },
            description = "LV 2 Projectile splits \nas well as it's damage.",
            description2 = "",
            Price = "2",
        };
        busterSkills[(-1, -1)] = new Skill {
            Name = "Spark Shot",
            IsUnlocked = p => p.BZSparkShot,
            CanUnlock = p => !p.BZSparkShot,
            CanLock = p => p.BZSparkShot && !HasAnyActive(p, "BlizzardArrow"),
            Unlock = new List<Action<Player>> { p => p.BZSparkShot = true, p => p.EXPCurrency = p.EXPCurrency - 2 },
            Lock = new List<Action<Player>> { p => p.BZSparkShot = false, p => p.EXPCurrency = p.EXPCurrency + 2 },
            Requirements = new List<Func<Player, bool>> { p => p.BZBuster2, p => p.BZLaserShot, p => p.EXPCurrency >= 2 },
            description = "LV 2 Projectiles generates \ntwo sparks on contact with an enemy.",
            description2 = "",
            Price = "2",
        };
        busterSkills[(-1, -2)] = new Skill {
            Name = "Blizzard A.",
            IsUnlocked = p => p.BZBlizzardArrow,
            CanUnlock = p => !p.BZBlizzardArrow,
            CanLock = p => p.BZBlizzardArrow,
            Unlock = new List<Action<Player>> { p => p.BZBlizzardArrow = true, p => p.EXPCurrency = p.EXPCurrency - 2 },
            Lock = new List<Action<Player>> { p => p.BZBlizzardArrow = false, p => p.EXPCurrency = p.EXPCurrency + 2 },
            Requirements = new List<Func<Player, bool>> { p => p.BZSparkShot, p => p.BZIceJavelin, p => p.EXPCurrency >= 2 },
            description = "LV 2 Projectiles generates \ntwo icicle blasts.",
            description2 = "",
            Price = "2",
        };
    }
    public void RenderDescription(Player p, int ud, int lr, uint GHW, uint GHH) {
        if (!busterSkills.TryGetValue((ud, lr), out var skill)) return;
        if (string.IsNullOrWhiteSpace(skill.description)) return;
        bool met = skill.RequirementsMet(p);
        FontType font = met ? FontType.Blue : FontType.Grey;
        int wsy = 162;
        DrawWrappers.DrawRect(
            20, wsy - 4, 364, wsy + 18, true, new Color(0, 0, 0, Helpers.toColorByte(Options.main.TreeOpacity / 3)),
            Options.main.TreeOpacity, ZIndex.HUD, false, outlineColor: Color.White
        );
        Fonts.drawTextEX(font, "Name:" + skill.Name, GHW + 60, GHH + 54, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
        Fonts.drawTextEX(font, skill.description2, GHW - 170, GHH + 64, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
        Fonts.drawTextEX(font, "Price: " + skill.Price, GHW + 60, GHH + 64, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
        Fonts.drawTextEX(font, skill.description, GHW - 170, GHH + 54, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
    }
    public void HandleInput(Player p, int ud, int lr) {
        if (!busterSkills.TryGetValue((ud, lr), out var skill)) return;
        if (Global.input.isPressedMenu(Control.MenuConfirm)
            && skill.CanUnlock(p) && skill.RequirementsMet(p)) {
            foreach (var action in skill.Unlock) action(p);
            Global.playSound("ching");
        } else if (Global.input.isPressedMenu(Control.MenuAlt) && skill.CanLock(p)) {
            foreach (var action in skill.Lock) action(p);
            Global.playSound("busterX3");
        }
    }

    public void RenderCursor(int slot, int ud, int lr, AnimData cursor, uint GHW, uint GHH) {
        Dictionary<(int, int), (float x, float y)> positions = new() {
            {(0, 0), (0, 0)},
            {(0, 1), (50, 0)},
            {(0, -1), (-50, 0)},
            {(0, -3), (-130, 0)},
            {(0, 3), (130, 0)},
            {(-1, 0), (0, -40)},
            {(-1, 1), (50, -40)},
            {(-1, -1), (-50, -40)},
            {(-1, -2), (-90, -40)},
            {(-1, 2), (90, -40)},
            {(0, -2), (-90, 0)},
            {(0, 2), (90, 0)},
            {(1, -2), (-70, 40)},
            {(1, 2), (70, 40)},
            {(1, 0), (0, 30)},
            {(1, 1), (30, 50)},
            {(1, -1), (-30, 50)},

        };
        if (positions.TryGetValue((ud, lr), out var pos))
            cursor.drawToHUD(0, GHW + pos.x, GHH + pos.y);
    }
    public void RenderIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {
        Dictionary<int, Point> busterIconPositions = new()
        {
            { 61, new Point(0, 0) },
            { 26, new Point(0, 30) },
            { 7, new Point(0, -40) },
            { 10, new Point(-25, -20) },
            { 0, new Point(-50, 0) },
            { 32, new Point(-70, -20) },
            { 12, new Point(-90, 0) },
            { 2, new Point(-50, -40) },
            { 5, new Point(-90, -40) },
            { 11, new Point(25, -20) },
            { 8, new Point(50, 0) },
            { 37, new Point(70, -20) },
            { 4, new Point(90, 0) },
            { 3, new Point(50, -40) },
            { 6, new Point(90, -40) },
            { 13, new Point(130, 0) },
            { 1, new Point(-130, 0) }
        };
        foreach (var icons in busterIconPositions) {
            icon.drawToHUD(icons.Key, GHW + icons.Value.x, GHH + icons.Value.y, opacity);
        }
    }
    public void RenderWIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {
        Dictionary<int, Point> spriteWeaponIconPositions = new()
       {
            { 18, new Point(30, 50) },
            { 6, new Point(-30, 50) }
        };
        foreach (var icons in spriteWeaponIconPositions) {
            icon.drawToHUD(icons.Key, GHW + icons.Value.x, GHH + icons.Value.y, opacity);
        }
    }
    private static readonly Dictionary<string, Func<Player, bool>> dependentsMap = new() {
        { "Buster", p => p.BZBuster },
        { "BusterPlus", p => p.BZBuster2 },
        { "BZVulcan", p => p.BZVulcan },
        { "ReflectLaser", p => p.BZReflectLaser },
        { "BurningShot", p => p.BZBurningShot },
        { "LaserShot", p => p.BZLaserShot },
        { "IceJavelin", p => p.BZIceJavelin },
        { "TripleShot", p => p.BZTripleShot },
        { "BlastShot", p => p.BZBlastShot },
        { "VShot", p => p.BZVShot },
        { "SparkShot", p => p.BZSparkShot },
        { "BlizzardArrow", p => p.BZBlizzardArrow },
        { "YammarkOption", p => p.BZYammarkOption },
        { "ParasiteBomb", p => p.BZParasiteBomb },
        { "ZDrones", p => p.BZZDrones },
    };
    private static bool HasAnyActive(Player p, params string[] fields) {
        foreach (var field in fields) {
            if (dependentsMap[field](p)) return true;
        }
        return false;
    }
}
public class BusterMenuHandler2 : IMenuHandler {
    private Dictionary<(int ud, int lr), Skill> busterSkills = new();

    public BusterMenuHandler2() {
        busterSkills[(0, 3)] = new Skill {
            Name = "Tri-Thunder",
            IsUnlocked = p => p.BZTriThunder,
            CanUnlock = p => !p.BZTriThunder && p.EXPCurrency >= 2,
            CanLock = p => p.BZTriThunder,
            Unlock = new List<Action<Player>> { p => p.BZTriThunder = true, p => p.EXPCurrency = p.EXPCurrency-2 },
            Lock = new List<Action<Player>> { p => p.BZTriThunder = false, p => p.EXPCurrency = p.EXPCurrency+2 },
            Requirements = new List<Func<Player, bool>> { p => p.BZBuster },
            description = "A Giga Attack capable\nto be charged for another state.",
            description2 = "",
            Price = "2",
        };
        busterSkills[(0, 1)] = new Skill {
            Name = "Time S.",
            IsUnlocked = p => p.BZTimeStopper,
            CanUnlock = p => !p.BZTimeStopper && p.EXPCurrency >= 2,
            CanLock = p => p.BZTimeStopper,
            Unlock = new List<Action<Player>> { p => p.BZTimeStopper = true, p => p.EXPCurrency = p.EXPCurrency-2 },
            Lock = new List<Action<Player>> { p => p.BZTimeStopper = false, p => p.EXPCurrency = p.EXPCurrency+2 },
            Requirements = new List<Func<Player, bool>> { p => p.BZBuster },
            description = "LV 2 Buster will\nSlow Down the area on hit.",
            description2 = "",
            Price = "2",
        };
        busterSkills[(0, 2)] = new Skill {
            Name = "Tractor S.",
            IsUnlocked = p => p.BZTractorShot,
            CanUnlock = p => !p.BZTractorShot && p.EXPCurrency >= 2,
            CanLock = p => p.BZTractorShot,
            Unlock = new List<Action<Player>> { p => p.BZTractorShot = true, p => p.EXPCurrency = p.EXPCurrency-2 },
            Lock = new List<Action<Player>> { p => p.BZTractorShot = false, p => p.EXPCurrency = p.EXPCurrency+2 },
            Requirements = new List<Func<Player, bool>> { p => p.BZBuster },
            description = "Enemy projectiles will be absorbed.",
            description2 = "",
            Price = "2",
        };
        busterSkills[(0, 0)] = new Skill {
            Name = "Lighting",
            IsUnlocked = p => p.BZZLighting,
            CanUnlock = p => !p.BZZLighting && p.EXPCurrency >= 2,
            CanLock = p => p.BZZLighting,
            Unlock = new List<Action<Player>> { p => p.BZZLighting = true, p => p.EXPCurrency = p.EXPCurrency-2 },
            Lock = new List<Action<Player>> { p => p.BZZLighting = false, p => p.EXPCurrency = p.EXPCurrency+2},
            Requirements = new List<Func<Player, bool>> { p => p.BZBuster },
            description = "A Giga Attack capable\nto be charged for another state.",
            description2 = "",
            Price = "2",
        };
    }
    public void RenderDescription(Player p, int ud, int lr, uint GHW, uint GHH) {
        if (!busterSkills.TryGetValue((ud, lr), out var skill)) return;
        if (string.IsNullOrWhiteSpace(skill.description)) return;
        bool met = skill.RequirementsMet(p);
        FontType font = met ? FontType.Blue : FontType.Grey;
        int wsy = 162;
        DrawWrappers.DrawRect(
            20, wsy - 4, 364, wsy + 18, true, new Color(0, 0, 0, Helpers.toColorByte(Options.main.TreeOpacity / 3)),
            Options.main.TreeOpacity, ZIndex.HUD, false, outlineColor: Color.White
        );
        Fonts.drawTextEX(font, "Name:" + skill.Name, GHW + 70, GHH + 54, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
        Fonts.drawTextEX(font, skill.description2, GHW - 170, GHH + 64, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
        Fonts.drawTextEX(font, "Price: " + skill.Price, GHW+70, GHH+64, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
        Fonts.drawTextEX(font, skill.description, GHW - 170, GHH + 54, Alignment.Left, alpha: Helpers.toColorByte(Options.main.TreeOpacity));
    }
    public void HandleInput(Player p, int ud, int lr) {
        if (!busterSkills.TryGetValue((ud, lr), out var skill)) return;
        if (Global.input.isPressedMenu(Control.MenuConfirm)
            && skill.CanUnlock(p) && skill.RequirementsMet(p)) {
            foreach (var action in skill.Unlock) action(p);
            Global.playSound("ching");
        } else if (Global.input.isPressedMenu(Control.MenuAlt) && skill.CanLock(p)) {
            foreach (var action in skill.Lock) action(p);
            Global.playSound("busterX3");
        }
    }

    public void RenderCursor(int slot, int ud, int lr, AnimData cursor, uint GHW, uint GHH) {
        Dictionary<(int, int), (float x, float y)> positions = new() {
            {(0, 0), (-70, -20)},
            {(0, 1), (-25, -20)},
            {(0, 2), (25, -20)},
            {(0, 3), (70, -20)}
        };
        if (positions.TryGetValue((ud, lr), out var pos))
            cursor.drawToHUD(0, GHW + pos.x, GHH + pos.y);
    }
    public void RenderIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {

    }
    public void RenderWIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {

    }
}

public class ArmorMenuHandler : IMenuHandler {
    private Dictionary<(int ud, int lr), Skill> armorSkills = new();

    public ArmorMenuHandler() {
        armorSkills[(0, 0)] = new Skill {
            Name = "ArmorZ",
            IsUnlocked = p => p.ArmorZ,
            CanUnlock = p => !p.ArmorZ && p.EXPCurrency >= 1,
            CanLock = p => p.ArmorZ && !HasAnyActive(p,
            "ArmorZUpgrade" ,"ArmorModeEnergy", "ArmorModeX", "ArmorModeActive" ,
            "ArmorModeDefense" , "ArmorModeRise" , "ArmorModePower" , "ArmorModeProto", "ArmorModeErase", "ArmorModeUltimate"),
            Unlock = new List<Action<Player>> { p => p.ArmorZ = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.ArmorZ = false, p => p.EXPCurrency = p.EXPCurrency + 1  },
        };
        armorSkills[(0, 1)] = new Skill {
            Name = "ArmorModeX",
            IsUnlocked = p => p.ArmorModeX,
            CanUnlock = p => !p.ArmorModeX && p.EXPCurrency >= 1,
            CanLock = p => p.ArmorModeX && !HasAnyActive(p, "ArmorModeErase", "ArmorModeEnergy"),
            Unlock = new List<Action<Player>> { p => p.ArmorModeX = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.ArmorModeX = false, p => p.EXPCurrency = p.EXPCurrency + 1 },
            Requirements = new List<Func<Player, bool>> {
                p => p.ArmorZ
            }
        };
        armorSkills[(0, -1)] = new Skill {
            Name = "ArmorModeRise",
            IsUnlocked = p => p.ArmorModeRise,
            CanUnlock = p => !p.ArmorModeRise && p.EXPCurrency >= 1,
            CanLock = p => p.ArmorModeRise && !HasAnyActive(p, "ArmorModeErase", "ArmorModeDefense"),
            Unlock = new List<Action<Player>> { p => p.ArmorModeRise = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.ArmorModeRise = false, p => p.EXPCurrency = p.EXPCurrency + 1 },
            Requirements = new List<Func<Player, bool>> {
                p => p.ArmorZ
            }
        };
        armorSkills[(1, 0)] = new Skill {
            Name = "ArmorModeActive",
            IsUnlocked = p => p.ArmorModeActive,
            CanUnlock = p => !p.ArmorModeActive && p.EXPCurrency >= 1,
            CanLock = p => p.ArmorModeActive && !HasAnyActive(p, "ArmorModePower", "ArmorModeEnergy"),
            Unlock = new List<Action<Player>> { p => p.ArmorModeActive = true, p => p.EXPCurrency = p.EXPCurrency - 1  },
            Lock = new List<Action<Player>> { p => p.ArmorModeActive = false, p => p.EXPCurrency = p.EXPCurrency + 1  },
        };
        armorSkills[(1, 1)] = new Skill {
            Name = "ArmorModeEnergy",
            IsUnlocked = p => p.ArmorModeEnergy,
            CanUnlock = p => !p.ArmorModeEnergy && p.EXPCurrency >= 2,
            CanLock = p => p.ArmorModeEnergy,
            Unlock = new List<Action<Player>> { p => p.ArmorModeEnergy = true, p => p.EXPCurrency = p.EXPCurrency - 2 },
            Lock = new List<Action<Player>> { p => p.ArmorModeEnergy = false, p => p.EXPCurrency = p.EXPCurrency + 2 },
            Requirements = new List<Func<Player, bool>> {
                p => p.ArmorZ,
                p => p.ArmorModeActive,
                p => p.ArmorModeX
                
            }
        };
        armorSkills[(1, -1)] = new Skill {
            Name = "ArmorModePower",
            IsUnlocked = p => p.ArmorModePower,
            CanUnlock = p => !p.ArmorModePower && p.EXPCurrency >= 2,
            CanLock = p => p.ArmorModePower,
            Unlock = new List<Action<Player>> { p => p.ArmorModePower = true, p => p.EXPCurrency = p.EXPCurrency - 2 },
            Lock = new List<Action<Player>> { p => p.ArmorModePower = false, p => p.EXPCurrency = p.EXPCurrency + 2 },
            Requirements = new List<Func<Player, bool>> {
                p => p.ArmorZ,
                p => p.ArmorModeActive,
                p => p.ArmorModeRise,
            }
        };
        armorSkills[(-1, 0)] = new Skill {
            Name = "ArmorModeProto",
            IsUnlocked = p => p.ArmorModeProto,
            CanUnlock = p => !p.ArmorModeProto && p.EXPCurrency >= 1,
            CanLock = p => p.ArmorModeProto && !HasAnyActive(p, "ArmorModeErase", "ArmorModeDefense"),
            Unlock = new List<Action<Player>> { p => p.ArmorModeProto = true, p => p.EXPCurrency = p.EXPCurrency - 1 },
            Lock = new List<Action<Player>> { p => p.ArmorModeProto = false, p => p.EXPCurrency = p.EXPCurrency + 1 },
            Requirements = new List<Func<Player, bool>> {
                p => p.ArmorZ,
            }
        };
        armorSkills[(-1, 1)] = new Skill {
            Name = "ArmorModeErase",
            IsUnlocked = p => p.ArmorModeErase,
            CanUnlock = p => !p.ArmorModeErase && p.EXPCurrency >= 2,
            CanLock = p => p.ArmorModeErase,
            Unlock = new List<Action<Player>> { p => p.ArmorModeErase = true, p => p.EXPCurrency = p.EXPCurrency - 2 },
            Lock = new List<Action<Player>> { p => p.ArmorModeErase = false, p => p.EXPCurrency = p.EXPCurrency + 2 },
            Requirements = new List<Func<Player, bool>> {
                p => p.ArmorZ,
                p => p.ArmorModeX,
                p => p.ArmorModeProto
            }
        };
        armorSkills[(-1, -1)] = new Skill {
            Name = "ArmorModeDefense",
            IsUnlocked = p => p.ArmorModeDefense,
            CanUnlock = p => !p.ArmorModeDefense && p.EXPCurrency >= 2,
            CanLock = p => p.ArmorModeDefense,
            Unlock = new List<Action<Player>> {p => p.ArmorModeDefense = true, p => p.EXPCurrency = p.EXPCurrency - 2 },
            Lock = new List<Action<Player>> { p => p.ArmorModeDefense = false, p => p.EXPCurrency = p.EXPCurrency + 2 },
            Requirements = new List<Func<Player, bool>> {
                p => p.ArmorZ,
                p => p.ArmorModeRise,
                p => p.ArmorModeProto
            }
        };
        armorSkills[(-2, 0)] = new Skill {
            Name = "ArmorZUpgrade",
            IsUnlocked = p => p.ArmorZUpgrade,
            CanUnlock = p => !p.ArmorZUpgrade && p.EXPCurrency >= 1,
            CanLock = p => p.ArmorZUpgrade,
            Unlock = new List<Action<Player>> { p => p.ArmorZUpgrade = true, p => p.EXPCurrency = p.EXPCurrency - 1  },
            Lock = new List<Action<Player>> { p => p.ArmorZUpgrade = false, p => p.EXPCurrency = p.EXPCurrency - 1  },
            Requirements = new List<Func<Player, bool>> {
                p => p.ArmorZ
            }
        };

    }
    public void RenderDescription(Player p, int ud, int lr, uint GHW, uint GHH) {
        if (!armorSkills.TryGetValue((ud, lr), out var skill)) return;
        if (string.IsNullOrWhiteSpace(skill.description)) return;
        bool met = skill.RequirementsMet(p);
        FontType font = met ? FontType.Blue : FontType.Grey;
        Fonts.drawTextEX(font, skill.description, GHW + 126, GHH + 65);
    }
    public void HandleInput(Player p, int ud, int lr) {
        if (!armorSkills.TryGetValue((ud, lr), out var skill)) return;
        if (Global.input.isPressedMenu(Control.MenuConfirm)
            && skill.CanUnlock(p) && skill.RequirementsMet(p)) {
            foreach (var action in skill.Unlock) action(p);
            Global.playSound("ching");
        } else if (Global.input.isPressedMenu(Control.MenuAlt) && skill.CanLock(p)) {
            foreach (var action in skill.Lock) action(p);
            Global.playSound("busterX3");
        }

    }
    public void RenderCursor(int slot, int ud, int lr, AnimData cursor, uint GHW, uint GHH) {
        Dictionary<(int, int), (float x, float y)> positions = new() {
            {(0, 0), (0, 0)},
            {(0, 1), (30, 0)},
            {(-2, 0), (0, -50)},
            {(2, 0), (0, 50)},
            {(0, -1), (-30, 0)},
            {(0, -2), (-50, 0)},
            {(1, 0), (0, 26)},
            {(1, 1), (30, 26)},
            {(1, -1), (-30, 26)},
            {(-1, 0), (0, -26)},
            {(-1, 1), (30, -26)},
            {(-1, -1), (-30, -26)},
            {(-1, -2), (-90, -40)},
            {(-1, 2), (90, -40)},


        };
        if (positions.TryGetValue((ud, lr), out var pos))
            cursor.drawToHUD(0, GHW + pos.x, GHH + pos.y);
    }
    public void RenderIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {
        Dictionary<int, Point> armorSlot1IconPositions = new()
         {
            { 53, new Point(0, 0) },        // Z-Armor
            { 42, new Point(30, 0) },       // X
            { 43, new Point(30, 26) },      // Energy
            { 66, new Point(30, -26) },     // Erase
            { 62, new Point(-30, -26) },    // Defense
            { 65, new Point(-30, 26) },     // Power
            { 67, new Point(0, 50) },       // Ultimate
            { 63, new Point(-30, 0) },      // Rise
            { 64, new Point(0, -26) },      // Proto
            { 44, new Point(0, 26) },       // Active
            { 17, new Point(0, -50) }       // Armor
        };
        foreach (var icons in armorSlot1IconPositions) {
            icon.drawToHUD(icons.Key, GHW + icons.Value.x, GHH + icons.Value.y, opacity);
        }

    }
    public void RenderWIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {

    }
    private static readonly Dictionary<string, Func<Player, bool>> dependentsMap = new() {
        { "ArmorZ", p => p.ArmorZ },
        { "ArmorZUpgrade", p => p.ArmorZUpgrade },
        { "ArmorModeEnergy", p => p.ArmorModeEnergy },
        { "ArmorModeX", p => p.ArmorModeX },
        { "ArmorModeActive", p => p.ArmorModeActive },
        { "ArmorModeDefense", p => p.ArmorModeDefense },
        { "ArmorModeRise", p => p.ArmorModeRise },
        { "ArmorModePower", p => p.ArmorModePower },
        { "ArmorModeProto", p => p.ArmorModeProto },
        { "ArmorModeErase", p => p.ArmorModeErase },
        { "ArmorModeUltimate", p => p.ArmorModeUltimate },
    };
    private static bool HasAnyActive(Player p, params string[] fields) {
        foreach (var field in fields) {
            if (dependentsMap[field](p)) return true;
        }
        return false;
    }
}
public class HelmetMenuHandler : IMenuHandler {
    private Dictionary<(int ud, int lr), Skill> helmetSkills = new();

    public HelmetMenuHandler() {
        helmetSkills[(0, 0)] = new Skill {
            Name = "HelmetZ",
            IsUnlocked = p => p.HelmetZ,
            CanUnlock = p => !p.HelmetZ,
            CanLock = p => p.HelmetZ,
            Unlock = new List<Action<Player>> { p => p.HelmetZ = true },
            Lock = new List<Action<Player>> { p => p.HelmetZ = false },
            Requirements = new List<Func<Player, bool>> {
                
            }
        };
        helmetSkills[(-1, 0)] = new Skill {
            Name = "HelmetAutoRecover",
            IsUnlocked = p => p.HelmetAutoRecover,
            CanUnlock = p => !p.HelmetAutoRecover,
            CanLock = p => p.HelmetAutoRecover,
            Unlock = new List<Action<Player>> { p => p.HelmetAutoRecover = true },
            Lock = new List<Action<Player>> { p => p.HelmetAutoRecover = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.HelmetZ
            }
        };
        helmetSkills[(-1, 1)] = new Skill {
            Name = "HelmetAutoCharge",
            IsUnlocked = p => p.HelmetAutoCharge,
            CanUnlock = p => !p.HelmetAutoCharge,
            CanLock = p => p.HelmetAutoCharge,
            Unlock = new List<Action<Player>> { p => p.HelmetAutoCharge = true },
            Lock = new List<Action<Player>> { p => p.HelmetAutoCharge = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.HelmetZ
            }
        };
        helmetSkills[(-1, -1)] = new Skill {
            Name = "HelmetQuickCharge",
            IsUnlocked = p => p.HelmetQuickCharge,
            CanUnlock = p => !p.HelmetQuickCharge,
            CanLock = p => p.HelmetQuickCharge,
            Unlock = new List<Action<Player>> { p => p.HelmetQuickCharge = true },
            Lock = new List<Action<Player>> { p => p.HelmetQuickCharge = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.HelmetZ
            }
        };
    }
    public void RenderDescription(Player p, int ud, int lr, uint GHW, uint GHH) {
        if (!helmetSkills.TryGetValue((ud, lr), out var skill)) return;
        if (string.IsNullOrWhiteSpace(skill.description)) return;
        bool met = skill.RequirementsMet(p);
        FontType font = met ? FontType.Blue : FontType.Grey;
        Fonts.drawTextEX(font, skill.description, GHW + 126, GHH + 65);
    }
    public void HandleInput(Player p, int ud, int lr) {
        if (!helmetSkills.TryGetValue((ud, lr), out var skill)) return;
        if (p.EXPCurrency >= 1 && Global.input.isPressedMenu(Control.MenuConfirm)
            && skill.CanUnlock(p) && skill.RequirementsMet(p)) {
            foreach (var action in skill.Unlock) action(p);
            p.EXPCurrency--;
            Global.playSound("ching");
        } else if (Global.input.isPressedMenu(Control.MenuAlt) && skill.CanLock(p)) {
            foreach (var action in skill.Lock) action(p);
            p.EXPCurrency++;
            Global.playSound("busterX3");
        }
    }
    public void RenderCursor(int slot, int ud, int lr, AnimData cursor, uint GHW, uint GHH) {
        Dictionary<(int, int), (float x, float y)> positions = new() {
            {(0, 0), (100, 0)},
            {(-1, 0), (100, -32)},
            {(-1, 1), (130, -32)},
            {(-1, -1), (70, -32)},
        };
        if (positions.TryGetValue((ud, lr), out var pos))
            cursor.drawToHUD(0, GHW + pos.x, GHH + pos.y);
    }
    public void RenderIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {
        Dictionary<int, Point> armorSlot2IconPositions = new()
        {
            { 52, new Point(100, 0) },
            { 14, new Point(100, -32) },
            { 15, new Point(70, -32) },
            { 16, new Point(130, -32) }
        };
        foreach (var icons in armorSlot2IconPositions)
        {
            icon.drawToHUD(icons.Key, GHW + icons.Value.x, GHH + icons.Value.y, opacity);
        }
    }
    public void RenderWIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {

    }
}

public class BootsMenuHandler : IMenuHandler {
    private Dictionary<(int ud, int lr), Skill> bootsSkills = new();
    public BootsMenuHandler() {
        bootsSkills[(0, 0)] = new Skill {
            Name = "BootsZ",
            IsUnlocked = p => p.BootsZ,
            CanUnlock = p => !p.BootsZ,
            CanLock = p => p.BootsZ && !HasAnyActive(p, "BootsZ", "BootsFastRun", "BootsJump", "BootsFrog", "BootsDash", "BootsSpark", "BootsHighJump"),
            Unlock = new List<Action<Player>> { p => p.BootsZ = true },
            Lock = new List<Action<Player>> { p => p.BootsZ = false },
        };
        bootsSkills[(0, -1)] = new Skill {
            Name = "BootsFastRun",
            IsUnlocked = p => p.BootsFastRun,
            CanUnlock = p => !p.BootsFastRun,
            CanLock = p => p.BootsFastRun && !HasAnyActive(p, "BootsSpark"),
            Unlock = new List<Action<Player>> { p => p.BootsFastRun = true },
            Lock = new List<Action<Player>> { p => p.BootsFastRun = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BootsZ
            }
        };
        bootsSkills[(0, 1)] = new Skill {
            Name = "BootsJump",
            IsUnlocked = p => p.BootsJump,
            CanUnlock = p => !p.BootsJump,
            CanLock = p => p.BootsJump && !HasAnyActive(p, "BootsHighJump"),
            Unlock = new List<Action<Player>> { p => p.BootsJump = true },
            Lock = new List<Action<Player>> { p => p.BootsJump = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BootsZ
            }
        };
        bootsSkills[(1, 0)] = new Skill {
            Name = "BootsFrog",
            IsUnlocked = p => p.BootsFrog,
            CanUnlock = p => !p.BootsFrog,
            CanLock = p => p.BootsFrog,
            Unlock = new List<Action<Player>> { p => p.BootsFrog = true },
            Lock = new List<Action<Player>> { p => p.BootsFrog = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BootsZ
            }
        };
        bootsSkills[(-1, 0)] = new Skill {
            Name = "BootsDash",
            IsUnlocked = p => p.BootsDash,
            CanUnlock = p => !p.BootsDash,
            CanLock = p => p.BootsDash && !HasAnyActive(p, "BootsHighJump", "BootsSpark"),
            Unlock = new List<Action<Player>> { p => p.BootsDash = true },
            Lock = new List<Action<Player>> { p => p.BootsDash = false },
        };
        bootsSkills[(-1, -1)] = new Skill {
            Name = "BootsSpark",
            IsUnlocked = p => p.BootsSpark,
            CanUnlock = p => !p.BootsSpark,
            CanLock = p => p.BootsSpark,
            Unlock = new List<Action<Player>> { p => p.BootsSpark = true },
            Lock = new List<Action<Player>> { p => p.BootsSpark = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BootsFastRun,
                p => p.BootsDash
            }
        };
        bootsSkills[(-1, 1)] = new Skill {
            Name = "BootsHighJump",
            IsUnlocked = p => p.BootsHighJump,
            CanUnlock = p => !p.BootsHighJump,
            CanLock = p => p.BootsHighJump,
            Unlock = new List<Action<Player>> { p => p.BootsHighJump = true },
            Lock = new List<Action<Player>> { p => p.BootsHighJump = false },
            Requirements = new List<Func<Player, bool>> {
                p => p.BootsDash,
                p => p.BootsJump
            }
        };
    }
    public void RenderDescription(Player p, int ud, int lr, uint GHW, uint GHH) {
        if (!bootsSkills.TryGetValue((ud, lr), out var skill)) return;
        if (string.IsNullOrWhiteSpace(skill.description)) return;
        bool met = skill.RequirementsMet(p);
        FontType font = met ? FontType.Blue : FontType.Grey;
        Fonts.drawTextEX(font, skill.description, GHW + 126, GHH + 65);
    }
    public void HandleInput(Player p, int ud, int lr) {
        if (!bootsSkills.TryGetValue((ud, lr), out var skill)) return;
        if (p.EXPCurrency >= 1 && Global.input.isPressedMenu(Control.MenuConfirm)
            && skill.CanUnlock(p) && skill.RequirementsMet(p)) {
            foreach (var action in skill.Unlock) action(p);
            p.EXPCurrency--;
            Global.playSound("ching");
        } else if (Global.input.isPressedMenu(Control.MenuAlt) && skill.CanLock(p)) {
            foreach (var action in skill.Lock) action(p);
            p.EXPCurrency++;
            Global.playSound("busterX3");
        }
    }
    public void RenderCursor(int slot, int ud, int lr, AnimData cursor, uint GHW, uint GHH) {
        Dictionary<(int, int), (float x, float y)> positions = new() {
            {(0, 0), (-100, 0)},
            {(0, 1), (-70, 0)},
            {(0, -1), (-130, 0)},
            {(-1, 0), (-100, -32)},
            {(-1, 1), (-70, -32)},
            {(-1, -1), (-130, -32)},
            {(1, 0), (-100, 32)},
        };
        if (positions.TryGetValue((ud, lr), out var pos))
            cursor.drawToHUD(0, GHW + pos.x, GHH + pos.y);
    }
    public void RenderIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {
        Dictionary<int, Point> armorSlot3IconPositions = new()
            {
        { 54, new Point(-100, 0) },
        { 20, new Point(-100, 32) },
        { 19, new Point(-100, -32) },
        { 24, new Point(-70, -32) },
        { 22, new Point(-130, -32) },
        { 23, new Point(-70, 0) },
        { 21, new Point(-130, 0) }
    };
        foreach (var icons in armorSlot3IconPositions) {
            icon.drawToHUD(icons.Key, GHW + icons.Value.x, GHH + icons.Value.y, opacity);
        }
    }
    public void RenderWIcons(int frame, AnimData icon, uint GHW, uint GHH, float opacity) {

    }
    private static readonly Dictionary<string, Func<Player, bool>> dependentsMap = new() {
        { "BootsZ", p => p.BootsZ },
        { "BootsSpark", p => p.BootsSpark },
        { "BootsDash", p => p.BootsDash },
        { "BootsFrog", p => p.BootsFrog },
        { "BootsJump", p => p.BootsJump },
        { "BootsFastRun", p => p.BootsFastRun },
        { "BootsHighJump", p => p.BootsHighJump },
    };
    private static bool HasAnyActive(Player p, params string[] fields) {
        foreach (var field in fields) {
            if (dependentsMap[field](p)) return true;
        }
        return false;
    }
}
    

public class BusterZeroArmorMenu : IMainMenu {
    public IMainMenu prevMenu;

    public BusterZeroArmorMenu(IMainMenu prevMenu) {
        this.prevMenu = prevMenu;
        ZBIcon = Global.sprites["hud_zb_icon"];
        Weapon = Global.sprites["hud_weapon_icon"];
        ZBMenu = Global.sprites["menu_x3zero"];
        ZBCursor = Global.sprites["axl_cursor"];
    }

    int SaberUD, SaberLR, BusterUD, BusterLR, ArmorUD, ArmorLR, HelmetUD, HelmetLR, BootsUD, BootsLR, frame;
    public static int xGame = 1;
    public static int Slot = 1;
    SaberMenuHandler saberMenu = new();
    SaberMenuHandler2 saberMenu2 = new();
    BusterMenuHandler busterMenu = new();
    BusterMenuHandler2 busterMenu2 = new();
    ArmorMenuHandler armorMenu = new();
    HelmetMenuHandler helmetMenu = new();
    BootsMenuHandler bootsMenu = new();
    AnimData ZBCursor, ZBMenu, ZBIcon, Weapon;
    Player MainP => Global.level.mainPlayer;
    uint GHW => Global.halfScreenW;
    uint GHH => Global.halfScreenH;

    public void update() {
        switch (xGame) {
            case 1:
                if (Global.input.isPressedMenu(Control.Special2)) {
                    Slot++;
                    Global.playSound("menuX3");
                    if (Slot > 2) Slot = 1;
                    BusterLR = 0;
                    BusterUD = 0;
                }
                if (Slot == 1) {
                    Helpers.menuLeftRightInc(ref BusterLR, -3, 3, true, true);
                    Helpers.menuUpDown(ref BusterUD, -1, 1);
                    busterMenu.HandleInput(MainP, BusterUD, BusterLR);
                }
                if (Slot == 2) {
                    Helpers.menuLeftRightInc(ref BusterLR, 0, 3, true, true);
                    busterMenu2.HandleInput(MainP, BusterUD, BusterLR);
                }
                break;
            case 2:
                if (Global.input.isPressedMenu(Control.Special2)) {
                    Slot++;
                    Global.playSound("menuX3");
                    if (Slot > 2) {
                        Slot = 1;
                    }
                    SaberLR = 0;
                    SaberUD = 0;
                }
                if (Slot == 1) {
                    Helpers.menuLeftRightInc(ref SaberLR, -2, 2, true, true);
                    Helpers.menuUpDown(ref SaberUD, -1, 1);
                    saberMenu.HandleInput(MainP, SaberUD, SaberLR);
                }
                if (Slot == 2) {
                    Helpers.menuLeftRightInc(ref SaberLR, 0, 1, true, true);
                    saberMenu2.HandleInput(MainP, SaberUD, SaberLR);

                }
                break;
            case 3:
                if (Global.input.isPressedMenu(Control.Special2)) {
                    Slot++;
                    Global.playSound("menuX3");
                    if (Slot > 3) {
                        Slot = 1;
                    }
                    ArmorLR = 0;
                    ArmorUD = 0;
                    HelmetLR = 0;
                    HelmetUD = 0;
                    BootsLR = 0;
                    BootsUD = 0;
                }
                if (Slot == 1) {
                    Helpers.menuLeftRightInc(ref ArmorLR, -1, 1, wrap: true, true);
                    Helpers.menuUpDown(ref ArmorUD, -2, 2);
                    armorMenu.HandleInput(MainP, ArmorUD, ArmorLR);
                    Slot1Fixes();
                }
                if (Slot == 2) {
                    Helpers.menuLeftRightInc(ref HelmetLR, -1, 1, wrap: true, true);
                    Helpers.menuUpDown(ref HelmetUD, -1, 0);
                    helmetMenu.HandleInput(MainP, HelmetUD, HelmetLR);
                    Slot2Fixes();
                }
                if (Slot == 3) {
                    Helpers.menuLeftRightInc(ref BootsLR, -1, 1, wrap: true, true);
                    Helpers.menuUpDown(ref BootsUD, -1, 1);
                    bootsMenu.HandleInput(MainP, BootsUD, BootsLR);
                    Slot3Fixes();
                }
                break;
        }
        xGameV();
    }

    public void render() {
        DrawWrappers.DrawTextureHUD(Global.textures["pausemenu"], 0, 0);
        DrawWrappers.DrawRect(364, 40, 20, 180, true, new Color(0, 0, 0, 100), 1,
        ZIndex.HUD, false, outlineColor: Color.White);
        ZBMenu.drawToHUD(0, GHW, GHH, 0.5f);
        TubeRender();
        DrawTextStuff();
        LowerMenuText();
        Fonts.drawTextEX(FontType.Blue, "Slot: ", GHW + 132, GHH - 67);
        Fonts.drawTextEX(FontType.Blue, Slot.ToString(), GHW + 164, GHH - 67);

        switch (xGame) {
            case 1:
                busterMenu.RenderIcons(frame, ZBIcon, GHW, GHH, 1);
                busterMenu.RenderWIcons(frame, Weapon, GHW, GHH, 1);
                BusterHUDRenderB();
                if (Slot == 1) {
                    busterMenu.RenderCursor(Slot, BusterUD, BusterLR, ZBCursor, GHW, GHH);
                    busterMenu.RenderDescription(MainP, BusterUD, BusterLR, GHW, GHH);
                } else {
                    busterMenu2.RenderCursor(Slot, BusterUD, BusterLR, ZBCursor, GHW, GHH);
                    busterMenu2.RenderDescription(MainP, BusterUD, BusterLR, GHW, GHH);
                }
                break;
            case 2:
                saberMenu.RenderIcons(frame, ZBIcon, GHW, GHH, 1);
                saberMenu.RenderWIcons(frame, Weapon, GHW, GHH, 1);
                ZSaberHUDRenderB();
                if (Slot == 1) {
                    saberMenu.RenderCursor(Slot, SaberUD, SaberLR, ZBCursor, GHW, GHH);
                    saberMenu.RenderDescription(MainP, SaberUD, SaberLR, GHW, GHH);
                } else {
                    saberMenu2.RenderCursor(Slot, SaberUD, SaberLR, ZBCursor, GHW, GHH);
                    saberMenu2.RenderDescription(MainP, SaberUD, SaberLR, GHW, GHH);
                }
                break;
            case 3:
                armorMenu.RenderIcons(frame, ZBIcon, GHW, GHH, 1);
                helmetMenu.RenderIcons(frame, ZBIcon, GHW, GHH, 1);
                bootsMenu.RenderIcons(frame, ZBIcon, GHW, GHH, 1);
                ArmorHUDRenderSlot1B();
                ArmorHUDRenderSlot2B();
                ArmorHUDRenderSlot3B();
                switch (Slot) {
                    case 1:
                        //    armorMenu.RenderDescription(MainP, SaberUD, SaberLR, GHW, GHH);
                        armorMenu.RenderCursor(Slot, ArmorUD, ArmorLR, ZBCursor, GHW, GHH);
                        break;
                    case 2:
                        //  helmetMenu.RenderDescription(MainP, SaberUD, SaberLR, GHW, GHH);
                        helmetMenu.RenderCursor(Slot, HelmetUD, HelmetLR, ZBCursor, GHW, GHH);
                        break;
                    case 3:
                        // bootsMenu.RenderDescription(MainP, SaberUD, SaberLR, GHW, GHH);
                        bootsMenu.RenderCursor(Slot, BootsUD, BootsLR, ZBCursor, GHW, GHH);
                        break;
                }
                break;
        }
    }
    public void xGameV() {
        if (Global.input.isPressedMenu(Control.WeaponLeft)) {
            xGame--;
            if (xGame < 1) {
                xGame = 1;
                if (!Global.level.server.disableHtSt) {
                    UpgradeMenu.onUpgradeMenu = true;
                    Menu.change(new UpgradeMenu(prevMenu));
                    return;
                }
            }
        } else if (Global.input.isPressedMenu(Control.WeaponRight)) {
            xGame++;
            if (xGame > 3) {
                xGame = 3;
                if (!Global.level.server.disableHtSt) {
                    UpgradeMenu.onUpgradeMenu = true;
                    Menu.change(new UpgradeMenu(prevMenu));
                    return;
                }
            }
        } else if (Global.input.isPressedMenu(Control.MenuBack)) {
            Menu.change(prevMenu);
        }
    }
    public void TubeRender() {
        //Tube Section (i expect a mess) (looks like it wont be that mess) (ohgoditis)
        switch (xGame) {
            case 1:
                for (int i = 0; i < 13; i++) {
                    ZBIcon.drawToHUD(48, GHW + i * 3 - 85.5f, GHH - 20);
                    ZBIcon.drawToHUD(48, GHW + i * 3 + 54.5f, GHH - 20);
                }
                for (int i = 0; i < 13; i++)
                    ZBIcon.drawToHUD(49, GHW - 0.5f, GHH + i * 5 - 36);
                for (int i = 0; i < 32; i++) {
                    ZBIcon.drawToHUD(48, GHW + i * 4, GHH);
                    ZBIcon.drawToHUD(48, GHW + i * -4, GHH);
                }
                for (int i = 0; i < 6; i++) {
                    ZBIcon.drawToHUD(49, GHW - 50.5f, GHH + i * 5 - 36);
                    ZBIcon.drawToHUD(49, GHW + 49.5f, GHH + i * 5 - 36);
                }
                for (int i = 0; i < 6; i++) {
                    ZBIcon.drawToHUD(49, GHW - 90.5f, GHH + i * 5 - 36);
                    ZBIcon.drawToHUD(49, GHW + 89.5f, GHH + i * 5 - 36);
                }
                for (int i = 0; i < 48; i++)
                    ZBIcon.drawToHUD(48, GHW + i * 4 - 95, GHH - 40);
                for (int i = 0; i < 16; i++)
                    ZBIcon.drawToHUD(48, GHW + i * 4 - 30, GHH + 30);
                for (int i = 0; i < 4; i++) {
                    ZBIcon.drawToHUD(49, GHW - 30.5f, GHH + i * 5 + 30);
                    ZBIcon.drawToHUD(49, GHW + 30.5f, GHH + i * 5 + 30);
                }
                for (int i = 0; i < 15; i++) {
                    ZBIcon.drawToHUD(48, GHW + i * 3 - 45.5f, GHH - 20);
                    ZBIcon.drawToHUD(48, GHW + i * 3 + 3.5f, GHH - 20);
                }
                for (int i = 0; i < 11; i++) {
                    ZBIcon.drawToHUD(49, GHW + 69.5f, GHH + i * 3 - 36);
                    ZBIcon.drawToHUD(49, GHW - 69.5f, GHH + i * 3 - 36);
                    ZBIcon.drawToHUD(49, GHW + 24.5f, GHH + i * 3 - 36);
                    ZBIcon.drawToHUD(49, GHW - 25.5f, GHH + i * 3 - 36);
                }


                break;
            case 2:
                for (int i = 0; i < 13; i++)
                    ZBIcon.drawToHUD(49, GHW - 0.5f, GHH + i * 5 - 36);
                for (int i = 0; i < 24; i++) {
                    ZBIcon.drawToHUD(48, GHW + i * 4, GHH);
                    ZBIcon.drawToHUD(48, GHW + i * -4, GHH);
                }
                for (int i = 0; i < 16; i++) {
                    ZBIcon.drawToHUD(49, GHW - 50.5f, GHH + i * 5 - 36);
                    ZBIcon.drawToHUD(49, GHW + 49.5f, GHH + i * 5 - 36);
                }
                for (int i = 0; i < 6; i++) {
                    ZBIcon.drawToHUD(49, GHW - 90.5f, GHH + i * 5 - 36);
                    ZBIcon.drawToHUD(49, GHW + 89.5f, GHH + i * 5 - 36);
                }
                for (int i = 0; i < 48; i++)
                    ZBIcon.drawToHUD(48, GHW + i * 4 - 95, GHH - 40);
                for (int i = 0; i < 15; i++) {
                    ZBIcon.drawToHUD(48, GHW + i * 3 - 45.5f, GHH - 20);
                    ZBIcon.drawToHUD(48, GHW + i * 3 + 3.5f, GHH - 20);
                }
                for (int i = 0; i < 11; i++) {
                    ZBIcon.drawToHUD(49, GHW + 24.5f, GHH + i * 3 - 36);
                    ZBIcon.drawToHUD(49, GHW - 25.5f, GHH + i * 3 - 36);
                }
                break;
            case 3:
                break;
        }
    }
    public void DrawTextStuff() {
        string upgrades = xGame switch {
            1 => "Z-Buster",
            2 => "Z-Saber",
            3 => "Armor",
            _ => "ERROR"
        };
        Fonts.drawText(
            FontType.Pink, string.Format($"Upgrade {upgrades}", xGame),
            Global.screenW * 0.5f, 20, Alignment.Center
        );
        if (Global.frameCount % 60 < 30) {
            bool stEnabled = !Global.level.server.disableHtSt;
            string leftText = xGame switch {
                1 when stEnabled => "Items",
                2 => "Z-Buster",
                3 => "Saber",
                _ => ""
            };
            string rightText = xGame switch {
                1 => "Saber",
                2 => "Armor",
                3 when stEnabled => "Items",
                _ => ""
            };
            if (leftText != "") {
                Fonts.drawText(
                    FontType.DarkPurple, "<",
                    14, GHH, Alignment.Center
                );
                Fonts.drawText(
                    FontType.DarkPurple, "",//leftText,
                    14, GHH + 15, Alignment.Center
                );
            }
            if (rightText != "") {
                Fonts.drawText(
                    FontType.DarkPurple, ">",
                    Global.screenW - 14, GHH, Alignment.Center
                );
                Fonts.drawText(
                    FontType.DarkPurple, "",//rightText,
                    Global.screenW - 14, GHH + 15, Alignment.Center
                );
            }
        }
        Fonts.drawText(FontType.Purple, "Skill Points: ", 64, 42, Alignment.Center);
        Fonts.drawText(FontType.Purple, MainP.EXPCurrency.ToString(), 110, 42, Alignment.Center);
        Fonts.drawText(FontType.Yellow, "Tree Active", 10, 10);
    }
    public void LowerMenuText() {
        //LowerMenu
        Fonts.drawTextEX(FontType.RedishOrange, "[CMD]: Change Row", 10, 184);
        Fonts.drawTextEX(FontType.RedishOrange, "[WeaponL]/[WeaponR]: Change Menu", 10, 198);
        Fonts.drawTextEX(FontType.Grey,
            "[OK]:Unlock [ALT]:Disable", 263, 198
        );
        Fonts.drawTextEX(FontType.Grey,
            "[MLEFT],[MRIGHT],[MDOWN],[MUP]: Travel", 216, 184
        );
    }

    public void Slot1Fixes() {
        if (ArmorUD == -2) {
            if (Global.input.isPressedMenu(Control.Right)) {
                ArmorLR = 1;
                ArmorUD = -1;
            } else if (Global.input.isPressedMenu(Control.Left)) {
                ArmorLR = -1;
                ArmorUD = -1;
            }
        }
        if (ArmorUD == 2) {
            if (Global.input.isPressedMenu(Control.Right)) {
                ArmorLR = 1;
                ArmorUD = 1;
            } else if (Global.input.isPressedMenu(Control.Left)) {
                ArmorLR = -1;
                ArmorUD = 1;
            }
        }
        if (ArmorUD == -2) {
            if (Global.input.isPressedMenu(Control.Up)) {
                ArmorLR = 0;
                ArmorUD = -2;
            }
        }
        if (ArmorUD == 2) {
            if (Global.input.isPressedMenu(Control.Down)) {
                ArmorLR = 0;
                ArmorUD = 2;
            }
        }
    }
    public void Slot2Fixes() {
        if (HelmetUD == 0) {
            if (Global.input.isPressedMenu(Control.Right)) {
                HelmetLR = 1;
                HelmetUD = -1;
            } else if (Global.input.isPressedMenu(Control.Left)) {
                HelmetLR = -1;
                HelmetUD = -1;
            }
        }
        if (HelmetLR == 1) {
            if (Global.input.isPressedMenu(Control.Down)) {
                HelmetLR = 0;
                HelmetUD = 0;
            } else if (Global.input.isPressedMenu(Control.Up)) {
                HelmetLR = 0;
                HelmetUD = 0;
            }
        } else if (HelmetLR == -1) {
            if (Global.input.isPressedMenu(Control.Down)) {
                HelmetLR = 0;
                HelmetUD = 0;
            } else if (Global.input.isPressedMenu(Control.Up)) {
                HelmetLR = 0;
                HelmetUD = 0;
            }
        }
    }
    public void Slot3Fixes() {
        if (BootsUD == 1) {
            if (Global.input.isPressedMenu(Control.Right)) {
                BootsLR = 1;
                BootsUD = 0;
            } else if (Global.input.isPressedMenu(Control.Left)) {
                BootsLR = -1;
                BootsUD = 0;
            }
        }
    }

    public void BusterHUDRenderB() {
        if (!MainP.BZBuster) ZBIcon.drawToHUD(58, GHW, GHH, 0.75f);
        if (!MainP.BZYammarkOption) ZBIcon.drawToHUD(58, GHW, GHH + 30, 0.75f);
        if (!MainP.BZBuster2) ZBIcon.drawToHUD(58, GHW, GHH - 40, 0.75f);
        if (!MainP.BZTimeStopper) ZBIcon.drawToHUD(58, GHW - 25, GHH - 20, 0.75f);
        if (!MainP.BZLaserShot) ZBIcon.drawToHUD(58, GHW - 50, GHH, 0.75f);
        if (!MainP.BZZLighting) ZBIcon.drawToHUD(58, GHW - 70, GHH - 20, 0.75f);
        if (!MainP.BZIceJavelin) ZBIcon.drawToHUD(58, GHW - 90, GHH, 0.75f);
        if (!MainP.BZSparkShot) ZBIcon.drawToHUD(58, GHW - 50, GHH - 40, 0.75f);
        if (!MainP.BZBlizzardArrow) ZBIcon.drawToHUD(58, GHW - 90, GHH - 40, 0.75f);
        if (!MainP.BZParasiteBomb) ZBIcon.drawToHUD(58, GHW + 30, GHH + 50, 0.75f);
        if (!MainP.BZZDrones) ZBIcon.drawToHUD(58, GHW - 30, GHH + 50, 0.75f);
        if (!MainP.BZTractorShot) ZBIcon.drawToHUD(58, GHW + 25, GHH - 20, 0.75f);
        if (!MainP.BZVulcan) ZBIcon.drawToHUD(58, GHW + 50, GHH, 0.75f);
        if (!MainP.BZTriThunder) ZBIcon.drawToHUD(58, GHW + 70, GHH - 20, 0.75f);
        if (!MainP.BZReflectLaser) ZBIcon.drawToHUD(58, GHW + 90, GHH, 0.75f);
        if (!MainP.BZBlastShot) ZBIcon.drawToHUD(58, GHW + 50, GHH - 40, 0.75f);
        if (!MainP.BZVShot) ZBIcon.drawToHUD(58, GHW + 90, GHH - 40, 0.75f);
        if (!MainP.BZBurningShot) ZBIcon.drawToHUD(58, GHW + 130, GHH, 0.75f);
        if (!MainP.BZTripleShot) ZBIcon.drawToHUD(58, GHW - 130, GHH, 0.75f);
    }
    public void ZSaberHUDRenderB() {
        if (!MainP.BZZSaber) ZBIcon.drawToHUD(58, GHW, GHH, 0.75f);
        if (!MainP.BZShieldBoomerang) ZBIcon.drawToHUD(58, GHW, GHH + 30, 0.75f);
        if (!MainP.BZZSaberExtend) ZBIcon.drawToHUD(58, GHW, GHH - 40, 0.75f);
        if (!MainP.BZSaberSlam) ZBIcon.drawToHUD(58, GHW - 50, GHH + 40, 0.75f);
        if (!MainP.BZDrillCrush) ZBIcon.drawToHUD(58, GHW - 50, GHH, 0.75f);
        if (!MainP.BZDash) ZBIcon.drawToHUD(58, GHW - 25, GHH - 20, 0.75f);
        ZBIcon.drawToHUD(58, GHW - 90, GHH, 0.75f);
        if (!MainP.BZDrillCrushSpark) ZBIcon.drawToHUD(58, GHW - 50, GHH - 40, 0.75f);
        if (!MainP.BZFirewave) ZBIcon.drawToHUD(58, GHW + 90, GHH - 40, 0.75f);
        if (!MainP.BZFrostShield) ZBIcon.drawToHUD(58, GHW - 90, GHH - 40, 0.75f);
        if (!MainP.BZDash2) ZBIcon.drawToHUD(58, GHW + 25, GHH - 20, 0.75f);
        if (!MainP.BZTenshouzan) ZBIcon.drawToHUD(58, GHW + 50, GHH, 0.75f);
        if (!MainP.BZFishFang) ZBIcon.drawToHUD(58, GHW + 50, GHH + 40, 0.75f);
        if (!MainP.BZBubblesplash) ZBIcon.drawToHUD(58, GHW + 50, GHH - 40, 0.75f);
        ZBIcon.drawToHUD(58, GHW + 90, GHH, 0.75f);
    }
    public void ArmorHUDRenderSlot3B() {
        if (!MainP.BootsZ) ZBIcon.drawToHUD(58, GHW - 100, GHH, 0.75f);
        if (!MainP.BootsFrog) ZBIcon.drawToHUD(58, GHW - 100, GHH + 32, 0.75f);
        if (!MainP.BootsDash) ZBIcon.drawToHUD(58, GHW - 100, GHH - 32, 0.75f);
        if (!MainP.BootsHighJump) ZBIcon.drawToHUD(58, GHW - 70, GHH - 32, 0.75f);
        if (!MainP.BootsSpark) ZBIcon.drawToHUD(58, GHW - 130, GHH - 32, 0.75f);
        if (!MainP.BootsJump) ZBIcon.drawToHUD(58, GHW - 70, GHH, 0.75f);
        if (!MainP.BootsFastRun) ZBIcon.drawToHUD(58, GHW - 130, GHH, 0.75f);
    }
    public void ArmorHUDRenderSlot2B() {
        if (!MainP.HelmetZ) ZBIcon.drawToHUD(58, GHW + 100, GHH, 0.75f);
        if (!MainP.HelmetAutoRecover) ZBIcon.drawToHUD(58, GHW + 100, GHH - 32, 0.75f);
        if (!MainP.HelmetQuickCharge) ZBIcon.drawToHUD(58, GHW + 70, GHH - 32, 0.75f);
        if (!MainP.HelmetAutoCharge) ZBIcon.drawToHUD(58, GHW + 130, GHH - 32, 0.75f);
    }
    public void ArmorHUDRenderSlot1B() {
        if (!MainP.ArmorZ) ZBIcon.drawToHUD(58, GHW, GHH, 0.75f);
        if (!MainP.ArmorModeX) ZBIcon.drawToHUD(58, GHW + 30, GHH, 0.75f); //X 
        if (!MainP.ArmorModeEnergy) ZBIcon.drawToHUD(58, GHW + 30, GHH + 26, 0.75f); //Energy
        if (!MainP.ArmorModeErase) ZBIcon.drawToHUD(58, GHW + 30, GHH - 26, 0.75f); //Erase
        if (!MainP.ArmorModeDefense) ZBIcon.drawToHUD(58, GHW - 30, GHH - 26, 0.75f); //Defense
        if (!MainP.ArmorModePower) ZBIcon.drawToHUD(58, GHW - 30, GHH + 26, 0.75f); //Power
        if (!MainP.ArmorModeUltimate) ZBIcon.drawToHUD(58, GHW, GHH + 50, 0.75f); //Ultimate
        if (!MainP.ArmorModeRise) ZBIcon.drawToHUD(58, GHW - 30, GHH, 0.75f); //Rise
        if (!MainP.ArmorModeProto) ZBIcon.drawToHUD(58, GHW, GHH - 26, 0.75f); //Proto
        if (!MainP.ArmorModeActive) ZBIcon.drawToHUD(58, GHW, GHH + 26, 0.75f); //Active
        if (!MainP.ArmorZUpgrade) ZBIcon.drawToHUD(58, GHW, GHH - 50, 0.75f); // Armor
    }
}
/*

-1 0 1 (-1)
-1 0 1 (0)
-1 0 1 (1)

*/