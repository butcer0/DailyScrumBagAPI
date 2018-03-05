using DailyScrumBagAPI.API.Entities;
using DailyScrumBagAPI.API.Helpers;
using DailyScrumBagAPI.API.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace DailyScrumBagAPI.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<int, UserItem> _storage = new ConcurrentDictionary<int, UserItem>();

        public UserItem GetSingle(int id)
        {
            UserItem userItem;
            return _storage.TryGetValue(id, out userItem) ? userItem : null;
        }

        public void Add(UserItem item)
        {
            item.Id = !_storage.Values.Any() ? 1 : _storage.Values.Max(x => x.Id) + 1;
            item.Created = DateTime.Now;
            if (!_storage.TryAdd(item.Id, item))
            {
                throw new Exception("Item could not be added");
            }
        }

        public void Delete(int id)
        {
            UserItem userItem;
            if (!_storage.TryRemove(id, out userItem))
            {
                throw new Exception("Item could not be added");
            }
        }

        public UserItem Update(int id, UserItem item)
        {
            _storage.TryUpdate(id, item, GetSingle(id));
            return item;
        }

        public IQueryable<UserItem> GetAll(QueryParameters queryParameters)
        {
            //Erik - 3/5/2018 Check for Default Values
            if (!string.IsNullOrEmpty(queryParameters.OrderBy)
                && queryParameters.OrderBy.Equals("name", StringComparison.InvariantCultureIgnoreCase))
            {
                queryParameters.OrderBy = "UserName";
            }


            IQueryable<UserItem> _allItems = _storage.Values.AsQueryable().OrderBy<UserItem>(queryParameters.OrderBy,
            queryParameters.IsDescending());

            if (queryParameters.HasQuery())
            {               
                _allItems = _allItems
                    .Where(x => x.UserName.ToString().Contains(queryParameters.Query.ToLowerInvariant())
                    || x.UserName.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);

        }

        public int Count()
        {
            return _storage.Count;
        }

        public bool Save()
        {
            //To keep interface consistent with Controllers, Tests & EF Interfaces
            return true;
        }
    }
}
