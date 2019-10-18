import styled from "styled-components";
import { typography, color } from "styled-system";

/** The most basic Typography:
 * span just to enable access to the system props,
 *
 * Should add Heading variants and divs and such later maybe?
 */

const Text = styled.span`
  ${typography};
  ${color};

  a {
    ${color};
    text-decoration: none;

    &:hover {
      text-decoration: underline;
    }
  }
`;

Text.defaultProps = {
  fontFamily: "sans-serif"
};

export default Text;
