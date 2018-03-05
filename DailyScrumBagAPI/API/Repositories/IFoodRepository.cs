using System.Collections.Generic;
using System.Linq;
using DailyScrumBagAPI.API.Entities;
using DailyScrumBagAPI.API.Models;

namespace DailyScrumBagAPI.API.Repositories
{
    public interface IFoodRepository
    {
        FoodItem GetSingle(int id);
        void Add(FoodItem item);
        void Delete(int id);
        FoodItem Update(int id, FoodItem item);
        IQueryable<FoodItem> GetAll(QueryParameters queryParameters);

        ICollection<FoodItem> GetRandomMeal();
        int Count();

        bool Save();
    }
}
