using Northwind.EntityModels; // To use NorthwindDb, Category, Product.
using Microsoft.EntityFrameworkCore; // To use DbSet<T>.
partial class Program
{
private static void FilterAndSort()
{
SectionTitle("Filter and sort");
using NorthwindDb db = new();
DbSet<Product> allProducts = db.Products;
IQueryable<Product> filteredProducts =
allProducts.Where(product => product.UnitPrice < 10M);
IOrderedQueryable<Product> sortedAndFilteredProducts =
filteredProducts.OrderByDescending(product => product.UnitPrice);

var projectedProducts = sortedAndFilteredProducts
.Select(product => new // Anonymous type.
{
product.ProductId,
product.ProductName,
product.UnitPrice
});
WriteLine("Products that cost less than $10:");
WriteLine(projectedProducts.ToQueryString());
foreach (var p in projectedProducts)
{
// WriteLine("Products that cost less than $10:");
// WriteLine(sortedAndFilteredProducts.ToQueryString());
// foreach (Product p in sortedAndFilteredProducts)
// {
WriteLine("{0}: {1} costs {2:$#,##0.00}",
p.ProductId, p.ProductName, p.UnitPrice);
}
WriteLine();
}

private static void JoinCategoriesAndProducts()
{
SectionTitle("Join categories and products");
using NorthwindDb db = new();
// Join every product to its category to return 77 matches.
var queryJoin = db.Categories.Join(
inner: db.Products,
outerKeySelector: category => category.CategoryId,
innerKeySelector: product => product.CategoryId,
resultSelector: (c, p) =>
new { c.CategoryName, p.ProductName, p.ProductId })
.OrderBy(cp => cp.CategoryName);
foreach (var p in queryJoin)
{
WriteLine($"{p.ProductId}: {p.ProductName} in {p.CategoryName}.");
}
}

private static void GroupJoinCategoriesAndProducts()
{
SectionTitle("Group join categories and products");
using NorthwindDb db = new();
// Group all products by their category to return 8 matches.
var queryGroup = db.Categories.AsEnumerable().GroupJoin(
inner: db.Products,
outerKeySelector: category => category.CategoryId,
innerKeySelector: product => product.CategoryId,
resultSelector: (c, matchingProducts) => new
{
c.CategoryName,
Products = matchingProducts.OrderBy(p => p.ProductName)
});
foreach (var c in queryGroup)
{
WriteLine($"{c.CategoryName} has {c.Products.Count()} products.");
foreach (var product in c.Products)
{
WriteLine($"{product.ProductName}");
}
}
}
private static void ProductsLookup()
{
SectionTitle("Products lookup");
using NorthwindDb db = new();
// Join all products to their category to return 77 matches.
var productQuery = db.Categories.Join(
inner: db.Products,
outerKeySelector: category => category.CategoryId,
innerKeySelector: product => product.CategoryId,
resultSelector: (c, p) => new { c.CategoryName, Product = p });
ILookup<string, Product> productLookup = productQuery.ToLookup(
keySelector: cp => cp.CategoryName,
elementSelector: cp => cp.Product);
foreach (IGrouping<string, Product> group in productLookup)
{
// Key is Beverages, Condiments, and so on.
WriteLine($"{group.Key} has {group.Count()} products.");
foreach (Product product in group)
{
WriteLine($" {product.ProductName}");
}
}
// We can look up the products by a category name.
Write("Enter a category name: ");
string categoryName = ReadLine()!;
WriteLine();
WriteLine($"Products in {categoryName}:");
IEnumerable<Product> productsInCategory = productLookup[categoryName];
foreach (Product product in productsInCategory)
{
WriteLine($"{product.ProductName}");
}
}

private static void OutputTableOfProducts(Product[] products,
int currentPage, int totalPages)
{
string line = new('-', count: 73);
string lineHalf = new('-', count: 30);
WriteLine(line);
WriteLine("{0,4} {1,-40} {2,12} {3,-15}",
"ID", "Product Name", "Unit Price", "Discontinued");
WriteLine(line);
foreach (Product p in products)
{
WriteLine("{0,4} {1,-40} {2,12:C} {3,-15}",
p.ProductId, p.ProductName, p.UnitPrice, p.Discontinued);
}
WriteLine("{0} Page {1} of {2} {3}",
lineHalf, currentPage + 1, totalPages + 1, lineHalf);
}

private static void OutputPageOfProducts(IQueryable<Product> products,
int pageSize, int currentPage, int totalPages)
{

var pagingQuery = products.OrderBy(p => p.ProductId)
.Skip(currentPage * pageSize).Take(pageSize);
Clear(); 
//SectionTitle(pagingQuery.ToQueryString());
OutputTableOfProducts(pagingQuery.ToArray(),
currentPage, totalPages);
}

private static void PagingProducts()
{
SectionTitle("Paging products");
using NorthwindDb db = new();
int pageSize = 10;
int currentPage = 0;
int productCount = db.Products.Count();
int totalPages = productCount / pageSize;
while (true) 
{
OutputPageOfProducts(db.Products, pageSize, currentPage, totalPages);
Write("Press <- to page back, press -> to page forward, any key to exit.");
ConsoleKey key = ReadKey().Key;
if (key == ConsoleKey.LeftArrow)
currentPage = currentPage == 0 ? totalPages : currentPage - 1;
else if (key == ConsoleKey.RightArrow)
currentPage = currentPage == totalPages ? 0 : currentPage + 1;
else
break; // Break out of the while loop.
WriteLine();
}
}
}