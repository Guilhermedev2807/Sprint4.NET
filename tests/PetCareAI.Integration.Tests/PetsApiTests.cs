using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace PetCareAI.Integration.Tests;

public class PetsApiTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task AutenticarAsync()
    {
        var email = $"{Guid.NewGuid():N}@teste.com";
        var resp = await _client.PostAsJsonAsync("/api/auth/register", new { nome = "Teste", email, senha = "123456" });
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
        var token = json.GetProperty("token").GetString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static object PetValido() => new
    {
        nome = "Rex",
        especie = "Cachorro",
        raca = "Labrador",
        dataNascimento = DateTime.UtcNow.AddYears(-2),
        peso = 12.5
    };

    [Fact]
    public async Task Listar_Pets_Sem_Token_Deve_Retornar_401()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var resp = await client.GetAsync("/api/pets");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Registrar_Com_Email_Duplicado_Deve_Retornar_409()
    {
        // Arrange
        var body = new { nome = "Ana", email = $"{Guid.NewGuid():N}@teste.com", senha = "123456" };
        await _client.PostAsJsonAsync("/api/auth/register", body);

        // Act
        var resp = await _client.PostAsJsonAsync("/api/auth/register", body);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
    }

    [Fact]
    public async Task Criar_Pet_Com_Token_Deve_Retornar_201_Com_Links()
    {
        // Arrange
        await AutenticarAsync();

        // Act
        var resp = await _client.PostAsJsonAsync("/api/pets", PetValido());

        // Assert
        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.GetProperty("links").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Criar_Pet_Com_Peso_Invalido_Deve_Retornar_400()
    {
        // Arrange
        await AutenticarAsync();
        var invalido = new { nome = "Rex", especie = "Cachorro", raca = "x", dataNascimento = DateTime.UtcNow.AddYears(-1), peso = 0 };

        // Act
        var resp = await _client.PostAsJsonAsync("/api/pets", invalido);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Listar_Pets_Deve_Retornar_Apenas_Do_Usuario_Autenticado()
    {
        // Arrange
        await AutenticarAsync();
        await _client.PostAsJsonAsync("/api/pets", PetValido());

        // Act
        var resp = await _client.GetAsync("/api/pets?page=1&pageSize=10&sort=nome");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(1, json.GetProperty("total").GetInt32());
    }
}