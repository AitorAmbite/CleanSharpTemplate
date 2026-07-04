namespace Template.Application.Mapping;

using Mapster;
using Template.Contracts.Todos;
using Template.Domain.Entities;

public class TodoMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Todo, TodoDto>()
            .Map(dst => dst.Id, src => src.Id)
            .Map(dst => dst.Title, src => src.Title)
            .Map(dst => dst.IsCompleted, src => src.IsCompleted)
            .Map(dst => dst.CreatedAt, src => src.CreatedAt);
    }
}