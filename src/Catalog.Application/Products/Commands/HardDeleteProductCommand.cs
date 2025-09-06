using Catalog.Domain.Common;
using MediatR;

namespace Catalog.Application.Products.Commands;

public record HardDeleteProductCommand(Guid ProductId) : IRequest<Result>;