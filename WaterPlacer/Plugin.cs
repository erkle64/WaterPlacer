using C3.ModKit;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unfoundry;
using UnityEngine;

namespace WaterPlacer
{
    [UnfoundryMod(GUID)]
    public class Plugin : UnfoundryPlugin
    {
        public const string
            MODNAME = "WaterPlacer",
            AUTHOR = "erkle64",
            GUID = AUTHOR + "." + MODNAME,
            VERSION = "0.2.0";

        public static LogSource log;

        public static TypedConfigEntry<KeyCode> configOpenWaterPlacerKey;
        public static TypedConfigEntry<bool> configPlaySounds;

        public WaterPlacerCHM _waterPlacerCHM = null;
        private int _waterPlacerModeIndex;

        public Plugin()
        {
            log = new LogSource(MODNAME);


            new Config(GUID)
                .Group("General")
                    .Entry(out configPlaySounds, "playSounds", true, "Play sounds on water placement.")
                .EndGroup()
                .Group("Input",
                    "Key Codes: Backspace, Tab, Clear, Return, Pause, Escape, Space, Exclaim,",
                    "DoubleQuote, Hash, Dollar, Percent, Ampersand, Quote, LeftParen, RightParen,",
                    "Asterisk, Plus, Comma, Minus, Period, Slash,",
                    "Alpha0, Alpha1, Alpha2, Alpha3, Alpha4, Alpha5, Alpha6, Alpha7, Alpha8, Alpha9,",
                    "Colon, Semicolon, Less, Equals, Greater, Question, At,",
                    "LeftBracket, Backslash, RightBracket, Caret, Underscore, BackQuote,",
                    "A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,",
                    "LeftCurlyBracket, Pipe, RightCurlyBracket, Tilde, Delete,",
                    "Keypad0, Keypad1, Keypad2, Keypad3, Keypad4, Keypad5, Keypad6, Keypad7, Keypad8, Keypad9,",
                    "KeypadPeriod, KeypadDivide, KeypadMultiply, KeypadMinus, KeypadPlus, KeypadEnter, KeypadEquals,",
                    "UpArrow, DownArrow, RightArrow, LeftArrow, Insert, Home, End, PageUp, PageDown,",
                    "F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15,",
                    "Numlock, CapsLock, ScrollLock,",
                    "RightShift, LeftShift, RightControl, LeftControl, RightAlt, LeftAlt, RightApple, RightApple,",
                    "LeftCommand, LeftCommand, LeftWindows, RightWindows, AltGr,",
                    "Help, Print, SysReq, Break, Menu,",
                    "Mouse0, Mouse1, Mouse2, Mouse3, Mouse4, Mouse5, Mouse6")
                    .Entry(out configOpenWaterPlacerKey, "openWaterPlacerKey", KeyCode.LeftBracket, "Keyboard shortcut for open the water placer tool.")
                .EndGroup()
                .Load()
                .Save();
        }

        public override void Load(Mod mod)
        {
            log.Log($"Loading {MODNAME}");

            var assets = typeof(Mod).GetField("assets", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(mod) as Dictionary<string, UnityEngine.Object>;
            _waterPlacerCHM = new WaterPlacerCHM(assets);
            _waterPlacerModeIndex = CustomHandheldModeManager.RegisterMode(_waterPlacerCHM);

            CommonEvents.OnUpdate += Update;
        }

        private void Update()
        {
            var clientCharacter = GameRoot.getClientCharacter();
            if (clientCharacter == null) return;

            if (Input.GetKeyDown(configOpenWaterPlacerKey.Get()) && InputHelpers.IsKeyboardInputAllowed)
            {
                CustomHandheldModeManager.ToggleMode(clientCharacter, _waterPlacerModeIndex);
            }
        }
    }
}


