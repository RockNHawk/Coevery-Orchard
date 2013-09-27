﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Services;
using Coevery.Core.Models;
using Coevery.Entities.Events;
using Coevery.Entities.Models;
using Coevery.Entities.Services;
using Coevery.Entities.Settings;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Utility.Extensions;

namespace Coevery.Entities.Controllers {
    public class EntityController : ApiController {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly ISchemaUpdateService _schemaUpdateService;
        private readonly IEntityEvents _entityEvents;
        private readonly IContentMetadataService _contentMetadataService;

        public EntityController(
            IContentDefinitionService contentDefinitionService,
            IContentMetadataService contentMetadataService,
            ISchemaUpdateService schemaUpdateService,
            IEntityEvents entityEvents) {
            _contentDefinitionService = contentDefinitionService;
            _contentMetadataService = contentMetadataService;
            _schemaUpdateService = schemaUpdateService;
            _entityEvents = entityEvents;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        //GET api/Entities/Entity
        public object Get(int rows, int page) {
            var metadataTypes = _contentMetadataService.GetRawEntities();

            var query = from type in metadataTypes
                        select new {
                            Id = type.Id,
                            Name = type.Name, 
                            DisplayName = type.DisplayName
                        };

            var totalRecords = query.Count();

            return new {
                total = Convert.ToInt32(Math.Ceiling((double)totalRecords / rows)),
                page = page,
                records = totalRecords,
                rows = query
            };
        }

        // DELETE api/Entities/Entity/:entityName
        public virtual HttpResponseMessage Delete(int name) {
            var entityName = _contentMetadataService.TryDeleteEntity(name);
            if (entityName == null) {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            var typeViewModel = _contentDefinitionService.GetType(entityName);

            if (typeViewModel == null) {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }
            _entityEvents.OnDeleting(entityName);
            _contentDefinitionService.RemoveType(entityName, true);
            _schemaUpdateService.DropTable(entityName);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}