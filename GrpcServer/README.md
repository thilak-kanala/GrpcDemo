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

## TODOs

- Update Validators with actual validation logic relevant for business rules.
- The Id property in User and Group is not an auto generated value. Update the DTOS, mappers etc. to expect this value from the client.
- User and Group DTOs are currently identical to the Entities. They have been separated for future extensibility.
- Some applications do not support every CRUD operation for Users and Groups. Respond with 501 Not Implemented for unsupported operations.
- Implement Health Checks. There would probably be a health check per every implemented repository.