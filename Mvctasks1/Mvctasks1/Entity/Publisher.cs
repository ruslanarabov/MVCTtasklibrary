using System;
namespace Mvctasks1.Entity
{
	public class Publisher : BaseEntity
	{
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}

