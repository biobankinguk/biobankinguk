// TODO: package

import React, { Children, isValidElement } from "react";

/** Used to identify our Condition Container Components */
const DISPLAY_NAME = n => `Conditional.Condition.${n || ""}`;

/**
 * Check if a component renders a Condition Container Component (e.g. `<True>`)
 */
const isConditionContainer = c =>
  c.type.displayName && c.type.displayName.startsWith(DISPLAY_NAME());

/**
 * Pre-renders internal Condition Container components to clean up the component tree.
 *
 * Leaves normal components alone to be rendered by React.
 */
const render = c => (isConditionContainer(c) ? c.type(c.props) : c);

/** Compare two values, optionally in a loose fashion */
const compare = (a, b, loose) => (loose ? a == b : a === b); // eslint-disable-line eqeqeq

/**
 * Internal Condition Container Component.
 *
 * Allows wrapping multiple children without
 * manually turning them into a component
 * or requiring another container like a <div>.
 *
 * Gets collapsed in the component tree by `render()`.
 */
const Condition = ({ children }) => children;
Condition.displayName = DISPLAY_NAME("Internal");

/**
 * Truthy container.
 *
 * Renders when `expression` is truthy,
 * or the result of any additional constraints are:
 *
 * e.g. `<True when={1} />` `<True condition={x => x % 2} />`
 */
const True = p => <Condition {...p} />;
True.displayName = DISPLAY_NAME("True");

const FalseyCondition = x => !x;
/**
 * Falsey container.
 *
 * Renders when `expression` is falsey.
 *
 * Throws away other constraints so they do not apply.
 * */
const False = ({ children }) => (
  <Condition condition={FalseyCondition}>{children}</Condition>
);
False.displayName = DISPLAY_NAME("False");

/**
 * General Condition Container
 *
 * Renders when
 *   - `value` is equal to `expression`
 *   - `condition` is truthy.
 *
 * Equivalent to `<True when={value} [loose] />`
 * or `<True condition={x => ...} />`
 *
 * Throws away other constraints so they do not apply.
 */
const When = ({ value, condition, loose, children }) => (
  <Condition when={value} condition={condition} loose={loose}>
    {children}
  </Condition>
);
When.displayName = DISPLAY_NAME("When");

/**
 * General Condition Container
 *
 * Renders when
 *   - `value` is equal to `expression`
 *   - `condition` is truthy.
 *
 * Equivalent to `<True when={value} [loose] />`
 * or `<True condition={x => ...} />`
 *
 * Throws away other constraints so they do not apply.
 */
const Case = When;

/**
 * Default / Fallback container
 *
 * Renders when no other immediate children matched on expression.
 */
const Default = ({ children }) => <Condition default>{children}</Condition>;
Default.displayName = DISPLAY_NAME("Default");

/**
 * Default / Fallback container
 *
 * Renders when no other immediate children matched on expression.
 */
const Fallback = Default;

/**
 * Conditionally renders immediate children based on their props
 * and the value of `expression`.
 *
 * By default all matching children will be rendered.
 *
 * `exclusive` makes only the first match rendered.
 *
 * `default` or `fallback` children will only render for no matches.
 */
const Conditional = ({ exclusive, expression, children }) => {
  const matches = [];
  const defaults = [];

  for (let c of Children.toArray(children)) {
    if (exclusive && matches.length) break;

    if (isValidElement(c)) {
      if (isConditionContainer(c)) {
        // render the underlying condition directly
        // and subject it to our standard rules
        c = c.type({ ...c.props, expression });
      }

      // extract relevant props so we can check the conditions
      const {
        default: defaultMatch,
        fallback,
        when,
        condition,
        loose
      } = c.props;

      // then handle conditions
      // in priority order based on what props they have
      // these are deliberately nested so we perform
      // the top level exclusive check first
      if (when) {
        if (compare(expression, when, loose)) matches.push(render(c));
      } else if (condition) {
        if (condition(expression)) matches.push(render(c));
      } else if (defaultMatch || fallback) {
        if (!exclusive || !defaults.length) defaults.push(render(c));
      } else {
        if (expression) matches.push(render(c));
      }
    } else {
      if (expression) matches.push(c);
    }
  }

  Children.forEach(children, c => {});

  return (matches.length && matches) || (defaults.length && defaults) || null;
};

/**
 * Conditionally renders immediate children based on their props
 * and the value of `expression`.
 *
 * By default all matching children will be rendered.
 *
 * `exclusive` makes only the first match rendered.
 *
 * `default` or `fallback` children will only render for no matches.
 */
const Switch = Conditional;

export default Conditional;
export { Conditional, Switch, True, False, When, Case, Default, Fallback };
