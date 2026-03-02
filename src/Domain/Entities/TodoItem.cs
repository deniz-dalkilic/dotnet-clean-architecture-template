using Template.Domain.Common;

namespace Template.Domain.Entities;

public sealed class TodoItem : Entity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public bool IsCompleted { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public TodoItem()
    {
        Id = Guid.NewGuid();
    }

    public void Rename(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        Title = title.Trim();
    }

    public void MarkComplete() => IsCompleted = true;
}
