using LimestoneDigital.QA.Core.Sut.Api;
using LimestoneDigital.QA.Core.Sut.Api.Models;
using LimestoneDigital.QA.Core.Tas.Config;
using RestSharp;

namespace LimestoneDigital.QA.Business;

public class ApiSteps
{
    private readonly JsonPlaceholderApiClient _client;

    public ApiSteps()
    {
        _client = new JsonPlaceholderApiClient(ConfigurationProvider.Settings.Api.BaseUrl);
    }

    public RestResponse<Post> GetPost(int id) => _client.GetPost(id);
}
