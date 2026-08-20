using FluentValidation;
using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(
                validator => validator.ValidateAsync(
                    context,
                    cancellationToken)));

        var errors = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .Select(error =>
                new Error(
                    $"Validation.{error.PropertyName}",
                    error.ErrorMessage,
                    ErrorType.Validation))
            .ToList();

        if (errors.Count == 0)
        {
            return await next(cancellationToken);
        }

        return CreateValidationResponse(errors);
    }

    private static TResponse CreateValidationResponse(
        IReadOnlyList<Error> errors)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(errors);
        }

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = typeof(TResponse).GetGenericArguments()[0];

            var resultType = typeof(Result<>).MakeGenericType(valueType);

            var failureMethod = resultType.GetMethod(
                nameof(Result<object>.Failure),
                [typeof(IEnumerable<Error>)]);

            if (failureMethod is null)
            {
                throw new InvalidOperationException(
                    $"Could not create validation result for {typeof(TResponse).Name}.");
            }

            return (TResponse)failureMethod.Invoke(
                null,
                [errors])!;
        }

        throw new InvalidOperationException(
            $"ValidationBehavior cannot handle response type " +
            $"{typeof(TResponse).Name}. " +
            "MediatR requests using this behavior must return " +
            "Result or Result<T>.");
    }
}