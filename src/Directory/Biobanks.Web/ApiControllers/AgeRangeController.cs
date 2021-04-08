using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;
using System.Xml;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/AgeRange")]
    public class AgeRangeController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AgeRangeController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListAgeRangesAsync())
                .Select(x =>
                    Task.Run(async () => new AgeRangeModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetAgeRangeUsageCount(x.Id),
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(AgeRangeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAgeRangeAsync(model.Description))
            {
                ModelState.AddModelError("AgeRange", "That description is already in use. Age ranges must be unique.");
            }

            if (!int.TryParse(model.LowerBound, out _) || !int.TryParse(model.UpperBound, out _))
            {
                ModelState.AddModelError("AgeRange", "Lower and Upper Bound values must be valid numbers.");
            }

            var convertedModel = new AgeRangeModel();

            if (ModelState.IsValid)
            {
                // Need to encode lower/upper bound with duration 
                convertedModel = ConversionToIsoDuration(new AgeRangeModel()
                {
                    Id = model.Id,
                    Description = model.Description,
                    SortOrder = model.SortOrder,
                    LowerBound = model.LowerBound,
                    UpperBound = model.UpperBound,
                    LowerDuration = model.LowerDuration,
                    UpperDuration = model.UpperDuration
                });
                if (XmlConvert.ToTimeSpan(convertedModel.LowerBound) > XmlConvert.ToTimeSpan(convertedModel.UpperBound))
                {
                    ModelState.AddModelError("AgeRange", "Upper Bound value must be greater than the Lower Bound value");
                }
            }



            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Add new Age Range
            var range = new AgeRange
            {
                Id = convertedModel.Id,
                Value = convertedModel.Description,
                SortOrder = convertedModel.SortOrder,
                LowerBound = convertedModel.LowerBound,
                UpperBound = convertedModel.UpperBound
            };


            await _biobankWriteService.AddAgeRangeAsync(range);
            await _biobankWriteService.UpdateAgeRangeAsync(range, true); // Ensure sortOrder is correct

            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, AgeRangeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAgeRangeAsync(id, model.Description))
            {
                ModelState.AddModelError("AgeRange", "That description is already in use. Age ranges must be unique.");
            }

            // If in use, prevent update
            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("AgeRange", $"The age range \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!int.TryParse(model.LowerBound, out _) || !int.TryParse(model.UpperBound, out _))
            {
                ModelState.AddModelError("AgeRange", "Lower and Upper Bound values must be valid numbers.");
            }

            var convertedModel = new AgeRangeModel();

            if (ModelState.IsValid)
            {
                // Need to encode lower/upper bound with duration 
                convertedModel = ConversionToIsoDuration(new AgeRangeModel()
                {
                    Id = model.Id,
                    Description = model.Description,
                    SortOrder = model.SortOrder,
                    LowerBound = model.LowerBound,
                    UpperBound = model.UpperBound,
                    LowerDuration = model.LowerDuration,
                    UpperDuration = model.UpperDuration
                });
                if (XmlConvert.ToTimeSpan(convertedModel.LowerBound) > XmlConvert.ToTimeSpan(convertedModel.UpperBound))
                {
                    ModelState.AddModelError("AgeRange", "Upper Bound value must be greater than the Lower Bound value");
                }
            }



            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateAgeRangeAsync(new AgeRange
            {
                Id = id,
                Value = convertedModel.Description,
                SortOrder = convertedModel.SortOrder,
                LowerBound = convertedModel.LowerBound,
                UpperBound = convertedModel.UpperBound
            });

            return Json(new
            {
                success = true,
                name = convertedModel.Description,
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListAgeRangesAsync()).Where(x => x.Id == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsAgeRangeInUse(id))
            {
                ModelState.AddModelError("AgeRange", $"The age range \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteAgeRangeAsync(new AgeRange
            {
                Id = model.Id,
                Value = model.Value,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
            });

            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, AgeRangeModel model)
        {

            var access = new AgeRange
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
            };

            await _biobankWriteService.UpdateAgeRangeAsync(access, true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }

        private AgeRangeModel ConversionToIsoDuration(AgeRangeModel model)
        {
            // Unable to use XmlConvert.toString as cannot create valid TimeSpan from Years/Months
            // Check for negatives
            if (int.Parse(model.LowerBound) < 0)
            {
                model.LowerBound = model.LowerBound.Replace("-", "");
                model.LowerBound = "-P" + model.LowerBound + model.LowerDuration;
            }
            else if (int.Parse(model.LowerBound) >= 0)
            {
                model.LowerBound = "P" + model.LowerBound + model.LowerDuration;
            }

            if (int.Parse(model.UpperBound) < 0)
            {
                model.UpperBound = model.UpperBound.Replace("-", "");
                model.UpperBound = "-P" + model.UpperBound + model.UpperDuration;
            }
            else if (int.Parse(model.UpperBound) >= 0)
            {
                model.UpperBound = "P" + model.UpperBound + model.UpperDuration;
            }

            return model;
        }

    }


}