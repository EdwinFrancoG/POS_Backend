using System;
using System.Collections.Generic;
using System.Text;
using POS_Backend.Logica.Domain;

namespace POS_Backend.Logica.Services
{
    public class CategoryService
    {
        private readonly CategoryDomain _categoryDomain;

        public CategoryService(CategoryDomain categoryDomain)
        {
            _categoryDomain = categoryDomain;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            var response = await _categoryDomain.GetCategories();
            return response;
        }


        public async Task<string> insertCategory(Category category)
        {
            var response = _categoryDomain.categoryEvaluate(category);

            if (response == "OK")
            {
                try
                {

                }
                catch (Exception)
                {

                    throw;
                }
            }
            return "OK";
        }
    }
}
