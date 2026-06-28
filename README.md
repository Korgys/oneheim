# oneheim

A detailed roguelike in console.

Explore the region :

![image](Screenshots/exploring.png)

Discuss with NPCs :

![image](Screenshots/dialogue.png)

Fights various enemies :

![image](Screenshots/combat.png)

## Run

```powershell
dotnet run --project Roguelike.Console
```

The game reads `Roguelike.Console/gameSettings.json` at startup. It controls language, difficulty, and key bindings.

Default FR controls:

- Move: `Z`, `Q`, `S`, `D`
- Choices: `W`, `X`, `C`
- Quit: `Escape`

## Gameplay loop

Explore around the base camp, collect treasures, fight waves, and prepare for boss attacks at major step milestones. The camp can be repaired, defended with hired guards, and lost if enough enemies reach its walls.

Additional travelers unlock during a run: Ichem sells boons, Eber hires guards, Omana reveals the next boss, Urd lets you gamble for items, and Ylva upgrades equipment after the first boss is defeated. A dungeon in the north-east offers a denser risk/reward area.

Difficulty is configured in `gameSettings.json` with `Normal`, `Hard`, or `Hell`.

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
