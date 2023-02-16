using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace commerce_challenge.Controllers
{
    /// <summary>
    /// Base controller for the commerce api
    /// </summary>
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    public class CommerceControllerBase : ControllerBase { }
}
