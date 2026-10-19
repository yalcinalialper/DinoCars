using Microsoft.AspNetCore.Components;

namespace DinoCars.Web.Components.Shared
{
    public abstract class ScopedComponentBase : ComponentBase
    {
        [Inject] private IServiceScopeFactory ScopeFactory { get; set; } = null!;

        protected async Task<TResult> RunScopedAsync<TService, TResult>(Func<TService, Task<TResult>> action)
            where TService : notnull
        {
            await using var scope = ScopeFactory.CreateAsyncScope();
            return await action(scope.ServiceProvider.GetRequiredService<TService>());
        }

        protected async Task RunScopedAsync<TService>(Func<TService, Task> action)
            where TService : notnull
        {
            await using var scope = ScopeFactory.CreateAsyncScope();
            await action(scope.ServiceProvider.GetRequiredService<TService>());
        }
    }
}
