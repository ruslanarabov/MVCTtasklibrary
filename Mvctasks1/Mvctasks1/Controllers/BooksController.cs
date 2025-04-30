using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mvctasks1.Data;
using Mvctasks1.Entity;

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
            var applicationDbContext = _context.Books
                .Include(b => b.BookCategory)
                .Include(b => b.Publisher);
            return View(await applicationDbContext.ToListAsync());
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

        // Create yeri
        public IActionResult Create()
        {
            ViewData["BookCategoryId"] = new SelectList(_context.BookCategories, "Id", "Name");
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name");
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "Id", "FullName");
            return View();
        }

        // Create yeri
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ISBN,PublicationDate,Price,BookCategoryId,PublisherId")] Book book, int[] selectedAuthors)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();

                // author add etme yeri
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
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Book created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookCategoryId"] = new SelectList(_context.BookCategories, "Id", "Name", book.BookCategoryId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", book.PublisherId);
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "Id", "FullName");
            TempData["Error"] = "There was an error creating the book.";
            return View(book);
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

        // POST: Books/Edit/5
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

                    // Remove existing author relationships
                    var existingBookAuthors = _context.BookAuthors.Where(ba => ba.BookId == book.Id);
                    _context.BookAuthors.RemoveRange(existingBookAuthors);

                    // Add selected authors
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

        // GET: Books/Delete/5
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

        // POST: Books/Delete/5
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

