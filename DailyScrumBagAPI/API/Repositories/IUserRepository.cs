using DailyScrumBagAPI.API.Entities;
using DailyScrumBagAPI.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyScrumBagAPI.API.Repositories
{
    public interface IUserRepository
    {
        UserItem GetSingle(int id);
        void Add(UserItem item);
        void Delete(int id);
        UserItem Update(int id, UserItem item);
        IQueryable<UserItem> GetAll(QueryParameters queryParameters);
        
        int Count();

        bool Save();
    }
}
