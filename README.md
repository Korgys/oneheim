# oneheim

A detailled roguelike in console.

Explore the region :

![image](screenshots/exploring.png)

Discuss with NPCs :

![image](screenshots/dialogue.png)

Fights various enemies :

![image](screenshots/combat.png)

## How to run unit tests and show code coverage

### 1. Run tests and collect data

```powershell
cd Roguelike.Core.Tests
dotnet test --collect:"XPlat Code Coverage"
```

Coverage results will be saved under: `TestResults/<GUID>/coverage.cobertura.xml`

### 2. Use ReportGenerator to create an HTML report

```powershell
reportgenerator -reports:TestResults/**/coverage.cobertura.xml -targetdir:coveragereport
```

### 3. Open the report in your browser

The report will be generated in `coveragereport/index.html`

#### Windows

```powershell
start coveragereport/index.html
```

#### macOS

```bash
open coveragereport/index.html
```

#### Linux

```bash
xdg-open coveragereport/index.html
```
