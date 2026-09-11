using LimestoneDigital.QA.Core.Sut.Api.Models;
using RestSharp;

namespace LimestoneDigital.QA.Core.Sut.Api;

public class JsonPlaceholderApiClient
{
    private readonly RestClient _client;

    public JsonPlaceholderApiClient(string baseUrl)
    {
        _client = new RestClient(baseUrl);
    }

    public RestResponse<Post> GetPost(int id)
    {
        var request = new RestRequest($"posts/{id}", Method.Get);
        return _client.Execute<Post>(request);
    }
}
