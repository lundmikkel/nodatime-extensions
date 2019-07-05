using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using NodaTime;

namespace NodaTime.WebApi.ModelBinding
{
    /// <summary>
    /// Model binder for <see cref="LocalDate"/>s that used to be a <see cref="System.DateTime"/>-representation of a date.
    /// </summary>
    public class LegacyLocalDateModelBinder : ModelBinder<LocalDate>
    {
        public LegacyLocalDateModelBinder() : base( Patterns.LocalDatePattern )
        { }

        public override bool BindModel( HttpActionContext actionContext, ModelBindingContext bindingContext )
        {
            var type = System.Nullable.GetUnderlyingType( bindingContext.ModelType ) ?? bindingContext.ModelType;
            if ( type != typeof( LocalDate ) )
            {
                return false;
            }

            // Try parsing as a LocalDate
            if ( base.BindModel( actionContext, bindingContext ) )
            {
                return true;
            }

            var value = bindingContext.ValueProvider.GetValue( bindingContext.ModelName );
            if ( value == null )
            {
                return false;
            }

            var rawValue = value.RawValue.ToString();

            // Parse as a DateTime
            if ( System.DateTime.TryParse( rawValue, out System.DateTime dateTime ) )
            {
                var date = dateTime.Date;
                bindingContext.Model = new LocalDate( date.Year, date.Month, date.Day );
                bindingContext.ModelState.Remove( bindingContext.ModelName );
                return true;
            }

            return false;
        }
    }
}