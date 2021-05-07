using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Services.Contracts;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Filters;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/RegistrationDomainRule")]
    public class RegistrationDomainRuleController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public RegistrationDomainRuleController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _biobankReadService.ListRegistrationDomainRulesAsync())
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

            return model;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(RegistrationDomainRuleModel model)
        {
            //Checking if the given value is valid
            if (!model.Value.Contains(".") && !model.Value.Contains("@"))
            {
                ModelState.AddModelError("Value", "That value is invalid and must contain at least one of '.' or '@'.");
            }


            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var type = new RegistrationDomainRule
            {
                Id = model.Id,
                RuleType = model.RuleType,
                Value = model.Value,
                Source = model.Source,
                DateModified = System.DateTime.Now
            };

            await _biobankWriteService.AddRegistrationDomainRuleAsync(type);
            await _biobankWriteService.UpdateRegistrationDomainRuleAsync(type); // Ensure sortOrder is correct

            // Success response
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, RegistrationDomainRuleModel model)
        {
            
            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateRegistrationDomainRuleAsync(new RegistrationDomainRule()
            {
                Id = id,
                RuleType = model.RuleType,
                Value = model.Value,
                Source = model.Source,
                DateModified = System.DateTime.Now
            });

            // Success message
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListRegistrationDomainRulesAsync()).First(x => x.Id == id);

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteRegistrationDomainRuleAsync(new RegistrationDomainRule()
            {
                Id = model.Id,
                RuleType = model.RuleType,
                Value = model.Value,
                Source = model.Source,
                DateModified = model.DateModified
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }
    }
}