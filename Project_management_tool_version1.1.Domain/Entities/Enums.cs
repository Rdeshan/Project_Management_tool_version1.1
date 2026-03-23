namespace ProjectManagementTool.Domain.Entities;

public enum UserStatus
{
    Active = 1,
    Inactive = 2,
    Locked = 3
}

public enum ProjectStatus
{
    Active = 1,
    OnHold = 2,
    Completed = 3,
    Archived = 4
}

public enum ProductStatus
{
    Planned = 1,
    InDevelopment = 2,
    InTesting = 3,
    Released = 4,
    Deprecated = 5
}

public enum SubProjectStatus
{
    NotStarted = 1,
    InProgress = 2,
    InReview = 3,
    Completed = 4
}

public enum TicketCategory
{
    Task = 1,
    Bug = 2,
    Feature = 3,
    Improvement = 4,
    ChangeRequest = 5,
    UserStory = 6,
    TestCase = 7
}

public enum TicketPriority
{
    Critical = 1,
    High = 2,
    Medium = 3,
    Low = 4
}

public enum TicketStatus
{
    Open = 1,
    NotStarted = 2,
    Implementing = 3,
    Paused = 4,
    InReview = 5,
    Qa = 6,
    Uat = 7,
    Completed = 8,
    Closed = 9,
    Cancelled = 10,
    Reopened = 11
}
