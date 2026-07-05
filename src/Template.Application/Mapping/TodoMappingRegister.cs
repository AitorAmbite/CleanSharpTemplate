namespace Template.Application.Mapping;

using Mapster;
using Template.Contracts.Todos;
using Template.Domain.Entities;

public class TodoMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Todo, TodoDto>();
    }
}