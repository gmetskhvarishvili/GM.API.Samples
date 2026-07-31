using Asp.Versioning;
using GM.API.Controllers;
using GM.API.Sample.Application.Samples.Commands.CreateSample;
using GM.API.Sample.Application.Samples.Commands.DeleteSample;
using GM.API.Sample.Application.Samples.Commands.UpdateSample;
using GM.API.Sample.Application.Samples.Queries.GetSampleDetails;
using GM.API.Sample.Application.Samples.Queries.GetSamplesList;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace GM.API.Sample.API.Sample;

/// <summary>
/// Sample Controller
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SampleController : BaseController
{
    /// <summary>
    /// Add Sample
    /// </summary>
    /// <param name="request">Sample Model to Add</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Sample Id</returns>
    [HttpPost(Name = nameof(AddSample))]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddSample(
        [FromBody] CreateSampleModel request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<CreateSampleCommand>();
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update Sample
    /// </summary>
    /// <param name="id">Sample Id to Update</param>
    /// <param name="request">Sample Model to Update</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Result</returns>
    [HttpPut("{id:int}", Name = nameof(UpdateSample))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSample(
        [FromRoute] int id,
        [FromBody] UpdateSampleModel request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<UpdateSampleCommand>();
        command.Id = id;
        await Mediator.Send(command, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Delete Sample
    /// </summary>
    /// <param name="id">Sample Id to Delete</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Result</returns>
    [HttpDelete("{id:int}", Name = nameof(DeleteSample))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSample(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSampleCommand
        {
            Id = id
        };
        await Mediator.Send(command, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Get Samples List
    /// </summary>
    /// <param name="request">Sample Model to Get</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IEnumerable of Samples</returns>
    [HttpGet(Name = nameof(GetSamplesList))]
    [ProducesResponseType(typeof(IEnumerable<SampleModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSamplesList(
        [FromQuery] GetSamplesListModel request,
        CancellationToken cancellationToken)
    {
        var query = request.Adapt<GetSamplesListQuery>();
        var response = await Mediator.Send(query, cancellationToken);
        var result = response.Adapt<IEnumerable<SampleModel>>();
        return Ok(result);
    }

    /// <summary>
    /// Get Sample Details
    /// </summary>
    /// <param name="id">Sample Id to Get</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Sample Details</returns>
    [HttpGet("{id:int}", Name = nameof(GetSampleDetails))]
    [ProducesResponseType(typeof(SampleDetailsModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSampleDetails(
        [FromQuery] int id,
        CancellationToken cancellationToken)
    {
        var query = new GetSampleDetailsQuery
        {
            Id = id
        };
        var response = await Mediator.Send(query, cancellationToken);
        var result = response.Adapt<SampleDetailsModel>();
        return Ok(result);
    }
}