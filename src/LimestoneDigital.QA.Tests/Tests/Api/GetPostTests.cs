using Allure.NUnit;
using Allure.NUnit.Attributes;
using LimestoneDigital.QA.Business;
using LimestoneDigital.QA.Core.Sut.Api.Models;
using NUnit.Framework;
using RestSharp;

namespace LimestoneDigital.QA.Tests.Api;

[TestFixture]
[AllureNUnit]
[AllureSuite("API")]
[Category("Smoke")]
[Category("Api")]
public class GetPostTests
{
    private RestResponse<Post> _response = null!;
    private AssertionSteps _assertions = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _response = new ApiSteps().GetPost(1);
        _assertions = new AssertionSteps();
    }

    [Test]
    public void GetPost_ExistingId_ReturnsSuccessStatusCode()
    {
        _assertions.AssertSuccessStatusCode(_response);
    }

    [Test]
    public void GetPost_ExistingId_ReturnsValidContract()
    {
        _assertions.AssertPostContract(_response.Data!);
    }
}
