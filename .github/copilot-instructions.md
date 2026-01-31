# Copilot Instructions

## General Guidelines
- Follow Clean Architecture principles by organizing code into separate layers: Domain, Application, Infrastructure, and API.

## Code Style
- Use specific formatting rules.
- Follow naming conventions.

## Project-Specific Rules
- When requested, remove the UserId property from the RefreshToken entity and the corresponding DB column/migration, as tokens should not be linked via UserId.
- DoctorCreateDTO should include UserId; doctor creation links to an existing user, and dto.UserId is required by the service and controller. Doctor retrieval should use the doctor's table primary key (Id).