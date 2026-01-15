namespace GrpcServer.Infrastructure.Mappers.Common;

public interface IMapper<TEntity, TRequestDto, TResponseDto>
{
    TResponseDto ToResponseDto(TEntity entity);
    TEntity FromRequestDto(TRequestDto dto);
}

