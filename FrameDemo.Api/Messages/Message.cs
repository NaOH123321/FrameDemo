using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrameDemo.Api.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FrameDemo.Api.Messages
{
    public abstract class Message<T> :IMessage<T>
    {
        public abstract int Code { get; set; }
        public abstract T Msg { get; set; }
        public abstract int ErrorCode { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                Code,
                Msg,
                ErrorCode
            }, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}
