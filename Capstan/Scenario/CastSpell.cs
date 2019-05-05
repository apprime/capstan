using System;
using System.Collections.Generic;
using System.Text;

namespace Capstan.Scenario
{
    class CastSpell<T>
    {
        public Player Caster { get; set; }
        public Location Target { get; set; }

        public T Materialize()
        {
            throw new NotImplementedException();
        }
    }
}
