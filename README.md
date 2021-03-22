# AppContext: A Unit-of-work Framework for .NET
## Premise
A set of classes that provide the ability for different UoW business logic to live in different, individually maintainable (SoC) libraries/projects while sharing data contexts (DbContext(s), external calls such as REST, SOAP, file access) while shielding said access these data sources from the consumers. Like "DbContext"... but for **app logic**.
## Background
Creating unit-of-work classes is fairly straightforward for singular data sources such as EntityFramework DbContexts. Complications arise when, for security reasons, you want to shield DbContext from the consumers (say, WebAPI or MVC controllers). Dependency injection and passing around DbContexts in constructors OR needlessly instantiating and destroying DbContexts in every single function (thus often making extra db call roundtrips that may have already been satisfied in prior DbContext queries) becomes the norm in these cases. And... of course... security. Lower level  developers getting access to data calls that they perhaps should not be allowed to make.
## Documentation
https://1drv.ms/w/s!AtUuwRmVvmd4g4YkWQT0SYfYnR7poQ
### Sample Project
*Comming soon*
