using System.Collections.Concurrent;
using POMuswick.Models;
using ZXing;

namespace POMuswick.Parsers
{
    public class CatNSubCatParser : BaseParser
    {
        public CatNSubCatParser()
        {

        }
        public async Task<CatNSubcatResult> Parse(string response)
        {
            Console.WriteLine("Get Categories and Subcategories returned");
            string sCategories = response;
            string[] aCategories = sCategories.Split('~');
            ConcurrentBag<Category> lstCategories = new ConcurrentBag<Category>();
            ConcurrentBag<Subcategory> lstSubcategories = new ConcurrentBag<Subcategory>();

            if (aCategories.Length > 1)
            {
                Parallel.ForEach(aCategories, s =>
                {
                    try
                    {
                        String[] aCategory = s.Split("|");

                        if (aCategory.Count() < 4)
                        {
                            return;
                        }

                        if (aCategory[1].Length == 0)
                        {
                            if (aCategory.Length < 5)
                            {
                                return;
                            }

                            Category cat = new Category()
                            {
                                Code = aCategory[0],
                                Description = aCategory[2].Trim(),
                                ImageURL = Constants.CategoryImageUrl + aCategory[0] + ".png",
                                Rank = GetIntegerValue("Category Rank", aCategory[3].Trim(), 0),
                                HomePage = GetIntegerValue("Category Home Page", aCategory[4].Trim(), 0)
                            };
                            lstCategories.Add(cat);
                        }
                        else
                        {
                            Subcategory subcat = new Subcategory()
                            {
                                Category = aCategory[0],
                                Code = aCategory[1],
                                Description = aCategory[2].Trim(),
                                Rank = GetIntegerValue("Subcategory Rank", aCategory[3].Trim(), 0)
                            };
                            lstSubcategories.Add(subcat);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Parsing Categories error" + e.Message);
                    }
                });
            }
            CatNSubcatResult catNSubcatResult = new CatNSubcatResult()
            {
                categories = lstCategories.ToList(),
                subcategories = lstSubcategories.ToList()
            };
            return catNSubcatResult;
        }
    }
}