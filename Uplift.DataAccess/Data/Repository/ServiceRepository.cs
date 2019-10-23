using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Uplift.DataAccess.Data.Repository.IRepository;
using Uplift.Models;
using System.Linq;

namespace Uplift.DataAccess.Data.Repository
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        private readonly ApplicationDbContext _db;
        public ServiceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Service service)
        {
            var objectFromDatabase = _db.Service.FirstOrDefault(s => s.Id == service.Id);

            objectFromDatabase.Name = service.Name;
            objectFromDatabase.LongDesc = service.LongDesc;
            objectFromDatabase.Price = service.Price;
            objectFromDatabase.ImageUrl = service.ImageUrl;
            objectFromDatabase.FrequencyId = service.FrequencyId;
            objectFromDatabase.CategoryId = service.CategoryId;

            _db.SaveChanges();
        }
    }
}
