## Notes

- **Structure:** Entities, DataContext, Repositories, HttpClients, Services, Validators, Models (Requests/Responses), Mappers, Controllers, Proto Definitions
- **Entities:** Use `{Prefix}User` and `{Prefix}Group` for app-specific models and repositories.
- **Design:** Avoid using interfaces for data models; interfaces should define behavior, not data.

## TODOs

- Implement business validation in Validators.
- `Id` in User/Group is client-supplied; update DTOs/mappers accordingly.
- Flesh out User/Group request/response DTOs.
- Return 501 for unsupported CRUD operations.
- Add health checks for each repository.
