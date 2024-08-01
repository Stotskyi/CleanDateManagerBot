
using Picker.Domain.Abstarctions;

namespace Picker.Application.Abstractions.Messaging;

public interface IQuery<TResposne>
{
}

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}


