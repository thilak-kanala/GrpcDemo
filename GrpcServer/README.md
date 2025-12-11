## TODOs

- Update message structures in most of the protos

- Add Mappers when necessary

- Q. Would it be a good design choice to implement a difference user service for each application?

- Q. If IUserService contains methods for CRUD, what if there's an application that doesn't support all of them? The implementing class would have to throw NotImplementedException for the unsupported methods. Is there a better way to handle this?
  - What's a good example for this scenario?

- Q. CRUD operations on groups are quite rare. How would an implementation of IGroupService look like in that case? Would the service still be SCIM compliant if CRUD on groups is not supported?

- Review logger settings and implementations
