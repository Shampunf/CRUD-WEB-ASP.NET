using BookStore.DataAccess.Entites;
using BookStore.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.DataAccess.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly BookStoreDbContext _context;

        public BooksRepository(BookStoreDbContext context) { 
            _context = context;
        }

        public async Task<List<Book>> Get ()
        {
            var bookEntities = await _context.Books
                .AsNoTracking()
                .ToListAsync();

            var books = bookEntities
                .Select(be => Book.Create(be.Id, be.Title, be.Description, be.Price).Book)
                .ToList();
            return books;
        }

        public async Task<Guid> Create(Book book)
        {
            var bookEntity = new BookEntity
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Price = book.Price
            };

            await _context.Books.AddAsync(bookEntity);
            await _context.SaveChangesAsync();

            return bookEntity.Id;
        }

        public async Task<Guid> Update(Guid id, string title, string description, decimal price)
        {
            await _context.Books
                .Where(b => b.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.Title, b => title)
                    .SetProperty(b => b.Description, description)
                    .SetProperty(b => b.Price, b => price));
            return id;
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _context.Books
                .Where (b => b.Id == id)
                .ExecuteDeleteAsync();
            return id;
        }
    }
}
