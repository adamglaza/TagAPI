using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using TagAPI.Data;
using static TagAPI.Data.SOAPI;


namespace TagAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagsController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly ILogger<TagsController> _logger;
        public bool dbReady = false;
        public TagsController(ILogger<TagsController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        private IActionResult RefreshDatabase()
        {
            _logger.LogInformation("Calling stack overflow API at {DT}", DateTime.UtcNow.ToLongTimeString());
            List<TagSQL> tags = CallApiAsync().Result;

            if (tags.Count == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "No connection with StackOverflow API" });
            }

            try
            {
                _db.Tags.ExecuteDeleteAsync().Wait();
                _db.Tags.AddRange(tags);
                _db.SaveChanges();
                _logger.LogInformation("Database succesfully updated at {DT}", DateTime.UtcNow.ToLongTimeString());
            }
            catch (Exception ex)
            {
                _logger.LogError("Lost connection with the database during update at {DT}", DateTime.UtcNow.ToLongTimeString());
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "No connection with database" });
            }

            return Ok("Database updated");
        }

        /// <summary>
        /// Returns a page of certain size of tags from StackOverflow
        /// </summary>
        /// <remarks>Can be ordered by count or name descending(desc) or ascending(asc)</remarks>
        /// <response code="400">Wrong values for page or pagesize</response>
        /// <response code="500">No connection with database/No connection with StackOverflow API</response>
        [HttpGet(Name = "Tags")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TagSQL>), (int)HttpStatusCode.OK)]
        [SwaggerOperation("GetAllTags")]
        public ActionResult<List<TagSQL>> Get(string? orderBy, string? order, int page = 1, int pagesize = 100)
        {
            if (!dbReady)
            {
                var status = RefreshDatabase();

                if (status.Equals(Ok("Database updated")))
                {
                    return new ActionResult<List<TagSQL>>((ActionResult)status);
                }
                dbReady = true;
            }

            if (pagesize < 1 || pagesize > 100 || page < 1)
            {
                return BadRequest("Wrong values for page or pagesize");
            }

            List<TagSQL> tagList = new List<TagSQL>();

            try
            {
                if (orderBy == "count")
                {
                    if (order == "desc")
                    {
                        tagList = _db.Tags.OrderByDescending(x => x.Count).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
                    }
                    else
                    {
                        tagList = _db.Tags.OrderBy(x => x.Count).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
                    }
                }
                else
                {
                    if (order == "desc")
                    {
                        tagList = _db.Tags.OrderByDescending(x => x.Name).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
                    }
                    else
                    {
                        tagList = _db.Tags.OrderBy(x => x.Name).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
                    }
                }
                _logger.LogInformation("Database succesfully selected tag table at {DT}", DateTime.UtcNow.ToLongTimeString());
            }
            catch (Exception ex)
            {
                _logger.LogError("Lost connection with the database while selecting tag table at {DT}", DateTime.UtcNow.ToLongTimeString()); 
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "No connection with database" });
            }

            return tagList;
        }

        /// <summary>
        /// Redownloads tags stored on a local database
        /// </summary>
        /// <response code="200">Database updated</response>
        /// <response code="500">No connection with database/No connection with StackOverflow API</response>
        [HttpPost("RefreshDatabase")]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [SwaggerOperation("GetAllTags")]
        public IActionResult Post()
        {
            return RefreshDatabase();
        }
    }
}
