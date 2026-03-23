# Project Management Tool - Visual Component Map

## 🎯 Component Priority Matrix

```
HIGH PRIORITY + CRITICAL DEPENDENCIES
┌─────────────────────────────────────────────────────────────┐
│                                                               │
│  User Management            Project Hierarchy              │
│  ├─ User                   ├─ Project                      │
│  ├─ Role                   ├─ Product                      │
│  ├─ Team                   ├─ SubProject                   │
│  └─ Auth Service           └─ Ticket (Core)                │
│                                                              │
│  ↓ CRITICAL FOUNDATION ↓                                    │
│                                                              │
│  Repository Pattern         Database Context                │
│  ├─ IRepository<T>         ├─ DbContext Config             │
│  ├─ Repository<T>          ├─ Entity Configurations        │
│  ├─ Specifications          └─ Migrations                  │
│  └─ UnitOfWork                                              │
│                                                              │
└─────────────────────────────────────────────────────────────┘

HIGH PRIORITY + DEPENDENT ON FOUNDATION
┌─────────────────────────────────────────────────────────────┐
│                                                               │
│  Ticket Extensions          Sprint Management               │
│  ├─ Comments               ├─ Sprint                       │
│  ├─ Attachments            ├─ Capacity                     │
│  ├─ Dependencies           ├─ Velocity Calculator          │
│  ├─ Labels & Tags          └─ Burndown Service             │
│  └─ Time Tracking                                           │
│                                                              │
│  ↓ CORE SERVICES ↓                                          │
│                                                              │
│  Ticket Service            Project Service                  │
│  ├─ CRUD Operations        ├─ Project CRUD                │
│  ├─ Status Workflow        ├─ Product Management           │
│  ├─ Bulk Operations        └─ SubProject Management        │
│  └─ Activity Logging                                        │
│                                                              │
└─────────────────────────────────────────────────────────────┘

MEDIUM PRIORITY + DEPENDENT ON CORE
┌─────────────────────────────────────────────────────────────┐
│                                                               │
│  Delay Tracking             Notifications                    │
│  ├─ Detection Service      ├─ Config & Rules               │
│  ├─ Indicators             ├─ Email Templates              │
│  ├─ Reports                ├─ Queue & Dispatch             │
│  └─ Escalation Rules       └─ In-App Notifications         │
│                                                              │
│  Email-to-Ticket           Search & Filtering               │
│  ├─ Email Parsing          ├─ Search Service               │
│  ├─ Auto-Creation          ├─ Filter Engine                │
│  └─ Customer Comm          └─ Saved Filters                │
│                                                              │
└─────────────────────────────────────────────────────────────┘

LOWER PRIORITY + DEPENDENT ON CORE
┌─────────────────────────────────────────────────────────────┐
│                                                               │
│  Visualizations             Reports                          │
│  ├─ Kanban Board           ├─ RTM Generator                │
│  ├─ Gantt Chart            ├─ Dependency Matrix            │
│  ├─ Charts (Burn/Velocity) ├─ Costing & Budget             │
│  └─ Dashboards             └─ Export Services              │
│                                                              │
│  API Layer                  Webhooks & Integration           │
│  ├─ REST Controllers       ├─ Webhook Events               │
│  ├─ DTOs & Mapping         ├─ Webhook Delivery             │
│  └─ OpenAPI Docs           └─ External Services            │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔄 Component Dependency Graph

```
User Management
    ↓
Authentication Service
    ↓
Authorization Service
    ├───────────────────────────────┐
    ↓                               ↓
Project Management          Ticket Management
    ├─ Project Service       ├─ Ticket Service
    ├─ Product Service       ├─ Comment Service
    └─ SubProject Service    ├─ Attachment Service
                             ├─ Dependency Service
    ↓                        └─ Time Tracking Service
Sprint Management                  ↓
    ├─ Sprint Service        Activity Logging
    ├─ Capacity Planning     
    ├─ Velocity Calculator   Delay Detection
    └─ Burndown Service      ├─ Detect Overdue
                             ├─ Detect Late Starts
                             ├─ Trigger Escalations
                             └─ Generate Reports
    
    ↓ All services feed into ↓

Notification Service
    ├─ Email Service
    ├─ In-App Notifications
    └─ Notification Queue
    
    ↓ Via ↓
    
Search Service
    ├─ Filter Engine
    └─ Saved Filters
    
    ↓ Used by ↓
    
Dashboard & Reports
    ├─ Kanban Board
    ├─ Gantt Chart
    ├─ Reports Generator
    └─ RTM Generator
    
    ↓ Exposed via ↓
    
REST API
    └─ Webhooks
```

---

## 📊 Entity Relationship Web

```
User ─── 1:1 ─── Profile
 │
 ├─── M:N ─── Team
 │
 ├─── M:N ─── Project (via ProjectUser)
 │             │
 │             └─── 1:N ─── Product
 │                           │
 │                           └─── 1:N ─── SubProject
 │                                        │
 │                                        ├─── 1:N ─── Ticket
 │                                        │             │
 │                                        │             ├─── 1:N ─── Comment
 │                                        │             │
 │                                        │             ├─── M:N ─── User (Assignees)
 │                                        │             │
 │                                        │             ├─── 1:N ─── Attachment
 │                                        │             │
 │                                        │             ├─── M:N ─── Ticket (Dependencies)
 │                                        │             │
 │                                        │             ├─── 1:N ─── Delay
 │                                        │             │
 │                                        │             └─── 1:N ─── TimeLog
 │                                        │
 │                                        ├─── 1:N ─── Sprint
 │                                        │             │
 │                                        │             ├─── M:N ─── Ticket
 │                                        │             │
 │                                        │             └─── 1:N ─── SprintCapacity
 │                                        │
 │                                        └─── 1:N ─── BacklogItem
 │                                                      │
 │                                                      ├─── 1:1 ─── BRD
 │                                                      │
 │                                                      ├─── 1:1 ─── UserStory
 │                                                      │
 │                                                      └─── 1:1 ─── UseCase
 │
 ├─── 1:N ─── Session
 │
 ├─── 1:N ─── LoginAttempt
 │
 ├─── 1:N ─── Comment (CreatedBy)
 │
 └─── 1:N ─── AuditLog

Role ─── M:N ─── Permission

NotificationSetting ─── 1:N ─── NotificationRule
                                │
                                └─── M:N ─── NotificationEvent

EmailTemplate ─── M:N ─── NotificationEvent
```

---

## 🏗️ Layer Architecture

```
┌──────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                     │
│  ┌─────────────────┬──────────────┬─────────────────┐   │
│  │  REST API       │  Web UI      │  SignalR        │   │
│  │  Controllers    │  (Blazor)    │  (Real-time)    │   │
│  └─────────────────┴──────────────┴─────────────────┘   │
└────────────────────────────┬─────────────────────────────┘
                             ↓
┌──────────────────────────────────────────────────────────┐
│                  APPLICATION LAYER                        │
│  ┌──────────────────────────────────────────────────┐   │
│  │  Services (Orchestration & Use Cases)            │   │
│  │  ├─ TicketService                               │   │
│  │  ├─ ProjectService                              │   │
│  │  ├─ NotificationService                         │   │
│  │  ├─ DelayDetectionService                       │   │
│  │  └─ ReportService                               │   │
│  ├──────────────────────────────────────────────────┤   │
│  │  DTOs, Validators, Handlers, Mappers             │   │
│  └──────────────────────────────────────────────────┘   │
└────────────────────────────┬─────────────────────────────┘
                             ↓
┌──────────────────────────────────────────────────────────┐
│                   DOMAIN LAYER                            │
│  ┌──────────────────────────────────────────────────┐   │
│  │  Rich Domain Model (NO Framework Dependencies)   │   │
│  │  ├─ Entities (User, Ticket, Project, etc.)      │   │
│  │  ├─ Value Objects                               │   │
│  │  ├─ Interfaces (IRepository, IService)          │   │
│  │  ├─ Specifications (Query Specs)                │   │
│  │  └─ Domain Rules & Validations                  │   │
│  └──────────────────────────────────────────────────┘   │
└────────────────────────────┬─────────────────────────────┘
                             ↓
┌──────────────────────────────────────────────────────────┐
│              INFRASTRUCTURE LAYER                         │
│  ┌──────────────────────────────────────────────────┐   │
│  │  Repositories, UnitOfWork, DbContext             │   │
│  │  External Services (Email, Backup, etc.)        │   │
│  │  Middleware, Caching, Logging                    │   │
│  │  ┌─────────────────────────────────────────┐    │   │
│  │  │  Data Access (EF Core, SQL)             │    │   │
│  │  │  ├─ DbContext Configuration             │    │   │
│  │  │  ├─ Entity Configurations               │    │   │
│  │  │  └─ Migrations                          │    │   │
│  │  └─────────────────────────────────────────┘    │   │
│  └──────────────────────────────────────────────────┘   │
└────────────────────────────┬─────────────────────────────┘
                             ↓
        ┌────────────────────────────────────────┐
        │  Database (SQL Server / PostgreSQL)    │
        │  Email Service                         │
        │  External APIs                         │
        │  File Storage                          │
        └────────────────────────────────────────┘
```

---

## 🔄 Data Flow Examples

### Creating a Ticket
```
User Request
    ↓
API Controller (POST /api/tickets)
    ↓
CreateTicketDto Validator
    ↓
TicketService.CreateTicket()
    ├─ Create Ticket Entity
    ├─ Set Default Values
    ├─ Apply Business Rules
    ├─ Generate Ticket ID (ProjectCode + Sequence)
    └─ Save via Repository
        ↓
    UnitOfWork.SaveChangesAsync()
        ↓
    DbContext.Tickets.Add()
        ↓
    Database
        ↓
    Return TicketId to Controller
        ↓
    Queue Notification (TicketCreated)
        ├─ Email Service
        └─ In-App Notification
            ↓
    Return Response to User
```

### Detecting & Escalating a Delay
```
Scheduled Job (Every hour)
    ↓
DelayDetectionService.DetectOverdueTickets()
    ├─ Query Tickets where DueDate < Today
    ├─ Check Status != Completed/Closed/Cancelled
    ├─ Create/Update Delay records
    └─ Determine Escalation Level
        ├─ 0 days: Level 1 (Assignee)
        ├─ 3+ days: Level 2 (Team Lead, PM)
        └─ 7+ days: Level 3 (Director)
            ↓
    EscalationService.TriggerEscalation()
        ├─ Fetch EscalationRules
        ├─ Resolve Recipients
        └─ Queue Notifications
            ├─ Email (via EmailService)
            ├─ In-App Notification
            └─ Webhook (if configured)
                ↓
    Update Ticket DelayIndicator
        ├─ Mark as Overdue
        ├─ Show badge on UI
        └─ Include in Delays Dashboard
            ↓
    Log to AuditLog
```

### Generating a Report
```
User Request
    ↓
ReportController.GenerateReport()
    ↓
ReportService.GenerateRTM() [Example]
    ├─ Fetch All BacklogItems (filtered)
    ├─ Fetch All Associated Tickets
    ├─ Fetch All TestCases
    ├─ Cross-reference & Match
    ├─ Identify Coverage Gaps
    └─ Generate Report Object
        ├─ Cache the result
        └─ Queue Export Jobs
            ├─ PDF Converter
            ├─ Excel Generator
            └─ CSV Serializer
                ↓
    Return Report to User with Download Links
```

---

## 📈 Scalability Layers

```
TIER 1: Single Instance (0-50 users)
├─ Application Server (1)
├─ Database Server (1)
└─ File Storage (Local or S3)

TIER 2: Redundancy (50-200 users)
├─ Load Balancer
├─ Application Servers (2+)
├─ Database (Primary + Replica)
├─ Redis Cache
└─ S3 / Cloud Storage

TIER 3: Distributed (200-1000 users)
├─ CDN (Static Assets)
├─ Load Balancer (Global)
├─ App Servers (Auto-scaling)
├─ Database (Sharded)
├─ Redis Cluster
├─ Message Queue (RabbitMQ)
└─ Search Engine (Elasticsearch)

TIER 4: Enterprise (1000+ users)
├─ Multi-region Deployment
├─ Database Replication
├─ Microservices
├─ API Gateway
├─ Message Bus
├─ Monitoring & Alerting
└─ DLP & Security
```

---

## 🧪 Testing Strategy

```
Unit Tests (Domain & Services)
├─ Entity validations
├─ Service logic
├─ Business rule enforcement
└─ Calculation correctness

Integration Tests (Repositories & DbContext)
├─ Database operations
├─ Entity relationships
├─ Migrations
└─ Query performance

API Tests (Controllers & Endpoints)
├─ Request/response validation
├─ Status codes
├─ Error handling
└─ Authorization

E2E Tests (Full workflows)
├─ User registration
├─ Create project → ticket → complete
├─ Delay detection & escalation
├─ Report generation
└─ Email-to-ticket

Performance Tests
├─ Load testing (200 concurrent users)
├─ Query performance (< 200ms)
├─ Search performance (< 1 second)
└─ Report generation (< 5 seconds)
```

---

## 🔐 Security Layers

```
┌─────────────────────────────────┐
│   EXTERNAL THREAT              │
│   (Internet)                    │
└──────────────┬──────────────────┘
               │
               ↓ TLS 1.2+
┌──────────────────────────────────┐
│   API Gateway / WAF              │
│   - Rate limiting                │
│   - DDoS protection              │
│   - IP filtering                 │
└──────────────┬───────────────────┘
               │
               ↓ Authentication
┌──────────────────────────────────┐
│   Authentication Layer           │
│   - JWT Tokens                   │
│   - Session Management           │
│   - MFA Verification             │
└──────────────┬───────────────────┘
               │
               ↓ Authorization
┌──────────────────────────────────┐
│   RBAC / ABAC Layer              │
│   - Role checks                  │
│   - Permission validation        │
│   - Resource-level access        │
└──────────────┬───────────────────┘
               │
               ↓ Validation
┌──────────────────────────────────┐
│   Input Validation               │
│   - XSS prevention               │
│   - SQL injection prevention     │
│   - CSRF protection              │
└──────────────┬───────────────────┘
               │
               ↓ Encryption
┌──────────────────────────────────┐
│   Data Protection                │
│   - Field-level encryption       │
│   - Sensitive data masking       │
│   - Audit logging                │
└──────────────┬───────────────────┘
               │
               ↓
┌──────────────────────────────────┐
│   Database                       │
│   - AES-256 encryption at rest   │
│   - Access controls              │
│   - Backup security              │
└──────────────────────────────────┘
```

---

## 🎯 Implementation Checklist Template

### Component: [Name]

```
Pre-Implementation:
☐ Review requirements in project_description.txt
☐ Identify dependencies
☐ Check COMPONENT_BREAKDOWN.md for specs
☐ Plan database schema

Development:
☐ Create entity/entities
☐ Add DbContext configuration
☐ Create repository interface/implementation
☐ Create service interface/implementation
☐ Create DTOs
☐ Create validators
☐ Write unit tests (target: 90%+ coverage)
☐ Create API controller(s)
☐ Write API tests

Integration:
☐ Update Dependency Injection
☐ Add database migration
☐ Test with other components
☐ Update OpenAPI documentation
☐ Performance testing (if applicable)

Documentation:
☐ Add XML comments
☐ Update README with new features
☐ Document breaking changes (if any)
☐ Add usage examples

Deployment:
☐ Code review approval
☐ Merge to main branch
☐ Run full test suite
☐ Deploy to staging
☐ Smoke testing on staging
☐ Deploy to production
```

---

## 📚 Reference Quick Links

| Need | Reference |
|------|-----------|
| Component Specs | → COMPONENT_BREAKDOWN.md |
| Sprint Plan | → IMPLEMENTATION_ROADMAP.md |
| Entity Lookup | → QUICK_REFERENCE.md |
| Original Requirements | → project_description.txt |
| Architecture | → This document |
| Project Overview | → PROJECT_SUMMARY.md |

---

**Ready to start building? Pick an entity from QUICK_REFERENCE.md and create it!**
