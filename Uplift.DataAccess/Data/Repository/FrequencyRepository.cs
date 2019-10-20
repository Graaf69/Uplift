using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Uplift.DataAccess.Data.Repository.IRepository;
using Uplift.Models;
using System.Linq;

namespace Uplift.DataAccess.Data.Repository
{
    public class FrequencyRepository : Repository<Frequency>, IFrequencyRepository
    {
        private readonly ApplicationDbContext _db;

        public FrequencyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetFrequencyListForDropdown()
        {
            return _db.Frequency.Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }

        public void Update(Frequency frequency)
        {
            var objectFromDatabase = _db.Frequency.FirstOrDefault(f => f.Id == frequency.Id);

            objectFromDatabase.Name = frequency.Name;
            objectFromDatabase.FrequencyCount = frequency.FrequencyCount;

            _db.SaveChanges();
        }
    }
}
