﻿// ------------------------------------------------------------------------------
// <copyright file="OSKeyType.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace War3Net.Runtime.Common.Enums
{
    public sealed class OSKeyType
    {
        private static readonly Dictionary<int, OSKeyType> _types = GetTypes().ToDictionary(t => (int)t, t => new OSKeyType(t));

        private readonly Type _type;

        private OSKeyType(Type type)
        {
            _type = type;
        }

        public enum Type
        {
            Backspace = 0x08,
            Tab = 0x09,
            Clear = 0x0C,
            Return = 0x0D,

            Shift = 0x10,
            Control = 0x11,
            Alt = 0x12,
            Pause = 0x13,
            Capslock = 0x14,
            Kana = 0x15,
            Hangul = 0x16,
            Junja = 0x17,
            Final = 0x18,
            Hanja = 0x19,
            // Kanji = 0x19,
            Escape = 0x1B,
            Convert = 0x1C,
            NonConvert = 0x1D,
            Accept = 0x1E,
            ModeChange = 0x1F,

            Space = 0x20,
            PageUp = 0x21,
            PageDown = 0x22,
            End = 0x23,
            Home = 0x24,
            Left = 0x25,
            Up = 0x26,
            Right = 0x27,
            Down = 0x28,
            Select = 0x29,
            Print = 0x2A,
            Execute = 0x2B,
            PrintScreen = 0x2C,
            Insert = 0x2D,
            Delete = 0x2E,
            Help = 0x2F,

            Number0 = 0x30,
            Number1 = 0x31,
            Number2 = 0x32,
            Number3 = 0x33,
            Number4 = 0x34,
            Number5 = 0x35,
            Number6 = 0x36,
            Number7 = 0x37,
            Number8 = 0x38,
            Number9 = 0x39,

            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            M = 0x4D,
            N = 0x4E,
            O = 0x4F,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x5A,

            LMeta = 0x5B,
            RMeta = 0x5C,
            Apps = 0x5D,
            Sleep = 0x5F,

            Numpad0 = 0x60,
            Numpad1 = 0x61,
            Numpad2 = 0x62,
            Numpad3 = 0x63,
            Numpad4 = 0x64,
            Numpad5 = 0x65,
            Numpad6 = 0x66,
            Numpad7 = 0x67,
            Numpad8 = 0x68,
            Numpad9 = 0x69,

            Multiply = 0x6A,
            Add = 0x6B,
            Separator = 0x6C,
            Subtract = 0x6D,
            Decimal = 0x6E,
            Divide = 0x6F,

            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            F13 = 0x7C,
            F14 = 0x7D,
            F15 = 0x7E,
            F16 = 0x7F,
            F17 = 0x80,
            F18 = 0x81,
            F19 = 0x82,
            F20 = 0x83,
            F21 = 0x84,
            F22 = 0x85,
            F23 = 0x86,
            F24 = 0x87,

            Numlock = 0x90,
            Scrolllock = 0x91,

            // OemNecEqual = 0x92,
            OemFJJisho = 0x92,
            OemFJMasshou = 0x93,
            OemFJTouroku = 0x94,
            OemFJLoya = 0x95,
            OemFJRoya = 0x96,

            LShift = 0xA0,
            RShift = 0xA1,
            LControl = 0xA2,
            RControl = 0xA3,
            LAlt = 0xA4,
            RAlt = 0xA5,

            BrowserBack = 0xA6,
            BrowserForward = 0xA7,
            BrowserRefresh = 0xA8,
            BrowserStop = 0xA9,
            BrowserSearch = 0xAA,
            BrowserFavorites = 0xAB,
            BrowserHome = 0xAC,

            VolumeMute = 0xAD,
            VolumeDown = 0xAE,
            VolumeUp = 0xAF,

            MediaNextTrack = 0xB0,
            MediaPreviousTrack = 0xB1,
            MediaStop = 0xB2,
            MediaPlayPause = 0xB3,

            LaunchMail = 0xB4,
            LaunchMediaSelect = 0xB5,
            LaunchApp1 = 0xB6,
            LaunchApp2 = 0xB7,

            Oem1 = 0xBA,
            OemPlus = 0xBB,
            OemComma = 0xBC,
            OemMinus = 0xBD,
            OemPeriod = 0xBE,
            Oem2 = 0xBF,
            Oem3 = 0xC0,
            Oem4 = 0xDB,
            Oem5 = 0xDC,
            Oem6 = 0xDD,
            Oem7 = 0xDE,
            Oem8 = 0xDF,
            OemAX = 0xE1,
            Oem102 = 0xE2,

            IcoHelp = 0xE3,
            Ico00 = 0xE4,
            ProcessKey = 0xE5,
            IcoClear = 0xE6,
            Packet = 0xE7,

            OemReset = 0xE9,
            OemJump = 0xEA,
            OemPA1 = 0xEB,
            OemPA2 = 0xEC,
            OemPA3 = 0xED,
            OemWSCtrl = 0xEE,
            OemCuSel = 0xEF,
            OemAttn = 0xF0,
            OemFinish = 0xF1,
            OemCopy = 0xF2,
            OemAuto = 0xF3,
            OemEnlW = 0xF4,
            OemBackTab = 0xF5,

            Attn = 0xF6,
            CrSel = 0xF7,
            ExSel = 0xF8,
            ErEof = 0xF9,
            Play = 0xFA,
            Zoom = 0xFB,
            NoName = 0xFC,
            PA1 = 0xFD,

            OemClear = 0xFE,
        }

        public static OSKeyType GetOSKeyType(int i)
        {
            if (!_types.TryGetValue(i, out var osKeyType))
            {
                osKeyType = new OSKeyType((Type)i);
                _types.Add(i, osKeyType);
            }

            return osKeyType;
        }

        private static IEnumerable<Type> GetTypes()
        {
            foreach (Type type in Enum.GetValues(typeof(Type)))
            {
                yield return type;
            }
        }
    }
}