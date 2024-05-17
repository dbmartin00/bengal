using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.FeatureManagement.FeatureFilters;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.ApplicationInsights;

namespace extra
{
    public class ExampleTargetingContextAccessor : ITargetingContextAccessor
    {
        private const string TargetingContextLookup = "ExampleTargetingContextAccessor.TargetingContext";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TelemetryClient _telemetryClient;

        public ExampleTargetingContextAccessor(IHttpContextAccessor httpContextAccessor, TelemetryClient telemetryClient)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _telemetryClient = telemetryClient;
        }

        public ValueTask<TargetingContext> GetContextAsync()
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            if (httpContext.Items.TryGetValue(TargetingContextLookup, out object value))
            {
                return new ValueTask<TargetingContext>((TargetingContext)value);
            }
            List<string> groups = new List<string>();
            if (httpContext.User.Identity.Name != null)
            {
                groups.Add(httpContext.User.Identity.Name.Split("@", StringSplitOptions.None)[1]);
            }
            if (string.IsNullOrEmpty(_telemetryClient.Context.User.AuthenticatedUserId))
            {
                _telemetryClient.Context.User.AuthenticatedUserId = Guid.NewGuid().ToString();
            }            
            TargetingContext targetingContext = new TargetingContext
            {
                UserId = _telemetryClient.Context.User.AuthenticatedUserId, //httpContext.User.Identity.Name,
                Groups = groups
            };

            // var claims = httpContext.User.Claims.ToList();

            // // Remove the existing name claim if it exists
            // var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            // if (nameClaim != null)
            // {
            //     claims.Remove(nameClaim);
            // }

            // // Add the new name claim
            // claims.Add(new Claim(ClaimTypes.Name, targetingContext.UserId));

            // // Create a new identity and principal
            // var newIdentity = new ClaimsIdentity(claims, "Custom");
            // var newPrincipal = new ClaimsPrincipal(newIdentity);

            // // Set the new principal on the HttpContext
            // httpContext.User = newPrincipal;

            _telemetryClient.Context.User.AuthenticatedUserId = targetingContext.UserId;
            // Console.WriteLine("ExampleTargetingContextAccessor traffic key: " + _telemetryClient.Context.User.AuthenticatedUserId);

            httpContext.Items[TargetingContextLookup] = targetingContext;
            return new ValueTask<TargetingContext>(targetingContext);
        }
    }
}
