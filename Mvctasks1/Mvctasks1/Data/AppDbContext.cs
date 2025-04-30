using System;
using Microsoft.EntityFrameworkCore;
using Mvctasks1.Entity;

namespace Mvctasks1.Data
{
	public class AppDbContext : DbContext
	{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorContact> AuthorContacts { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<BookAuthors> BookAuthors { get; set; }
    }
}

