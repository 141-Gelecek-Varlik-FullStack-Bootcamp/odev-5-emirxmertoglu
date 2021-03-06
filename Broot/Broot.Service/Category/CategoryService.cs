using AutoMapper;
using Broot.DB.Entities.DataContext;
using Broot.Model;
using Broot.Model.CategoryModel;
using System.Collections.Generic;
using System.Linq;

namespace Broot.Service.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper mapper;

        public CategoryService(IMapper _mapper)
        {
            mapper = _mapper;
        }

        // Insert Category
        public General<CategoryDetail> Insert(InsertCategory newCategory)
        {
            var result = new General<CategoryDetail>() { IsSuccess = false };
            try
            {
                var model = mapper.Map<Broot.DB.Entities.Category>(newCategory);
                using (var srv = new BrootContext())
                {
                    model.Idate = System.DateTime.Now;
                    model.IsActive = true;
                    model.Iuser = 2;

                    // Adding changes to db
                    srv.Category.Add(model);
                    srv.SaveChanges();

                    // Returning category details to entity
                    result.Entity = mapper.Map<CategoryDetail>(model);
                    result.IsSuccess = true;
                }
            }
            catch (System.Exception)
            {
                result.ExceptionMessage = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin!";
            }


            return result;
        }

        // Update Category
        public General<CategoryDetail> Uptade(InsertCategory updatedCategory, int id, int updater)
        {
            var result = new General<CategoryDetail>() { IsSuccess = false };
            using (var srv = new BrootContext())
            {
                var category = srv.Category.SingleOrDefault(c => c.Id == id);
                // Checking category exists
                if (category is null)
                {
                    result.ExceptionMessage = "Verilen id numarasiyla iliskili bir kategori bulunamadi.";
                    return result;
                }

                // Updating category values
                category.Name = updatedCategory.Name != default ? updatedCategory.Name : category.Name;
                category.DisplayName = updatedCategory.DisplayName != default ? updatedCategory.DisplayName : category.DisplayName;
                category.Udate = System.DateTime.Now;
                category.Uuser = updater;

                // Saving user with new values to db
                srv.SaveChanges();

                // Updating result
                result.Entity = mapper.Map<CategoryDetail>(category);
                result.IsSuccess = true;
            }
            return result;
        }

        // Delete Category
        public General<CategoryDetail> Delete(int id, int updater)
        {
            var result = new General<CategoryDetail>() { IsSuccess = false };
            using (var srv = new BrootContext())
            {
                var category = srv.Category.SingleOrDefault(c => c.Id == id);

                // Checking if category exists
                if (category is null)
                {
                    result.ExceptionMessage = "Bu id ile bir kategori bulunamadi!";
                    return result;
                }

                if (category.IsDeleted)
                {
                    result.ExceptionMessage = "Bu kategori zaten silinmis!";
                    return result;
                }

                // Deactivating category
                category.IsDeleted = true;
                category.IsActive = false;
                category.Udate = System.DateTime.Now;
                category.Uuser = updater;

                // Saving category with new values to db
                srv.SaveChanges();

                // Updating result values
                result.Entity = mapper.Map<CategoryDetail>(category);
                result.IsSuccess = true;
            }
            return result;
        }

        // Get All Categories
        public General<CategoryDetail> Get()
        {
            var result = new General<CategoryDetail>() { IsSuccess = false };
            using (var srv = new BrootContext())
            {
                // Get categories which categories state is active & order by id
                var categories = srv.Category.Where(c => c.IsActive && !c.IsDeleted).OrderBy(c => c.Id);

                if (categories is null)
                {
                    result.ExceptionMessage = "Kategori verileri cekilemedi!";
                    return result;
                }

                // Mapping categories
                result.List = mapper.Map<List<CategoryDetail>>(categories);
                result.TotalCount = categories.Count();
                result.IsSuccess = true;
            }

            return result;
        }
    }
}
