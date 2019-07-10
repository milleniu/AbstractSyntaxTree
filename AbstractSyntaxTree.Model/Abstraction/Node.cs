namespace AbstractSyntaxTree.Model.Abstraction
{
    public abstract class Node
    {
        public abstract void Accept( NodeVisitor nodeVisitor );
        public abstract Node Accept( NodeMutator nodeMutator );
    }
}
