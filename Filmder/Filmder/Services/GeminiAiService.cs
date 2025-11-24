using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Filmder.DTOs;
using Filmder.Models;

namespace Filmder.Services;

public class GeminiAiService : IAIService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public GeminiAiService(IConfiguration config)
    {
        _http = new HttpClient();
        _apiKey = config["Gemini:ApiKey"];
    }

   
    public async Task<string> GenerateText(string prompt)
    {
        var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        var request = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[] { new { text = prompt } }
                }
            }
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Headers.Add("X-goog-api-key", _apiKey);
        httpRequest.Content = JsonContent.Create(request);

        var response = await _http.SendAsync(httpRequest);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return $"Error {response.StatusCode}: {json}";

        var result = JsonSerializer.Deserialize<GeminiResponse>(json);
        var raw = result?.candidates?[0]?.content?.parts?[0]?.text ?? "No response";

        return CleanAiResponse(raw);
    }

   
    private string CleanAiResponse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        return text
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();
    }
    

    public Task<string> EmojiSequence(Difficulty difficulty)
    {
        string prompt = $@"
        You are generating content for a movie-guessing game.

        THE GAME:
        1. Show the player an emoji sequence that represents a real movie.
        2. Give them four answer options (1 correct, 3 incorrect).

        YOUR TASK:
        - Pick a movie that matches the requested difficulty.
        - Create an emoji sequence that clearly hints at the movie, use atleast 8 emojis.
        - Create 3 incorrect options that are plausible but NOT from the same franchise.
        - Output ONLY valid JSON.

        DIFFICULTY DEFINITIONS (VERY IMPORTANT):
        - EASY:
          - Choose extremely popular, mainstream movies.
          - Examples: Marvel movies, Disney, major blockbusters, iconic modern films.
          - Emoji should be obvious and easy to decode.
          - Wrong options should be clearly different from the right movie.

        - MEDIUM:
          - Choose well-known but not ultra-mainstream movies.
          - Movies with a dedicated fan base, Oscar nominees, classics from the 90s–2000s.
          - Emoji sequence should require some thinking but still solvable.
          - Wrong options should be believable and from related genres.

        - HARD:
          - Choose cult classics, older films, international cinema, or critically acclaimed but less mainstream movies.
          - Emoji sequence should be clever and indirect, requiring real knowledge.
          - Wrong options must be very convincing alternatives (same era or tone).

        JSON FORMAT (MANDATORY):
        {{
          ""movie"": ""Correct movie title"",
          ""emoji"": ""Emoji sequence"",
          ""options"": [
            ""Correct movie title"",
            ""Wrong option 1"",
            ""Wrong option 2"",
            ""Wrong option 3""
          ]
        }}

        OUTPUT RULES (ABSOLUTELY REQUIRED):
        - Output ONLY valid JSON.
        - Do NOT wrap JSON in ``` or ```json.
        - No markdown.
        - No extra text before or after the JSON.
        - No explanations.
        - No comments.

        Now generate the puzzle.
        Difficulty: {difficulty}
        ";

        return GenerateText(prompt);
    }

    public async Task<PersonalityMatchResultDto> MatchPersonalityToCharacters(PersonalityQuizSubmissionDto submission)
    {
        var answersText = string.Join("\n", submission.Answers.Select((a, i) => 
            $"Question {a.QuestionId}: {a.Answer}"));

        string prompt = $@"
        You are a personality analyzer that matches users to movie/TV characters based on their quiz answers.

        USER'S QUIZ ANSWERS:
        {answersText}

        YOUR TASK:
        1. FOCUS ON GIVING CHARACTERS FROM KNOWN MOVIES, DONT GIVE CHARACTERS FROM VERY OLD MOVIES. THE MOVIES OR TV SHOWS HAVE TO BE POPULAR   
        2. Analyze the personality traits revealed by these answers
        3. Match the user to exactly 3 movie or TV series characters
        4. Calculate a match percentage (60-99%) for each character
        5. Provide a brief explanation for each match
        6. Find a real image URL for each character from popular sources
        7. Create a short personality profile summary

        MATCHING CRITERIA:
        - Consider values, decision-making style, social preferences, conflict handling, and life outlook
        - Choose characters from popular movies and TV series that users would recognize
        - Match percentages should be realistic (60-99%, with the best match being highest)
        - Explanations should reference specific traits from the quiz answers

        IMAGE URL REQUIREMENTS:
        - Provide real, working image URLs for each character
        - Use high-quality promotional images or official posters
        - Prefer URLs from TMDB, IMDb, or official movie databases
        - Format: https://image.tmdb.org/t/p/w500/[image-path] or similar
        - If no real URL available, use placeholder: https://via.placeholder.com/300x450?text=[CharacterName]

        JSON FORMAT (MANDATORY):
        {{
          ""matches"": [
            {{
              ""characterName"": ""Character Name"",
              ""movieOrSeries"": ""Movie/Series Title"",
              ""matchPercentage"": 85,
              ""explanation"": ""Brief explanation of why this character matches the user's personality"",
              ""imageUrl"": ""https://image.tmdb.org/t/p/w500/characterimage.jpg""
            }},
            {{
              ""characterName"": ""Character Name 2"",
              ""movieOrSeries"": ""Movie/Series Title 2"",
              ""matchPercentage"": 78,
              ""explanation"": ""Brief explanation for second match"",
              ""imageUrl"": ""https://image.tmdb.org/t/p/w500/characterimage2.jpg""
            }},
            {{
              ""characterName"": ""Character Name 3"",
              ""movieOrSeries"": ""Movie/Series Title 3"",
              ""matchPercentage"": 72,
              ""explanation"": ""Brief explanation for third match"",
              ""imageUrl"": ""https://image.tmdb.org/t/p/w500/characterimage3.jpg""
            }}
          ],
          ""personalityProfile"": ""A concise 2-3 sentence summary of the user's personality based on their answers""
        }}

        OUTPUT RULES (ABSOLUTELY REQUIRED):
        - Output ONLY valid JSON
        - Do NOT wrap JSON in ``` or ```json
        - No markdown formatting
        - No extra text before or after the JSON
        - No explanations outside the JSON
        - No comments
        - Match percentages must be integers between 60-99
        - Order matches from highest to lowest percentage
        - ImageUrl must be a valid URL or placeholder

        Now analyze the personality and provide matches.
        ";

        string raw = await GenerateText(prompt);
        
        var settings = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        try
        {
            var result = JsonSerializer.Deserialize<PersonalityMatchResultDto>(raw, settings);
            return result ?? new PersonalityMatchResultDto();
        }
        catch (JsonException)
        {
            return new PersonalityMatchResultDto
            {
                PersonalityProfile = "Unable to analyze personality at this time.",
                Matches = new List<CharacterMatchDto>()
            };
        }
    }

 
    private EmojiPuzzleDto ParseMovieJson(string json)
    {
        var settings = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<EmojiPuzzleDto>(json, settings);
    }

 
    public async Task<EmojiPuzzleDto> EmojiSequenceParsed(Difficulty difficulty)
    {
        string raw = await EmojiSequence(difficulty);
        return ParseMovieJson(raw);
    }
}


public class GeminiResponse
{
    public Candidate[] candidates { get; set; }
}

public class Candidate
{
    public Content content { get; set; }
}

public class Content
{
    public Part[] parts { get; set; }
}

public class Part
{
    public string text { get; set; }
}