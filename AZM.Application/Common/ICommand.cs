using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Common
{
    public interface ICommand : IRequest<Result> { }
    public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
}