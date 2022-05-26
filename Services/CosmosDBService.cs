using System.Net;
using bmiWebAPI_3.Models;
using Microsoft.Azure.Cosmos;

namespace bmiWebAPI_3.Services;

public class CosmosDbService : ICosmosDbService
{
    private readonly Container _container;

    public CosmosDbService(
        CosmosClient dbClient,
        string databaseName,
        string containerName)
    {
        _container = dbClient.GetContainer(databaseName, containerName);
    }

    public async Task AddUserAsync(ApplicationUser user)
    {
        await _container.CreateItemAsync(user, new PartitionKey(user.Id));
    }

    public async Task DeleteUserAsync(string id)
    {
        await _container.DeleteItemAsync<ApplicationUser>(id, new PartitionKey(id));
    }

    public async Task<ApplicationUser> GetUserAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<ApplicationUser>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(string queryString)
    {
        var query = _container.GetItemQueryIterator<ApplicationUser>(new QueryDefinition(queryString));
        var results = new List<ApplicationUser>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();

            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task UpdateUserAsync(string id, ApplicationUser user)
    {
        await _container.UpsertItemAsync(user, new PartitionKey(id));
    }
}