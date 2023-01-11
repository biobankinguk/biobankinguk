$(function () {
  var minPasswordLength = 8;

  $(":password").pwstrength({
    common: {
      minChar: minPasswordLength,
    },
    ui: {
      showErrors: true,
      showVerdicts: true,
      showVerdictsInsideProgressBar: false,
      errorMessages: {
        wordNotEmail: "Do not use an email address as a password",
        wordMinLength:
          "Your password is too short - it must be at least " +
          minPasswordLength +
          " characters",
        wordTwoCharacterClasses: "Use different character classes",
        wordRepetitions: "Too many repetitions",
        wordLowercase: "At least one lowercase letter required",
        wordUppercase: "At least one uppercase letter required",
        wordOneNumber: "At least one number required",
        wordOneSpecialChar: "At least one special character required",
      },
      verdicts: [
        "Your password is <strong>very weak</strong>",
        "Your password is <strong>weak</strong>",
        "Your password is <strong>average</strong>",
        "Your password is <strong>strong</strong>",
        "Your password is <strong>very strong</strong>",
      ],
      viewports: {
        progress: ".pwstrength_viewport_progress",
        verdict: ".pwstrength_viewport_verdict",
      },
    },
    rules: {
      activated: {
        wordNotEmail: false,
        wordMinLength: true,
        wordMaxLength: false,
        wordInvalidChar: false,
        wordSimilarToUsername: false,
        wordSequences: false,
        wordTwoCharacterClasses: false,
        wordRepetitions: false,
        wordLowercase: true,
        wordUppercase: true,
        wordOneNumber: true,
        wordThreeNumbers: false,
        wordOneSpecialChar: true,
        wordTwoSpecialChar: false,
        // These below options are already checked by virtue of those above
        wordUpperLowerCombo: false,
        wordLetterNumberCombo: false,
        wordLetterNumberCharCombo: false,
      },
    },
  });
});
