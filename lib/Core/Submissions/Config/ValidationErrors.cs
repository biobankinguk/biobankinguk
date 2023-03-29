using System;
using System.Collections.Generic;

namespace Core.Submissions.Config
{
    public static class ValidationErrors
    {
        #region Required

        public static string RequiredIsNull =>
            "This required item is missing";

        public static string RequiredIsNullOrWhitespace =>
            "This required item is missing, empty or contains only whitespace";

        //TODO rework conditions
        public static string ConditionallyRequiredIsNullOrWhitespace(params KeyValuePair<string, string>[] conditions)
        {
            var result = "This item is missing, empty or contains only whitespace, but is required";

            for (int i = 0; i < conditions.Length; i++)
            {
                if (i == 0)
                    result += " when ";
                else
                    result += " and ";

                result += $"{conditions[i].Key} is {conditions[i].Value}";
            }

            return result;
        }

        #endregion

        #region Types

        public static string IdPropertiesPrefix(string barcode, string individualReferenceId)
        {
            var barcodeText = string.Empty;
            if (!string.IsNullOrEmpty(barcode))
                barcodeText = $"{nameof(barcode)}: {barcode}; ";

            return $"{barcodeText}{nameof(individualReferenceId)}: {individualReferenceId} - ";
        }

        public static string Invalid(string message, string barcode, string individualReferenceId)
            => $"{IdPropertiesPrefix(barcode, individualReferenceId)} {message}";

        public static string InvalidForType(string value, string type, string barcode, string individualReferenceId) =>
            Invalid($"'{value}' is not a valid {type} value", barcode, individualReferenceId);

        public static string ExtractionProcedureMaterialTypeMismatch(string epValue, string mtValue, string barcode, string individualReferenceId) =>
            Invalid($"ExtractionProcedure'{epValue}' is not a valid for the MaterialType {mtValue}", barcode, individualReferenceId);

        public static string PreservationType(string value, string barcode, string individualReferenceId) =>
            InvalidForType(value, "Preservation Type", barcode, individualReferenceId);

        public static string SnomedDiagnosis(string value, string field, string individualReferenceId) =>
            InvalidForType(value, $"SNOMED Diagnosis {field}", null, individualReferenceId);

        public static string SnomedTreatment(string value, string field, string individualReferenceId) =>
            InvalidForType(value, $"SNOMED Treatment {field}", null, individualReferenceId);

        public static string SnomedBodyOrgan(string value, string field, string barcode, string individualReferenceId) =>
            InvalidForType(value, $"SNOMED Body Organ {field}", barcode, individualReferenceId);

        public static string SnomedSampleContent(string value, string field, string barcode, string individualReferenceId) =>
            InvalidForType(value, $"SNOMED Sample Content {field}", barcode, individualReferenceId);

        public static string SnomedExtractionProcedure(string value, string field, string barcode, string individualReferenceId) =>
            InvalidForType(value, $"SNOMED Extraction Procedure  {field}", barcode, individualReferenceId);

        public static string MaterialType(string value, string barcode, string individualReferenceId) =>
            InvalidForType(value, "Material Type", barcode, individualReferenceId);

        public static string StorageTemperature(string value, string barcode, string individualReferenceId) =>
            InvalidForType(value, "Storage Temperature", barcode, individualReferenceId);

        public static string SampleContentMethod(string value, string field, string barcode, string individualReferenceId) =>
            InvalidForType(value, $"Sample Content Method {field}", barcode, individualReferenceId);

        public static string TreatmentLocation(string value, string individualReferenceId) =>
            InvalidForType(value, "Treatment Location", null, individualReferenceId);

        public static string TreatmentCodeOntology(string value, string individualReferenceId) =>
            InvalidForType(value, "TreatmentCodeOntology", null, individualReferenceId);

        public static string TreatmentCodeOntologyVersion(string value, string individualReferenceId) =>
            InvalidForType(value, "TreatmentCodeOntologyVersion", null, individualReferenceId);

        public static string DiagnosisCodeOntology(string value, string individualReferenceId) =>
            InvalidForType(value, "DiagnosisCodeOntology", null, individualReferenceId);

        public static string DiagnosisCodeOntologyVersion(string value, string individualReferenceId) =>
            InvalidForType(value, "DiagnosisCodeOntologyVersion", null, individualReferenceId);

        public static string ExtractionSiteOntology(string value, string barcode, string individualReferenceId) =>
            InvalidForType(value, "ExtractionSiteOntology", barcode, individualReferenceId);

        public static string ExtractionSiteOntologyVersion(string value, string barcode, string individualReferenceId) =>
            InvalidForType(value, "ExtractionSiteOntologyVersion", barcode, individualReferenceId);

        public static string Sex(string value, string barcode, string individualReferenceId) =>
            InvalidForType(value, "Sex", barcode, individualReferenceId);

        public static string DateInFuture(DateTime value, string individualReferenceId) =>
            $"{IdPropertiesPrefix(null, individualReferenceId)}'DateTime {value}' is in the future";

        #endregion

        #region Formats

        public static string YYYY(string value) =>
            $"'{value}' is not in ISO-8601 YYYY or IETF RFC-3339 date-fullyear format, e.g. 1976";

        #endregion
    }
}
