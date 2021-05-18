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
        private readonly IRegistrationDomainService _registrationDomainService;

        public RegistrationDomainRuleController(IRegistrationDomainService registrationDomainService)
        {
            _registrationDomainService = registrationDomainService;
        }

        [HttpGet]
        [Route("")]
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

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(RegistrationDomainRuleModel model)
        {
            //Checking if the given value is valid
            if (!model.Value.Contains(".") && !model.Value.Contains("@"))
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

            await _registrationDomainService.Add(type);
            
            // Success response
            return Json(new
            {
                success = true,
                name = model.Value
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

            await _registrationDomainService.Update(new RegistrationDomainRule()
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
                name = model.Value
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _registrationDomainService.ListRules()).FirstOrDefault(x => x.Id == id);

            if (model == null)
            {
                return Json(new
                {
                    success = true
                });
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
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
            return Json(new
            {
                success = true,
                name = model.Value
            });
        }
    }
}