﻿// ------------------------------------------------------------------------------
// <copyright file="HeroAttribute.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace War3Net.Runtime.Enums
{
    public sealed class HeroAttribute
    {
        private static readonly Dictionary<int, HeroAttribute> _attributes = GetTypes().ToDictionary(t => (int)t, t => new HeroAttribute(t));

        private readonly Type _type;

        private HeroAttribute(Type type)
        {
            _type = type;
        }

        public enum Type
        {
            Strength = 1,
            Intelligence = 2,
            Agility = 3,
        }

        public static HeroAttribute GetHeroAttribute(int i)
        {
            if (!_attributes.TryGetValue(i, out var heroAttribute))
            {
                heroAttribute = new HeroAttribute((Type)i);
                _attributes.Add(i, heroAttribute);
            }

            return heroAttribute;
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