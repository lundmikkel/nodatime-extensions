using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime.WebApi.ModelBinding
{
    public class IsoIntervalModelBinder : IModelBinder
    {
        private readonly IPattern<Instant> Pattern;

        public IsoIntervalModelBinder( IPattern<Instant> pattern ) => Pattern = pattern;

        public bool BindModel( HttpActionContext actionContext, ModelBindingContext bindingContext )
        {
            var modelType = Nullable.GetUnderlyingType( bindingContext.ModelType ) ?? bindingContext.ModelType;
            if ( modelType != typeof( Interval ) )
            {
                return false;
            }

            var value = bindingContext.ValueProvider.GetValue( bindingContext.ModelName );
            if ( value == null )
            {
                return false;
            }

            var rawValue = value.RawValue.ToString();

            var slash = rawValue.IndexOf( '/' );
            if ( slash == -1 )
            {
                throw new InvalidNodaDataException( "Expected ISO-8601-formatted interval; slash was missing." );
            }

            var startText = rawValue.Substring( 0, slash );
            var endText = rawValue.Substring( slash + 1 );

            var startInstant = default( Instant );
            var endInstant = default( Instant );

            if ( startText != string.Empty )
            {
                startInstant = Pattern.Parse( startText ).Value;
            }

            if ( endText != string.Empty )
            {
                endInstant = Pattern.Parse( endText ).Value;
            }

            bindingContext.Model = new Interval( startInstant, endInstant );
            return true;
        }
    }
}