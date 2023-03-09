using System.Collections.Generic;

namespace SH.StateMachine.Nodes
{
    public class SeparatorNode : BaseNode
    {
        public override IReadOnlyList<BaseNode> Outputs => (outputs[0] != null) ? outputs[0].Outputs : null;
    }
}