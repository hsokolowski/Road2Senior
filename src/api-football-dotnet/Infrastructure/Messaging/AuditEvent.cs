namespace Infrastructure.Messaging;

public record AuditEvent(
    DateTime Utc,
    string Action,          // np. GET /api/externalleague/leagues/{id}
    string Controller,
    string Method,          // nazwa metody w kontrolerze
    string HttpMethod,
    string? RouteId,        // np. id z route/query
    int? StatusCode,
    object? Payload         // opcjonalnie body/request/extra
);