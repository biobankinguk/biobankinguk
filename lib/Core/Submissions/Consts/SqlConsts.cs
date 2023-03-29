namespace Core.Submissions.Consts
{
    /// <summary>
    /// SQL 'constants'
    /// </summary>
    public static class SqlConsts
    {
        /// <summary>
        /// Deletes all live diagnoses for a given organisation.
        /// </summary>
        /// <param name="organisationId">The organisation ID to delete live diagnoses for.</param>
        /// <returns></returns>
        public static string DeleteAllLiveDiagnoses(int organisationId) => $"DELETE FROM Diagnoses WHERE OrganisationId = {organisationId};";

        /// <summary>
        /// Deletes all live samples for a given organisation.
        /// </summary>
        /// <param name="organisationId">The organisation ID to delete live samples for.</param>
        /// <returns></returns>
        public static string DeleteAllLiveSamples(int organisationId) => $"UPDATE Samples SET IsDirty = 1, IsDeleted = 1 WHERE OrganisationId = {organisationId};";

        /// <summary>
        /// Deletes all live treatments for a given organisation.
        /// </summary>
        /// <param name="organisationId">The organisation ID to delete live treatments for.</param>
        /// <returns></returns>
        public static string DeleteAllLiveTreatments(int organisationId) => $"DELETE FROM Treatments WHERE OrganisationId = {organisationId};";

        /// <summary>
        /// Merges staged diagnoses into the live diagnoses area for a given organisation.
        /// </summary>
        /// <param name="organisationId">The organisation ID to merge diagnoses for.</param>
        /// <returns></returns>
        public static string MergeStagedDiagnosesIntoLive(int organisationId)
            => $@"DELETE Diagnoses
	            FROM Diagnoses
	            INNER JOIN StagedDiagnoses
		            ON Diagnoses.OrganisationId = StagedDiagnoses.OrganisationId
		            AND Diagnoses.IndividualReferenceId = StagedDiagnoses.IndividualReferenceId
		            AND Diagnoses.DateDiagnosed = StagedDiagnoses.DateDiagnosed
		            AND Diagnoses.DiagnosisCodeId = StagedDiagnoses.DiagnosisCodeId
		            AND Diagnoses.OrganisationId = {organisationId};

                DELETE FROM Diagnoses WHERE Id IN (SELECT Id FROM StagedDiagnosisDeletes);

                INSERT INTO Diagnoses (OrganisationId, SubmissionTimestamp, IndividualReferenceId, DateDiagnosed, DiagnosisCodeId, DiagnosisCodeOntologyVersionId)
	                SELECT OrganisationId, SubmissionTimestamp, IndividualReferenceId, DateDiagnosed, DiagnosisCodeId, DiagnosisCodeOntologyVersionId FROM StagedDiagnoses WHERE OrganisationId = {organisationId};

                DELETE FROM StagedDiagnoses WHERE OrganisationId = {organisationId};";

        /// <summary>
        /// Merges staged samples into the live samples area for a given organisation.
        /// </summary>
        /// <param name="organisationId">The organisation ID to merge samples for.</param>
        /// <returns></returns>
        public static string MergeStagedSamplesIntoLive(int organisationId)
            => $@"DELETE Samples
	            FROM Samples
	            INNER JOIN StagedSamples
		            ON Samples.OrganisationId = StagedSamples.OrganisationId
		            AND Samples.IndividualReferenceId = StagedSamples.IndividualReferenceId
		            AND Samples.Barcode = StagedSamples.Barcode
		            AND (
                        (Samples.CollectionName IS NULL AND StagedSamples.CollectionName IS NULL)
                        OR (Samples.CollectionName = StagedSamples.CollectionName)
                    )
		            AND Samples.OrganisationId = {organisationId};

                DELETE FROM Samples WHERE Id IN (SELECT Id FROM StagedSampleDeletes);

                INSERT INTO Samples (OrganisationId, SubmissionTimestamp, IndividualReferenceId, Barcode, YearOfBirth, AgeAtDonation, MaterialTypeId, StorageTemperatureId, DateCreated, ExtractionSiteId, ExtractionSiteOntologyVersionId, ExtractionProcedureId, SampleContentId, SampleContentMethodId, SexId, CollectionName, IsDirty, IsDeleted)
	                SELECT OrganisationId, SubmissionTimestamp, IndividualReferenceId, Barcode, YearOfBirth, AgeAtDonation, MaterialTypeId, StorageTemperatureId, DateCreated, ExtractionSiteId, ExtractionSiteOntologyVersionId, ExtractionProcedureId, SampleContentId, SampleContentMethodId, SexId, CollectionName, 1, 0 FROM StagedSamples WHERE OrganisationId = {organisationId};
                     
                DELETE FROM StagedSamples WHERE OrganisationId = {organisationId};";

        /// <summary>
        /// Merges staged treatments into the live treatments area for a given organisation.
        /// </summary>
        /// <param name="organisationId">The organisation ID to merge treatments for.</param>
        /// <returns></returns>
        public static string MergeStagedTreatmentsIntoLive(int organisationId)
            => $@"DELETE Treatments
	            FROM Treatments
	            INNER JOIN StagedTreatments
		            ON Treatments.OrganisationId = StagedTreatments.OrganisationId
		            AND Treatments.IndividualReferenceId = StagedTreatments.IndividualReferenceId
		            AND Treatments.DateTreated = StagedTreatments.DateTreated
		            AND Treatments.TreatmentCodeId = StagedTreatments.TreatmentCodeId
		            AND Treatments.OrganisationId = {organisationId};

                DELETE FROM Treatments WHERE Id IN (SELECT Id FROM StagedTreatmentDeletes);

                INSERT INTO Treatments (OrganisationId, SubmissionTimestamp, IndividualReferenceId, DateTreated, TreatmentCodeId, TreatmentLocationId, TreatmentCodeOntologyVersionId)
	                SELECT OrganisationId, SubmissionTimestamp, IndividualReferenceId, DateTreated, TreatmentCodeId, TreatmentLocationId, TreatmentCodeOntologyVersionId FROM StagedTreatments WHERE OrganisationId = {organisationId};

                DELETE FROM StagedTreatments WHERE OrganisationId = {organisationId};";
    }
}
