using DnsClient.Protocol;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.ObjectPool;
using Microsoft.FeatureManagement;
using System.Security.Claims;

namespace extra.Pages;


public class Quote
{
    public string Message { get; set; }

    public string Author { get; set; }
}

public class QuoteButton
{
    public string Label { get; set;}
}


public class IndexModel : PageModel
{
    private readonly IVariantFeatureManagerSnapshot _featureManager;
    private readonly TelemetryClient _telemetryClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IndexModel(IVariantFeatureManagerSnapshot featureManager, TelemetryClient telemetryClient, IHttpContextAccessor httpContextAccessor)
    {
        _featureManager = featureManager;
        _telemetryClient = telemetryClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private Quote[] _quotes = [
        new Quote()
        {
            Message = "You cannot change what you are, only what you do.",
            Author = "Philip Pullman"
        },
        new Quote()
        {
            Message = "Never let what you cannot do interfere with what you can.",
            Author = "John Wooden"
        },
        new Quote()
        {   
            Message = "However difficult life may seem, there is always something you can do and succeed at",
            Author = "Stephen Hawking"
        },
        new Quote()
        {
            Message = "The best way to predict the future is to create it.",
            Author = "Peter Drucker"
        },
        new Quote()
        {
            Message = "Keep your face always toward the sunshine—and shadows will fall behind you.",
            Author = "Walt Whitman"
        },
        new Quote()
        {
            Message = "Believe you can and you're halfway there.",
            Author = "Theodore Roosevelt"
        },
        new Quote()
        {
            Message = "The only way to do great work is to love what you do.",
            Author = "Steve Jobs"
        },
        new Quote()
        {
            Message = "Act as if what you do makes a difference. It does.",
            Author = "William James"
        },
        new Quote()
        {
            Message = "Success is not how high you have climbed, but how you make a positive difference to the world.",
            Author = "Roy T. Bennett"
        },
        new Quote()
        {
            Message = "The future belongs to those who believe in the beauty of their dreams.",
            Author = "Eleanor Roosevelt"
        },
        new Quote()
        {
            Message = "You are never too old to set another goal or to dream a new dream.",
            Author = "C.S. Lewis"
        },
        new Quote()
        {
            Message = "Start where you are. Use what you have. Do what you can.",
            Author = "Arthur Ashe"
        },
        new Quote()
        {
            Message = "It does not matter how slowly you go as long as you do not stop.",
            Author = "Confucius"
        },
        new Quote()
        {
            Message = "Your time is limited, so don't waste it living someone else's life.",
            Author = "Steve Jobs"
        },
        new Quote()
        {
            Message = "You miss 100% of the shots you don't take.",
            Author = "Wayne Gretzky"
        },
        new Quote()
        {
            Message = "Don't watch the clock; do what it does. Keep going.",
            Author = "Sam Levenson"
        },
        new Quote()
        {
            Message = "The harder the conflict, the greater the triumph.",
            Author = "George Washington"
        },
        new Quote()
        {
            Message = "Believe in yourself and all that you are. Know that there is something inside you that is greater than any obstacle.",
            Author = "Christian D. Larson"
        },
        new Quote()
        {
            Message = "What lies behind us and what lies before us are tiny matters compared to what lies within us.",
            Author = "Ralph Waldo Emerson"
        },
        new Quote()
        {
            Message = "The only limit to our realization of tomorrow is our doubts of today.",
            Author = "Franklin D. Roosevelt"
        },
        new Quote()
        {
            Message = "The biggest adventure you can take is to live the life of your dreams.",
            Author = "Oprah Winfrey"
        },
        new Quote()
        {
            Message = "Happiness is not something ready-made. It comes from your own actions.",
            Author = "Dalai Lama"
        },
        new Quote()
        {
            Message = "Do not wait to strike till the iron is hot; but make it hot by striking.",
            Author = "William Butler Yeats"
        },
        new Quote()
        {
            Message = "You are braver than you believe, stronger than you seem, and smarter than you think.",
            Author = "A.A. Milne"
        },
        new Quote()
        {
            Message = "The best revenge is massive success.",
            Author = "Frank Sinatra"
        },
        new Quote()
        {
            Message = "The only way to achieve the impossible is to believe it is possible.",
            Author = "Charles Kingsleigh"
        },
        new Quote()
        {
            Message = "You must be the change you wish to see in the world.",
            Author = "Mahatma Gandhi"
        }
    ];


    public Quote? Quote { get; set; }

    public QuoteButton[] _quoteButtons = [
        new QuoteButton() {
            Label = "get a new quote..."
        },
        new QuoteButton() {
            Label = "one more quote..."
        }
    ];
    public QuoteButton? QuoteButton { get; set;}
    
    private string OnTrack(
        string eventTypeId,
        string valueString,
        long value)
    {
        Dictionary<string, string> props = new Dictionary<string, string>();
        props.Add("TargetingId", _telemetryClient.Context.User.AuthenticatedUserId);
        props.Add(valueString, "" + value);
        _telemetryClient.TrackEvent(eventTypeId, props);

        return "sent";
    }

    private static int QuoteIndex;

    public async Task OnGetAsync()
    {
        // Console.WriteLine("traffic key is " + _telemetryClient.Context.User.AuthenticatedUserId );
        _telemetryClient.Context.User.AuthenticatedUserId = Guid.NewGuid().ToString();
        // Console.WriteLine("setting new traffic key: " + _telemetryClient.Context.User.AuthenticatedUserId);
        _telemetryClient.Context.User.Id = _telemetryClient.Context.User.AuthenticatedUserId;

        Variant nextButtonTextVariant = await _featureManager.GetVariantAsync("next_button_text", HttpContext.RequestAborted);
        string nextButton = nextButtonTextVariant.Configuration.Get<string>() ?? "control";

        Variant erratumVariant = await _featureManager.GetVariantAsync("erratum", HttpContext.RequestAborted);
        string erratum = erratumVariant.Configuration.Get<string>() ?? "control";

        Console.WriteLine("nextButton: " + nextButton + " erratum: " + erratum);

        Quote = _quotes[new Random().Next(_quotes.Length)];
        QuoteIndex = nextButton == "On" ? 0 : 1;
        QuoteButton = _quoteButtons[QuoteIndex];

        if(erratum == "On") {
            if(new Random().Next(10) < 4) {
                OnTrack("error", "count", 1L);
            }
        } else {
            if(new Random().Next(10) < 1) {
                OnTrack("error", "count", 1L);
            }
        }

        await Task.CompletedTask;
    }

    public IActionResult OnGetNextQuote()
    {
        // Console.WriteLine("OnGetNextQuote");
        OnTrack("quote_click", "count", 1L);
        QuoteIndex = (QuoteIndex + 1) % _quotes.Length;
        Quote = _quotes[QuoteIndex];
        return new JsonResult(Quote);
    }

    public IActionResult OnPostHeartQuote()
    {
        Console.WriteLine("OnPostHeartQuote");
        OnTrack("like_click", "count", 1L);
        return new JsonResult(new { success = true });
    }
}
