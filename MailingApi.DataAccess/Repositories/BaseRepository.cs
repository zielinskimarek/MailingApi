using System;
using System.Threading.Tasks;
using MailingApi.DataAccess.Dtos;

namespace MailingApi.DataAccess.Repositories
{
    public abstract class BaseRepository
    {
        protected async Task<ActionResponse<T>> ExecuteDbOperation<T>(Func<Task<ActionResponse<T>>> databaseOperation)
        {
            try
            {
                return await databaseOperation();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ActionResponse<T>(default(T), e.Message);
            }
        }

        protected async Task ExecuteDbOperation(Func<Task> databaseOperation)
        {
            try
            {
                await databaseOperation();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
