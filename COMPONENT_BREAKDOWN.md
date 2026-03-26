# Project Management Tool v2.2 - Component Breakdown

## Overview
A comprehensive project management system with a 4-level hierarchy (Project → Product → Sub-Project → Ticket) supporting team collaboration, sprint planning, delay tracking, and advanced reporting.

---

## ARCHITECTURE LAYERS

### 1. **Domain Layer** (`Project_management_tool_version1.1.Domain`)
Business logic, entities, and domain rules

### 2. **Application Layer** (`Project_management_tool_version1.1.Application`)
Use cases, DTOs, validators, and application services

### 3. **Infrastructure Layer** (`Project_management_tool_version1.1.Infrastructure`)
Data access, external services, and technical implementations

### 4. **Presentation Layer** (To be created)
Web API, Controllers, Views

---

## MINI-COMPONENTS BREAKDOWN

### **TIER 1: CORE DOMAIN ENTITIES** (Priority: CRITICAL)

#### 1.1 User Management System
- **Component**: `User`, `Role`, `UserRole`, `UserTeam`, `TeamMembership`
- **Responsibilities**:
  - User authentication (Email/OAuth/MFA)
  - Role-based access control (Admin, PM, Developer, QA, BA, Viewer, Guest)
  - Team assignments
  - User profiles and preferences
  - Account lockout and password management
- **Storage**: Database entities
- **Files to Create**:
  - `Domain/Entities/User/User.cs`
  - `Domain/Entities/User/Role.cs`
  - `Domain/Entities/User/Team.cs`
  - `Domain/Entities/User/UserRole.cs`
  - `Domain/Entities/User/UserTeam.cs`

#### 1.2 Project Hierarchy
- **Component**: `Project`, `Product`, `SubProject`, `ProjectCode`
- **Responsibilities**:
  - 4-level hierarchy structure
  - Project status (Active, On Hold, Completed, Archived)
  - Team assignments to projects
  - Project metadata (name, description, client, dates, avatar, color)
- **Files to Create**:
  - `Domain/Entities/Projects/Project.cs`
  - `Domain/Entities/Projects/Product.cs`
  - `Domain/Entities/Projects/SubProject.cs`
  - `Domain/Entities/Projects/ProjectCode.cs`

#### 1.3 Ticket System (Core)
- **Component**: `Ticket`, `TicketStatus`, `TicketCategory`, `Priority`
- **Responsibilities**:
  - Ticket lifecycle management
  - Status workflows (Open, Not Started, Implementing, Paused, In Review, QA, UAT, Completed, Closed, Cancelled, Reopened)
  - Ticket categories (Task, Bug, Feature, Improvement, Change Request, User Story, Test Case)
  - Priority levels (Critical, High, Medium, Low)
  - Custom status support
- **Files to Create**:
  - `Domain/Entities/Tickets/Ticket.cs`
  - `Domain/Entities/Tickets/TicketStatus.cs`
  - `Domain/Entities/Tickets/TicketCategory.cs`
  - `Domain/Entities/Tickets/Priority.cs`

#### 1.4 Sprint Management
- **Component**: `Sprint`, `SprintCapacity`, `MemberCapacity`
- **Responsibilities**:
  - Sprint creation and closure
  - Sprint goals and capacity planning
  - Story points tracking
  - Sprint-ticket associations
  - Velocity calculations
- **Files to Create**:
  - `Domain/Entities/Sprints/Sprint.cs`
  - `Domain/Entities/Sprints/SprintCapacity.cs`
  - `Domain/Entities/Sprints/MemberCapacity.cs`

#### 1.5 Backlog & Requirements
- **Component**: `BacklogItem`, `BRD`, `UserStory`, `UseCase`, `Epic`, `ChangeRequest`
- **Responsibilities**:
  - Project-level and product-level backlogs
  - Requirements documentation
  - Backlog item status (Draft, Approved, In Progress, Done)
  - Version history for requirements
  - Traceability linking
- **Files to Create**:
  - `Domain/Entities/Backlog/BacklogItem.cs`
  - `Domain/Entities/Backlog/BRD.cs`
  - `Domain/Entities/Backlog/UserStory.cs`
  - `Domain/Entities/Backlog/UseCase.cs`

---

### **TIER 2: TICKET FEATURES & EXTENSIONS** (Priority: HIGH)

#### 2.1 Ticket Fields & Metadata
- **Component**: `TicketField`, `TicketAssignee`, `TicketLabel`, `TicketTag`
- **Responsibilities**:
  - All ticket fields (title, description, dates, effort, severity, environment)
  - Multi-assignee support
  - Label/tag system for categorization
  - Bug-specific fields (severity, steps to reproduce, environment, expected vs. actual)
- **Files to Create**:
  - `Domain/Entities/Tickets/TicketAssignee.cs`
  - `Domain/Entities/Tickets/TicketLabel.cs`
  - `Domain/Entities/Tickets/TicketTag.cs`

#### 2.2 Ticket Attachments & Media
- **Component**: `Attachment`, `AttachmentVersion`, `AttachmentType`
- **Responsibilities**:
  - File upload support (max 25 MB)
  - Image/screenshot support (max 10 MB, PNG, JPG, GIF, WEBP)
  - Document preview (PDF, DOCX, XLSX, PPTX)
  - External link attachments
  - Version control for re-uploaded files
- **Files to Create**:
  - `Domain/Entities/Attachments/Attachment.cs`
  - `Domain/Entities/Attachments/AttachmentVersion.cs`
  - `Domain/Entities/Attachments/AttachmentType.cs`

#### 2.3 Ticket Relationships & Dependencies
- **Component**: `TicketDependency`, `LinkedTicket`, `BlockingRelationship`
- **Responsibilities**:
  - Ticket-to-ticket links (blocks, is blocked by, relates to, duplicates)
  - Sub-project dependencies
  - Dependency blocking and auto-blocking of overdue items
- **Files to Create**:
  - `Domain/Entities/Tickets/TicketDependency.cs`
  - `Domain/Entities/Tickets/LinkedTicket.cs`

#### 2.4 Ticket Comments & Collaboration
- **Component**: `Comment`, `CommentReaction`, `Mention`, `InternalNote`
- **Responsibilities**:
  - Threaded comments with markdown support
  - Emoji reactions
  - @mention notifications
  - Internal notes (team-only visibility)
  - Activity log tracking
- **Files to Create**:
  - `Domain/Entities/Comments/Comment.cs`
  - `Domain/Entities/Comments/CommentReaction.cs`
  - `Domain/Entities/Comments/Mention.cs`
  - `Domain/Entities/Comments/InternalNote.cs`

#### 2.5 Ticket Watchers & Subscriptions
- **Component**: `TicketWatcher`, `Subscription`
- **Responsibilities**:
  - Ticket subscription management
  - Watcher tracking
  - Notification preferences per watcher
- **Files to Create**:
  - `Domain/Entities/Tickets/TicketWatcher.cs`

#### 2.6 Ticket Time Tracking & Effort
- **Component**: `TimeLog`, `EffortTracking`, `BurndownData`
- **Responsibilities**:
  - Time logging per ticket
  - Effort estimation (story points or hours)
  - Actual vs. estimated comparisons
- **Files to Create**:
  - `Domain/Entities/TimeTracking/TimeLog.cs`
  - `Domain/Entities/TimeTracking/EffortTracking.cs`

---

### **TIER 3: DELAY TRACKING SYSTEM** (Priority: HIGH)

#### 3.1 Delay Detection & Indicators
- **Component**: `Delay`, `DelayType`, `DelayIndicator`, `DelayReason`
- **Responsibilities**:
  - Detect overdue tickets
  - Detect late starts
  - Extended paused tickets
  - Sprint overruns
  - Milestone at-risk detection
  - Blocked unresolved tickets
  - Automatic badge generation (Overdue, Not Started - Late)
  - Revised due date support
- **Files to Create**:
  - `Domain/Entities/Delays/Delay.cs`
  - `Domain/Entities/Delays/DelayType.cs`
  - `Domain/Entities/Delays/DelayIndicator.cs`
  - `Domain/Entities/Delays/DelayReason.cs`

#### 3.2 Delay Reports & Analytics
- **Component**: `DelayReport`, `DelayAnalytics`, `OnTimeDeliveryMetric`
- **Responsibilities**:
  - Delay history reporting
  - Delay frequency by team
  - Average delay duration
  - On-time delivery rate trends
  - Export to PDF, CSV, Excel
- **Files to Create**:
  - `Domain/Entities/Reports/DelayReport.cs`

#### 3.3 Delay Dashboards
- **Component**: `DelayDashboard`, `DelayFilter`
- **Responsibilities**:
  - Project-level delay overview
  - Filterable by delay type, team, sub-project, assignee
  - Kanban/Gantt risk highlighting
  - Sprint/Milestone delay indicators
- **Files to Create**:
  - `Domain/Entities/Dashboards/DelayDashboard.cs`

#### 3.4 Delay Escalation Rules
- **Component**: `EscalationRule`, `EscalationThreshold`, `EscalationAction`
- **Responsibilities**:
  - Configure escalation recipients
  - Immediate, progressive, and repeated escalation
  - Escalation suppression logic
  - Escalation suppression when revised dates set
- **Files to Create**:
  - `Domain/Entities/Escalation/EscalationRule.cs`
  - `Domain/Entities/Escalation/EscalationThreshold.cs`
  - `Domain/Entities/Escalation/EscalationAction.cs`

---

### **TIER 4: NOTIFICATIONS SYSTEM** (Priority: HIGH)

#### 4.1 Notification Configuration
- **Component**: `NotificationSetting`, `NotificationEvent`, `NotificationRule`
- **Responsibilities**:
  - Organization-level default settings
  - Project-level settings (override defaults)
  - User-level preferences
  - Event types (25+ configurable events)
  - Recipient types (Assignee, Reporter, Watchers, Team Lead, PM, Director, etc.)
  - Delivery channels (In-App, Email, Both)
  - Delivery timing (Immediate, Daily Digest)
- **Files to Create**:
  - `Domain/Entities/Notifications/NotificationSetting.cs`
  - `Domain/Entities/Notifications/NotificationEvent.cs`
  - `Domain/Entities/Notifications/NotificationRule.cs`

#### 4.2 Notification Templates & Content
- **Component**: `EmailTemplate`, `EmailContentTemplate`, `NotificationTemplate`
- **Responsibilities**:
  - Configurable email subject templates
  - Configurable email body templates
  - Dynamic variable substitution ({ticket_id}, {ticket_title}, {days_overdue}, etc.)
  - Custom introductory messages
  - Branding (logo, colors)
  - Reply-to address configuration
- **Files to Create**:
  - `Domain/Entities/Notifications/EmailTemplate.cs`
  - `Domain/Entities/Notifications/NotificationTemplate.cs`

#### 4.3 Notification Queue & Delivery
- **Component**: `NotificationQueue`, `QueuedNotification`, `NotificationDelivery`
- **Responsibilities**:
  - Queue in-app and email notifications
  - Batch processing for digest emails
  - Delivery status tracking (Sent, Delivered, Bounced, Opened)
  - Bounce handling and email validation
  - Quiet hours support (suppress emails in off-hours)
- **Files to Create**:
  - `Domain/Entities/Notifications/NotificationQueue.cs`
  - `Domain/Entities/Notifications/QueuedNotification.cs`
  - `Domain/Entities/Notifications/NotificationDelivery.cs`

#### 4.4 In-App Notifications
- **Component**: `InAppNotification`, `NotificationBell`, `NotificationPanel`
- **Responsibilities**:
  - In-app notification display
  - Mark as read functionality
  - Notification bell with unread count
  - Notification history
- **Files to Create**:
  - `Domain/Entities/Notifications/InAppNotification.cs`

#### 4.5 Notification Audit & Logs
- **Component**: `NotificationLog`, `EmailLog`, `BounceLog`
- **Responsibilities**:
  - Log all sent notifications
  - Email delivery status tracking
  - Bounce handling and logging
  - Audit trail of notification setting changes
  - User unsubscribe tracking
- **Files to Create**:
  - `Domain/Entities/Notifications/NotificationLog.cs`
  - `Domain/Entities/Notifications/EmailLog.cs`
  - `Domain/Entities/Notifications/BounceLog.cs`

---

### **TIER 5: CUSTOMER BUG REPORTING (EMAIL-TO-TICKET)** (Priority: MEDIUM)

#### 5.1 Email Intake System
- **Component**: `EmailInbox`, `EmailTemplate`, `EmailParsing`
- **Responsibilities**:
  - Project-specific intake email addresses (bugs.projectcode@company.com)
  - Template distribution to customers
  - Email parsing and field mapping
  - Auto-reply with ticket ID
  - Invalid format detection and handling
- **Files to Create**:
  - `Domain/Entities/EmailIntake/EmailInbox.cs`
  - `Domain/Entities/EmailIntake/IncomingEmail.cs`
  - `Domain/Entities/EmailIntake/EmailParseResult.cs`

#### 5.2 Automatic Ticket Creation
- **Component**: `AutoTicketCreator`, `TemplateFieldMapper`, `DuplicateDetector`
- **Responsibilities**:
  - Auto-create bug tickets from parsed emails
  - Default assignment (to team lead or QA team)
  - Automatic categorization as Bug with Open status
  - Attachment handling
  - Duplicate detection
- **Files to Create**:
  - `Domain/Entities/EmailIntake/EmailTicketMapping.cs`

#### 5.3 Customer Communication
- **Component**: `CustomerTicketUpdater`, `CustomerNotificationHandler`
- **Responsibilities**:
  - Send status update emails to customers
  - Send resolution confirmation emails
  - No login required customer experience
- **Files to Create**:
  - `Domain/Services/Contracts/ICustomerNotificationService.cs`

---

### **TIER 6: VISUALIZATION & REPORTING** (Priority: MEDIUM)

#### 6.1 Gantt Charts
- **Component**: `GanttChart`, `GanttBar`, `GanttDependencyLine`, `CriticalPath`
- **Responsibilities**:
  - Ticket-level Gantt visualization
  - Sub-project Gantt
  - Product Gantt
  - Dependency visualization
  - Critical path highlighting
  - Drag-to-reschedule functionality
  - Today marker
  - Export to PDF/PNG
- **Files to Create**:
  - `Domain/Entities/Visualizations/GanttChart.cs`
  - `Domain/Entities/Visualizations/GanttBar.cs`

#### 6.2 Kanban Boards
- **Component**: `KanbanBoard`, `KanbanColumn`, `KanbanCard`, `WIPLimit`
- **Responsibilities**:
  - Kanban board per sub-project and per team
  - Status column mapping
  - Drag-and-drop tickets between columns
  - WIP limits per column
  - Swimlanes (by assignee, team, priority, epic)
  - Card customization
  - Real-time updates
- **Files to Create**:
  - `Domain/Entities/Visualizations/KanbanBoard.cs`
  - `Domain/Entities/Visualizations/KanbanColumn.cs`
  - `Domain/Entities/Visualizations/KanbanCard.cs`

#### 6.3 Charts & Metrics
- **Component**: `BurndownChart`, `VelocityChart`, `SummaryChart`, `TrendChart`
- **Responsibilities**:
  - Sprint burndown (ideal vs. actual points)
  - Velocity charts (story points per sprint)
  - Status distribution charts
  - Category breakdown charts
  - Team workload charts
  - Milestone progress bars
  - Bug trend charts
  - Delivery forecasting
- **Files to Create**:
  - `Domain/Entities/Visualizations/BurndownChart.cs`
  - `Domain/Entities/Visualizations/VelocityChart.cs`
  - `Domain/Entities/Visualizations/SummaryChart.cs`

#### 6.4 Reports
- **Component**: `Report`, `RTM`, `DependencyMatrix`, `CostingReport`, `AdditionalReports`
- **Responsibilities**:
  - Requirements Traceability Matrix (RTM)
  - Dependency Matrix
  - Costing & Budget Report
  - Sprint Report
  - Bug Report
  - Workload Report
  - Ticket Age Report
  - Change Request Log
  - Export to PDF, CSV, Excel
  - Scheduled delivery of reports
- **Files to Create**:
  - `Domain/Entities/Reports/Report.cs`
  - `Domain/Entities/Reports/RTM.cs`
  - `Domain/Entities/Reports/DependencyMatrix.cs`
  - `Domain/Entities/Reports/CostingReport.cs`

#### 6.5 Ticket Search & Filtering
- **Component**: `TicketSearch`, `TicketFilter`, `SavedFilter`, `SearchIndex`
- **Responsibilities**:
  - Global full-text search (< 1 second for 10k tickets)
  - Multi-field filtering
  - Saved filter views
  - Quick filter presets
  - Attachment file name search
- **Files to Create**:
  - `Domain/Entities/Search/TicketSearch.cs`
  - `Domain/Entities/Search/TicketFilter.cs`
  - `Domain/Entities/Search/SavedFilter.cs`

---

### **TIER 7: AUTHENTICATION & SECURITY** (Priority: CRITICAL)

#### 7.1 Authentication
- **Component**: `AuthenticationService`, `LoginAttempt`, `SessionManagement`
- **Responsibilities**:
  - Email/password login
  - OAuth SSO (Google, Microsoft)
  - MFA (authenticator app)
  - Session timeout (configurable, default 30 min)
  - Password reset (time-limited email link)
  - Account lockout (10 failed attempts in 15 min)
  - Password hashing and validation
- **Files to Create**:
  - `Domain/Entities/Auth/LoginAttempt.cs`
  - `Domain/Entities/Auth/Session.cs`
  - `Domain/Entities/Auth/PasswordReset.cs`

#### 7.2 Authorization & RBAC
- **Component**: `Permission`, `RolePermission`, `ResourceAccess`
- **Responsibilities**:
  - Permission-based access control
  - Role-based permission assignment
  - Project-level role assignments
  - Resource-level access checks
- **Files to Create**:
  - `Domain/Entities/Auth/Permission.cs`
  - `Domain/Entities/Auth/RolePermission.cs`

#### 7.3 Audit & Compliance
- **Component**: `AuditLog`, `ActivityLog`, `UserAction`
- **Responsibilities**:
  - Log all user actions with identity and timestamp
  - Track affected records
  - 12-month retention
  - OWASP Top 10 compliance checks
- **Files to Create**:
  - `Domain/Entities/Audit/AuditLog.cs`
  - `Domain/Entities/Audit/ActivityLog.cs`

---

### **TIER 8: DATA ACCESS & PERSISTENCE** (Priority: CRITICAL)

#### 8.1 Database Context & Mapping
- **Component**: `ApplicationDbContext`, `EntityConfigurations`
- **Responsibilities**:
  - EF Core DbContext setup
  - Entity configurations and relationships
  - Database migrations
  - Encryption at rest (AES-256)
  - Backup configurations
- **Files to Create**:
  - `Infrastructure/Data/ApplicationDbContext.cs` (update)
  - `Infrastructure/Data/Configurations/*`
  - `Infrastructure/Data/Migrations/*`

#### 8.2 Repositories
- **Component**: `Repository<T>`, `UnitOfWork`
- **Responsibilities**:
  - Generic repository pattern implementation
  - CRUD operations
  - Query specifications
  - Unit of Work pattern
- **Files to Create**:
  - `Infrastructure/Repositories/Repository.cs`
  - `Infrastructure/Repositories/UnitOfWork.cs`
  - `Infrastructure/Repositories/IRepository.cs`
  - `Infrastructure/Repositories/IUnitOfWork.cs`

#### 8.3 Database Backup & Recovery
- **Component**: `BackupService`, `RecoveryService`
- **Responsibilities**:
  - Automated backups every 6 hours
  - 30-day retention
  - RTO: 4 hours
  - RPO: 6 hours
- **Files to Create**:
  - `Infrastructure/Services/BackupService.cs`
  - `Infrastructure/Services/RecoveryService.cs`

---

### **TIER 9: API & INTEGRATION** (Priority: MEDIUM)

#### 9.1 RESTful API
- **Component**: Controllers, Endpoints, OpenAPI Documentation
- **Responsibilities**:
  - RESTful API endpoints for all core features
  - OpenAPI 3.0 specification
  - API versioning (v1, v2, etc.)
  - API response formatting
  - Error handling and status codes
- **Files to Create**:
  - `API/Controllers/*`
  - `API/Dtos/*`
  - `API/Specifications/openapi.json`

#### 9.2 Webhook Support
- **Component**: `WebhookEndpoint`, `WebhookEvent`, `WebhookDelivery`
- **Responsibilities**:
  - Configure webhook URLs
  - POST event notifications to webhooks
  - Webhook delivery retry logic
  - Webhook payload formatting
- **Files to Create**:
  - `Domain/Entities/Integrations/WebhookEndpoint.cs`
  - `Domain/Entities/Integrations/WebhookEvent.cs`
  - `Domain/Entities/Integrations/WebhookDelivery.cs`

---

### **TIER 10: BUSINESS LOGIC & SERVICES** (Priority: HIGH)

#### 10.1 Ticket Management Service
- **Component**: `TicketService`, `TicketCreationHandler`, `TicketStatusTransition`
- **Responsibilities**:
  - Create/update/delete tickets
  - Status transition rules
  - Workflow validation
  - Bulk operations
  - Auto-ID generation
- **Files to Create**:
  - `Application/Services/TicketService.cs`
  - `Application/Handlers/CreateTicketHandler.cs`
  - `Application/Handlers/UpdateTicketStatusHandler.cs`

#### 10.2 Project Management Service
- **Component**: `ProjectService`, `ProductService`, `SubProjectService`
- **Responsibilities**:
  - Project/Product/Sub-Project CRUD
  - Hierarchy validation
  - Team assignment
  - Archive functionality
- **Files to Create**:
  - `Application/Services/ProjectService.cs`
  - `Application/Services/ProductService.cs`
  - `Application/Services/SubProjectService.cs`

#### 10.3 Sprint Service
- **Component**: `SprintService`, `SprintClosureHandler`, `VelocityCalculator`
- **Responsibilities**:
  - Sprint creation and management
  - Capacity planning
  - Sprint closure with disposition
  - Velocity calculations
  - Burndown calculations
- **Files to Create**:
  - `Application/Services/SprintService.cs`
  - `Application/Handlers/CloseSprintHandler.cs`
  - `Application/Services/VelocityCalculator.cs`

#### 10.4 Delay Tracking Service
- **Component**: `DelayDetectionService`, `DelayEscalationHandler`
- **Responsibilities**:
  - Detect delays automatically
  - Calculate delay metrics
  - Trigger escalation notifications
  - Update delay indicators
- **Files to Create**:
  - `Application/Services/DelayDetectionService.cs`
  - `Application/Handlers/DelayEscalationHandler.cs`

#### 10.5 Notification Service
- **Component**: `NotificationService`, `EmailService`, `NotificationDispatcher`
- **Responsibilities**:
  - Queue notifications
  - Send in-app notifications
  - Send email notifications
  - Batch digest emails
  - Handle unsubscribes
  - Apply quiet hours
- **Files to Create**:
  - `Application/Services/NotificationService.cs`
  - `Infrastructure/Services/EmailService.cs`
  - `Application/Services/NotificationDispatcher.cs`

#### 10.6 Report Generation Service
- **Component**: `ReportService`, `RTMGenerator`, `CostingCalculator`
- **Responsibilities**:
  - Generate RTM
  - Generate dependency matrix
  - Calculate costing & budget
  - Generate all report types
  - Export to PDF, CSV, Excel
- **Files to Create**:
  - `Application/Services/ReportService.cs`
  - `Application/Services/RTMGenerator.cs`
  - `Application/Services/CostingCalculator.cs`

---

### **TIER 11: CROSS-CUTTING CONCERNS** (Priority: MEDIUM)

#### 11.1 Caching
- **Component**: `CacheService`, `CacheKeys`, `CacheInvalidation`
- **Responsibilities**:
  - Cache frequently accessed data
  - Cache invalidation strategies
  - Distributed caching support
- **Files to Create**:
  - `Infrastructure/Caching/CacheService.cs`
  - `Infrastructure/Caching/CacheKeys.cs`

#### 11.2 Logging & Monitoring
- **Component**: `Logger`, `PerformanceMonitor`, `HealthCheck`
- **Responsibilities**:
  - Structured logging
  - Performance monitoring
  - Health checks
  - Error tracking
- **Files to Create**:
  - `Infrastructure/Logging/LoggingService.cs`
  - `Infrastructure/Monitoring/PerformanceMonitor.cs`

#### 11.3 Validation & Error Handling
- **Component**: `Validator`, `ErrorHandler`, `ExceptionMiddleware`
- **Responsibilities**:
  - Input validation
  - Business rule validation
  - Error response formatting
  - Global exception handling
- **Files to Create**:
  - `Application/Validators/*`
  - `Infrastructure/Middleware/ExceptionMiddleware.cs`

#### 11.4 Scheduling & Background Jobs
- **Component**: `ScheduledJob`, `BackgroundWorker`, `JobScheduler`
- **Responsibilities**:
  - Schedule periodic tasks
  - Background job execution
  - Delay detection (recurring)
  - Report generation (scheduled)
  - Email sending queue
- **Files to Create**:
  - `Infrastructure/Services/JobScheduler.cs`
  - `Infrastructure/BackgroundJobs/DelayDetectionJob.cs`
  - `Infrastructure/BackgroundJobs/EmailQueueJob.cs`

---

## BUILD ORDER (RECOMMENDED SEQUENCE)

### **Phase 1: Foundation (Week 1-2)**
1. Core Domain Entities (Users, Projects, Tickets, Sprints)
2. Database Context & Migrations
3. Repository Pattern & Unit of Work
4. Authentication & Authorization

### **Phase 2: Core Features (Week 3-4)**
1. Ticket Management Service
2. Project/Product/Sub-Project Management
3. Sprint Management
4. Backlog & Requirements

### **Phase 3: Advanced Features (Week 5-6)**
1. Delay Tracking System
2. Notifications System
3. Email-to-Ticket Integration
4. Visualization (Kanban, Gantt, Charts)

### **Phase 4: Reporting & Polish (Week 7-8)**
1. Reports & RTM
2. Search & Filtering
3. API Endpoints
4. Testing & Optimization

### **Phase 5: Integration & Deployment (Week 9+)**
1. Webhook Support
2. External Service Integration
3. Performance Optimization
4. Security Hardening

---

## TECHNICAL REQUIREMENTS SUMMARY

| Requirement | Target |
|------------|--------|
| Page Load Time | < 2 seconds (1,000 tickets) |
| Search Response | < 1 second (10,000 tickets) |
| Report Generation | < 5 seconds |
| Concurrent Users | 200+ |
| Uptime | 99.9% |
| Encryption | TLS 1.2+, AES-256 |
| Audit Retention | 12 months |
| Browser Support | Latest 2 versions |
| WCAG Compliance | Level AA 2.1 |

---

## FILES TO CREATE (SUMMARY)

**Total Estimated Files**: 150+

- Domain Entities: 45+
- Application Services: 30+
- Infrastructure Services: 25+
- API Controllers: 20+
- DTOs & Validators: 20+
- Configurations & Mapping: 15+

---

## NEXT STEPS

1. ✅ Component breakdown complete
2. Create all Domain Layer entities
3. Set up Database Context with migrations
4. Implement Repository Pattern
5. Build Application Services iteratively
6. Create API Controllers and endpoints
7. Build UI components
8. Integration Testing
9. Performance Testing & Optimization
10. Deployment

---
