namespace Template.Application.Features.Todos.Queries.GetTodos;

using FluentValidation;

public class GetTodosQueryValidator : AbstractValidator<GetTodosQuery>
{
    public const int MaxPageSize = 100;

    public GetTodosQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThan(0);

        RuleFor(query => query.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(MaxPageSize);
    }
}