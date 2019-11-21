import React, { useState } from "react";
import PasswordField from "./PasswordField";
import { SimpleGrid, Stack, Box } from "@chakra-ui/core";
import { Field } from "formik";
import BasicInput from "./BasicInput";
import PasswordRequirementsInfo from "../PasswordRequirementsInfo";

const SetPasswordFieldGroup = ({ initialHidden, ...p }) => {
  const [hidePasswordConfirm, setHidePasswordConfirm] = useState(initialHidden);
  const touchPassword = () => setHidePasswordConfirm(false);

  return (
    <SimpleGrid minChildWidth="300px">
      <Stack spacing={3} flexGrow={1} flexBasis="50%">
        <Box>
          <PasswordField onFocus={touchPassword} />
        </Box>
        <Box hidden={hidePasswordConfirm}>
          <Field name="PasswordConfirm">
            {rp => (
              <BasicInput
                {...rp}
                label="Confirm Password"
                placeholder="Password"
                isRequired
                isPassword
              />
            )}
          </Field>
        </Box>
      </Stack>

      <PasswordRequirementsInfo m={2} flexBasis="40%" />
    </SimpleGrid>
  );
};

export default SetPasswordFieldGroup;
