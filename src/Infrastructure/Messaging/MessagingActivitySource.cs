using System.Diagnostics;

namespace Template.Infrastructure.Messaging;

public static class MessagingActivitySource
{
    public const string Name = "Template.Infrastructure.Messaging";
    public static readonly ActivitySource Instance = new(Name);
}
