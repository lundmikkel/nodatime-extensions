using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace NodaTime.WebApi.ModelBinding
{
    internal class ActionValueBinder : DefaultActionValueBinder
    {
        public readonly ModelBinderResolver Resolver;

        public ActionValueBinder( IDateTimeZoneProvider dateTimeZoneProvider ) => Resolver = new ModelBinderResolver( dateTimeZoneProvider );

        protected override HttpParameterBinding GetParameterBinding( HttpParameterDescriptor parameter )
        {
            if ( parameter.ParameterBinderAttribute == null )
            {
                var parameterType = Nullable.GetUnderlyingType( parameter.ParameterType ) ?? parameter.ParameterType;
                var modelBinder = Resolver.GetModelBinder( parameterType );

                if ( modelBinder != null )
                {
                    return parameter.BindWithModelBinding( modelBinder );
                }

                var supportedHttpMethods = parameter.ActionDescriptor.SupportedHttpMethods;
                if ( supportedHttpMethods.Contains( HttpMethod.Get ) || supportedHttpMethods.Contains( HttpMethod.Head ) )
                {
                    return parameter.BindWithAttribute( new FromUriAttribute() );
                }
            }

            return base.GetParameterBinding( parameter );
        }
    }
}