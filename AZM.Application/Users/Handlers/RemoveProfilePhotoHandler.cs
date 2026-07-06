using AZM.Application.Users.Commands;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Handlers
{
    public class RemoveProfilePhotoHandler : IRequestHandler<RemoveProfilePhotoCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhotoService _photoService;

        public RemoveProfilePhotoHandler(IUserRepository userRepository, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _photoService = photoService;
        }

        public async Task<Unit> Handle(RemoveProfilePhotoCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId.ToString())
                ?? throw new KeyNotFoundException("User not found.");

            if (string.IsNullOrEmpty(user.ProfilePhotoUrl))
                return Unit.Value; // nothing to remove

            var publicId = $"profile_photos/profile_{request.UserId}";
            await _photoService.DeletePhotoAsync(publicId);

            user.ProfilePhotoUrl = null;
            await _userRepository.UpdateAsync(user);

            return Unit.Value;
        }
    }
}
