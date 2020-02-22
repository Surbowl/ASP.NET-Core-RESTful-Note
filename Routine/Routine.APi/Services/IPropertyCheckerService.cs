namespace Routine.APi.Services
{
    /// <summary>
    /// PropertyCheckerService 的接口，用于判断 Uri Query 中的 fields 字符串是否合法（视频P39）
    /// </summary>
    public interface IPropertyCheckerService
    {
        bool TypeHasProperties<T>(string fields);
    }
}