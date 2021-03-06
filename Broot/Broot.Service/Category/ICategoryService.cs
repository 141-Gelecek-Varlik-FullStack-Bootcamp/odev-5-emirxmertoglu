using Broot.Model;
using Broot.Model.CategoryModel;

namespace Broot.Service.Category
{
    public interface ICategoryService
    {
        // Insert Category
        public General<CategoryDetail> Insert(InsertCategory newCategory);

        // Update Category
        public General<CategoryDetail> Uptade(InsertCategory updatedCategory, int id, int updater);

        // Delete Category
        public General<CategoryDetail> Delete(int id, int updater);

        // Get Categories
        public General<CategoryDetail> Get();
    }
}
