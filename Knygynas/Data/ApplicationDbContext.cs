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
        public DbSet<Postomat> Postomats { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

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

            // Seed Bookstores
            modelBuilder.Entity<Bookstore>().HasData(
                new Bookstore { Id = 1, City = "Vilnius", Address = "Gedimino pr. 1", CreatedDate = new DateTime(2024, 1, 15) },
                new Bookstore { Id = 2, City = "Kaunas", Address = "Laisves al. 10", CreatedDate = new DateTime(2024, 2, 5) },
                new Bookstore { Id = 3, City = "Klaipeda", Address = "H. Manto g. 7", CreatedDate = new DateTime(2024, 3, 1) },
                new Bookstore { Id = 4, City = "Šiauliai", Address = "Tilžės g. 109", CreatedDate = new DateTime(2024, 3, 15) },
                new Bookstore { Id = 5, City = "Panevėžys", Address = "Laisvės a. 5", CreatedDate = new DateTime(2024, 4, 1) },
                new Bookstore { Id = 6, City = "Alytus", Address = "Pulko g. 12", CreatedDate = new DateTime(2024, 4, 10) },
                new Bookstore { Id = 7, City = "Marijampolė", Address = "J. Basanavičiaus a. 8", CreatedDate = new DateTime(2024, 4, 20) },
                new Bookstore { Id = 8, City = "Mažeikiai", Address = "Laisvės g. 24", CreatedDate = new DateTime(2024, 5, 1) }
            );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fiction", Description = "Fiction" },
                new Category { Id = 2, Name = "Non-Fiction", Description = "Non-Fiction" },
                new Category { Id = 3, Name = "Science Fiction", Description = "Science Fiction" },
                new Category { Id = 4, Name = "History", Description = "History" }
            );

            // Seed Books
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    ISBN = "978-0-14-028329-3",
                    Title = "1984",
                    Quantity = 10,
                    ReleaseDate = new DateTime(1949, 6, 8),
                    PageCount = 328,
                    Language = "English",
                    EANCode = "9780140283293",
                    CoverImageUrl = "/images/books/1984.svg",
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
                    Quantity = 10,
                    ReleaseDate = new DateTime(1960, 7, 11),
                    PageCount = 324,
                    Language = "English",
                    EANCode = "9780061120084",
                    CoverImageUrl = "/images/books/to-kill-a-mockingbird.svg",
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
                    Quantity = 10,
                    ReleaseDate = new DateTime(1965, 8, 1),
                    PageCount = 688,
                    Language = "English",
                    EANCode = "9780765377937",
                    CoverImageUrl = "/images/books/dune.svg",
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

            // Seed Postomats
            modelBuilder.Entity<Postomat>().HasData(
                new Postomat { Id = 1, City = "Vilnius", Address = "Saltoniškių g. 9 (PLC Panorama)", Company = "Omniva" },
                new Postomat { Id = 2, City = "Vilnius", Address = "Ozo g. 25 (PPC Akropolis)", Company = "DPD" },
                new Postomat { Id = 3, City = "Vilnius", Address = "Vokiečių g. 15", Company = "LP Express" },
                new Postomat { Id = 4, City = "Kaunas", Address = "Karaliaus Mindaugo pr. 49 (Akropolis)", Company = "Omniva" },
                new Postomat { Id = 5, City = "Kaunas", Address = "Savanorių pr. 255 (Rimi)", Company = "LP Express" },
                new Postomat { Id = 6, City = "Klaipėda", Address = "Taikos pr. 61 (Akropolis)", Company = "Omniva" },
                new Postomat { Id = 7, City = "Klaipėda", Address = "H. Manto g. 90", Company = "DPD" },
                new Postomat { Id = 8, City = "Šiauliai", Address = "Aido g. 8 (Akropolis)", Company = "LP Express" },
                new Postomat { Id = 9, City = "Panevėžys", Address = "Klaipėdos g. 143a (Babilonas)", Company = "Omniva" },
                new Postomat { Id = 10, City = "Alytus", Address = "Naujoji g. 2c", Company = "DPD" }
            );
        }
    }
}
