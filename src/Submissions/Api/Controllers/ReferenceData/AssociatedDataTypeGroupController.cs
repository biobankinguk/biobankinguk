﻿using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Models.Submissions;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class AssociatedDataTypeGroupController : ControllerBase
    {
        private readonly IReferenceDataService<AssociatedDataTypeGroup> _associatedDataTypeGroupService;

        public AssociatedDataTypeGroupController(IReferenceDataService<AssociatedDataTypeGroup> associatedDataTypeGroupService)
        {
            _associatedDataTypeGroupService = associatedDataTypeGroupService;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<IList> Get()
        {
            var model = (await _associatedDataTypeGroupService.List())
                .Select(x =>
                    Task.Run(async () => new ReadAssociatedDataTypeGroupModel
                    {
                        AssociatedDataTypeGroupId = x.Id,
                        Name = x.Value,
                        AssociatedDataTypeGroupCount = await _associatedDataTypeGroupService.GetUsageCount(x.Id)
                    })
                    .Result
                 )
                .ToList();

            return model;
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _associatedDataTypeGroupService.Get(id);

            if (await _associatedDataTypeGroupService.IsInUse(id))
            {
                ModelState.AddModelError("Name", $"The associated data type group \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _associatedDataTypeGroupService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(AssociatedDataTypeGroupModel model)
        {
            //If this name is valid, it already exists
            if (await _associatedDataTypeGroupService.Exists(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Associated Data Type Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _associatedDataTypeGroupService.Add(new AssociatedDataTypeGroup
            {
                Value = model.Name
            });

            //Everything went A-OK!
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, AssociatedDataTypeGroupModel model)
        {
            var exisiting = await _associatedDataTypeGroupService.Get(model.Name);

            //If this name is valid, it already exists
            if (exisiting != null && exisiting.Id != id)
            {
                ModelState.AddModelError("Name", "That name is already in use by another asscoiated data type group. Associated Data Type Group names must be unique.");
            }

            if (await _associatedDataTypeGroupService.IsInUse(id))
            {
                ModelState.AddModelError("Name", "This associated data type group is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _associatedDataTypeGroupService.Update(new AssociatedDataTypeGroup
            {
                Id = id,
                Value = model.Name
            });

            //Everything went A-OK!
            return Ok(model);
        }
    }
}