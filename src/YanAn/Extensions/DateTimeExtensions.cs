
namespace YanAn.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 时间转时间戳Unix-时间戳精确到毫秒,13位
        /// <paramref name="dt">时间</paramref>
        /// <paramref name="isMillisecond">true=毫秒时间戳(13位),false=精确到秒时间戳(10位)</paramref>
        /// </summary> 
        public static long ToUnixTimestamp(this DateTime dt, bool isMillisecond = true)
        {
            DateTimeOffset dto = new(dt);
            if (isMillisecond)
                return dto.ToUnixTimeMilliseconds();
            else
                return dto.ToUnixTimeSeconds();
        }

        /// <summary>
        ///  时间戳转本地时间-时间戳精确到毫秒
        /// </summary> 
        public static DateTime ToLocalTimeDateByMilliseconds(this long unix)
        {
            DateTimeOffset dto;
            if (unix > 10000000000)//13位毫秒时间戳
                dto = DateTimeOffset.FromUnixTimeMilliseconds(unix);
            else//10位秒时间戳
                dto = DateTimeOffset.FromUnixTimeSeconds(unix);
            return dto.ToLocalTime().DateTime;
        }
    }
}
