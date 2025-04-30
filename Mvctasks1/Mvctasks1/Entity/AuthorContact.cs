using System;
using System.ComponentModel.DataAnnotations;

namespace Mvctasks1.Entity
{
	public class AuthorContact : BaseEntity
	{
        public string Email { get; set; }

        public string Phone { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; }
    }
}

