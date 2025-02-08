# ADR 001: initial design choice

## Context

The (business) need is a small app to be able to export quotations taken
by the user on an e-reader (such as a Kobo e-reader).

Enriching the quotes with some context if the original ePub file is available
would be interesting. An ePub file can go up to a dozen of MB.

## Decision

We have decided to use Blazor WebAssembly (WASM) to build a local application
running in the web browser.

### Rationale

Blazor WASM allows us to:
- Develop the application using C# and .NET 
- Run entirely client-side in the browser, eliminating the need for a backend service. 
- Distribute the application as static files, which simplifies deployment and installation. 
- Leverage modern web UI technologies while maintaining a single-language stack. 
- Enable offline functionality since the entire application runs in the browser.

## Alternatives Considered

### Desktop application

Pros:
- Native integration with the operating system.
- Better performance for heavy computations.
- Can provide a richer UI experience with system-level access.

Cons:
- Requires platform-specific builds (Windows, macOS, Linux). 
- Installation and updates are more complex than distributing static web files.
- Potentially higher maintenance overhead.

### Web application

Pros:
- Centralized deployment and maintenance.
- No need for client-side processing power.

Cons:
- Requires an active network connection.
- Introduces server dependencies that increase complexity and cost.

## Consequences

- Users will need a modern browser that supports WebAssembly. 
- Initial load time may be higher compared to native desktop apps due to WASM download. 
- Limited access to system resources compared to a native application. 
- Easier cross-platform compatibility compared to a desktop app.

## Conclusion

Blazor WASM strikes a balance between ease of development, deployment simplicity, and cross-platform support
while enabling a local-only execution model.
