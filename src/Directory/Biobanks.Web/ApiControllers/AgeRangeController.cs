﻿using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;
using System.Xml;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/AgeRange")]
    public class AgeRangeController : ApiBaseController
    {
        private readonly IReferenceDataService<AgeRange> _ageRangeService;

        public AgeRangeController(IReferenceDataService<AgeRange> ageRangeService)
        {
            _ageRangeService = ageRangeService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _ageRangeService.List())
                .Select(x =>
                    Task.Run(async () => new AgeRangeModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _ageRangeService.GetUsageCount(x.Id),
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
            if (model.LowerDuration == "N/A") { model.LowerDuration = null;  }
            if (model.UpperDuration == "N/A") { model.UpperDuration = null;  }

            // Validate model
            if (await _ageRangeService.Exists(model.Description))
            {
                ModelState.AddModelError("AgeRange", "That description is already in use. Age ranges must be unique.");
            }

            if (model.LowerDuration != null)
            {
                if (!int.TryParse(model.LowerBound, out _))
                {
                    ModelState.AddModelError("AgeRange", "Lower Bound value must be a valid number.");
                }
            }
            if (model.UpperDuration != null)
            {
                if (!int.TryParse(model.UpperBound, out _))
                {
                    ModelState.AddModelError("AgeRange", "Upper Bound value must be a valid number.");
                }
            }

            if (string.IsNullOrEmpty(model.LowerDuration) && string.IsNullOrEmpty(model.UpperDuration))
            {
                ModelState.AddModelError("AgeRange", "Both Upper and Lower Bounds must not be null.");
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
                if (!string.IsNullOrEmpty(convertedModel.LowerBound) && !string.IsNullOrEmpty(convertedModel.UpperBound))
                {
                    if (XmlConvert.ToTimeSpan(convertedModel.LowerBound) >= XmlConvert.ToTimeSpan(convertedModel.UpperBound))
                    {
                        ModelState.AddModelError("AgeRange", "Upper Bound value must be greater than the Lower Bound value");
                    }
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


            await _ageRangeService.Add(range);
            await _ageRangeService.Update(range); // Ensure sortOrder is correct

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
            var exisiting = await _ageRangeService.Get(model.Description);

            if (model.LowerDuration == "N/A") { model.LowerDuration = null; }
            if (model.UpperDuration == "N/A") { model.UpperDuration = null; }

            // Validate model
            if (exisiting != null && exisiting.Id != id)
            {
                ModelState.AddModelError("AgeRange", "That description is already in use. Age ranges must be unique.");
            }
            

            // If in use, prevent update
            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("AgeRange", $"The age range \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (model.LowerBound != null || model.UpperBound != null)
            {
                if (model.LowerDuration != null)
                {
                    if (!int.TryParse(model.LowerBound, out _))
                    {
                        ModelState.AddModelError("AgeRange", "Lower Bound value must be a valid number.");
                    }
                }
                if (model.UpperDuration != null)
                {
                    if (!int.TryParse(model.UpperBound, out _))
                    {
                        ModelState.AddModelError("AgeRange", "Upper Bound value must be a valid number.");
                    }
                }

            }

            // Checks if entry already had both null values prior to edit
            if (exisiting != null)
            {
                var nullBefore = string.IsNullOrEmpty(exisiting.LowerBound) && string.IsNullOrEmpty(exisiting.UpperBound);
                var nullAfter = string.IsNullOrEmpty(model.LowerBound) && string.IsNullOrEmpty(model.UpperBound);

                if (!nullBefore && nullAfter)
                {
                    ModelState.AddModelError("AgeRange", "Both Upper and Lower Bounds must not be null.");
                }
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
                if (!string.IsNullOrEmpty(convertedModel.LowerBound) && !string.IsNullOrEmpty(convertedModel.UpperBound))
                {
                    if (XmlConvert.ToTimeSpan(convertedModel.LowerBound) >= XmlConvert.ToTimeSpan(convertedModel.UpperBound))
                    {
                        ModelState.AddModelError("AgeRange", "Upper Bound value must be greater than the Lower Bound value");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _ageRangeService.Update(new AgeRange
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
            var model = await _ageRangeService.Get(id);

            // If in use, prevent update
            if (await _ageRangeService.IsInUse(id))
            {
                ModelState.AddModelError("AgeRange", $"The age range \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _ageRangeService.Delete(id);

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

            await _ageRangeService.Update(access);

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
            if (model.LowerDuration != null)
            {
                if (int.Parse(model.LowerBound) < 0)
                {
                    model.LowerBound = model.LowerBound.Replace("-", "");
                    model.LowerBound = "-P" + model.LowerBound + model.LowerDuration;
                }
                else if (int.Parse(model.LowerBound) >= 0)
                {
                    model.LowerBound = "P" + model.LowerBound + model.LowerDuration;
                }
            }
            else
            {
                model.LowerBound = null;
                model.LowerDuration = null;
            }
            if (model.UpperDuration != null)
            {
                if (int.Parse(model.UpperBound) < 0)
                {
                    model.UpperBound = model.UpperBound.Replace("-", "");
                    model.UpperBound = "-P" + model.UpperBound + model.UpperDuration;
                }
                else if (int.Parse(model.UpperBound) >= 0)
                {
                    model.UpperBound = "P" + model.UpperBound + model.UpperDuration;
                }
            }
            else
            {
                model.UpperBound = null;
                model.UpperDuration = null;
            }

            return model;
        }

    }
}