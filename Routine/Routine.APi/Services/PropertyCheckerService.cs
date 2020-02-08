using System.Reflection;

namespace Routine.APi.Services
{
    public class PropertyCheckerService : IPropertyCheckerService
    {
        /// <summary>
        /// fields 字符串是否合法（视频P39）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
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
                                                        BindingFlags.IgnoreCase
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
