﻿
using Coevery.Entities.Settings;

namespace Coevery.Fields.Settings {
    public class EmailFieldSettings : FieldSettings {
        public string DefaultValue { get; set; }

        public EmailFieldSettings()
        {
            DefaultValue = null;
        }
    }
}
