Veterinary Clinic Appointment & Patient Tracking System

Student Number: YOUR_STUDENT_NUMBER
Name Surname: YOUR NAME SURNAME
Course Group: YOUR_GROUP (e.g. 1-A)
GitHub: https://github.com/YOUR_USERNAME/VetClinicSystem

About the Project

This is the term project for the Web Programming course (2026-2027 Fall), Sakarya University.

It is a web application built with ASP.NET Core MVC (.NET 10). A veterinary clinic can use it to manage:

the services it offers (exam, vaccination, neutering, dental cleaning, grooming…) with their duration and price
veterinarians, their specialties, and their available hours
pet owners and their pets (species, breed, birth date, weight)
vaccine records and exam notes for each pet
appointments, with conflict checks and an approval process (Pending → Approved / Rejected)

The application also has a REST API for reporting and an MCP (Model Context Protocol) server. Through the MCP server, an AI assistant can answer questions like "When is Pamuk's next vaccine?" using the application's own data.

Technologies
ASP.NET Core MVC (.NET 10 LTS), C#
Entity Framework Core (Code First, Migrations), LINQ
ASP.NET Core Identity (roles: Admin, Veterinarian, Member)
SQL Server
ModelContextProtocol.AspNetCore (MCP C# SDK)
Bootstrap 5, HTML5, CSS3, JavaScript, jQuery
Status

🚧 In development. This README will grow into the full project report: ER diagram, screenshots, API endpoints and MCP tools.
