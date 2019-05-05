using System;
using System.Collections.Generic;
using System.Text;

namespace Capstan.Scenario
{
    public class Fireball
    {
        public Fireball()
        {
            Damage = new Damage { From = 9, To = 16 };
        }

        public Damage Damage { get; set; }
        public int Radius { get; set; }
        public DamageTypes DamageType { get; set; }
    }

    public class Damage
    {
        public int From { get; set; }
        public int To { get; set; }
    }

    public enum DamageTypes
    {
        Normal = 0,
        Fire
    }
}
