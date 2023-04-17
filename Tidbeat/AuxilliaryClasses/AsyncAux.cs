namespace Tidbeat.AuxilliaryClasses
{
    public class AsyncAux
    {
        // static method that returns a Task with a certain bool value
        public async static Task<bool> ReturnBoolAsync(bool val)
        {
            return await Task.Run(() => val);
        }
    }
}
