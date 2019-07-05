using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using NodaTime.Text;

namespace NodaTime.WebApi.ModelBinding
{
    public class ModelBinder<T> : IModelBinder
    {
        private readonly IPattern<T> Pattern;

        public ModelBinder( IPattern<T> pattern ) => Pattern = pattern;

        public virtual bool BindModel( HttpActionContext actionContext, ModelBindingContext bindingContext )
        {
            var type = Nullable.GetUnderlyingType( bindingContext.ModelType ) ?? bindingContext.ModelType;

            if ( type != typeof( T ) )
            {
                return false;
            }

            var value = bindingContext.ValueProvider.GetValue( bindingContext.ModelName );
            if ( value == null )
            {
                return IsNullableType( bindingContext.ModelType );
            }

            var rawValue = value.RawValue.ToString();

            var result = Pattern.Parse( rawValue );
            if ( !result.Success )
            {
                bindingContext.ModelState.AddModelError( bindingContext.ModelName, result.Exception );
                return false;
            }

            bindingContext.Model = result.Value;
            return true;
        }

        private static bool IsNullableType( Type type ) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Nullable<> );
    }
}