namespace Model
{
    public class NP_CharacterBehaviorTree : NP_BaseBehaviorTree
    {
        public CharacterComponent Component;

        public NP_CharacterBehaviorTree(CharacterComponent component, BaseGraph graph) : base(graph)
        {
            Component = component;
        }
    }
}