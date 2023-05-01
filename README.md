# Generic API Controller

Tired of always doing the same to start a project?<br>
This is for you! The perfect solution for starting .NET Core API Projects. <br>

Provides and generates API controllers and business logic CRUD methods for any object you want with the GeneratedController annotation.<br>

These methods will be available in the generated API controllers:
-Create
-Read
-Update
-Delete
-Recover
-ReadByKey

Include Swagger Documentation API with JWT authentication.<br>

# How to install & start

1. Get the code:

  ### `git clone` https://github.com/marcosbustamantemateo/GenericAPIController

2. Modify appsettings.json and update it with your SQL Server connection:

3. Then, in order to create the database, you have to execute it with nuget in the project directory:

  ### `update-database`

# Documentation

- Generate API controller and methods:
1. Put the following annotation in your entity model

  ```cs class:"lineNo"
  
  [GeneratedController]
  
  ```
