﻿// ------------------------------------------------------------------------------
// <copyright file="PlayerSlot.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

namespace War3Net.Build
{
    [System.Obsolete(null, true)]
    public class PlayerSlot
    {
        private readonly float _startX;
        private readonly float _startY;
        private readonly int _team;
        private readonly bool _raceSelectable;
        private readonly bool _user;

        public PlayerSlot(bool user = true)
            : this(0f, 0f, user)
        {
        }

        public PlayerSlot(float x, float y, bool user = true)
            : this(x, y, 0, user)
        {
        }

        public PlayerSlot(float x, float y, int team, bool user = true)
        {
            _startX = x;
            _startY = y;
            _team = team;
            _raceSelectable = false;
            _user = user;
        }

        public float StartLocationX => _startX;

        public float StartLocationY => _startY;

        public int Team => _team;

        public string RacePreference => "RACE_PREF_HUMAN";

        public bool RaceSelectable => _raceSelectable;

        public string Controller => _user ? "MAP_CONTROL_USER" : "MAP_CONTROL_COMPUTER";
    }
}