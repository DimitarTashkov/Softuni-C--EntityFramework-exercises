namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);
            //string input = Console.ReadLine();
            //int year = int.Parse(Console.ReadLine());
            //string input = Console.ReadLine();
            //string date = Console.ReadLine();
            //string name = Console.ReadLine();
            //int lenght = int.Parse(Console.ReadLine());

            //Console.WriteLine(GetBooksByAgeRestriction(db,input));
            //Console.WriteLine(GetGoldenBooks(db));
            //Console.WriteLine(GetBooksByPrice(db));
            //Console.WriteLine(GetBooksNotReleasedIn(db,year));
            //Console.WriteLine(GetBooksByCategory(db,input));
            // Console.WriteLine(GetBooksReleasedBefore(db,date));
            //Console.WriteLine(GetAuthorNamesEndingIn(db,name));
            //Console.WriteLine(GetBookTitlesContaining(db,input));
            //Console.WriteLine(GetBooksByAuthor(db,input));
            //Console.WriteLine(CountBooks(db,lenght));
            //Console.WriteLine(CountCopiesByAuthor(db));
            //Console.WriteLine(GetTotalProfitByCategory(db));
            //Console.WriteLine(GetMostRecentBooks(db));
            //IncreasePrices(db);
            //Console.WriteLine(RemoveBooks(db));
        }
        //ex2
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            if(!Enum.TryParse<AgeRestriction>(command,true, out var ageRestriction))
            {
                return $"{command} is not valid age restriction";
            }
            var books = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title)
                .ToList();

            return $"{string.Join(Environment.NewLine, books.Select(b => b.Title))}";


        }
        //ex3
        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .ToList();

            return $"{string.Join(Environment.NewLine, books.Select(b => b.Title))}";
        }
        //ex4
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title
                    ,
                    b.Price
                }).OrderByDescending(b => b.Price)
                .ToList();
            return $"{string.Join(Environment.NewLine, books.Select(b => $"{b.Title} - ${b.Price:f2}"))}";
        }
        //ex5
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .ToList();

            return $"{string.Join(Environment.NewLine, books.Select(b => b.Title))}";
        }
        //ex6
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.Split(" ",StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();
            var books = context.Books
                .Where(b => b.BookCategories.Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .ToList();

            return $"{string.Join(Environment.NewLine, books.Select(b => b.Title))}";
        }
        //ex7
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy",CultureInfo.InvariantCulture);
            var books = context.Books
                .Where(b => b.ReleaseDate.Value < parsedDate)
                .Select(b => new
                {
                    b.Title
                    ,
                    b.EditionType
                    ,
                    b.Price
                    ,
                    b.ReleaseDate
                }).OrderByDescending(b => b.ReleaseDate);
            return $"{string.Join(Environment.NewLine, books.Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}"))}";
        }
        //ex8
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName
                })
                .OrderBy(a => a.FullName);

            return $"{string.Join(Environment.NewLine, authors.Select(a => $"{a.FullName}"))}";
        }
        //ex9
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Title.Contains(input))
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title);

            return $"{string.Join(Environment.NewLine, books.Select(b => b.Title))}";
        }
        //ex10
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Author.LastName.StartsWith(input))
                .Select(b => new
                {
                    b.BookId
                    ,
                    BookTitle = b.Title
                    ,
                    AuthorName = b.Author.FirstName + " " + b.Author.LastName
                })
                .OrderBy(b=> b.BookId)
                .ToList();
            return $"{string.Join(Environment.NewLine, books.Select(b => $"{b.BookTitle} ({b.AuthorName})"))}";
        }
        //ex11
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                .Count(b => b.Title.Length > lengthCheck);
        }
        //ex12
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new
                {
                    AuthorFullName = a.FirstName + " " + a.LastName
                    ,TotalBooksByAuthor = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.TotalBooksByAuthor)
                .ToList();

            return $"{string.Join(Environment.NewLine, authors.Select(a => $"{a.AuthorFullName} - {a.TotalBooksByAuthor}"))}";
        }
        //ex13
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var profitByCategory = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name
                    ,
                    TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(tp => tp.TotalProfit)
                .ThenBy(cn => cn.CategoryName)
                .ToList();

            return $"{string.Join(Environment.NewLine, profitByCategory.Select(pbc => $"{pbc.CategoryName} ${pbc.TotalProfit:f2}"))}";

        }
        //ex14
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var booksAndCategories = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name
                    ,
                    MostRecentBooks = c.CategoryBooks.OrderByDescending(cb => cb.Book.ReleaseDate)
                    .Take(3)
                    .Select(cb => new
                    {
                        BookTitle = cb.Book.Title
                        ,
                        ReleaseDate = cb.Book.ReleaseDate.Value.Year
                    })
                })
                .OrderBy(c => c.CategoryName)
                .ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var category in booksAndCategories)
            {
                sb.AppendLine($"--{category.CategoryName}");
                foreach (var book in category.MostRecentBooks)
                {
                    sb.AppendLine($"{book.BookTitle} ({book.ReleaseDate})");
                }
            }
            return sb.ToString().TrimEnd();
                
        }
        //ex15
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year < 2010);

            foreach (var book in books)
            {
                book.Price += 5;
            }
            context.SaveChanges();
                
        }
        //ex16
        public static int RemoveBooks(BookShopContext context)
        {
            int count = 0;
            var booksToRemove = context.Books
                .Where(b => b.Copies < 4200);
            count = booksToRemove.Count();

            context.Books.RemoveRange(booksToRemove);
            context.SaveChanges();
            return count;
        }

    }
}


