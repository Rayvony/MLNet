using MLNetProyecto.Entidades;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MLNetProyecto.Web.Services;
public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7268/");
    }

    public async Task<List<LabelResult>> PostAsync(string endpoint, IFormFile data)
    {
        using (var content = new MultipartFormDataContent())
        {
            using (var stream = data.OpenReadStream())
            {
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(data.ContentType);
                content.Add(streamContent, "file", data.FileName);

                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<List<LabelResult>>(jsonResponse, options);
                }

                // Manejo de errores
                throw new HttpRequestException($"Error en la petición: {response.StatusCode}");
            }
        }
    }
}
