using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Auth;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class RegistrationDomainRuleController : ControllerBase
    {
        private readonly IRegistrationDomainService _registrationDomainService;
        
        public RegistrationDomainRuleController(IRegistrationDomainService registrationDomainService)
        {
            _registrationDomainService = registrationDomainService;
        }

        /// <summary>
        /// Generate a list of Registration Domain Rules.
        /// </summary>
        /// <returns>The list of Registration Domain Rules.</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(RegistrationDomainRuleModel))]
        public async Task<IList> Get() =>
            (await _registrationDomainService.ListRules())
                .Select(x =>
                    new RegistrationDomainRuleModel()
                    {
                        Id = x.Id,
                        RuleType = x.RuleType,
                        Value = x.Value,
                        Source = x.Source,
                        DateModified = x.DateModified
                    }
                )
                .ToList();

        /// <summary>
        /// Insert a new Registration Domain Rule.
        /// </summary>
        /// <param name="model">The rule to insert.</param>
        /// <returns>The inserted rule.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(RegistrationDomainRuleModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(RegistrationDomainRuleModel model)
        {
            //Checking if the given value is valid
            var validPattern = new Regex(@"^[^@\s]*@?[^@\s]*\.[^@\s]+[^@\.]$");
            if (!validPattern.IsMatch(model.Value))
            {
                ModelState.AddModelError("Value", "That value is invalid and must contain at least one of '.' or '@'.");
            }

            if (model.Value.Length <= 1)
            {
                ModelState.AddModelError("Value", "That value is invalid and must contain more than one character.");
            }

            if ((await _registrationDomainService.GetRuleByValue(model.Value)) != null)
            {
                ModelState.AddModelError("Value", "A rule with that value already exists");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var type = new RegistrationDomainRule
            {
                Id = model.Id,
                RuleType = model.RuleType,
                Value = model.Value,
                Source = model.Source,
                DateModified = System.DateTime.Now
            };

            await _registrationDomainService.Add(type);
            
            // Success response
            return Ok(model);
        }

        /// <summary>
        /// Update a Registration Domain Rule.
        /// </summary>
        /// <param name="id">Id of the rule to update.</param>
        /// <param name="model">Model with updated values.</param>
        /// <returns>The updated rule.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPut("{id:int}")]
        [SwaggerResponse(200, Type = typeof(RegistrationDomainRuleModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(int id, RegistrationDomainRuleModel model)
        {           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _registrationDomainService.Update(new RegistrationDomainRule()
            {
                Id = id,
                RuleType = model.RuleType,
                Value = model.Value,
                Source = model.Source,
                DateModified = System.DateTime.Now
            });

            // Success message
            return Ok(model);
        }

        /// <summary>
        /// Delete a Registration Domain Rule.
        /// </summary>
        /// <param name="id">Id of the rule to delete.</param>
        /// <returns>The deleted rule.</returns>
        /// <response code="200">Request Successful</response>
        [HttpDelete("{id:int}")]
        [SwaggerResponse(200, Type = typeof(RegistrationDomainRuleModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = (await _registrationDomainService.ListRules()).FirstOrDefault(x => x.Id == id);

            if (model == null)
            {
                return Ok();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _registrationDomainService.Delete(new RegistrationDomainRule()
            {
                Id = model.Id,
                RuleType = model.RuleType,
                Value = model.Value,
                Source = model.Source,
                DateModified = model.DateModified
            });

            //Everything went A-OK!
            return Ok(model);
        }
        
    }
}
