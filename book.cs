using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    public class Program
    {
        static void Main()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                List<Author> authors = new List<Author>()
                {
                    new Author { Name = "Author1" },
                    new Author { Name = "Author2" }
                };

                List<Book> books = new List<Book>()
                {
                    new Book { Title = "Book1", Price = 50, Author = authors[0] },
                    new Book { Title = "Book2", Price = 75, Author = authors[1] },
                    new Book { Title = "Book3", Price = 350, Author = authors[0] }
                };

                db.AddRange(books);
                db.SaveChanges();

                string authorName = "Author1";
                decimal price = 400;
                db.UpdateBookPriceByAuthor(authorName, price);
            }
        }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=BooksDB;Trusted_Connection=True;");
        }

        public void UpdateBookPriceByAuthor(string authorName, decimal newPrice)
        {
            Database.ExecuteSqlRaw($"EXEC sp_UpdateBookPricesByAuthor @authorName={authorName}, @newPrice={newPrice}");
        }
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
