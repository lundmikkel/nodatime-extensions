using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace NodaTime.WebApi.ModelBinding
{
    public class DateTimeZoneModelBinder : IModelBinder
    {
        public IDateTimeZoneProvider Provider { get; }

        public DateTimeZoneModelBinder( IDateTimeZoneProvider provider )
        {
            Provider = provider;
        }

        public bool BindModel( HttpActionContext actionContext, ModelBindingContext bindingContext )
        {
            var modelType = Nullable.GetUnderlyingType( bindingContext.ModelType ) ?? bindingContext.ModelType;
            if ( modelType != typeof( DateTimeZone ) )
            {
                return false;
            }

            var value = bindingContext.ValueProvider.GetValue( bindingContext.ModelName );
            if ( value == null )
            {
                return false;
            }

            var rawValue = value.RawValue.ToString();

            var model = Provider.GetZoneOrNull( rawValue );
            if ( model == null )
            {
                bindingContext.ModelState.AddModelError( bindingContext.ModelName, "Unknown time zone ID." );
                return false;
            }

            bindingContext.Model = model;
            return true;
        }
    }
}