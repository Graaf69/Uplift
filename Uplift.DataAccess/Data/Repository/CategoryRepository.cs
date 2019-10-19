using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Uplift.DataAccess.Data.Repository.IRepository;
using Uplift.Models;
using System.Linq;

namespace Uplift.DataAccess.Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetCategoryListForDropdown()
        {
            return _db.Category.Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }

        public void Update(Category category)
        {
            var objectFromDatabase = _db.Category.FirstOrDefault(s => s.Id == category.Id);

            objectFromDatabase.Name = category.Name;
            objectFromDatabase.DisplayOrder = category.DisplayOrder;

            _db.SaveChanges();
        }
    }
}
