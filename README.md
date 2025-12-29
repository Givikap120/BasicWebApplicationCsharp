# Basic Web Appliction Backend API (C# / ASP.NET)

A simple store-type backend API built with **ASP.NET**, **Entity Framework**, and **JWT Bearer authentication**.  
The project demonstrates clean separation of concerns, role-based authorization, and basic order management.

This repository is intended as a project for resume to demonstrate my skills.

## Features

- User registration and login
- JWT Bearer authentication
- Role-based authorization (Customer / Manager / Admin)
- Order creation and management
- Business logic control
- Entity Framework with SQL Server
- Swagger documentation
- Docker support (WIP)
- Unit tests (WIP)

## Tech Stack

- **ASP.NET**
- **Entity Framework**
- **Microsoft SQL Server (Express)**
- **JWT Bearer Authentication**
- **Swagger**
- **Docker**

## Building and running

1. Clone the project
2. Restore dependencies
3. Install the database (this was built for MS SQL, but other DBs will work as well if you will configure it correctly)
4. Run the schema.sql script to create the database and tables
5. Add the connection string to the appsettings.json and EntityGenerate.ps1
6. Run the EntityGenerate.ps1 script in the Package Manager Console to generate the database models, to ensure that the database entities are matching (or edit them manually)
7. (Optional) Replace the JWT key with your own
8. Run the application
9. Register your admin account via Swagger or Postman (or manually insert it into the database)
10. Change the role of your account to Admin in the database, so you could promote other users to Admin using this account

## Swagger (API Documentation)

Swagger is available in **Development** mode.

1. Build and run the application
2. Open http://localhost:5170 in the browser
3. (If not done) Call `POST /users/register`
4. Call `POST /users/login`
5. Copy the returned JWT token
6. Click **Authorize** in Swagger and enter the token here
7. Now you could access endpoints that require auth

Note: you need to have at least one admin account to be able to promote others to admin. 
You could create first admin account by manually changing the role in database.

