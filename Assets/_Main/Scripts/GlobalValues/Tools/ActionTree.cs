namespace MeteorMadness.GlobalValues._Main.Scripts.GlobalValues.Tools
{

    public interface INode
    {
        void Execute();
    }
    
    public class ActionNode : INode
    {
        public delegate void myDelegate();
        private readonly myDelegate _action;
        
        public ActionNode(myDelegate action)
        {
            _action = action;
        }
        public void Execute()
        {
            if (_action == null) return;
            _action();
        }
    }
    
    public class QuestionNode : INode
    {
        //Firma de la funcion ()=>bool
        public delegate bool myDelegate();
        private readonly myDelegate _question;
        private readonly INode _falseNode;
        private readonly INode _trueNode;
        
        public QuestionNode(myDelegate question, INode trueNode, INode falseNode)
        {
            _question = question;
            _trueNode = trueNode;
            _falseNode = falseNode;
        }

        //()=>()
        public void Execute()
        {
            if (_question == null) return;
            if (_question())
            {
                if (_trueNode != null)
                    _trueNode.Execute();
            }
            else
            {
                if (_falseNode != null)
                    _falseNode.Execute();
            }
        }
    }
}