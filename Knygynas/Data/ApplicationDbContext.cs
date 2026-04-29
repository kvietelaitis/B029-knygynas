using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Knygynas.Models;

namespace Knygynas.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorRole> AuthorRoles { get; set; }
        public DbSet<Bookstore> Bookstores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships if needed
            modelBuilder.Entity<BookCategory>()
                .HasKey(bc => new { bc.BookISBN, bc.CategoryID });
            
            modelBuilder.Entity<AuthorRole>()
                .HasKey(ar => new { ar.AuthorID, ar.BookISBN });

            modelBuilder.Entity<AuthorRole>()
                .HasOne(ar => ar.Author)
                .WithMany(a => a.AuthorRoles)
                .HasForeignKey(ar => ar.AuthorID);

            modelBuilder.Entity<AuthorRole>()
                .HasOne(ar => ar.Book)
                .WithMany(b => b.AuthorRoles)
                .HasForeignKey(ar => ar.BookISBN);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "George Orwell", Biography = "English novelist and essayist." },
                new Author { Id = 2, Name = "Harper Lee", Biography = "American novelist." },
                new Author { Id = 3, Name = "Frank Herbert", Biography = "American science fiction author." }
            );
            
            modelBuilder.Entity<AuthorRole>().HasData(
                new AuthorRole { AuthorID = 1, BookISBN = "978-0-14-028329-3", RoleName = "Author" }, // 1984
                new AuthorRole { AuthorID = 2, BookISBN = "978-0-06-112008-4", RoleName = "Author" }, // Mockingbird
                new AuthorRole { AuthorID = 3, BookISBN = "978-0-7653-7793-1", RoleName = "Author" }  // Dune
            );
            
            // Seed Discounts
            modelBuilder.Entity<Discount>().HasData(
                new Discount { Id = 1, Amount = 0 },
                new Discount { Id = 2, Amount = 15 },
                new Discount { Id = 3, Amount = 30 }
            );

            // Seed Publishers
            modelBuilder.Entity<Publisher>().HasData(
                new Publisher { Id = 1, Name = "Penguin Books", Address = "Address 1, UK", PhoneNumber = "+35988888888" },
                new Publisher { Id = 2, Name = "HarperCollins", Address = "Address 2, USA", PhoneNumber = "+35988888888" },
                new Publisher { Id = 3, Name = "Alma Littera", Address = "Address3, Lithuania", PhoneNumber = "+35988888888" }
            );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fiction",  Description = "Fiction" },
                new Category { Id = 2, Name = "Non-Fiction",  Description = "Non-Fiction" },
                new Category { Id = 3, Name = "Science Fiction",  Description = "Science Fiction" },
                new Category { Id = 4, Name = "History", Description = "History" }
            );

            // Seed Books
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    ISBN = "978-0-14-028329-3",
                    Title = "1984",
                    Available = true,
                    ReleaseDate = new DateTime(1949, 6, 8),
                    PageCount = 328,
                    Language = "English",
                    EANCode = "9780140283293",
                    Length = 19.8f,
                    Width = 12.9,
                    Height = 2.0,
                    Weight = 0.24,
                    Price = 12.99,
                    DiscountId = 1
                },
                new Book
                {
                    ISBN = "978-0-06-112008-4",
                    Title = "To Kill a Mockingbird",
                    Available = true,
                    ReleaseDate = new DateTime(1960, 7, 11),
                    PageCount = 324,
                    Language = "English",
                    EANCode = "9780061120084",
                    Length = 20.3f,
                    Width = 13.5,
                    Height = 2.1,
                    Weight = 0.26,
                    Price = 14.99,
                    DiscountId = 2
                },
                new Book
                {
                    ISBN = "978-0-7653-7793-1",
                    Title = "Dune",
                    Available = true,
                    ReleaseDate = new DateTime(1965, 8, 1),
                    PageCount = 688,
                    Language = "English",
                    EANCode = "9780765377937",
                    Length = 23.5f,
                    Width = 15.5,
                    Height = 4.2,
                    Weight = 0.68,
                    Price = 18.99,
                    DiscountId = 1
                }
            );

            // Seed BookCategories (many-to-many relationships)
            modelBuilder.Entity<BookCategory>().HasData(
                new BookCategory { BookISBN = "978-0-14-028329-3", CategoryID = 1 }, // 1984 - Fiction
                new BookCategory { BookISBN = "978-0-14-028329-3", CategoryID = 3 }, // 1984 - Science Fiction
                new BookCategory { BookISBN = "978-0-06-112008-4", CategoryID = 1 }, // To Kill a Mockingbird - Fiction
                new BookCategory { BookISBN = "978-0-7653-7793-1", CategoryID = 3 }  // Dune - Science Fiction
            );
        }
    }
}
