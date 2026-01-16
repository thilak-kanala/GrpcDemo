namespace GrpcServer.Infrastructure.Mappers.Common;

/// <summary>
/// Generic interface for mapping between domain entities and Protocol Buffer messages.
/// Provides type-safe bidirectional conversions for gRPC services.
/// </summary>
/// <typeparam name="TEntity">Domain entity type</typeparam>
/// <typeparam name="TMessage">Proto message type</typeparam>
/// <typeparam name="TRequest">Proto request type (unified for both create and update)</typeparam>
public interface IProtoMapper<TEntity, TMessage, TRequest>
{
    /// <summary>
    /// Converts a domain entity to a proto message.
    /// </summary>
    TMessage ToMessage(TEntity entity);
    
    /// <summary>
    /// Converts a proto request to a domain entity.
    /// Used for both create and update operations.
    /// </summary>
    TEntity FromRequest(TRequest request);
}

