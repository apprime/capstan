using System;
using System.Collections.Generic;

namespace Capstan.Scenario
{
    public class Area
    {
        public List<Entity> Entities { get; set; }

        public Area()
        {
            Entities.Add(new Zombie());
            Entities.Add(new Zombie());
            Entities.Add(new Chest());
        }

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
            //Area subscribes to something here.
        }

        public void RemoveEntity(Entity entity)
        {
            Entities.Remove(entity);
            //Area removes subscription.
        }

        public List<Entity> GetEntities(Location location, int Radius)
        {
            throw new NotImplementedException();
        }
    }
}
