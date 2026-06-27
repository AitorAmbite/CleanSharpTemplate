namespace Conductor.Application.Features.Todos;

using Conductor.Domain;
using Conductor.Domain.Entities;
using Conductor.Domain.Events;
using Conductor.Domain.Repositories;

public class CreateTodoHandler
{
    private readonly ITodoRepository _todoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTodoHandler(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
    {
        _todoRepository = todoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TodoCreated> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        var todo = new Todo(command.Title);

        await _todoRepository.AddAsync(todo, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TodoCreated(todo.Id, todo.Title);
    }
}
