# FundTracker

![CI](https://github.com/Ouija1733/fund-tracker/actions/workflows/ci.yml/badge.svg)

A small full-stack application to manage investment funds, their NAV history
and investor transactions, including a bulk CSV import.
I built it in October 2026 to learn the .NET stack, coming from several years
of PHP/Laravel development.

## Tech stack

- **Backend:** ASP.NET Core 10 Web API, Entity Framework Core, SQL Server
- **Frontend:** React, TypeScript, Vite, TanStack Query
- **Testing:** xUnit
- **DevOps:** Docker (multi-stage builds), Docker Compose, GitHub Actions

## Features

- Funds CRUD with validation and consistent error responses (ProblemDetails)
- Server-side search and pagination
- NAV history with day-over-day change, computed with a SQL window function
- Fund flows report (subscriptions, redemptions, net flow) aggregated in SQL
- CSV import of transactions: streaming, row-level validation (including
  ISIN check digits), batched inserts, idempotent re-imports

## Getting started

Requirements: Docker.

    docker compose up --build

- App: http://localhost:3000
- API reference: http://localhost:5080/scalar

## Local development

    docker compose up -d db
    cd src/FundTracker.Api && dotnet ef database update && dotnet watch --launch-profile http
    cd client && npm install && npm run dev

## Tests

    dotnet test

## Project structure

    src/FundTracker.Api     ASP.NET Core API (controllers, services, EF Core)
    tests/FundTracker.Tests xUnit tests
    client                  React + TypeScript frontend
    sql                     Seed script and T-SQL exercises
    tools                   CSV test data generator

## Design decisions

- **Thin controllers, logic in services** behind interfaces, wired with
  dependency injection, so the logic is reusable and testable.
- **DTOs** instead of exposing entities, to avoid over-posting and keep the
  API contract stable.
- **Read queries with `AsNoTracking` and projections**, so SQL Server returns
  only the needed columns.
- **Idempotent import** based on a unique `ExternalId`, so a file can be
  safely re-processed.

## What I would do next

- Integration tests against a real SQL Server (Testcontainers)
- Move the import to a background job and use `SqlBulkCopy` for very large files
- Authentication (JWT) and role-based authorization
- Deployment pipeline to a cloud environment
