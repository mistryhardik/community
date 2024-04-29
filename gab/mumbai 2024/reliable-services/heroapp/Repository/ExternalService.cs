namespace heroapp.Repository;

public interface IExternalService
{
    Task<string> RunDependencyService(string input);
}

public class ExternalService : IExternalService
{
    public async Task<string> RunDependencyService(string input)
    {
        await Task.Run(() => Thread.Sleep(1000));

        return $"Hello {input}";
    }
}