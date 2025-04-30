using System;
namespace Mvctasks1.Entity
{
	public class Author : BaseEntity
	{
        public string FullName { get; set; }

        public AuthorContact AuthorContact { get; set; }

        public ICollection<BookAuthors> BookAuthors { get; set; }
    }
}

