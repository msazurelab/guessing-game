using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Repository
{
    public interface IRepository<T> where T : EntityBase
    {
        T GetById(string id);

        void Add(T entity);

        void Update(T entity);

        bool Exists(string id);
    }
}
