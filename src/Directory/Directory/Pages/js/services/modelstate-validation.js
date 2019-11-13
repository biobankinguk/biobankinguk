export const hasErrors = (state, ...fields) => {
  if (!state) return false;
  
  var stateKeys = Object.keys(state);
  if (!stateKeys.length) return false;

  const fieldHasErrors = field => state[field] && state[field].length;

  if (fields.length) return fields.some(fieldHasErrors);
  return stateKeys.some(fieldHasErrors);
};
