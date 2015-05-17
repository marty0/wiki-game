using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WikiGame.Models
{
    public interface ICategoryProvider
    {
        List<GameCategory> GetAllCategories();
        bool ClearCategories();
        bool AddCategory(GameCategory category);
        GameCategory GetCategoryByName(string name);
    }
}