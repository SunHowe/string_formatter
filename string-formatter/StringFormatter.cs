using System;
using System.Collections.Generic;
using System.Text;

namespace StringFormatter
{
    public delegate string StringFormatFunc();

    public delegate string StringFormatWithParametersFunc(int index, params object[] parameters);

    public sealed class StringFormatter : IDisposable
    {
        private readonly Dictionary<string, StringFormatFunc> _formatFuncs = new Dictionary<string, StringFormatFunc>();
        private readonly Dictionary<string, StringFormatWithParametersFunc> _formatWithParametersFuncs = new Dictionary<string, StringFormatWithParametersFunc>();

        private static readonly Queue<StringBuilder> _sbPools = new Queue<StringBuilder>();

        private const char LeftChar = '{';
        private const char RightChar = '}';
        private const char SplitChar = ':';

        public void Dispose()
        {
            _formatFuncs.Clear();
            _formatWithParametersFuncs.Clear();
        }

        /// <summary>
        /// 注册字符串格式化自定义规则的处理器
        /// </summary>
        public void RegisterFormatFunc(string rule, StringFormatFunc func)
        {
            _formatFuncs[rule] = func;
        }

        /// <summary>
        /// 注册带参数处理的字符串格式化自定义规则的处理器
        /// </summary>
        public void RegisterFormatFunc(string rule, StringFormatWithParametersFunc func)
        {
            _formatWithParametersFuncs[rule] = func;
        }

        public string Format(string format, params object[] parameters)
        {
            var sb = GetStringBuilder();

            // 文本内容{0}，{1}，去除特殊符号{{}}
            var strLength = format.Length;
            int pos1 = 0, pos2, pos3;
            int paramIndex;
            string tag;

            while ((pos2 = format.IndexOf(LeftChar, pos1)) != -1)
            {
                // pos1 -> pos2 文本内容
                if (pos1 < pos2)
                    sb.Append(format.Substring(pos1, pos2 - pos1));

                // next char {{
                pos3 = pos2 + 1;
                if (pos3 < strLength && format[pos3] == LeftChar)
                {
                    sb.Append(LeftChar);
                    pos1 = pos3 + 1;
                    continue;
                }

                pos1 = pos2 + 1;

                pos2 = format.IndexOf(RightChar, pos1);
                if (pos2 == -1 || pos2 == pos1)
                    throw new FormatException("Input string was not in a correct format.");

                pos3 = format.IndexOf(SplitChar, pos1);
                if (pos3 == -1 || pos3 > pos2)
                {
                    // {tag}
                    tag = format.Substring(pos1, pos2 - pos1);

                    if (int.TryParse(tag, out paramIndex))
                    {
                        // {0}
                        if (parameters == null || parameters.Length <= paramIndex)
                            throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");

                        sb.Append(parameters[paramIndex]);
                    }
                    else
                    {
                        // {tag}
                        if (!_formatFuncs.TryGetValue(tag, out var func))
                            throw new FormatException($"Tag {tag} format func is missing");

                        sb.Append(func.Invoke());
                    }
                }
                else
                {
                    // {1:tag}
                    tag = format.Substring(pos1, pos3 - pos1);

                    if (!int.TryParse(tag, out paramIndex))
                        throw new FormatException("Input string was not in a correct format.");

                    if (parameters == null || parameters.Length <= paramIndex)
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");

                    tag = format.Substring(pos3 + 1, pos2 - pos3 - 1);

                    if (!_formatWithParametersFuncs.TryGetValue(tag, out var func))
                        throw new FormatException($"Tag {tag} format func with parameter is missing");

                    sb.Append(func.Invoke(paramIndex, parameters));
                }

                pos1 = pos2 + 1;
            }

            while ((pos2 = format.IndexOf(RightChar, pos1)) != -1)
            {
                // pos1 -> pos2 文本内容
                if (pos1 < pos2)
                    sb.Append(format.Substring(pos1, pos2 - pos1));

                // next char }}
                pos3 = pos2 + 1;
                if (pos3 < strLength && format[pos3] == RightChar)
                {
                    sb.Append(RightChar);
                    pos1 = pos3 + 1;
                    continue;
                }

                throw new FormatException("Input string was not in a correct format.");
            }

            if (pos1 == 0)
            {
                ReleaseStringBuilder(sb);
                return format;
            }

            if (pos1 < strLength)
                sb.Append(format.Substring(pos1, strLength - pos1));

            var str = sb.ToString();
            ReleaseStringBuilder(sb);
            return str;
        }

        private static StringBuilder GetStringBuilder()
        {
            if (_sbPools.Count == 0)
                return new StringBuilder();

            var sb = _sbPools.Dequeue();
            sb.Clear();
            return sb;
        }

        private static void ReleaseStringBuilder(StringBuilder sb)
        {
            _sbPools.Enqueue(sb);
        }
    }
}