
namespace DotNet.Tools.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举属性的自定义描述值
        /// </summary>
        /// <param name="_enum"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum _enum)
        {
            string text = string.Empty;
            var field = _enum.GetType().GetField(_enum.ToString());
            if (field != null && field.GetCustomAttributes().FirstOrDefault(x => x.GetType() == typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                text = attr.Description;
            return text;
        }
    }
}
