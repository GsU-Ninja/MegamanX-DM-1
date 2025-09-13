namespace MMXOnline;
using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Audio;
using SFML.Graphics;

/*
public class BusterZeroArmorMenu : IMainMenu {
    public int ArmorLR, ArmorUD, SaberLR, SaberUD, BusterLR, BusterUD;
    public IMainMenu prevMenu;
    public static int xGame = 1;
    public static int Slot = 1;
    public uint GHW = Global.halfScreenW, GHH = Global.halfScreenH;
    public AnimData ZBIcon, ZBCursor, ZBMenu;
    public Level level { get { return Global.level; } }
    public Player MainP { get { return Global.level.mainPlayer; } }

    public BusterZeroArmorMenu(IMainMenu prevMenu) {
        this.prevMenu = prevMenu;
        ZBIcon = Global.sprites["hud_zb_icon"];
        ZBCursor = Global.sprites["axl_cursor"];
        ZBMenu = Global.sprites["menu_x3zero"];
    }

    public void update() {
        MenuChangeLogic();
        SlotLeftRightLogic();
    }
    public void render() {
        DrawWrappers.DrawTextureHUD(Global.textures["pausemenu"], 0, 0);
        MenuBGZeroRender();
        RectRender();
        DrawTextStuff();
        TubeRender();
        HUDIconsRender();
        ArmorCursorRender();
        SaberCursorRender();
        BusterCursorRender();
        LowerMenuText();
        Fonts.drawTextEX(FontType.Blue, "Slot: " + Slot.ToString(), GHW + 126, GHH + 65);
    }
    public void SoundPlay(string sound) {
        Global.playSound(sound);
    }
    public void ZSaberBuyMenuSlot1() {
        if (MainP.EXPCurrency >= 1) {
            if (Global.input.isPressedMenu(Control.MenuConfirm)) {
                foreach (var s in MainP.SkillsS) {

                }

                switch (SaberUD) {
                    case 0:
                        switch (SaberLR) {
                            case 0 when !MainP.BZZSaber:
                                MainP.BZZSaber = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case 1 when !MainP.BZTenshouzan:
                                MainP.BZTenshouzan = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case -1 when !MainP.BZDrillCrush:
                                MainP.BZDrillCrush = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        }
                        break;
                    case 1:
                        switch (SaberLR) {
                            case 0 when !MainP.BZShieldBoomerang:
                                MainP.BZShieldBoomerang = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case 1 when !MainP.BZFishFang:
                                MainP.BZFishFang = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case -1 when !MainP.BZSaberSlam:
                                MainP.BZSaberSlam = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        }
                        break;
                    case -1:
                        switch (SaberLR) {
                            case 0 when !MainP.BZZSaberExtend:
                                MainP.BZZSaberExtend = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case 1 when MainP.BZBubblesplash:
                                MainP.BZBubblesplash = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case 2 when !MainP.BZFrostShield:
                                MainP.BZFrostShield = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case -1 when !MainP.BZDrillCrushSpark:
                                MainP.BZDrillCrushSpark = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case -2 when !MainP.BZFirewave:
                                MainP.BZFirewave = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        }
                        break;
                }
            }
            if (Global.input.isPressedMenu(Control.MenuAlt)) {
                switch (SaberUD) {
                    case 0:
                        switch (SaberLR) {
                            case 0 when MainP.BZZSaber:
                                MainP.BZZSaber = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                            case 1 when MainP.BZTenshouzan:
                                MainP.BZTenshouzan = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                            case -1 when MainP.BZDrillCrush:
                                MainP.BZDrillCrush = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        }
                        break;
                    case 1:
                        switch (SaberLR) {
                            case 0 when MainP.BZShieldBoomerang:
                                MainP.BZShieldBoomerang = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                            case 1 when MainP.BZFishFang:
                                MainP.BZFishFang = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                            case -1 when MainP.BZSaberSlam:
                                MainP.BZSaberSlam = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        }
                        break;
                    case -1:
                        switch (SaberLR) {
                            case 0 when MainP.BZZSaberExtend:
                                MainP.BZZSaberExtend = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                            case 1 when MainP.BZBubblesplash:
                                MainP.BZBubblesplash = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                            case 2 when MainP.BZFrostShield:
                                MainP.BZFrostShield = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                            case -1 when MainP.BZDrillCrushSpark:
                                MainP.BZDrillCrushSpark = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                            case -2 when MainP.BZFirewave:
                                MainP.BZFirewave = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        }
                        break;
                }
            }
        }
    }
    public void ZSaberBuyMenuSlot2() {
        if (MainP.EXPCurrency >= 1) {
            if (Global.input.isPressedMenu(Control.MenuConfirm)) {
                switch (SaberLR) {
                    case 0 when !MainP.BZDash:
                        MainP.BZDash = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                    case 1 when !MainP.BZDash2:
                        MainP.BZDash2 = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                }
            }
            if (Global.input.isPressedMenu(Control.MenuAlt)) {
                switch (SaberLR) {
                    case 0 when MainP.BZDash:
                        MainP.BZDash = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                    case 1 when MainP.BZDash2:
                        MainP.BZDash2 = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                }
            }
        }
    }
    public void BusterBuyMenuSlot1() {
        if (Global.input.isPressedMenu(Control.MenuConfirm)) {
            switch (BusterUD) {
                case 0:
                    switch (BusterLR) {
                        case 0 when !MainP.BZBuster:
                            MainP.BZBuster = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case 1 when !MainP.BZVulcan:
                            MainP.BZVulcan = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case 2 when !MainP.BZTripleShot:
                            MainP.BZTripleShot = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case 3 when !MainP.BZTractorShot:
                            MainP.BZTractorShot = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case -1 when !MainP.BZReflectLaser:
                            MainP.BZReflectLaser = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case -2 when !MainP.BZIceJavelin:
                            MainP.BZIceJavelin = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case -3 when !MainP.BZLaserShot:
                            MainP.BZLaserShot = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                    }
                    break;
                case 1:
                    switch (BusterLR) {
                        case 0 when !MainP.BZYammarkOption:
                            MainP.BZYammarkOption = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case 1 when !MainP.BZParasiteBomb:
                            MainP.BZParasiteBomb = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case 2 when !MainP.BZTriThunder:
                            MainP.BZTriThunder = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case -1 when !MainP.BZZDrones:
                            MainP.BZZDrones = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case -2 when !MainP.BZZLighting:
                            MainP.BZZLighting = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                    }
                    break;
                case -1:
                    switch (BusterLR) {
                        case 0 when !MainP.BZBuster2:
                            MainP.BZBuster2 = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case 1 when !MainP.BZBlastShot:
                            MainP.BZBlastShot = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case 2 when !MainP.BZTimeStopper:
                            MainP.BZTimeStopper = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case -1 when !MainP.BZSparkShot:
                            MainP.BZSparkShot = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        case -2 when !MainP.BZBlizzardArrow:
                            MainP.BZBlizzardArrow = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                    }
                    break;
            }
        }
        if (Global.input.isPressedMenu(Control.MenuAlt)) {
            switch (BusterUD) {
                case 0:
                    switch (BusterLR) {
                        case 0 when MainP.BZBuster:
                            MainP.BZBuster = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case 1 when MainP.BZVulcan:
                            MainP.BZVulcan = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case 2 when MainP.BZTripleShot:
                            MainP.BZTripleShot = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case 3 when MainP.BZTractorShot:
                            MainP.BZTractorShot = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case -1 when MainP.BZReflectLaser:
                            MainP.BZReflectLaser = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case -2 when MainP.BZIceJavelin:
                            MainP.BZIceJavelin = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case -3 when MainP.BZLaserShot:
                            MainP.BZLaserShot = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                    }
                    break;
                case 1:
                    switch (BusterLR) {
                        case 0 when MainP.BZYammarkOption:
                            MainP.BZYammarkOption = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case 1 when MainP.BZParasiteBomb:
                            MainP.BZParasiteBomb = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case 2 when MainP.BZTriThunder:
                            MainP.BZTriThunder = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case -1 when MainP.BZZDrones:
                            MainP.BZZDrones = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case -2 when MainP.BZZLighting:
                            MainP.BZZLighting = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                    }
                    break;
                case -1:
                    switch (BusterLR) {
                        case 0 when MainP.BZBuster2:
                            MainP.BZBuster2 = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case 1 when MainP.BZBlastShot:
                            MainP.BZBlastShot = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case 2 when MainP.BZTimeStopper:
                            MainP.BZTimeStopper = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case -1 when MainP.BZSparkShot:
                            MainP.BZSparkShot = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        case -2 when MainP.BZBlizzardArrow:
                            MainP.BZBlizzardArrow = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                    }
                    break;
            }
        }

    }
    public void BusterBuyMenuSlot2() {
        if (MainP.EXPCurrency >= 1) {
            if (Global.input.isPressedMenu(Control.MenuConfirm)) {
                switch (BusterLR) {
                    case 0 when !MainP.BZVShot:
                        MainP.BZVShot = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                    case 1 when !MainP.BZBurningShot:
                        MainP.BZBurningShot = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                }
            }
            if (Global.input.isPressedMenu(Control.MenuAlt)) {
                switch (BusterLR) {
                    case 0 when MainP.BZVShot:
                        MainP.BZVShot = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                    case 1 when MainP.BZBurningShot:
                        MainP.BZBurningShot = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                }
            }
        }
    }
    public void AllArmorModeBought() {
        if (MainP.ArmorModeActive && MainP.ArmorModeProto &&
              MainP.ArmorModeRise && MainP.ArmorModeX && MainP.ArmorModeDefense &&
             MainP.ArmorModeErase && MainP.ArmorModePower && MainP.ArmorModeEnergy) {
            MainP.ArmorModeUltimate = true;
            SoundPlay("chingX4");
        }
    }
    public void AllArmorModeBoughtB() {
        if (!MainP.ArmorModeActive || !MainP.ArmorModeProto! ||
              !MainP.ArmorModeRise || !MainP.ArmorModeX! || !MainP.ArmorModeDefense ||
             !MainP.ArmorModeErase || !MainP.ArmorModePower! || !MainP.ArmorModeEnergy) {
            MainP.ArmorModeUltimate = false;
        }
        if (!MainP.ArmorModeX || !MainP.ArmorModeActive) MainP.ArmorModeEnergy = false;
        if (!MainP.ArmorModeX || !MainP.ArmorModeProto) MainP.ArmorModeErase = false;
        if (!MainP.ArmorModeProto || !MainP.ArmorModeRise) MainP.ArmorModeDefense = false;
        if (!MainP.ArmorModeProto || !MainP.ArmorModeRise) MainP.ArmorModeDefense = false;
        if (!MainP.ArmorModeActive || !MainP.ArmorModeRise) MainP.ArmorModePower = false;

    }
    public void ArmorMenuBuySlot1() {
        if (MainP.EXPCurrency >= 1) {
            if (Global.input.isPressedMenu(Control.MenuConfirm)) {
                switch (ArmorUD) {
                    case 0:
                        switch (ArmorLR) {
                            case 0 when !MainP.ArmorZ:
                                MainP.ArmorZ = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case 1 when !MainP.ArmorModeX && MainP.ArmorZ:
                                MainP.ArmorModeX = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case -1 when !MainP.ArmorModeRise && MainP.ArmorZ:
                                MainP.ArmorModeRise = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        }
                        break;
                    case 1:
                        switch (ArmorLR) {
                            case 0 when !MainP.ArmorModeActive && MainP.ArmorZ:
                                MainP.ArmorModeActive = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case 1 when !MainP.ArmorModeEnergy && MainP.ArmorZ && MainP.ArmorModeActive && MainP.ArmorModeX:
                                MainP.ArmorModeEnergy = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case -1 when !MainP.ArmorModePower && MainP.ArmorZ && MainP.ArmorModeActive && MainP.ArmorModeRise:
                                MainP.ArmorModePower = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        }
                        break;
                    case -1:
                        switch (ArmorLR) {
                            case 0 when !MainP.ArmorModeProto && MainP.ArmorZ:
                                MainP.ArmorModeProto = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case 1 when !MainP.ArmorModeErase && MainP.ArmorZ && MainP.ArmorModeProto && MainP.ArmorModeX:
                                MainP.ArmorModeErase = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                            case -1 when !MainP.ArmorModeDefense && MainP.ArmorZ && MainP.ArmorModeProto && MainP.ArmorModeRise:
                                MainP.ArmorModeDefense = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        }
                        break;
                    case -2:
                        switch (ArmorLR) {
                            case 0 when !MainP.ArmorZUpgrade && MainP.ArmorModeProto:
                                MainP.ArmorZUpgrade = true; MainP.EXPCurrency -= 1; SoundPlay("ching"); break;
                        }
                        break;
                }
                AllArmorModeBought();
            }
            if (Global.input.isPressedMenu(Control.MenuAlt)) {
                switch (ArmorUD) {
                    case 0:
                        switch (ArmorLR) {
                            case 0 when MainP.ArmorZ && !(MainP.ArmorZUpgrade || MainP.ArmorModeX || MainP.ArmorModeActive || MainP.ArmorModeErase):
                                MainP.ArmorZ = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                            case 1 when MainP.ArmorModeX && MainP.ArmorZ:
                                MainP.ArmorModeX = false; MainP.EXPCurrency += 1; SoundPlay("busterX3");
                                if (MainP.ArmorModeEnergy || MainP.ArmorModeErase) MainP.EXPCurrency += 1;
                                if (MainP.ArmorModeEnergy && MainP.ArmorModeErase) MainP.EXPCurrency += 1;
                                break;
                            case -1 when MainP.ArmorModeRise && MainP.ArmorZ:
                                MainP.ArmorModeRise = false; MainP.EXPCurrency += 1; SoundPlay("busterX3");
                                if (MainP.ArmorModePower || MainP.ArmorModeDefense) MainP.EXPCurrency += 1;
                                if (MainP.ArmorModePower && MainP.ArmorModeDefense) MainP.EXPCurrency += 1;
                                break;
                        }
                        break;
                    case 1:
                        switch (ArmorLR) {
                            case 0 when MainP.ArmorModeActive && MainP.ArmorZ:
                                MainP.ArmorModeActive = false; MainP.EXPCurrency += 1; SoundPlay("busterX3");
                                if (MainP.ArmorModeEnergy || MainP.ArmorModePower) MainP.EXPCurrency += 1;
                                if (MainP.ArmorModeEnergy && MainP.ArmorModePower) MainP.EXPCurrency += 1;
                                break;
                            case 1 when MainP.ArmorModeEnergy:
                                MainP.ArmorModeEnergy = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                            case -1 when MainP.ArmorModePower && MainP.ArmorZ && MainP.ArmorModeActive && MainP.ArmorModeRise:
                                MainP.ArmorModePower = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        }
                        break;
                    case -1:
                        switch (ArmorLR) {
                            case 0 when MainP.ArmorModeProto && MainP.ArmorZ:
                                MainP.ArmorModeProto = false; MainP.EXPCurrency += 1; SoundPlay("busterX3");
                                if (MainP.ArmorModeErase || MainP.ArmorModeDefense) MainP.EXPCurrency += 1;
                                if (MainP.ArmorModeErase && MainP.ArmorModeDefense) MainP.EXPCurrency += 1;
                                break;
                            case 1 when MainP.ArmorModeErase && MainP.ArmorZ && MainP.ArmorModeProto && MainP.ArmorModeX:
                                MainP.ArmorModeErase = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                            case -1 when MainP.ArmorModeDefense && MainP.ArmorZ && MainP.ArmorModeProto && MainP.ArmorModeRise:
                                MainP.ArmorModeDefense = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        }
                        break;
                    case -2:
                        switch (ArmorLR) {
                            case 0 when MainP.ArmorZUpgrade:
                                MainP.ArmorZUpgrade = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); break;
                        }
                        break;
                }
                AllArmorModeBoughtB();
            }
        }
    }
    public void MenuChangeLogic() {
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
    public void SlotLeftRightLogic() {
        if (xGame == 1) {
            if (Global.input.isPressedMenu(Control.Special2)) {
                Slot++;
                SoundPlay("menuX3");
                if (Slot > 2) {
                    Slot = 1;
                }
                BusterLR = 0;
                BusterUD = 0;
            }
            if (Slot == 1) {
                Helpers.menuLeftRightInc(ref BusterLR, -3, 3, true, true);
                Helpers.menuUpDown(ref BusterUD, -1, 1);
                BusterBuyMenuSlot1();
            }
            if (Slot == 2) {
                Helpers.menuLeftRightInc(ref BusterLR, -0, 1, true, true);
                BusterBuyMenuSlot2();
            }
        }
        if (xGame == 2) {
            if (Global.input.isPressedMenu(Control.Special2)) {
                Slot++;
                SoundPlay("menuX3");
                if (Slot > 2) {
                    Slot = 1;
                }
                SaberLR = 0;
                SaberUD = 0;
            }
            if (Slot == 1) {
                Helpers.menuLeftRightInc(ref SaberLR, -2, 2, true, true);
                Helpers.menuUpDown(ref SaberUD, -1, 1);
                ZSaberBuyMenuSlot1();
            }
            if (Slot == 2) {
                Helpers.menuLeftRightInc(ref SaberLR, -0, 1, true, true);
                ZSaberBuyMenuSlot2();
            }
        }
        if (xGame == 3) {
            if (Global.input.isPressedMenu(Control.Special2)) {
                Slot++;
                SoundPlay("menuX3");
                if (Slot > 3) {
                    Slot = 1;
                }
                ArmorLR = 0;
                ArmorUD = 0;
            }
            switch (Slot) {
                case 1:
                    Helpers.menuLeftRightInc(ref ArmorLR, -1, 1, wrap: true, true);
                    Helpers.menuUpDown(ref ArmorUD, -2, 2);
                    Slot1Fixes();
                    ArmorMenuBuySlot1();
                    break;
                case 2:
                    Helpers.menuLeftRightInc(ref ArmorLR, -1, 1, wrap: true, true);
                    Helpers.menuUpDown(ref ArmorUD, -1, 0);
                    Slot2Fixes();
                    ArmorMenuBuySlot2();
                    break;
                case 3:
                    Helpers.menuLeftRightInc(ref ArmorLR, -1, 1, wrap: true, true);
                    Helpers.menuUpDown(ref ArmorUD, -1, 1);
                    Slot3Fixes();
                    ArmorMenuBuySlot3();
                    break;
            }
        }
    }
    public void ArmorMenuBuySlot2() {
        if (MainP.EXPCurrency >= 1) {
            if (Global.input.isPressedMenu(Control.MenuConfirm)) {
                if (!MainP.HelmetZ && ArmorUD == 0 && ArmorLR == 0) {
                    MainP.HelmetZ = true; MainP.EXPCurrency -= 1; SoundPlay("ching");
                }
                if (!MainP.HelmetAutoRecover && ArmorUD == -1 && ArmorLR == 0 && MainP.HelmetZ) {
                    MainP.HelmetAutoRecover = true; MainP.EXPCurrency -= 1; SoundPlay("ching");
                }
                if (!MainP.HelmetAutoCharge && ArmorUD == -1 && ArmorLR == 1 && MainP.HelmetZ) {
                    MainP.HelmetAutoCharge = true; MainP.EXPCurrency -= 1; SoundPlay("ching");
                }
                if (!MainP.HelmetQuickCharge && ArmorUD == -1 && ArmorLR == -1 && MainP.HelmetZ) {
                    MainP.HelmetQuickCharge = true; MainP.EXPCurrency -= 1; SoundPlay("ching");
                }
            }
            if (Global.input.isPressedMenu(Control.MenuAlt)) {
                if (MainP.HelmetZ && ArmorLR == 0 && ArmorUD == 0 &&
                    !(MainP.HelmetAutoRecover || MainP.HelmetAutoCharge
                    || MainP.HelmetQuickCharge)) {
                    MainP.HelmetZ = false;
                    MainP.EXPCurrency += 1;
                    SoundPlay("BusterX3");
                }
                if (MainP.HelmetAutoRecover && ArmorUD == -1 && ArmorLR == 0 && MainP.HelmetZ) {
                    MainP.HelmetAutoRecover = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); ;
                }
                if (MainP.HelmetAutoCharge && ArmorUD == -1 && ArmorLR == 1 && MainP.HelmetZ) {
                    MainP.HelmetAutoCharge = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); ;
                }
                if (MainP.HelmetQuickCharge && ArmorUD == -1 && ArmorLR == -1 && MainP.HelmetZ) {
                    MainP.HelmetQuickCharge = false; MainP.EXPCurrency += 1; SoundPlay("busterX3"); ;
                }
            }
        }
    }
    public void ArmorMenuBuySlot3() {
        if (MainP.EXPCurrency >= 1) {
            if (Global.input.isPressedMenu(Control.MenuConfirm)) {
                if (!MainP.BootsZ && ArmorUD == 0 && ArmorLR == 0) {
                    MainP.BootsZ = true; MainP.EXPCurrency -= 1; SoundPlay("ching");
                }
                if (!MainP.BootsFrog && ArmorUD == 1 && ArmorLR == 0 && MainP.BootsZ) {
                    MainP.BootsFrog = true; MainP.EXPCurrency -= 1; SoundPlay("ching");
                }
                if (!MainP.BootsDash && ArmorUD == -1 && ArmorLR == 0 && MainP.BootsZ) {
                    MainP.BootsDash = true; MainP.EXPCurrency -= 1; SoundPlay("ching");
                }
                if (!MainP.BootsFastRun && ArmorLR == -1 && ArmorUD == 0 && MainP.BootsZ) {
                    MainP.BootsFastRun = true; MainP.EXPCurrency -= 1; SoundPlay("ching");
                }
                if (!MainP.BootsJump && ArmorLR == 1 && ArmorUD == 0 && MainP.BootsZ) {
                    MainP.BootsJump = true; MainP.EXPCurrency -= 1; SoundPlay("ching");
                }
                if (!MainP.BootsSpark && ArmorLR == -1 && ArmorUD == -1 &&
                    MainP.BootsDash && MainP.BootsFastRun) {
                    MainP.BootsSpark = true; MainP.EXPCurrency -= 1; SoundPlay("ching");
                }
                if (!MainP.BootsHighJump && ArmorLR == 1 && ArmorUD == -1 &&
                    MainP.BootsDash && MainP.BootsJump) {
                    MainP.BootsHighJump = true; MainP.EXPCurrency -= 1; SoundPlay("ching");
                }
            }
            if (Global.input.isPressedMenu(Control.MenuAlt)) {
                if (MainP.BootsZ && ArmorUD == 0 && ArmorLR == 0 &&
                    !(MainP.BootsFrog || MainP.BootsDash
                    || MainP.BootsFastRun || MainP.BootsJump
                    || MainP.BootsSpark || MainP.BootsHighJump)) {
                    MainP.BootsZ = false; MainP.EXPCurrency += 1; SoundPlay("busterX3");
                }
                if (MainP.BootsFrog && ArmorUD == 1 && ArmorLR == 0 && MainP.BootsZ) {
                    MainP.BootsFrog = false; MainP.EXPCurrency += 1; SoundPlay("busterX3");
                }
                if (MainP.BootsDash && ArmorUD == -1 && ArmorLR == 0 && MainP.BootsZ) {
                    MainP.BootsDash = false; MainP.EXPCurrency += 1; SoundPlay("busterX3");
                    if (MainP.BootsSpark || MainP.BootsHighJump) MainP.EXPCurrency += 1;
                    if (MainP.BootsSpark && MainP.BootsHighJump) MainP.EXPCurrency += 1;
                }
                if (MainP.BootsFastRun && ArmorLR == -1 && ArmorUD == 0 && MainP.BootsZ) {
                    MainP.BootsFastRun = false; MainP.EXPCurrency += 1; SoundPlay("busterX3");
                    if (MainP.BootsSpark) MainP.EXPCurrency += 1;
                }
                if (MainP.BootsJump && ArmorLR == 1 && ArmorUD == 0 && MainP.BootsZ) {
                    MainP.BootsJump = false; MainP.EXPCurrency += 1; SoundPlay("busterX3");
                    if (MainP.BootsHighJump) MainP.EXPCurrency += 1;
                }
                if (MainP.BootsSpark && ArmorLR == -1 && ArmorUD == -1 &&
                    MainP.BootsDash && MainP.BootsFastRun) {
                    MainP.BootsSpark = false; MainP.EXPCurrency += 1; SoundPlay("busterX3");
                }
                if (MainP.BootsHighJump && ArmorLR == 1 && ArmorUD == -1 &&
                    MainP.BootsDash && MainP.BootsJump) {
                    MainP.BootsHighJump = false; MainP.EXPCurrency += 1; SoundPlay("busterX3");
                }
                BoughtSlot3();
            }
        }
    }
    public void BoughtSlot3() {
        if (!MainP.BootsDash || !MainP.BootsFastRun) MainP.BootsSpark = false;
        if (!MainP.BootsDash || !MainP.BootsJump) MainP.BootsHighJump = false;
    }
    public void Slot1Fixes() {
        if (ArmorUD == -2) {
            if (Global.input.isPressedMenu(Control.Right)) {
                ArmorLR = 1;
                ArmorUD = 0;
            } else if (Global.input.isPressedMenu(Control.Left)) {
                ArmorLR = -1;
                ArmorUD = 0;
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
        if (ArmorUD == 0) {
            if (Global.input.isPressedMenu(Control.Right)) {
                ArmorLR = 1;
                ArmorUD = -1;
            } else if (Global.input.isPressedMenu(Control.Left)) {
                ArmorLR = -1;
                ArmorUD = -1;
            }
        }
        if (ArmorLR == 1) {
            if (Global.input.isPressedMenu(Control.Down)) {
                ArmorLR = 0;
                ArmorUD = 0;
            } else if (Global.input.isPressedMenu(Control.Up)) {
                ArmorLR = 0;
                ArmorUD = 0;
            }
        } else if (ArmorLR == -1) {
            if (Global.input.isPressedMenu(Control.Down)) {
                ArmorLR = 0;
                ArmorUD = 0;
            } else if (Global.input.isPressedMenu(Control.Up)) {
                ArmorLR = 0;
                ArmorUD = 0;
            }
        }
    }
    public void Slot3Fixes() {
        if (ArmorUD == 1) {
            if (Global.input.isPressedMenu(Control.Right)) {
                ArmorLR = 1;
                ArmorUD = 0;
            } else if (Global.input.isPressedMenu(Control.Left)) {
                ArmorLR = -1;
                ArmorUD = 0;
            }
        }
    }
    public void SaberCursorRender() {
        if (xGame == 2) {
            if (Slot == 1) {
                switch (SaberUD) {
                    case 0:
                        switch (SaberLR) {
                            case 0: ZBCursor.drawToHUD(0, GHW, GHH); break;
                            case -1: ZBCursor.drawToHUD(0, GHW - 50, GHH); break;
                            case -2: ZBCursor.drawToHUD(0, GHW - 90, GHH); break;
                            case 1: ZBCursor.drawToHUD(0, GHW + 50, GHH); break;
                            case 2: ZBCursor.drawToHUD(0, GHW + 90, GHH); break;
                        }
                        break;

                    case 1:
                        switch (SaberLR) {
                            case 0: ZBCursor.drawToHUD(0, GHW, GHH + 30); break;
                            case -1: ZBCursor.drawToHUD(0, GHW - 50, GHH + 40); break;
                            case -2: ZBCursor.drawToHUD(0, GHW - 90, GHH + 40); break;
                            case 1: ZBCursor.drawToHUD(0, GHW + 50, GHH + 40); break;
                            case 2: ZBCursor.drawToHUD(0, GHW + 90, GHH + 40); break;
                        }
                        break;
                    case -1:
                        switch (SaberLR) {
                            case 0: ZBCursor.drawToHUD(0, GHW, GHH - 40); break;
                            case -1: ZBCursor.drawToHUD(0, GHW - 50, GHH - 40); break;
                            case -2: ZBCursor.drawToHUD(0, GHW - 90, GHH - 40); break;
                            case 1: ZBCursor.drawToHUD(0, GHW + 50, GHH - 40); break;
                            case 2: ZBCursor.drawToHUD(0, GHW + 90, GHH - 40); break;
                        }
                        break;
                }
            }
            if (Slot == 2) {
                switch (SaberLR) {
                    case 0: ZBCursor.drawToHUD(0, GHW - 25, GHH - 20); break;
                    case 1: ZBCursor.drawToHUD(0, GHW + 25, GHH - 20); break;
                }
            }
        }
    }
    public void BusterCursorRender() {
        if (xGame == 1) {
            if (Slot == 1) {
                switch (BusterUD) {
                    case 0:
                        switch (BusterLR) {
                            case 0: ZBCursor.drawToHUD(0, GHW, GHH); break;
                            case -1: ZBCursor.drawToHUD(0, GHW - 50, GHH); break;
                            case -2: ZBCursor.drawToHUD(0, GHW - 90, GHH); break;
                            case -3: ZBCursor.drawToHUD(0, GHW - 130, GHH); break;
                            case 1: ZBCursor.drawToHUD(0, GHW + 50, GHH); break;
                            case 2: ZBCursor.drawToHUD(0, GHW + 90, GHH); break;
                            case 3: ZBCursor.drawToHUD(0, GHW + 130, GHH); break;
                        }
                        break;

                    case 1:
                        switch (BusterLR) {
                            case 0: ZBCursor.drawToHUD(0, GHW, GHH + 30); break;
                            case -1: ZBCursor.drawToHUD(0, GHW - 30, GHH + 50); break;
                            case -2: ZBCursor.drawToHUD(0, GHW - 70, GHH + 40); break;
                            case -3: ZBCursor.drawToHUD(0, GHW - 130, GHH + 40); break;
                            case 1: ZBCursor.drawToHUD(0, GHW + 30, GHH + 50); break;
                            case 2: ZBCursor.drawToHUD(0, GHW + 70, GHH + 40); break;
                            case 3: ZBCursor.drawToHUD(0, GHW + 130, GHH + 40); break;
                        }
                        break;

                    case -1:
                        switch (BusterLR) {
                            case 0: ZBCursor.drawToHUD(0, GHW, GHH - 40); break;
                            case -1: ZBCursor.drawToHUD(0, GHW - 50, GHH - 40); break;
                            case -2: ZBCursor.drawToHUD(0, GHW - 90, GHH - 40); break;
                            case -3: ZBCursor.drawToHUD(0, GHW - 130, GHH - 40); break;
                            case 1: ZBCursor.drawToHUD(0, GHW + 50, GHH - 40); break;
                            case 2: ZBCursor.drawToHUD(0, GHW + 90, GHH - 40); break;
                            case 3: ZBCursor.drawToHUD(0, GHW + 130, GHH - 40); break;
                        }
                        break;
                }
            }
            if (Slot == 2) {
                switch (BusterLR) {
                    case 0: ZBCursor.drawToHUD(0, GHW - 25, GHH - 20); break;
                    case 1: ZBCursor.drawToHUD(0, GHW + 25, GHH - 20); break;
                }
            }
        }

    }
    public void ArmorCursorRender() {
        if (xGame == 3) {
            if (Slot == 1) {
                switch (ArmorUD) {
                    case 0:
                        switch (ArmorLR) {
                            case 0: ZBCursor.drawToHUD(0, GHW, GHH); break;
                            case 1: ZBCursor.drawToHUD(0, GHW + 30, GHH); break;
                            case -1: ZBCursor.drawToHUD(0, GHW - 30, GHH); break;
                        }
                        break;
                    case 1:
                        switch (ArmorLR) {
                            case 0: ZBCursor.drawToHUD(0, GHW, GHH + 25); break;
                            case 1: ZBCursor.drawToHUD(0, GHW + 30, GHH + 25); break;
                            case -1: ZBCursor.drawToHUD(0, GHW - 30, GHH + 25); break;
                        }
                        break;
                    case 2:
                        ZBCursor.drawToHUD(0, GHW, GHH + 50); break;
                    case -1:
                        switch (ArmorLR) {
                            case 0: ZBCursor.drawToHUD(0, GHW, GHH - 33); break;
                            case 1: ZBCursor.drawToHUD(0, GHW + 30, GHH - 33); break;
                            case -1: ZBCursor.drawToHUD(0, GHW - 30, GHH - 33); break;
                        }
                        break;
                    case -2:
                        ZBCursor.drawToHUD(0, GHW, GHH - 56); break;
                }
            }
            if (Slot == 2) {
                switch (ArmorUD) {
                    case 0:
                        ZBCursor.drawToHUD(0, GHW + 100, GHH); break;
                    case -1:
                        switch (ArmorLR) {
                            case 0: ZBCursor.drawToHUD(0, GHW + 100, GHH - 32); break;
                            case 1: ZBCursor.drawToHUD(0, GHW + 130, GHH - 32); break;
                            case -1: ZBCursor.drawToHUD(0, GHW + 70, GHH - 32); break;
                        }
                        break;
                }
            }
            if (Slot == 3) {
                switch (ArmorUD) {
                    case 0:
                        switch (ArmorLR) {
                            case 0:
                                ZBCursor.drawToHUD(0, GHW - 100, GHH); break;
                            case 1:
                                ZBCursor.drawToHUD(0, GHW - 70, GHH); break;
                            case -1:
                                ZBCursor.drawToHUD(0, GHW - 130, GHH); break;
                        }
                        break;
                    case -1:
                        switch (ArmorLR) {
                            case 0:
                                ZBCursor.drawToHUD(0, GHW - 100, GHH - 32); break;
                            case 1:
                                ZBCursor.drawToHUD(0, GHW - 70, GHH - 32); break;
                            case -1:
                                ZBCursor.drawToHUD(0, GHW - 130, GHH - 32); break;
                        }
                        break;
                    case 1:
                        ZBCursor.drawToHUD(0, GHW - 100, GHH + 32);
                        break;
                }
            }
        }
    }
    public void HUDIconsRender() {
        switch (xGame) {
            case 1: //ZBuster
                BusterHUDRenderA();
                BusterHUDRenderB();
                break;
            case 2: // ZSaber
                ZSaberHUDRenderA();
                ZSaberHUDRenderB();
                break;
            case 3: //Armor part
                ArmorHUDRenderSlot1A();
                ArmorHUDRenderSlot2A();
                ArmorHUDRenderSlot3A();
                ArmorHUDRenderSlot1B();
                ArmorHUDRenderSlot2B();
                ArmorHUDRenderSlot3B();
                break;
        }
    }
    public void BusterHUDRenderA() {
        ZBIcon.drawToHUD(61, GHW, GHH);
        ZBIcon.drawToHUD(26, GHW, GHH + 30);
        ZBIcon.drawToHUD(7, GHW, GHH - 40);
        ZBIcon.drawToHUD(6, GHW - 25, GHH - 20);
        ZBIcon.drawToHUD(4, GHW - 50, GHH);
        ZBIcon.drawToHUD(32, GHW - 70, GHH + 40);
        ZBIcon.drawToHUD(12, GHW - 90, GHH);
        ZBIcon.drawToHUD(2, GHW - 50, GHH - 40);
        ZBIcon.drawToHUD(5, GHW - 90, GHH - 40);
        Global.sprites["hud_weapon_icon"].drawToHUD(18, GHW + 30, GHH + 50);
        Global.sprites["hud_weapon_icon"].drawToHUD(6, GHW - 30, GHH + 50);
        ZBIcon.drawToHUD(13, GHW + 25, GHH - 20);
        ZBIcon.drawToHUD(9, GHW + 50, GHH);
        ZBIcon.drawToHUD(37, GHW + 70, GHH + 40);
        ZBIcon.drawToHUD(1, GHW + 90, GHH);
        ZBIcon.drawToHUD(3, GHW + 50, GHH - 40);
        ZBIcon.drawToHUD(10, GHW + 90, GHH - 40);
        ZBIcon.drawToHUD(11, GHW + 130, GHH);
        ZBIcon.drawToHUD(0, GHW - 130, GHH);
    }
    public void BusterHUDRenderB() {
        if (!MainP.BZBuster) ZBIcon.drawToHUD(58, GHW, GHH, 0.75f);
        if (!MainP.BZYammarkOption) ZBIcon.drawToHUD(58, GHW, GHH + 30, 0.75f);
        if (!MainP.BZBuster2) ZBIcon.drawToHUD(58, GHW, GHH - 40, 0.75f);
        if (!MainP.BZVShot) ZBIcon.drawToHUD(58, GHW - 25, GHH - 20, 0.75f);
        if (!MainP.BZReflectLaser) ZBIcon.drawToHUD(58, GHW - 50, GHH, 0.75f);
        if (!MainP.BZZLighting) ZBIcon.drawToHUD(58, GHW - 70, GHH + 40, 0.75f);
        if (!MainP.BZIceJavelin) ZBIcon.drawToHUD(58, GHW - 90, GHH, 0.75f);
        if (!MainP.BZSparkShot) ZBIcon.drawToHUD(58, GHW - 50, GHH - 40, 0.75f);
        if (!MainP.BZBlizzardArrow) ZBIcon.drawToHUD(58, GHW - 90, GHH - 40, 0.75f);
        if (!MainP.BZParasiteBomb) ZBIcon.drawToHUD(58, GHW + 30, GHH + 50, 0.75f);
        if (!MainP.BZZDrones) ZBIcon.drawToHUD(58, GHW - 30, GHH + 50, 0.75f);
        if (!MainP.BZBurningShot) ZBIcon.drawToHUD(58, GHW + 25, GHH - 20, 0.75f);
        if (!MainP.BZVulcan) ZBIcon.drawToHUD(58, GHW + 50, GHH, 0.75f);
        if (!MainP.BZTriThunder) ZBIcon.drawToHUD(58, GHW + 70, GHH + 40, 0.75f);
        if (!MainP.BZTripleShot) ZBIcon.drawToHUD(58, GHW + 90, GHH, 0.75f);
        if (!MainP.BZBlastShot) ZBIcon.drawToHUD(58, GHW + 50, GHH - 40, 0.75f);
        if (!MainP.BZTimeStopper) ZBIcon.drawToHUD(58, GHW + 90, GHH - 40, 0.75f);
        if (!MainP.BZTractorShot) ZBIcon.drawToHUD(58, GHW + 130, GHH, 0.75f);
        if (!MainP.BZLaserShot) ZBIcon.drawToHUD(58, GHW - 130, GHH, 0.75f);
    }
    public void ZSaberHUDRenderA() {
        ZBIcon.drawToHUD(25, GHW, GHH);
        ZBIcon.drawToHUD(33, GHW, GHH + 30);
        ZBIcon.drawToHUD(27, GHW, GHH - 40);
        ZBIcon.drawToHUD(28, GHW - 50, GHH + 40);
        ZBIcon.drawToHUD(31, GHW - 50, GHH);
        Global.sprites["hud_weapon_icon"].drawToHUD(48, GHW - 25, GHH - 20);
        ZBIcon.drawToHUD(60, GHW - 90, GHH);
        Global.sprites["hud_weapon_icon"].drawToHUD(6, GHW - 50, GHH - 40);
        ZBIcon.drawToHUD(36, GHW - 90, GHH - 40);
        ZBIcon.drawToHUD(12, GHW + 25, GHH - 20);
        ZBIcon.drawToHUD(34, GHW + 50, GHH);
        ZBIcon.drawToHUD(30, GHW + 50, GHH + 40);
        ZBIcon.drawToHUD(56, GHW + 50, GHH - 40);
        ZBIcon.drawToHUD(57, GHW + 90, GHH - 40);
        ZBIcon.drawToHUD(60, GHW + 90, GHH);
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
        if (!MainP.BZFirewave) ZBIcon.drawToHUD(58, GHW - 90, GHH - 40, 0.75f);
        if (!MainP.BZDash2) ZBIcon.drawToHUD(58, GHW + 25, GHH - 20, 0.75f);
        if (!MainP.BZTenshouzan) ZBIcon.drawToHUD(58, GHW + 50, GHH, 0.75f);
        if (!MainP.BZFishFang) ZBIcon.drawToHUD(58, GHW + 50, GHH + 40, 0.75f);
        if (!MainP.BZBubblesplash) ZBIcon.drawToHUD(58, GHW + 50, GHH - 40, 0.75f);
        if (!MainP.BZFrostShield) ZBIcon.drawToHUD(58, GHW + 90, GHH - 40, 0.75f);
        ZBIcon.drawToHUD(58, GHW + 90, GHH, 0.75f);
    }
    public void ArmorHUDRenderSlot3A() {
        ZBIcon.drawToHUD(54, GHW - 100, GHH); //botas
        ZBIcon.drawToHUD(20, GHW - 100, GHH + 32);
        ZBIcon.drawToHUD(19, GHW - 100, GHH - 32);
        ZBIcon.drawToHUD(24, GHW - 70, GHH - 32);
        ZBIcon.drawToHUD(22, GHW - 130, GHH - 32);
        ZBIcon.drawToHUD(23, GHW - 70, GHH);
        ZBIcon.drawToHUD(21, GHW - 130, GHH);
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
    public void ArmorHUDRenderSlot2A() {
        ZBIcon.drawToHUD(52, GHW + 100, GHH);
        ZBIcon.drawToHUD(14, GHW + 100, GHH - 32);
        ZBIcon.drawToHUD(15, GHW + 70, GHH - 32);
        ZBIcon.drawToHUD(16, GHW + 130, GHH - 32);
    }
    public void ArmorHUDRenderSlot2B() {
        if (!MainP.HelmetZ) ZBIcon.drawToHUD(58, GHW + 100, GHH, 0.75f);
        if (!MainP.HelmetAutoRecover) ZBIcon.drawToHUD(58, GHW + 100, GHH - 32, 0.75f);
        if (!MainP.HelmetQuickCharge) ZBIcon.drawToHUD(58, GHW + 70, GHH - 32, 0.75f);
        if (!MainP.HelmetAutoCharge) ZBIcon.drawToHUD(58, GHW + 130, GHH - 32, 0.75f);
    }
    public void ArmorHUDRenderSlot1A() {
        ZBIcon.drawToHUD(53, GHW, GHH); //Z-Armor
        ZBIcon.drawToHUD(42, GHW + 30, GHH); //X 
        ZBIcon.drawToHUD(43, GHW + 30, GHH + 26); //Energy
        ZBIcon.drawToHUD(66, GHW + 30, GHH - 32); //Erase
        ZBIcon.drawToHUD(62, GHW - 30, GHH - 32); //Defense
        ZBIcon.drawToHUD(65, GHW - 30, GHH + 26); //Power
        ZBIcon.drawToHUD(67, GHW, GHH + 50); //Ultimate
        ZBIcon.drawToHUD(63, GHW - 30, GHH); //Rise
        ZBIcon.drawToHUD(64, GHW, GHH - 32); //Proto
        ZBIcon.drawToHUD(44, GHW, GHH + 26); //Active
        ZBIcon.drawToHUD(17, GHW, GHH - 55); // Armor
    }
    public void ArmorHUDRenderSlot1B() {
        if (!MainP.ArmorZ) ZBIcon.drawToHUD(58, GHW, GHH, 0.75f);
        if (!MainP.ArmorModeX) ZBIcon.drawToHUD(58, GHW + 30, GHH, 0.75f); //X 
        if (!MainP.ArmorModeEnergy) ZBIcon.drawToHUD(58, GHW + 30, GHH + 26, 0.75f); //Energy
        if (!MainP.ArmorModeErase) ZBIcon.drawToHUD(58, GHW + 30, GHH - 32, 0.75f); //Erase
        if (!MainP.ArmorModeDefense) ZBIcon.drawToHUD(58, GHW - 30, GHH - 32, 0.75f); //Defense
        if (!MainP.ArmorModePower) ZBIcon.drawToHUD(58, GHW - 30, GHH + 26, 0.75f); //Power
        if (!MainP.ArmorModeUltimate) ZBIcon.drawToHUD(58, GHW, GHH + 50, 0.75f); //Ultimate
        if (!MainP.ArmorModeRise) ZBIcon.drawToHUD(58, GHW - 30, GHH, 0.75f); //Rise
        if (!MainP.ArmorModeProto) ZBIcon.drawToHUD(58, GHW, GHH - 32, 0.75f); //Proto
        if (!MainP.ArmorModeActive) ZBIcon.drawToHUD(58, GHW, GHH + 26, 0.75f); //Active
        if (!MainP.ArmorZUpgrade) ZBIcon.drawToHUD(58, GHW, GHH - 55, 0.75f); // Armor
    }
    public void TubeRender() {
        //Tube Section (i expect a mess) (looks like it wont be that mess) (ohgoditis)
        switch (xGame) {
            case 1:
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
                    ZBIcon.drawToHUD(49, GHW + 24.5f, GHH + i * 3 - 36);
                    ZBIcon.drawToHUD(49, GHW - 25.5f, GHH + i * 3 - 36);
                }
                for (int i = 0; i < 10; i++) {
                    ZBIcon.drawToHUD(48, GHW + i * 4 - 68.5f, GHH + 52);
                    ZBIcon.drawToHUD(48, GHW + i * 4 + 30.5f, GHH + 52);
                }
                ZBIcon.drawToHUD(49, GHW + 70, GHH + 51);
                ZBIcon.drawToHUD(49, GHW + 70, GHH + 48);
                ZBIcon.drawToHUD(49, GHW - 70, GHH + 51);
                ZBIcon.drawToHUD(49, GHW - 70, GHH + 48);
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
    public void RectRender() {
        //Rect thing
        switch (xGame) {
            case 1:
            case 2:
            case 3:
                DrawWrappers.DrawRect(364, 40, 20, 180, true, new Color(0, 0, 0, 100), 1,
                ZIndex.HUD, false, outlineColor: Color.White);
                break;
        }
    }
    public void MenuBGZeroRender() {
        if (MainP.realCharNum == (int)CharIds.BusterZero) {
            if (!MainP.isblack)
                switch (xGame) {
                    case 1: case 2: case 3: ZBMenu.drawToHUD(0, GHW, GHH, 0.5f); break;
                } else {
                switch (xGame) {
                    case 1: case 2: case 3: ZBMenu.drawToHUD(1, GHW, GHH, 0.5f); break;
                }
            }
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
        Fonts.drawText(FontType.Purple, "Skill Points: ", 70, 170, Alignment.Center);
        Fonts.drawText(FontType.Purple, MainP.EXPCurrency.ToString(), 116, 170, Alignment.Center);
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
}
*/