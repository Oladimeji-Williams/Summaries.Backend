using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Users.Application.Commands.UpdateProfileCommand;

public sealed record UpdateProfileCommand(
    string FirstName, string LastName, string? PhoneNumber,
    string? Address, string? City, string? Country) : IRequest<Result>;