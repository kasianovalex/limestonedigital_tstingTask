using System.Net;
using LimestoneDigital.QA.Core.Sut.Api.Models;
using LimestoneDigital.QA.Core.Tas.Matchers;
using RestSharp;

namespace LimestoneDigital.QA.Business;

/// <summary>
/// One assertion concern per method, each built from Core/Tas matchers rather than inline
/// Assert.That calls, so wording and logic for a given check live in exactly one place.
/// </summary>
public class AssertionSteps
{
    public void AssertCartContainsItem(IReadOnlyList<string> cartItems, string expectedItem)
    {
        cartItems.ShouldContain(expectedItem, "cart items");
    }

    public void AssertSuccessStatusCode(RestResponse response)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.OK, "status code");
    }

    public void AssertPostContract(Post post)
    {
        post.Id.ShouldBeGreaterThan(0, nameof(post.Id));
        post.UserId.ShouldBeGreaterThan(0, nameof(post.UserId));
        post.Title.ShouldNotBeNullOrEmpty(nameof(post.Title));
        post.Body.ShouldNotBeNullOrEmpty(nameof(post.Body));
    }
}
