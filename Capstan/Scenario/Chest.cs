namespace Capstan.Scenario
{
    public class Chest : Entity
    {
        public DamageTypes Immunity = DamageTypes.Fire;

        public Chest()
        {
            Health = 10;
        }
    }
}
