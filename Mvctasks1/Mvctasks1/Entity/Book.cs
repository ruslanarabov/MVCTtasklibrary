using System;
namespace Mvctasks1.Entity
{
	public class Book : BaseEntity
	{
        public string Title { get; set; }
        public int BookCategoryId { get; set; }
        public BookCategory BookCategory { get; set; }

        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }

        public ICollection<BookAuthors> BookAuthors { get; set; }
    }
}

