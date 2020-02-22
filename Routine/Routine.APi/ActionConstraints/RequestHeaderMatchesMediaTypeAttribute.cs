using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;

namespace Routine.APi.ActionConstraints
{
    /// <summary>
    /// 自定义 Attribute，根据 Header 信息判断方法是否能处理请求（视频P44）
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class RequestHeaderMatchesMediaTypeAttribute : Attribute, IActionConstraint
    {
        private readonly string _requestHeaderToMatch;
        //支持的媒体类型集合
        private readonly MediaTypeCollection _mediaTypes = new MediaTypeCollection();

        public RequestHeaderMatchesMediaTypeAttribute(string requestHeaderToMatch,     //指明要匹配 Header 中的哪一项（key）
                                                      string mediaType,                //key 对应的 value
                                                      params string[] otherMediaTypes) //params 关键字的作用 https://www.cnblogs.com/GreenLeaves/p/6756637.html
        {
            _requestHeaderToMatch = requestHeaderToMatch ?? throw new ArgumentNullException(nameof(requestHeaderToMatch));

            //解析 mediaType 的 MediaTypeHeaderValue
            if (MediaTypeHeaderValue.TryParse(mediaType,out MediaTypeHeaderValue parsedMediaType))
            {
                //解析成功，添加到集合中
                _mediaTypes.Add(parsedMediaType);
            }
            else
            {
                throw new ArgumentException(nameof(mediaType));
            }

            //如果还有 otherMediaTypes，依次进行解析并添加到集合中
            foreach (var otherMediaType in otherMediaTypes)
            {
                if (MediaTypeHeaderValue.TryParse(otherMediaType, out MediaTypeHeaderValue parsedOtherMediaType))
                {
                    _mediaTypes.Add(parsedOtherMediaType);
                }
                else
                {
                    throw new ArgumentException(nameof(otherMediaTypes));
                }
            }
        }

        /// <summary>
        /// 判断该方法是否能处理这个请求
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>是否能处理这个请求</returns>
        public bool Accept(ActionConstraintContext context)
        {
            //判断 Header 中是否含有 _requestHeaderToMatch（key）
            var requestHeaders = context.RouteContext.HttpContext.Request.Headers;
            if (! requestHeaders.ContainsKey(_requestHeaderToMatch))
            {
                return false;
            }

            //如果含有该 key，判断 value 是否包含在支持的媒体类型集合中
            var parsedRequestMediaType = new MediaType(requestHeaders[_requestHeaderToMatch]);
            foreach (var mediaType in _mediaTypes)
            {
                var parsedMediaType = new MediaType(mediaType);
                if (parsedRequestMediaType.Equals(parsedMediaType))
                {
                    return true;
                }
            }

            return false;
        }

        public int Order => 0;

    }
}
