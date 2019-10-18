import styled from "styled-components";
import { space, color, layout, background, shadow } from "styled-system";

const Box = styled.div`
  ${space};
  ${color};
  ${layout};
  ${background};
  ${shadow};
  box-sizing: border-box;
  min-width: 0;
`;

export default Box;
