namespace FrameDemo.Api.Messages
{
    public static class ErrorCodeStatus
    {
        /// <summary>
        /// 服务器错误
        /// </summary>
        public const int ErrorCode999 = 999;
        /// <summary>
        /// 参数错误
        /// </summary>
        public const int ErrorCode10000 = 10000;
        /// <summary>
        /// 控制器或方法不存在
        /// </summary>
        public const int ErrorCode40000 = 40000;
        /// <summary>
        /// 没有权限
        /// </summary>
        public const int ErrorCode40001 = 40001;
        /// <summary>
        /// 不支持的MediaType
        /// </summary>
        public const int ErrorCode40002 = 40002;
        /// <summary>
        /// 参数不符合验证
        /// </summary>
        public const int ErrorCode40003 = 40003;
        /// <summary>
        /// 请求的资源不存在
        /// </summary>
        public const int ErrorCode40004 = 40004;

        /// <summary>
        /// 
        /// </summary>
        public const int ErrorCode10001 = 10001;
        public const int ErrorCode10002 = 10002;
    }
}
