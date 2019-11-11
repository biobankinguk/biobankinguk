namespace Directory.Pages.Account
{
    public class ForgotPasswordModel : BaseReactModel
    {
        protected ForgotPasswordModel()
            : base(ReactRoutes.ForgotPassword) { }

        public void OnGet()
        {
        }

        public void OnGetReset()
        {
            Route = ReactRoutes.ResetPassword;
            Page();
        }
    }
}
