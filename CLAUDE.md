The project is a small local only WebAssembly app made with .NET Blazor and MudBlazor.

It extracts quotes from a standard XML file (used by Kobo e-readers) to display them and store them as a Markdown file.
The original ePub file can also be uploaded so that we can extract the summary from it.

The project follows a clean architecture approach:
- the UI is in `BookQuotes.App`: it uses MudBlazor, and the business logic is in `BookQuotes.Application`
- the business logic is in `BookQuotes.Application`
- the domain is in `BookQuotes.Domain`, it contains the entities and value objects. The tests are in `BookQuotes.Domain.Tests`
- the code using external dependencies (e.g. XML parser, ePub parser...) is in `BookQuotes.Infrastructure`. The tests are in `BookQuotes.Infrastructure.Tests`
