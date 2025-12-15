## Notes

- Structure
  - Entities, DataContext
  - Repositories, HttpClients
  - Services, Validators
  - Requests, Responses Models, Mappers
  - Controllers, Proto Definitions

- Entities
  - User and Group implementations represent mock data models for users and groups.
  - App specific implementations would be named according to {preFix}User and {preFix}Group.
  - Similarly, GroupRepository and UserRepository are mock implementations of repositories.
  - App specific implementations would be named according to {preFix}UserRepository and {preFix}GroupRepository.

- Defining the common user model as an interface is bad design.
  - No, this is a poor interface design—it's modeling data/state (DTO-like) instead of behavior/capability, and violates the Single Responsibility Principle.
  - Interfaces should define what you can do, not what you are.

## TODOs

- Update Validators with actual validation logic relevant for business rules.
- The Id property in User and Group is not an auto generated value. Update the DTOS, mappers etc. to expect this value from the client.
- Request/Response DTOs for Users/Groups are not fully fleshed out. Add additional properties as needed.
- Some applications do not support every CRUD operation for Users and Groups. Respond with 501 Not Implemented for unsupported operations.
- Implement Health Checks. There would probably be a health check per every implemented repository.