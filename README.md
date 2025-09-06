# Catalog API

A production-ready .NET 8 e-commerce catalog API built with Clean Architecture principles, featuring product management, order processing, and robust concurrency control.

## Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Setup Instructions](#setup-instructions)
- [API Documentation](#api-documentation)
- [Design Decisions](#design-decisions)
- [Features](#features)
- [Testing](#testing)

## Overview

The Catalog API is an enterprise-grade e-commerce solution that demonstrates modern .NET development practices. It provides comprehensive product catalog management and order processing capabilities with emphasis on data consistency, scalability, and maintainability.

### Key Capabilities

- **Product Management**: Full CRUD operations with soft/hard delete options
- **Order Processing**: Transactional order creation with stock reservation
- **Concurrency Control**: Optimistic locking and row-level locking for data consistency
- **API Versioning**: Future-proof API design with version management
- **Comprehensive Logging**: Structured logging with Serilog
- **Health Monitoring**: Built-in health checks and monitoring endpoints

## Tech Stack

### Core Technologies

- **.NET 8** - Latest LTS version with performance improvements
- **ASP.NET Core** - Web API framework
- **PostgreSQL** - Robust, ACID-compliant database
- **Entity Framework Core** - ORM with code-first migrations

### Architecture & Patterns

- **Clean Architecture** - Separation of concerns and dependency inversion
- **CQRS** - Command Query Responsibility Segregation with MediatR
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management
- **Domain-Driven Design** - Rich domain models with business logic

### Supporting Libraries

- **MediatR** - CQRS implementation and domain event handling
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **Asp.Versioning** - API versioning support

## Architecture

The solution follows Clean Architecture principles with clear separation of concerns:

```
┌─────────────────┐
│ API Layer       │ ← References: Application, Infrastructure, Contracts
│                 │
├─────────────────┤
│ Application     │ ← References: Domain only
│ Layer           │
├─────────────────┤
│ Domain Layer    │ ← No references to other layers
│                 │
├─────────────────┤
│ Infrastructure  │ ← References: Application, Domain
│ Layer           │
└─────────────────┘
```

### Layer Responsibilities

1. **Domain Layer** (`Catalog.Domain`)

   - Core business logic and entities
   - Domain events for decoupled communication
   - Result pattern for error handling
   - Aggregate roots with encapsulated invariants

2. **Application Layer** (`Catalog.Application`)

   - Use cases and application services
   - CQRS commands and queries
   - Input validation with FluentValidation
   - Domain event handlers

3. **Infrastructure Layer** (`Catalog.Infrastructure`)

   - Data persistence with EF Core
   - Repository implementations
   - External service integrations
   - Database configurations and migrations

4. **Contracts Layer** (`Catalog.Contracts`)

   - API DTOs and request/response models
   - Shared contracts between layers

5. **API Layer** (`Catalog.Api`)
   - RESTful controllers
   - Global exception handling
   - API versioning
   - Health checks and monitoring

## Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 17](https://www.postgresql.org/download/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/IEdiong/Catalog.git
   cd Catalog
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Configure database connection**

   Update `src/Catalog.Api/appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=CatalogDb;Username=postgres;Password=your_password"
     }
   }
   ```

4. **Run database migrations**

   ```bash
   dotnet ef database update --project src/Catalog.Infrastructure --startup-project src/Catalog.Api
   ```

5. **Start the application**

   ```bash
   dotnet run --project src/Catalog.Api
   ```

6. **Access the API**
   - API Base URL: `https://localhost:7279`
   - Swagger UI: `https://localhost:7279/swagger`
   - Health Check: `https://localhost:7279/health`

## API Documentation

### Base URL

```
https://localhost:7001/api/v1
```

### Endpoints

#### Products

| Method   | Endpoint                   | Description                    |
| -------- | -------------------------- | ------------------------------ |
| `GET`    | `/products`                | Get all products (paginated)   |
| `POST`   | `/products`                | Create a new product           |
| `GET`    | `/products/{id}`           | Get product by ID              |
| `PUT`    | `/products/{id}`           | Update a product               |
| `DELETE` | `/products/{id}`           | Soft delete a product          |
| `DELETE` | `/products/{id}/permanent` | Hard delete a product          |
| `PATCH`  | `/products/{id}/restore`   | Restore a soft-deleted product |

#### Orders

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/orders` | Get all orders |
| `POST` | `/orders` | Create a new order |
| `GET` | `/orders/{id}` | Get order by ID |
| `GET` | `/orders/customer/{email}` | Get orders by customer email (paginated) |

### API Versioning

The API supports multiple versioning strategies:

- **URL Segment**: `/api/v1/products`
- **Query String**: `/api/products?version=1.0`
- **Header**: `X-Version: 1.0`
- **Media Type**: `application/json;ver=1.0`

### Response Format

All API responses follow a consistent structure:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { ... },
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

## Design Decisions

### 1. Clean Architecture Implementation

**Decision**: Implemented strict Clean Architecture with dependency inversion.

**Rationale**:

- Ensures business logic remains independent of external concerns
- Facilitates testing and maintainability
- Enables technology stack changes without affecting core business logic
- Promotes separation of concerns

**Implementation**: Each layer only references layers below it, with interfaces defining contracts between layers.

### 2. CQRS with MediatR

**Decision**: Used Command Query Responsibility Segregation pattern.

**Rationale**:

- Separates read and write operations for better scalability
- Enables different optimization strategies for queries vs commands
- Simplifies testing by isolating concerns
- Provides natural extension points for cross-cutting concerns

**Implementation**: Commands for write operations, Queries for read operations, with MediatR handling the dispatch.

### 3. Result Pattern for Error Handling

**Decision**: Implemented Result pattern instead of exceptions for business logic errors.

**Rationale**:

- Makes error handling explicit and predictable
- Avoids performance overhead of exceptions for expected failures
- Provides better API responses with structured error information
- Enables functional programming patterns

**Implementation**: `Result<T>` and `Result` classes that encapsulate success/failure states with error messages.

### 4. Domain Events

**Decision**: Implemented domain events for decoupled communication.

**Rationale**:

- Enables loose coupling between aggregates
- Supports eventual consistency where appropriate
- Provides audit trails and integration points
- Follows DDD principles for domain modeling

**Implementation**: Domain events are raised by aggregates and handled by application services.

### 5. Concurrency Control Strategy

**Decision**: Combined optimistic concurrency with row-level locking.

**Rationale**:

- Optimistic concurrency for high-throughput scenarios
- Row-level locking for critical operations like stock reservation
- Prevents overselling and maintains data consistency
- Balances performance with data integrity

**Implementation**: Version fields for optimistic locking, `SELECT ... FOR UPDATE` for critical sections.

### 6. API Versioning

**Decision**: Implemented URL-based API versioning with multiple strategies.

**Rationale**:

- Future-proofs the API for breaking changes
- Provides flexibility for different client needs
- Enables gradual migration strategies
- Supports multiple versioning approaches

**Implementation**: `Asp.Versioning` package with URL segment as primary strategy.

### 7. Soft Delete Implementation

**Decision**: Implemented both soft and hard delete options for products.

**Rationale**:

- Soft delete preserves data for audit and recovery purposes
- Hard delete provides data cleanup capabilities
- Supports different business requirements
- Maintains referential integrity

**Implementation**: `IsActive` flag for soft delete, physical removal for hard delete.

## Features

### Core Features

- **Product Management**

  - Full CRUD operations
  - Search and pagination
  - Soft and hard delete options
  - Stock quantity management
  - Product activation/deactivation

- **Order Processing**

  - Transactional order creation
  - Stock reservation with concurrency control
  - Order status tracking
  - Customer order history

- **Data Consistency**
  - Optimistic concurrency control
  - Row-level locking for critical operations
  - Transactional processing
  - Domain event handling

### Enterprise Features

- **Logging & Monitoring**

  - Structured logging with Serilog
  - Health check endpoints
  - Performance monitoring
  - Error tracking

- **API Management**

  - Versioned API endpoints
  - Comprehensive Swagger documentation
  - Consistent response formats
  - Global exception handling

- **Security & Validation**
  - Input validation at all layers
  - SQL injection prevention
  - Proper error handling without information leakage
  - CORS configuration

## Testing

### Current Testing Status

The project currently lacks comprehensive test coverage. This is identified as a critical improvement area.
