using System.Reflection;

namespace Routine.APi.Services
{
    public class PropertyCheckerService : IPropertyCheckerService
    {
        /// <summary>
        /// 判断 Uri Query 中的 fields 字符串是否合法（视频P39）
        /// </summary>
        /// <typeparam name="T">待返回的资源类型</typeparam>
        /// <param name="fields">Uri Query 中的 fields 字符串，大小写不敏感，允许为 null</param>
        /// <returns>fields 字符串是否合法</returns>
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(",");
            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(T).GetProperty(propertyName,
                                                         BindingFlags.IgnoreCase  //大小写不敏感
                                                         | BindingFlags.Public
                                                         | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
