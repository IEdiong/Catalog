using Catalog.Domain.Common;
using MediatR;

namespace Catalog.Application.Products.Commands;

public record SoftDeleteProductCommand(Guid ProductId) : IRequest<Result>;