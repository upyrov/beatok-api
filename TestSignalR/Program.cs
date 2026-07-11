using System.Net.Http.Json;
using Beatok.Application.DTOs.Lobby;
using Microsoft.AspNetCore.SignalR.Client;

public class Program
{
    static async Task Main()
    {
        var httpClient = new HttpClient();
        var lobbies = await httpClient.GetFromJsonAsync<List<LobbyDto>>("https://localhost:7184/lobbies");

        Console.WriteLine("--- Поточні лобі ---");
        foreach (var lobby in lobbies)
        {
            Console.WriteLine($"ID: {lobby.Id} | Назва: {lobby.Name}");
        }
        
        var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7184/hubs/lobbies")
            .Build();

        connection.On<LobbyDto>("LobbyCreated", (newLobby) => 
        {
            Console.WriteLine($"\n[LIVE] Додано нове лобі: {newLobby.Name}");
        });

        await connection.StartAsync();
        Console.WriteLine("\nСлухаю нові лобі... Натисни Enter, щоб вийти.");

        Console.ReadLine();
    }
}