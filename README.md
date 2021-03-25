# AppContext: A Unit-of-work Framework for .NET
## Premise
A set of classes that provide the ability for different UoW business logic to live in different, individually maintainable (SoC) libraries/projects while sharing disparate data contexts (DbContext(s) and external calls such as REST, SOAP, file access) while shielding said access to these data sources from the consumers. Like "DbContext"... but for **app logic**. It started as a way to shield DbContext from business logic (while still enjoying the performance benefits of a shared context whithin the entire duration of the API call) and has since morphed to all forms of data access.
## Background
Creating unit-of-work classes is fairly straightforward for singular data sources such as EntityFramework. Complications arise when, for security reasons, you want to shield DbContext from the consumers (say, WebAPI or MVC controllers). Dependency injection and passing around DbContexts in constructors OR needlessly instantiating and destroying DbContexts in every single function (thus often making extra db call roundtrips that may have already been satisfied in prior DbContext queries) becomes the norm in these cases. And... of course... security. That is, lower level  developers getting access to data calls that they perhaps should not be allowed to make.
## Result
Business logic classes contain just business logic, are extraordinarly easy to mock for TDD, sensitive data calls can be more easily protected, and *true* SoC more easily achieved. 
## Documentation
https://1drv.ms/w/s!AtUuwRmVvmd4g4YkWQT0SYfYnR7poQ
### Source Code
https://github.com/cmoya212/Cflashsoft.Framework.AppServices
### Sample Project
*Coming soon* 
