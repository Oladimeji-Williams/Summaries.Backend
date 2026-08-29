using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Users.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(string FirstName, string LastName) : IRequest<Result>;