using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MyTCGBinder.Application.UseCases;

public class DeleteUserDataUseCase(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
    ) : IDeleteUserDataUseCase{

    public async Task ExecuteAsync(Guid userId)
    {
        await userRepository.DeleteAsync(userId);

        await unitOfWork.SaveChangesAsync();
    }

}
