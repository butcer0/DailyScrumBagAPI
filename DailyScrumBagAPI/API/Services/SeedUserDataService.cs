using DailyScrumBagAPI.API.Entities;
using DailyScrumBagAPI.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyScrumBagAPI.API.Services
{
    public class SeedUserDataService : ISeedUserDataService
    {
        IUserRepository _repository;

        public SeedUserDataService(IUserRepository repository)
        {
            _repository = repository;
        }

        public void EnsureSeedData()
        {
            //TODO: Erik - 3/5/2018 Seed Users
            _repository.Add(new UserItem {Id = 1, UserName = "BAnderson", FirstName = "Bob", LastName = "Anderson", EmailAddress = "bAnderson@gmail.com", Created = DateTime.Now });
            _repository.Add(new UserItem {Id = 2, UserName = "JThomas", FirstName = "Jill", LastName = "Thomas", EmailAddress = "jThomas@gmail.com", Created = DateTime.Now });
            _repository.Add(new UserItem {Id = 3, UserName = "DOthers", FirstName = "Dunn", LastName = "Others", EmailAddress = "dOthers@gmail.com", Created = DateTime.Now });
            _repository.Add(new UserItem {Id = 4, UserName = "BThomas", FirstName = "Bill", LastName = "Thomas", EmailAddress = "bThomas@gmail.com", Created = DateTime.Now });
        }

    }
}
