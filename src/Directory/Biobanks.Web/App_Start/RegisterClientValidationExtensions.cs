using DataAnnotationsExtensions.ClientValidation;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Biobanks.Web.App_Start.RegisterClientValidationExtensions), "Start")]
 
namespace Biobanks.Web.App_Start {
    public static class RegisterClientValidationExtensions {
        public static void Start() {
            DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();            
        }
    }
}