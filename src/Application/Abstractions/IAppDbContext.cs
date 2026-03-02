using Microsoft.EntityFrameworkCore;
using Template.Domain.Entities;

namespace Template.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<TodoItem> TodoItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
