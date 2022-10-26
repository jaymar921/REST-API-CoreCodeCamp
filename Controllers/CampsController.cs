using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController: ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsAsync(includeTalks);
                /*
                 * The reason why we don't provide the Entity directly
                 * to our user because we don't want to expose everything
                 * to the users.
                 * 
                 * We use models and map them from the entity
                 */
                CampModel[] models = _mapper.Map<CampModel[]>(results);
                return Ok(models);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker);
                if (result == null) return NotFound();
                /*
                 * The reason why we don't provide the Entity directly
                 * to our user because we don't want to expose everything
                 * to the users.
                 * 
                 * We use models and map them from the entity
                 */
                CampModel model = _mapper.Map<CampModel>(result);
                return Ok(model);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        // -> localhost:5000/api/camps/search?theDate=2018-10-18&includeTalks=false
        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsByEventDate(theDate, includeTalks);

                if (!results.Any()) return NotFound(); // check if query has contain any elements
                /*
                 * The reason why we don't provide the Entity directly
                 * to our user because we don't want to expose everything
                 * to the users.
                 * 
                 * We use models and map them from the entity
                 */

                CampModel[] models = _mapper.Map<CampModel[]>(results);
                return Ok(models);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        // we are binding the raw data from body to the model by using [ApiController] attribute
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                // check existing camp
                var existing = await _repository.GetCampAsync(model.Moniker);
                if (existing != null)
                    return BadRequest("Moniker in use");

                // the first param is the name of the Get action method, we named it 'Get'
                // the second param is the name of the controller which is CampsController [Camps]
                // the third param is the action method arguments, we have to specify an anonymous class
                var location = _linkGenerator.GetPathByAction("Get", "Camps", new { moniker = model.Moniker});

                if (string.IsNullOrWhiteSpace(location))
                    return BadRequest("Could not use current moniker");
                // create a new camp
                // we are mapping the CampModel back to Camp entity by using the mapper
                var camp = _mapper.Map<Camp>(model);
                _repository.Add(camp);

                if(await _repository.SaveChangesAsync())
                {
                    // we should specify the uri of the Get method and we will use a link generator
                    // we should also map Camp to CampModel
                    return Created($"/api/camps/{camp.Moniker}", _mapper.Map<CampModel>(camp));
                }
                
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure "+e.Message);
            }
            return BadRequest("Internal Server Error");
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound($"Could not find camp with moniker of {moniker}");

                // the mapper will automatically update the oldCamp with the new model provided
                _mapper.Map(model, oldCamp);

                if(await _repository.SaveChangesAsync())
                {
                    // Put doesn't required you to have a special return type, Ok or 200 is the correct return type
                    // since we are using an ActionResult, we can just return the object without putting the Ok method
                    return _mapper.Map<CampModel>(oldCamp); 
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure " + e.Message);
            }
            return BadRequest("Internal Server Error");
        }


        // Since we are just returning a status code, we dont need the ActionResult<T>
        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound($"Could not find camp with moniker of {moniker}");

                _repository.Delete(oldCamp);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure " + e.Message);
            }
            return BadRequest("Internal Server Error");
        }
    }
}
