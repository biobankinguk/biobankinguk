const nullRender = () => null;

const ConditionalContent = ({ condition, trueRender, falseRender }) => {
  let render = (condition ? trueRender : falseRender) || nullRender;
  return render();
};

export default ConditionalContent;
