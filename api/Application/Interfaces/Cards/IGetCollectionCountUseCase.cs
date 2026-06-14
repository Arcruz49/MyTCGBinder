namespace MyTCGBinder.Application.Interfaces;

public interface IGetCollectionCountUseCase
{
    Task<int> ExecuteAsync(Guid userId);
}