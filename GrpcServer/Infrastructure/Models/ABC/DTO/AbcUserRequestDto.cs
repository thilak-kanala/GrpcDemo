namespace GrpcServer.Infrastructure.Models.ABC.DTO;

public record AbcUserRequestDto(int Id, string? UserName, string? Email, string? SourceSystem);

