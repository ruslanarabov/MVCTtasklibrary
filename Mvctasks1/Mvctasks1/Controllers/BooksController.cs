using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mvctasks1.Data;
using Mvctasks1.Entity;
using Mvctasks1.ViewModel.Book;

namespace Mvctasks1.Controllers
{
	public class BooksController : Controller
	{
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // Books
        public async Task<IActionResult> Index()
        {
            var applicationDbContext =await _context.Books
                .Include(b => b.BookCategory)
                .Include(b => b.Publisher).ToListAsync();

            //var datas = await applicationDbContext.ToListAsync();
            return View(applicationDbContext);
        }

        // Details yeri
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.BookCategory)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // Create (GET)
        public IActionResult Create()
        {
            var vm = new BookCreateVM
            {
                BookCategories = _context.BookCategories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),
                Publishers = _context.Publishers
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList(),
                Authors = _context.Authors
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName }).ToList()
            };
            return View(vm);
        }

        // Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.BookCategories = _context.BookCategories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
                vm.Publishers = _context.Publishers
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();
                vm.Authors = _context.Authors
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName }).ToList();
                return View(vm);
            }

            var book = new Mvctasks1.Entity.Book
            {
                Title = vm.Title,
                Description = vm.Description,
                PublicationDate = vm.PublicationDate,
                Price = vm.Price,
                BookCategoryId = vm.BookCategoryId,
                PublisherId = vm.PublisherId
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            if (vm.SelectedAuthorIds != null && vm.SelectedAuthorIds.Any())
            {
                foreach (var authorId in vm.SelectedAuthorIds)
                {
                    _context.BookAuthors.Add(new BookAuthors
                    {
                        BookId = book.Id,
                        AuthorId = authorId
                    });
                }
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Book created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Edit yeri (update)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            ViewData["BookCategoryId"] = new SelectList(_context.BookCategories, "Id", "Name", book.BookCategoryId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", book.PublisherId);

          
            var selectedAuthors = book.BookAuthors.Select(ba => ba.AuthorId).ToArray();
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "Id", "FullName", selectedAuthors);

            return View(book);
        }

        // Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ISBN,PublicationDate,Price,BookCategoryId,PublisherId")] Book book, int[] selectedAuthors)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);

                    
                    var existingBookAuthors = _context.BookAuthors.Where(ba => ba.BookId == book.Id);
                    _context.BookAuthors.RemoveRange(existingBookAuthors);

                    
                    if (selectedAuthors != null)
                    {
                        foreach (var authorId in selectedAuthors)
                        {
                            _context.BookAuthors.Add(new BookAuthors
                            {
                                BookId = book.Id,
                                AuthorId = authorId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Book updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookCategoryId"] = new SelectList(_context.BookCategories, "Id", "Name", book.BookCategoryId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", book.PublisherId);
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "Id", "FullName", selectedAuthors);
            TempData["Error"] = "There was an error updating the book.";
            return View(book);
        }

        // Delete yeri
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.BookCategory)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Book deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}

