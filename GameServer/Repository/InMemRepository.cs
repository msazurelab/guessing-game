using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Repository
{
    public class InMemRepository<T> : IRepository<T> where T : EntityBase
    {
        private Dictionary<string, T> Entities { get; set; }

        public InMemRepository()
        {
            Entities = new Dictionary<string, T>();
        }

        public void Add(T entity)
        {
            if (Entities.ContainsKey(entity.Id))
            {
                throw new Exception($"Entity {entity.Id} already exists");
            }
            Entities.Add(entity.Id, entity);
        }

        public T GetById(string id)
        {
            if (!Entities.ContainsKey(id))
            {
                throw new Exception($"Entity id does not exist");
            }
            return Entities[id];
        }

        public void Update(T entity)
        {
            if (!Entities.ContainsKey(entity.Id))
            {
                throw new Exception($"Entity id does not exist");
            }
            Entities[entity.Id] = entity;
        }

        public bool Exists(string id)
        {
            return Entities.ContainsKey(id);
        }
    }
}
