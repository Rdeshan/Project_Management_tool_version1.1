# Quick Reference Guide - PM Tool Components

## 1️⃣ CORE DOMAIN ENTITIES (Must Build First)

### User Management
| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `User` | System user account | Id, Email, Name, Avatar, IsActive, CreatedAt |
| `Role` | Permission role | Id, Name (Admin, PM, Dev, QA, BA, Viewer, Guest) |
| `Team` | Team grouping | Id, Name, Description, TeamLead |
| `UserRole` | User → Role assignment | UserId, RoleId, ProjectId |
| `UserTeam` | User → Team membership | UserId, TeamId |

### Project Hierarchy
| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `Project` | Top-level container | Id, Code, Name, ClientName, Status (Active, OnHold, Completed, Archived) |
| `Product` | Release version | Id, ProjectId, Name, Version, ReleaseType (Major, Minor, Patch, Hotfix), Status |
| `SubProject` | Module/feature | Id, ProductId, Name, Status (NotStarted, InProgress, InReview, Completed), TeamId |
| `ProjectCode` | Unique code generator | Id, ProjectId, Code, NextSequence |

### Ticket System
| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `Ticket` | Work unit | Id, SubProjectId, Title, Description, Category, Priority, Status, CreatedAt, DueDate |
| `TicketStatus` | Status definition | Id, ProjectId, Name, Order (for workflow) |
| `TicketCategory` | Category enum | Task, Bug, Feature, Improvement, ChangeRequest, UserStory, TestCase |
| `Priority` | Priority enum | Critical, High, Medium, Low |
| `TicketAssignee` | Multi-assignee | TicketId, UserId |
| `TicketLabel` | Tag system | Id, Name, TicketId |

### Sprint System
| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `Sprint` | Sprint container | Id, Name, Goal, StartDate, EndDate, CapacityPoints, SubProjectId |
| `SprintCapacity` | Capacity per team | Id, SprintId, TeamId, AllocatedPoints |
| `MemberCapacity` | Per-member availability | Id, SprintId, UserId, AvailabilityPercentage, Notes |

### Backlog & Requirements
| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `BacklogItem` | Requirement | Id, ProjectId/ProductId, Title, Type (BRD, UserStory, UseCase, Epic, ChangeRequest), Status, Priority |
| `BRD` | Business Requirement Doc | Id, BacklogItemId, Content, Owner, Version |
| `UserStory` | User story | Id, BacklogItemId, AsA, IWant, SoThat, AcceptanceCriteria |
| `UseCase` | Use case doc | Id, BacklogItemId, Scenario, StepsToExecute |

---

## 2️⃣ TICKET EXTENSIONS

### Attachments & Media
| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `Attachment` | File attachment | Id, TicketId, FileName, FileSize, FileType, StoragePath, UploadedBy, UploadedAt |
| `AttachmentVersion` | Version history | Id, AttachmentId, Version, StoragePath, UploadedAt |

### Comments & Collaboration
| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `Comment` | Comment on ticket | Id, TicketId, Content, CreatedBy, CreatedAt, IsInternal |
| `CommentReaction` | Emoji reactions | Id, CommentId, UserId, Emoji |
| `Mention` | @mention tracking | Id, CommentId/DescriptionId, MentionedUserId, CreatedAt |

### Relationships
| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `TicketDependency` | Ticket relationship | Id, FromTicketId, ToTicketId, RelationType (Blocks, BlockedBy, RelatesTo, Duplicates) |
| `TicketWatcher` | Subscribers | Id, TicketId, UserId |

### Time & Effort
| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `TimeLog` | Time tracking | Id, TicketId, UserId, Hours, LoggedAt, Description |
| `EffortTracking` | Story points | TicketId, EstimatedPoints, ActualPoints |

---

## 3️⃣ DELAY TRACKING

| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `Delay` | Delay record | Id, TicketId, DelayType (Overdue, NotStartedLate, PausedExtended, etc.), StartDate, EndDate, ReasonId |
| `DelayReason` | Delay explanation | Id, Code, Description |
| `Delay Indicator` | Current status flag | TicketId, IsOverdue, DaysOverdue, RevisedDueDate |
| `EscalationRule` | Escalation config | Id, ProjectId, DelayType, DaysBeforeEscalate, EscalationLevel, RecipientIds |

---

## 4️⃣ NOTIFICATIONS

| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `NotificationSetting` | Global config | Id, ProjectId, Level (Org/Project), DefaultRecipients |
| `NotificationEvent` | Event type | Id, Code (TicketCreated, TicketAssigned, etc.), Name |
| `NotificationRule` | Rule per event | Id, ProjectId, EventId, RecipientTypes[], DeliveryChannel, IsImmediate |
| `EmailTemplate` | Email template | Id, EventId, SubjectTemplate, BodyTemplate, BrandingId |
| `NotificationQueue` | Queue item | Id, EventId, TicketId, RecipientId, IsSent, SentAt |
| `InAppNotification` | In-app message | Id, UserId, Title, Message, IsRead, CreatedAt |

---

## 5️⃣ EMAIL-TO-TICKET

| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `EmailInbox` | Project email | Id, ProjectId, EmailAddress, AssignedTeam |
| `IncomingEmail` | Parsed email | Id, EmailInboxId, FromAddress, Subject, Body, ParsedStatus |
| `EmailTicketMapping` | Email → Ticket | EmailId, TicketId, CreatedAt |

---

## 6️⃣ VISUALIZATIONS & REPORTS

| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `KanbanBoard` | Kanban view | Id, SubProjectId, Columns[] (mapped to statuses) |
| `GanttChart` | Timeline view | Id, SubProjectId/ProductId, Bars[], DependencyLines[] |
| `BurndownChart` | Sprint burndown | Id, SprintId, DayNumber, IdealPoints, ActualPoints |
| `VelocityChart` | Velocity tracking | Id, ProjectId, SprintNumber, PlannedPoints, CompletedPoints |
| `Report` | Generic report | Id, Type, ProjectId, GeneratedAt, Content, ExportFormat |
| `RTM` | Traceability | Id, ProjectId, Requirements[], Tickets[], Coverage |

---

## 7️⃣ AUTHENTICATION & SECURITY

| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| `LoginAttempt` | Login tracking | Id, UserId, AttemptAt, IsSuccess, IpAddress |
| `Session` | Session tracking | Id, UserId, TokenHash, ExpiresAt |
| `PasswordReset` | Reset request | Id, UserId, TokenHash, ExpiresAt, IsUsed |
| `AuditLog` | Action tracking | Id, UserId, Action, ResourceType, ResourceId, Timestamp |
| `Permission` | Permission def | Id, Code (CanCreateTicket, CanAssignTicket, etc.) |
| `RolePermission` | Role → Permission | RoleId, PermissionId |

---

## 8️⃣ SERVICE LAYER (Application)

### Core Services
```csharp
ITicketService
├── CreateTicket()
├── UpdateTicket()
├── UpdateStatus()
├── AssignTicket()
├── AddComment()
├── AddAttachment()
└── LinkDependency()

IProjectService
├── CreateProject()
├── CreateProduct()
├── CreateSubProject()
├── UpdateStatus()
├── AssignTeam()
└── ArchiveProject()

ISprintService
├── CreateSprint()
├── PopulateSprintFromBacklog()
├── CloseSprint()
├── CalculateVelocity()
└── CalculateBurndown()

INotificationService
├── QueueNotification()
├── SendInAppNotification()
├── SendEmailNotification()
├── ProcessEmailQueue()
└── ApplyQuietHours()

IDelayDetectionService
├── DetectOverdueTickets()
├── DetectLateStarts()
├── DetectExtendedPauses()
├── TriggerEscalation()
└── GenerateDelayReport()

IReportService
├── GenerateRTM()
├── GenerateDependencyMatrix()
├── GenerateCostingReport()
├── ExportToPdf()
├── ExportToCsv()
└── ExportToExcel()
```

---

## 9️⃣ DATABASE SCHEMA OVERVIEW

```
Users
├── Roles
├── Teams
├── UserRoles (Many-to-Many)
├── UserTeams (Many-to-Many)
├── Sessions
├── LoginAttempts
└── PasswordResets

Projects
├── Products
│   ├── SubProjects
│   │   ├── Tickets
│   │   │   ├── TicketAssignees (Many-to-Many)
│   │   │   ├── Comments
│   │   │   ├── Attachments
│   │   │   │   └── AttachmentVersions
│   │   │   ├── TicketLabels
│   │   │   ├── Dependencies
│   │   │   ├── Watchers
│   │   │   ├── TimeLogs
│   │   │   └── EffortTracking
│   │   ├── Sprints
│   │   │   ├── SprintCapacities
│   │   │   └── MemberCapacities
│   │   ├── Backlog
│   │   └── Delays
│   └── BacklogItems
│       ├── BRDs
│       ├── UserStories
│       └── UseCases
├── ProjectTeams (Many-to-Many)
└── ProjectUsers (Many-to-Many)

Notifications
├── NotificationSettings
├── NotificationEvents
├── NotificationRules
├── EmailTemplates
├── NotificationQueue
├── InAppNotifications
└── NotificationLogs

Reports
├── Dashboards
├── SavedFilters
├── RTMs
└── DependencyMatrices

Security & Audit
├── Permissions
├── RolePermissions
└── AuditLogs
```

---

## 🔟 FOLDER STRUCTURE TO CREATE

```
Project_management_tool_version1.1.Domain/
├── Entities/
│   ├── Users/
│   ├── Projects/
│   ├── Tickets/
│   ├── Sprints/
│   ├── Backlog/
│   ├── Attachments/
│   ├── Comments/
│   ├── Delays/
│   ├── Notifications/
│   ├── Reports/
│   └── Audit/
├── Enums/
├── Interfaces/
│   ├── Repositories/
│   └── Services/
├── ValueObjects/
└── Specifications/

Project_management_tool_version1.1.Application/
├── Services/
├── DTOs/
│   ├── Tickets/
│   ├── Projects/
│   ├── Sprints/
│   ├── Notifications/
│   └── Reports/
├── Handlers/
├── Validators/
├── Mappers/
└── Interfaces/

Project_management_tool_version1.1.Infrastructure/
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── Configurations/
│   ├── Migrations/
│   └── Repositories/
├── Services/
│   ├── EmailService.cs
│   ├── BackupService.cs
│   └── ... (other infra services)
├── Caching/
├── Logging/
├── Middleware/
└── Seeders/

Project_management_tool_version1.1/
├── Controllers/
│   ├── ProjectController.cs
│   ├── TicketController.cs
│   ├── SprintController.cs
│   ├── NotificationController.cs
│   └── ReportController.cs
├── Program.cs
├── appsettings.json
└── wwwroot/ (static files)
```

---

## ⚡ QUICK START (First 5 Tasks)

1. **Create User.cs** - Basic user entity with email, name, password hash
2. **Create Project.cs** - With Code, Name, Status fields
3. **Create Ticket.cs** - With Title, Description, Status, Priority, DueDate
4. **Create ApplicationDbContext.cs** - DbSets for User, Project, Ticket
5. **Create initial migration** - Run `dotnet ef migrations add Initial`

---

## 📚 Naming Conventions

- **Entities**: Singular, PascalCase (`User`, `Ticket`, `Comment`)
- **Tables**: Plural (`Users`, `Tickets`, `Comments`)
- **Foreign Keys**: `{EntityName}Id` (`UserId`, `TicketId`)
- **Navigation Properties**: Singular for one-to-many parent, Plural for collection (`User.Tickets`, `Ticket.Comments`)
- **Interfaces**: `I{EntityName}Repository`, `I{ServiceName}Service`
- **DTOs**: `Create{EntityName}Dto`, `Update{EntityName}Dto`, `Get{EntityName}Dto`
- **Enums**: Suffix with Enum (`TicketStatusEnum`, `PriorityEnum`)

---

## 🎯 Success Checklist

- [ ] All Domain entities created
- [ ] DbContext configured with all entities
- [ ] Initial migration generated
- [ ] Repository pattern implemented
- [ ] Unit of Work pattern implemented
- [ ] First CRUD service created
- [ ] Basic API controller working
- [ ] Authentication service started
- [ ] First unit tests passing
- [ ] Database seeded with test data

---

**Start with Task #1 and we'll build component by component!**
