using System;
using EPiServer.Construction;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.Internal;
using EPiServer.ServiceLocation;
//using Geta.EpiCategories.SpecializedProperties;

namespace Geta.EpiCategories
{
    //[ServiceConfiguration(typeof(BackingTypeResolver), Lifecycle = ServiceInstanceScope.Singleton)]
    //[ServiceConfiguration(typeof(IBackingTypeResolver), IncludeServiceAccessor = false, Lifecycle = ServiceInstanceScope.Singleton)]
    //public class CategoryBackingTypeResolver : BackingTypeResolver
    //{
    //    public CategoryBackingTypeResolver(IPropertyDefinitionTypeRepository propertyDefinitionTypeRepository, IPropertyDataFactory propertyDataFactory) : base(propertyDefinitionTypeRepository, propertyDataFactory)
    //    {
    //    }

    //    public override Type Resolve(Type type)
    //    {
    //        if (type == typeof (ContentCategoryList))
    //        {
    //            return typeof (PropertyContentCategoryList);
    //        }

    //        return base.Resolve(type);
    //    }
    //}
}