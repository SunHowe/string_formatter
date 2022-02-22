namespace StringFormatter
{
    public static class StringEx
    {
        private static readonly StringFormatter _globalFormatter = new StringFormatter();

        /// <summary>
        /// 注册字符串格式化自定义规则的处理器
        /// </summary>
        public static void RegisterFormatFunc(string rule, StringFormatFunc func)
        {
            _globalFormatter.RegisterFormatFunc(rule, func);
        }

        /// <summary>
        /// 注册带参数处理的字符串格式化自定义规则的处理器
        /// </summary>
        public static void RegisterFormatFunc(string rule, StringFormatWithParametersFunc func)
        {
            _globalFormatter.RegisterFormatFunc(rule, func);
        }

        public static string Format(string format, params object[] parameters)
        {
            return _globalFormatter.Format(format, parameters);
        }
    }
}