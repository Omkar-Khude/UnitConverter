# Unit Converter API

A RESTful ASP.NET Core Web API for converting values between units of measurement.

## Supported Categories

| Category    | Example units                                           |
|-------------|--------------------------------------------------------|
| Length      | meter, kilometer, mile, foot, inch, yard, nautical mile |
| Mass        | kilogram, gram, pound, ounce, stone, tonne              |
| Temperature | celsius, fahrenheit, kelvin, rankine                    |
| Speed       | m/s, km/h, mph, knot, ft/s                             |
| Area        | m², km², ft², acre, hectare                            |
| Volume      | liter, ml, gallon (US/UK), cup, tablespoon              |
| Pressure    | pascal, bar, PSI, atm, torr                             |
| Energy      | joule, calorie, kilocalorie, kWh, BTU                   |
| Data        | byte, KB, MB, GB, TB, bit, kbit                        |

---

## Prerequisites

| Tool        | Minimum version | Install                              |
|-------------|-----------------|--------------------------------------|
| .NET SDK    | 8.0             | https://dot.net/download             |
| VS Code     | any             | https://code.visualstudio.com        |

Verify your .NET installation:

```bash
dotnet --version   # should print 8.x.x
```

---

## Getting Started

### 1 – Clone / extract the project

```bash
# If you downloaded the zip:
unzip UnitConverter.zip -d UnitConverter
cd UnitConverter
```

### 2 – Restore & build

```bash
dotnet build UnitConverter.sln
```

### 3 – Run the API

```bash
cd src/UnitConverter.API
dotnet run
```

The API starts on **http://localhost:5000** (HTTP) by default.

Swagger UI is available at the root URL in development mode:

```
http://localhost:5000
```

### 4 – Run with hot-reload (recommended for development)

```bash
cd src/UnitConverter.API
dotnet watch run
```

---

## VS Code Workflow

1. Open the workspace root in VS Code.
2. Install the recommended extensions when prompted (C# Dev Kit, REST Client).
3. Press **F5** to build and launch with the debugger attached.
4. Open `requests.http` and click **Send Request** above any block to test endpoints.

---

## Running Tests

```bash
# All tests
dotnet test UnitConverter.sln

# With coverage report (requires coverlet)
dotnet test UnitConverter.sln --collect:"XPlat Code Coverage"
```

---

## API Reference

### Base URL

```
http://localhost:5000/api
```

---

### `POST /api/conversions`

Convert a value from one unit to another.

**Request body**

```json
{
  "value": 100,
  "fromUnit": "celsius",
  "toUnit": "fahrenheit"
}
```

**Response `200 OK`**

```json
{
  "inputValue": 100,
  "fromUnit": "Celsius",
  "toUnit": "Fahrenheit",
  "outputValue": 212.0,
  "category": "Temperature",
  "summary": "100 °C = 212 °F"
}
```

**Error responses**

| Code | Reason                                      |
|------|---------------------------------------------|
| 400  | `fromUnit` or `toUnit` is not recognised    |
| 422  | The two units belong to different categories |

---

### `GET /api/conversions`

Quick conversion via query string (handy for browser/cURL testing).

```
GET /api/conversions?value=1&from=kilometer&to=mile
```

---

### `GET /api/units`

List all supported units, optionally filtered by category.

```
GET /api/units
GET /api/units?category=Temperature
```

---

### `GET /api/units/categories`

List all available category names.

```json
["Area", "Data", "Energy", "Length", "Mass", "Pressure", "Speed", "Temperature", "Volume"]
```

---

### `GET /health`

Health check endpoint.

```json
{ "status": "Healthy", "timestamp": "2025-01-01T00:00:00Z" }
```

---

## cURL Examples

```bash
# Celsius to Fahrenheit
curl -X POST http://localhost:5000/api/conversions \
     -H "Content-Type: application/json" \
     -d '{"value":100,"fromUnit":"celsius","toUnit":"fahrenheit"}'

# Kilometer to mile (GET shorthand)
curl "http://localhost:5000/api/conversions?value=1&from=kilometer&to=mile"

# List all units in the Mass category
curl "http://localhost:5000/api/units?category=Mass"

# List all categories
curl http://localhost:5000/api/units/categories
```

---

## Project Structure

```
UnitConverter/
├── src/
│   └── UnitConverter.API/
│       ├── Controllers/
│       │   ├── ConversionController.cs   # POST & GET /api/conversions
│       │   └── UnitsController.cs        # GET /api/units, /api/units/categories
│       ├── Models/
│       │   └── ConversionModels.cs       # Request / Response / Error records
│       ├── Services/
│       │   ├── IConversionService.cs     # Interface + custom exceptions
│       │   ├── ConversionService.cs      # Conversion logic
│       │   └── UnitRegistry.cs           # All unit definitions & factors
│       ├── Middleware/
│       │   └── GlobalExceptionMiddleware.cs
│       ├── Extensions/
│       │   └── ServiceCollectionExtensions.cs
│       ├── Program.cs
│       ├── appsettings.json
│       └── appsettings.Development.json
├── tests/
│   └── UnitConverter.Tests/
│       ├── Controllers/
│       │   └── ConversionControllerIntegrationTests.cs
│       └── Services/
│           └── ConversionServiceTests.cs
├── .vscode/
│   ├── launch.json
│   ├── tasks.json
│   └── extensions.json
├── requests.http           # REST Client test file
├── UnitConverter.sln
└── README.md
```

---

## Adding New Units

All unit definitions live in `src/UnitConverter.API/Services/UnitRegistry.cs`.

**For a linear unit** (simple multiplication factor), add one line to the registry list:

```csharp
Linear("furlong", "Furlong", "fur", "Length", 201.168, "furlongs"),
//      key        displayName  sym   category  factor_to_base   aliases...
```

**For a non-linear unit** (e.g. a new temperature scale), add a full `UnitEntry`:

```csharp
new()
{
    Key = "delisle", DisplayName = "Delisle", Symbol = "°De",
    Category = "Temperature", Aliases = ["de"],
    ToBase   = de => 373.15 - de * 2.0 / 3.0,
    FromBase = k  => (373.15 - k) * 3.0 / 2.0
},
```

No other code changes are needed.

---

## Design Decisions

- **Two-step conversion via base unit** – every category has one canonical base (e.g. Kelvin for temperature, meter for length). Converting A→B goes through: A→base→B. This keeps the number of conversion factors at O(n) rather than O(n²).
- **Singleton service** – `ConversionService` and `UnitRegistry` hold no mutable state, so they are safe to register as singletons and incur zero allocation per request.
- **Alias resolution at startup** – the alias dictionary is built once when the application starts, keeping per-request lookup to a simple dictionary access.
- **Global exception middleware** – centralises error formatting and maps domain exceptions to appropriate HTTP status codes.
