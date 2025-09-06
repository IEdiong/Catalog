using Catalog.Domain.Common;
using MediatR;

namespace Catalog.Application.Products.Commands;

public record RestoreProductCommand(Guid Id) : IRequest<Result>;