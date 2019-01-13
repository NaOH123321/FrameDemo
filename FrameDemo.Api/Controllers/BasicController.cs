using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using FrameDemo.Api.Helpers;
using FrameDemo.Api.Messages;
using FrameDemo.Core.Entities;
using FrameDemo.Core.Interfaces;
using FrameDemo.Infrastructure.Resources;
using FrameDemo.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FrameDemo.Api.Controllers
{
    public class BasicController<TResource, TEntity> : Controller where TEntity : IEntity
    {
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingContainer _propertyMappingContainer;
        private readonly ITypeHelperService _typeHelperService;

        public BasicController(
            IUrlHelper urlHelper,
            IPropertyMappingContainer propertyMappingContainer,
            ITypeHelperService typeHelperService)
        {
            _urlHelper = urlHelper;
            _propertyMappingContainer = propertyMappingContainer;
            _typeHelperService = typeHelperService;

            Results = new List<ObjectResult>();
        }

        //protected Type EntityType { private get; set; }
        //protected Type ResourceType { private get; set; }
        //protected Type UpdateOrAddResourceType { private get; set; }
        //protected Type ParametersType { private get; set; }

        protected List<ObjectResult> Results { get; }

        /// <summary>
        /// 判断相应的字段是否能排序
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        protected void ValidateMapping(string orderBy)
        {
            //MethodInfo mi = _propertyMappingContainer.GetType()
            //    .GetMethod(nameof(_propertyMappingContainer.ValidateMappingExistsFor));
            //var valueInvoke = mi.MakeGenericMethod(new Type[] {ResourceType, EntityType})
            //    .Invoke(_propertyMappingContainer, new object[] {orderBy});

            //if (!(bool) valueInvoke)
            //{
            //    Results.Add(BadRequest(new BadRequestForSortingMessage()));
            //}

            if (!_propertyMappingContainer.ValidateMappingExistsFor<TResource, TEntity>(orderBy))
            {
                Results.Add(BadRequest(new BadRequestForSortingMessage()));
            }
        }

        /// <summary>
        /// 判断相应的字段是否能塑形
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected void ValidateFields(string fields)
        {
            //MethodInfo mi = _typeHelperService.GetType().GetMethod(nameof(_typeHelperService.TypeHasProperties));
            //var valueInvoke = mi.MakeGenericMethod(new Type[] {ResourceType})
            //    .Invoke(_typeHelperService, new object[] {fields});

            //if (!(bool) valueInvoke)
            //{
            //    Results.Add(BadRequest(new BadRequestFieldsMessage()));
            //}
            if (!_typeHelperService.TypeHasProperties<TResource>(fields))
            {
                Results.Add(BadRequest(new BadRequestFieldsMessage()));
            }
        }

        /// <summary>
        /// 判断参数能否通过验证
        /// </summary>
        protected void ValidateParameters()
        {
            if (!ModelState.IsValid)
            {
                Results.Add(new MyUnprocessableEntityObjectResult(ModelState));
            }
        }

        /// <summary>
        /// 判断参数是否为空
        /// </summary>
        /// <param name="parameter"></param>
        protected void ValidateNotNull(object parameter)
        {
            if (parameter == null)
            {
                Results.Add(BadRequest(new BadRequestMessage()));
            }
        }

        /// <summary>
        /// 判断能否找到相应的数据
        /// </summary>
        /// <param name="parameter"></param>
        protected void ValidateNotFound(object parameter)
        {
            if (parameter == null)
            {
                Results.Add(BadRequest(new NotFoundResourceMessage()));
            }
        }

        /// <summary>
        /// 创建包含X-Pagination的header
        /// </summary>
        /// <param name="sampleParameters"></param>
        /// <param name="sampleList"></param>
        /// <param name="routeName"></param>
        /// <param name="withLinks">是否在header中加入前后页的链接</param>
        /// <returns></returns>
        protected void CreateHeader(QueryParameters sampleParameters, PaginatedList<TEntity> sampleList,
            string routeName, bool withLinks = true)
        {
            var meta = new Dictionary<string, object>
            {
                {nameof(sampleList.PageIndex), sampleList.PageIndex},
                {nameof(sampleList.PageSize), sampleList.PageSize},
                {nameof(sampleList.PageCount), sampleList.PageCount},
                {nameof(sampleList.TotalItemsCount), sampleList.TotalItemsCount}
            };

            if (withLinks)
            {
                var previousPageLink = sampleList.HasPrevious
                    ? CreateUri(sampleParameters, PaginationResourceUriType.PreviousPage, routeName)
                    : null;

                var nextPageLink = sampleList.HasNext
                    ? CreateUri(sampleParameters, PaginationResourceUriType.NextPage, routeName)
                    : null;

                meta.Add(nameof(previousPageLink), previousPageLink);
                meta.Add(nameof(nextPageLink), nextPageLink);

                //var meta = new
                //{
                //    sampleList.PageIndex,
                //    sampleList.PageSize,
                //    sampleList.PageCount,
                //    sampleList.TotalItemsCount,
                //    previousPageLink,
                //    nextPageLink
                //};

                //return meta;
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }

        /// <summary>
        /// 创建Uri
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="uriType"></param>
        /// <param name="routeName">路由名称</param>
        /// <returns></returns>
        private string CreateUri(QueryParameters parameters, PaginationResourceUriType uriType, string routeName = null)
        {
            switch (uriType)
            {
                case PaginationResourceUriType.PreviousPage:
                    var previousParameters = new
                    {
                        pageIndex = parameters.PageIndex - 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link(routeName, previousParameters);
                case PaginationResourceUriType.NextPage:
                    var nextParameters = new
                    {
                        pageIndex = parameters.PageIndex + 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link(routeName, nextParameters);
                default:
                    var currentParameters = new
                    {
                        pageIndex = parameters.PageIndex,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link(routeName, currentParameters);
            }
        }
    }
}
