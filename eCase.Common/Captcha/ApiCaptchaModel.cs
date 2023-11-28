namespace eCase.Common.Captcha
{
    public class ApiCaptchaModel
    {
        public string Captcha { get; set; }
        public string captchaGuid { get; set; }
        public bool? captchaValid { get; set; }
    }
}
