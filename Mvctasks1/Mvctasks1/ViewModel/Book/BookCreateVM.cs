using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Mvctasks1.ViewModel.Book
{
	public class BookCreateVM
	{

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }

        public decimal Price { get; set; }

        [Display(Name = "Book Category")]
        public int BookCategoryId { get; set; }

        [Display(Name = "Publisher")]
        public int PublisherId { get; set; }

        [Display(Name = "Authors")]
        public List<int> SelectedAuthorIds { get; set; }

        public List<SelectListItem> BookCategories { get; set; }
        public List<SelectListItem> Publishers { get; set; }
        public List<SelectListItem> Authors { get; set; }
    }
}

